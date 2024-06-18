// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Formatting
{
  public class OutputFormatter : IOutputFormatter
  {
    public string GetShortFormattedTypeName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var name = BuildUnnestedTypeName (type);

      if (type.IsNested)
        name = GetShortFormattedTypeName (type.DeclaringType) + "+" + name;

      return name;
    }

    public string GetFullFormattedTypeName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var name = BuildUnnestedTypeName (type);

      return type.IsNested ? string.Format ("{0}.{1}+{2}", type.DeclaringType.Namespace, GetShortFormattedTypeName (type.DeclaringType), name)
                           : string.Format ("{0}.{1}", type.Namespace, name);
    }

    public string GetConstructorName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return Regex.Replace (type.Name, @"`\d+$", "");
    }

    private string BuildUnnestedTypeName (Type type)
    {
      var name = type.Name;

      if (type.IsGenericType)
      {
        int index = name.IndexOf ('`');
        if (index != -1) // Happens for weird types
          name = name.Substring (0, index);
        name = name + BuildGenericSignature (type);
      }
      return name;
    }

    private string BuildGenericSignature (Type type)
    {
      var enclosingTypeGenericArgumentsCount = type.DeclaringType == null
                                                 ? 0
                                                 : type.DeclaringType.GetGenericArguments ().Length;

      if (enclosingTypeGenericArgumentsCount == type.GetGenericArguments ().Length)
        return "";

      var genericArguments = type.GetGenericArguments ()
        .Skip (enclosingTypeGenericArgumentsCount)
        .Select (a => a.IsGenericParameter ? a.Name : GetFullFormattedTypeName (a))
        .Aggregate ((a1, a2) => string.Format ("{0}, {1}", a1, a2));

      return string.Format ("<{0}>", genericArguments);
    }

    public XElement CreateModifierMarkup (string attributes, string keywords)
    {
      var modifiers = new XElement ("Modifiers");

      foreach (var attribute in attributes.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
      {
        modifiers.Add (CreateElement ("Text", "["));
        modifiers.Add (CreateElement ("Type", attribute));
        modifiers.Add (CreateElement ("Text", "]"));
      }

      foreach (var keyword in keywords.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        modifiers.Add (CreateElement ("Keyword", keyword));

      return modifiers;
    }

    public void AddParameterMarkup (ParameterInfo[] parameterInfos, XElement signatureElement)
    {
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);
      ArgumentUtility.CheckNotNull ("signatureElement", signatureElement);

      signatureElement.Add (CreateElement ("Text", "("));

      for (int i = 0; i < parameterInfos.Length; i++)
      {
        if (i != 0)
          signatureElement.Add (CreateElement ("Text", ","));

        signatureElement.Add (CreateTypeElement (parameterInfos[i].ParameterType));
        signatureElement.Add (CreateElement ("ParameterName", parameterInfos[i].Name));
      }

      signatureElement.Add (CreateElement ("Text", ")"));
    }

    public XElement CreateConstructorMarkup (string name, ParameterInfo[] parameterInfos)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);

      return CreateMemberMarkup (null, null, name, parameterInfos);
    }

    public XElement CreateMethodMarkup (string methodName, Type returnType, ParameterInfo[] parameterInfos, Type[] genericParameters = null)
    {
      ArgumentUtility.CheckNotNull ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);

      return CreateMemberMarkup (null, returnType, methodName, parameterInfos, genericParameters);
    }

    public XElement CreateEventMarkup (string eventName, Type handlerType)
    {
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("handlerType", handlerType);

      return CreateMemberMarkup ("event", handlerType, eventName, null);
    }

    public XElement CreateFieldMarkup (string fieldName, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("fieldName", fieldName);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return CreateMemberMarkup (null, fieldType, fieldName, null);
    }

    public XElement CreatePropertyMarkup (string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      return CreateMemberMarkup (null, propertyType, propertyName, null);
    }

    public XElement CreateNestedTypeMarkup (Type nestedType)
    {
      if (nestedType.IsEnum)
        return CreateMemberMarkup ("enum", null, nestedType.Name, null);

      string prefix;
      if (nestedType.IsClass)
        prefix = "class";
      else if (nestedType.IsInterface)
        prefix = "interface";
      else if (nestedType.IsValueType)
        prefix = "struct";
      else
        prefix = "unknown";

      var nestedTypeMarkup = CreateMemberMarkup (prefix, null, nestedType.Name, null);

      var inheritance = new List<Type> ();
      if (nestedType.BaseType != null && nestedType.BaseType != typeof (object) && nestedType.BaseType != typeof (ValueType))
        inheritance.Add (nestedType.BaseType);
      inheritance.AddRange (nestedType.GetInterfaces ());

      for (int i = 0; i < inheritance.Count; i++)
      {
        if (i == 0)
          nestedTypeMarkup.Add (CreateElement ("Text", ":"));
        else
          nestedTypeMarkup.Add (CreateElement ("Text", ","));
        nestedTypeMarkup.Add (CreateElement ("Type", GetFullFormattedTypeName (inheritance[i])));
      }

      return nestedTypeMarkup;
    }

    private XElement CreateMemberMarkup (string prefix, Type type, string memberName, ParameterInfo[] parameterInfos, Type[] genericParameters = null)
    {
      var markup = new XElement ("Signature");
      markup.Add (CreateElement ("Keyword", prefix));
      markup.Add (CreateTypeElement (type));

      if (memberName.Contains ("."))
      {
        var parts = memberName.Split ('.');
        var partCount = parts.Length;
        memberName = parts[partCount - 1];
        markup.Add (CreateElement ("ExplicitInterfaceName", parts[partCount - 2]));
        markup.Add (CreateElement ("Text", "."));
      }

      markup.Add (CreateElement ("Name", memberName));

      if (genericParameters != null && genericParameters.Length > 0)
      {
        markup.Add (CreateElement ("Text", "<"));
        var i = 0;
        foreach (var genericParameter in genericParameters)
        {
          if (i++ != 0)
            markup.Add (CreateElement ("Text", ","));
          markup.Add (CreateElement ("GenericMethodParameter", genericParameter.Name));
        }
        markup.Add (CreateElement ("Text", ">"));
      }

      if (parameterInfos != null)
        AddParameterMarkup (parameterInfos, markup);

      return markup;
    }

    private XElement CreateTypeElement (Type type)
    {
      if (type == null)
        return null;

      XElement element;
      if (type.IsGenericParameter)
      {
        element = CreateElement ("Type", type.Name);

        if (type.DeclaringMethod != null)
        {
          var index = type.DeclaringMethod.GetGenericArguments ().Select (t => t.Name).ToList ().IndexOf (type.Name);
          element.Add (new XAttribute ("genericParameter", string.Format ("!!{0}", index)));
        }
        else
        {
          var index = type.DeclaringType.GetGenericArguments ().Select (t => t.Name).ToList ().IndexOf (type.Name);
          element.Add (new XAttribute ("genericParameter", string.Format ("!{0}", index)));
        }
      }
      else
      {
        element = CreateElement ("Type", GetFullFormattedTypeName (type));
      }

      if (type.IsPrimitive || type == typeof (string) || type == typeof (void))
        element.Add (new XAttribute ("languageType", "Keyword"));
      else
        element.Add (new XAttribute ("languageType", "Type"));

      return element;
    }

    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }
  }
}