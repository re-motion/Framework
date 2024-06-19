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
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins.Context;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Report
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public MixinReferenceReportGenerator (
        InvolvedType involvedType,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IRemotionReflector remotionReflector,
        IOutputFormatter outputFormatter
    )
    {
      ArgumentUtility.CheckNotNull("involvedType", involvedType);
      ArgumentUtility.CheckNotNull("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _involvedType = involvedType;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      if (!_involvedType.IsTarget)
        return null;

      return new XElement(
          "Mixins",
          from mixin in _involvedType.ClassContext.Mixins
          select GenerateMixinElement(mixin));
    }

    private XElement GenerateMixinElement (MixinContext mixinContext)
    {
      var mixinType = mixinContext.MixinType;

      var mixinElement = new XElement(
          "Mixin",
          new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(mixinType)),
          new XAttribute("index", "n/a"),
          new XAttribute("relation", GetRelationName(mixinContext)),
          // property MixinType on mixinContext always return the generic type definition, not the type of the actual instance
          new XAttribute("instance-name", _outputFormatter.GetShortFormattedTypeName(mixinType)),
          new XAttribute("introduced-member-visibility", mixinContext.IntroducedMemberVisibility.ToString().ToLower()),
          new AdditionalDependencyReportGenerator(
              mixinContext.ExplicitDependencies,
              _involvedTypeIdentifierGenerator,
              _outputFormatter).GenerateXml()
      );

      if (_involvedType.HasTargetClassDefintion)
      {
        var mixinDefinition = _involvedType.TargetClassDefinition.GetMixinByConfiguredType(mixinType);

        // set more specific name for mixin references
        mixinElement.SetAttributeValue("instance-name", _outputFormatter.GetShortFormattedTypeName(mixinDefinition.Type));
        // set mixin index
        mixinElement.SetAttributeValue("index", mixinDefinition.MixinIndex);

        mixinElement.Add(
            new InterfaceIntroductionReportGenerator(mixinDefinition.InterfaceIntroductions, _interfaceIdentifierGenerator).GenerateXml());
        mixinElement.Add(
            new AttributeIntroductionReportGenerator(
                mixinDefinition.AttributeIntroductions,
                _attributeIdentifierGenerator,
                _remotionReflector).GenerateXml());
        mixinElement.Add(
            new MemberOverrideReportGenerator(mixinDefinition.GetAllOverrides()).GenerateXml());
        mixinElement.Add(new TargetCallDependenciesReportGenerator(mixinDefinition, _assemblyIdentifierGenerator, _remotionReflector, _outputFormatter).GenerateXml());
      }

      return mixinElement;
    }

    private string GetRelationName (MixinContext mixinContext)
    {
      if (mixinContext.MixinKind.ToString().Equals("Extending"))
        return "Extends";
      return "Used by";
    }
  }
}
