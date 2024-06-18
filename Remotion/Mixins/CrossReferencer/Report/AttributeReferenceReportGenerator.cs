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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
{
  public class AttributeReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;

    public AttributeReferenceReportGenerator (
        Type type, IIdentifierGenerator<Type> attributeIdentifierGenerator, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _type = type;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflector = remotionReflector;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "HasAttributes",
          from attribute in CustomAttributeData.GetCustomAttributes (_type)
          where !_remotionReflector.IsInfrastructureType (attribute.Constructor.DeclaringType)
          select GenerateAttributeReference (attribute)
          );
    }

    private XElement GenerateAttributeReference (CustomAttributeData attribute)
    {
      var attributeElement = new XElement (
          "HasAttribute", new XAttribute ("ref", _attributeIdentifierGenerator.GetIdentifier (attribute.Constructor.DeclaringType)));

      for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
      {
        var constructorArgument = attribute.ConstructorArguments[i];
        var parameterName = attribute.Constructor.GetParameters()[i].Name;
        attributeElement.Add (GenerateParameterElement ("constructor", constructorArgument.ArgumentType, parameterName, constructorArgument.Value));
      }

      foreach (var namedArgument in attribute.NamedArguments)
      {
        attributeElement.Add (
            GenerateParameterElement ("named", namedArgument.TypedValue.ArgumentType, namedArgument.MemberInfo.Name, namedArgument.TypedValue.Value));
      }

      return attributeElement;
    }

    private XElement GenerateParameterElement (string kind, Type type, string name, object value)
    {
      var demultiplexedValue = RecursiveToString (type, value);

      return new XElement (
          "Argument",
          new XAttribute ("kind", kind),
          new XAttribute ("type", type.Name),
          new XAttribute ("name", name),
          new XAttribute ("value", demultiplexedValue));
    }

    private string RecursiveToString (Type argumentType, object argumentValue)
    {
      if (!argumentType.IsArray)
      {
        if (argumentValue == null)
          return "";
        return argumentValue.ToString();
      }

      var valueCollection = (ReadOnlyCollection<CustomAttributeTypedArgument>) argumentValue;

      var concatenatedValues = new StringBuilder ("{");
      for (int i = 0; i < valueCollection.Count; i++)
      {
        if (i != 0)
          concatenatedValues.Append (", ");
        concatenatedValues.Append (RecursiveToString (valueCollection[i].ArgumentType, valueCollection[i].Value));
      }
      concatenatedValues.Append ("}");

      return concatenatedValues.ToString();
    }
  }
}