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
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public AttributeReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IOutputFormatter outputFormatter
    )
    {
      ArgumentUtility.CheckNotNull("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement(
          "Attributes",
          from attribute in allAttributes.Keys
          where _attributeIdentifierGenerator.GetIdentifier(attribute, "none") != "none"
          select GenerateAttributeElement(attribute, allAttributes));
    }

    private Dictionary<Type, List<Type>> GetAllAttributes ()
    {
      var allAttributes = new Dictionary<Type, List<Type>>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var attribute in CustomAttributeData.GetCustomAttributes(involvedType.Type))
        {
          //var attributeType = attribute.GetType();
          var attributeType = Assertion.IsNotNull(attribute.Constructor.DeclaringType, "attribute.Constructor.DeclaringType != null");

          if (CrossReferencerReflectionUtility.IsInfrastructureType(attributeType))
            continue;

          if (!allAttributes.ContainsKey(attributeType))
            allAttributes.Add(attributeType, new List<Type>());

          var values = allAttributes[attributeType];
          if (!values.Contains(involvedType.Type))
            values.Add(involvedType.Type);
        }
      }

      return allAttributes;
    }

    private XElement GenerateAttributeElement (Type attribute, Dictionary<Type, List<Type>> allAttributes)
    {
      return new XElement(
          "Attribute",
          new XAttribute("id", _attributeIdentifierGenerator.GetIdentifier(attribute)),
          new XAttribute("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier(attribute.Assembly)),
          new XAttribute("namespace", attribute.Namespace ?? ""),
          new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(attribute)),
          new XElement(
              "AppliedTo",
              from appliedToType in allAttributes[attribute]
              select
                  new XElement(
                      "InvolvedType-Reference",
                      new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(appliedToType)))
          )
      );
    }
  }
}
