// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class AttributeReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeReferenceReportGenerator (
        Type type,
        IIdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull("type", type);
      ArgumentUtility.CheckNotNull("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _type = type;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement(
          "HasAttributes",
          from attribute in CustomAttributeData.GetCustomAttributes(_type)
          where !CrossReferencerReflectionUtility.IsInfrastructureType(Assertion.IsNotNull(attribute.Constructor.DeclaringType, "attribute.Constructor.DeclaringType != null"))
          select GenerateAttributeReference(attribute)
      );
    }

    private XElement GenerateAttributeReference (CustomAttributeData attribute)
    {
      var attributeElement = new XElement(
          "HasAttribute",
          new XAttribute(
              "ref",
              _attributeIdentifierGenerator.GetIdentifier(Assertion.IsNotNull(attribute.Constructor.DeclaringType, "attribute.Constructor.DeclaringType != null"))));

      for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
      {
        var constructorArgument = attribute.ConstructorArguments[i];
        var parameterName = attribute.Constructor.GetParameters()[i].Name ?? $"P_{i}";
        attributeElement.Add(GenerateParameterElement("constructor", constructorArgument.ArgumentType, parameterName, constructorArgument.Value));
      }

      foreach (var namedArgument in attribute.NamedArguments)
      {
        attributeElement.Add(
            GenerateParameterElement("named", namedArgument.TypedValue.ArgumentType, namedArgument.MemberInfo.Name, namedArgument.TypedValue.Value));
      }

      return attributeElement;
    }

    private XElement GenerateParameterElement (string kind, Type type, string name, object? value)
    {
      var demultiplexedValue = RecursiveToString(type, value);

      return new XElement(
          "Argument",
          new XAttribute("kind", kind),
          new XAttribute("type", type.Name),
          new XAttribute("name", name),
          new XAttribute("value", demultiplexedValue));
    }

    private string RecursiveToString (Type argumentType, object? argumentValue)
    {
      if (!argumentType.IsArray)
      {
        if (argumentValue == null)
          return "";
        return argumentValue.ToString() ?? "";
      }

      var valueCollection = (IReadOnlyList<CustomAttributeTypedArgument>?)argumentValue ?? Array.Empty<CustomAttributeTypedArgument>();

      var concatenatedValues = new StringBuilder("{");
      for (int i = 0; i < valueCollection.Count; i++)
      {
        if (i != 0)
          concatenatedValues.Append(", ");
        concatenatedValues.Append(RecursiveToString(valueCollection[i].ArgumentType, valueCollection[i].Value));
      }

      concatenatedValues.Append("}");

      return concatenatedValues.ToString();
    }
  }
}
