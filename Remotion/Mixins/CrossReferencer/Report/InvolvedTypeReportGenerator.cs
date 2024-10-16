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
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;

    // MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<MemberInfo> _memberIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    private readonly SummaryPicker _summaryPicker = new();
    private readonly TypeModifierUtility _typeModifierUtility = new();

    public InvolvedTypeReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<MemberInfo> memberIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("memberIdentifierGenerator", memberIdentifierGenerator);
      ArgumentUtility.CheckNotNull("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _memberIdentifierGenerator = memberIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement("InvolvedTypes");
      foreach (var involvedType in _involvedTypes)
        involvedTypesElement.Add(CreateInvolvedTypeElement(involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (InvolvedType involvedType)
    {
      var realType = involvedType.Type;

      var element = new XElement(
          "InvolvedType",
          new XAttribute("id", _involvedTypeIdentifierGenerator.GetIdentifier(realType)),
          new XAttribute("metadataToken", realType.MetadataToken),
          new XAttribute("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier(realType.Assembly)),
          new XAttribute("namespace", realType.Namespace ?? ""),
          new XAttribute("name", _outputFormatter.GetShortFormattedTypeName(realType)),
          new XAttribute("base", GetCSharpLikeNameForBaseType(realType)),
          new XAttribute("base-ref", GetBaseReference(realType)),
          new XAttribute("is-target", involvedType.IsTarget),
          new XAttribute("is-mixin", involvedType.IsMixin),
          new XAttribute(
              "is-unusedmixin",
              !involvedType.IsTarget && !involvedType.IsMixin &&
              ReflectionUtility.IsMixinType(involvedType.Type) &&
              !CrossReferencerReflectionUtility.IsInfrastructureType(involvedType.Type)),
          new XAttribute("is-generic-definition", realType.IsGenericTypeDefinition),
          new XAttribute("is-interface", realType.IsInterface),
          _outputFormatter.CreateModifierMarkup(
              GetAlphabeticOrderingAttribute(involvedType),
              _typeModifierUtility.GetTypeModifiers(realType)),
          _summaryPicker.GetSummary(realType),
          new MemberReportGenerator(
              realType,
              involvedType,
              _involvedTypeIdentifierGenerator,
              _memberIdentifierGenerator,
              _outputFormatter).GenerateXml(),
          new InterfaceReferenceReportGenerator(
              involvedType,
              _interfaceIdentifierGenerator).GenerateXml(),
          new AttributeReferenceReportGenerator(
              realType,
              _attributeIdentifierGenerator).GenerateXml(),
          new MixinReferenceReportGenerator(
              involvedType,
              _assemblyIdentifierGenerator,
              _involvedTypeIdentifierGenerator,
              _interfaceIdentifierGenerator,
              _attributeIdentifierGenerator,
              _outputFormatter).GenerateXml(),
          new TargetReferenceReportGenerator(
              involvedType,
              _involvedTypeIdentifierGenerator).GenerateXml()
      );

      if (realType.IsGenericType && !realType.IsGenericTypeDefinition)
        element.Add(
            new XAttribute(
                "generic-definition-ref",
                _involvedTypeIdentifierGenerator.GetIdentifier(realType.GetGenericTypeDefinition())));

      return element;
    }

    public string GetAlphabeticOrderingAttribute (InvolvedType involvedType)
    {
      ArgumentUtility.CheckNotNull("involvedType", involvedType);

      foreach (var mixinDefinition in involvedType.TargetTypes.Values)
      {
        if (mixinDefinition == null)
          continue;

        if (mixinDefinition.AcceptsAlphabeticOrdering)
          return "AcceptsAlphabeticOrdering ";
      }

      return "";
    }

    private string GetCSharpLikeNameForBaseType (Type type)
    {
      return type.BaseType == null ? "none" : _outputFormatter.GetShortFormattedTypeName(type.BaseType);
    }

    private string GetBaseReference (Type realType)
    {
      // System.Object
      if (realType.BaseType == null)
        return "none";

      var baseType = realType.BaseType;
      // get type definition if base is a generic type
      if (baseType.IsGenericType)
        baseType = baseType.GetGenericTypeDefinition();

      return _involvedTypeIdentifierGenerator.GetIdentifier(baseType);
    }
  }
}
