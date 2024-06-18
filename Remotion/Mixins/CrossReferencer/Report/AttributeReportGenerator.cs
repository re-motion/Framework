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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public AttributeReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IRemotionReflector remotionReflector,
        IOutputFormatter outputFormatter
        )
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement (
          "Attributes",
          from attribute in allAttributes.Keys
          where _attributeIdentifierGenerator.GetIdentifier(attribute, "none") != "none"
          select GenerateAttributeElement (attribute, allAttributes));
    }

    private Dictionary<Type, List<Type>> GetAllAttributes ()
    {
      var allAttributes = new Dictionary<Type, List<Type>>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var attribute in CustomAttributeData.GetCustomAttributes (involvedType.Type))
        {
          //var attributeType = attribute.GetType();
          var attributeType = attribute.Constructor.DeclaringType;

          if (_remotionReflector.IsInfrastructureType (attributeType))
            continue;

          if (!allAttributes.ContainsKey (attributeType))
            allAttributes.Add (attributeType, new List<Type>());

          var values = allAttributes[attributeType];
          if (!values.Contains (involvedType.Type))
            values.Add (involvedType.Type);
        }
      }
      return allAttributes;
    }

    private XElement GenerateAttributeElement (Type attribute, Dictionary<Type, List<Type>> allAttributes)
    {
      return new XElement (
          "Attribute",
          new XAttribute ("id", _attributeIdentifierGenerator.GetIdentifier (attribute)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (attribute.Assembly)),
          new XAttribute ("namespace", attribute.Namespace),
          new XAttribute ("name", _outputFormatter.GetShortFormattedTypeName(attribute)),
          new XElement (
              "AppliedTo",
              from appliedToType in allAttributes[attribute]
              select
                  new XElement (
                  "InvolvedType-Reference",
                  new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (appliedToType)))
              )
          );
    }
  }
}