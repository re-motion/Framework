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
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;
using IRemotionReflector = MixinXRef.Reflection.RemotionReflector.IRemotionReflector;

namespace MixinXRef.Report
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
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

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

      return new XElement (
          "Mixins",
          from mixin in _involvedType.ClassContext.GetProperty ("Mixins")
          select GenerateMixinElement (mixin));
    }

    private XElement GenerateMixinElement (ReflectedObject mixinContext)
    {
      var mixinType = mixinContext.GetProperty ("MixinType").To<Type>();

      var mixinElement = new XElement (
          "Mixin",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (mixinType)),
          new XAttribute ("index", "n/a"),
          new XAttribute ("relation", GetRelationName(mixinContext)),
          // property MixinType on mixinContext always return the generic type definition, not the type of the actual instance
          new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (mixinType)),
          new XAttribute ("introduced-member-visibility", mixinContext.GetProperty ("IntroducedMemberVisibility").ToString().ToLower()),
          new AdditionalDependencyReportGenerator (
              mixinContext.GetProperty ("ExplicitDependencies"), _involvedTypeIdentifierGenerator, _outputFormatter).GenerateXml()
          );

      if (_involvedType.HasTargetClassDefintion)
      {
        var mixinDefinition = _involvedType.TargetClassDefinition.CallMethod (
            "GetMixinByConfiguredType", mixinContext.GetProperty ("MixinType").To<Type>());

        // set more specific name for mixin references
        mixinElement.SetAttributeValue ("instance-name", _outputFormatter.GetShortFormattedTypeName (mixinDefinition.GetProperty ("Type").To<Type>()));
        // set mixin index
        mixinElement.SetAttributeValue ("index", mixinDefinition.GetProperty ("MixinIndex").To<int>());

        mixinElement.Add (
            new InterfaceIntroductionReportGenerator (mixinDefinition.GetProperty ("InterfaceIntroductions"), _interfaceIdentifierGenerator).
                GenerateXml());
        mixinElement.Add (
            new AttributeIntroductionReportGenerator (
                mixinDefinition.GetProperty ("AttributeIntroductions"), _attributeIdentifierGenerator, _remotionReflector).GenerateXml());
        mixinElement.Add (
            new MemberOverrideReportGenerator (mixinDefinition.CallMethod ("GetAllOverrides")).GenerateXml());
        mixinElement.Add (new TargetCallDependenciesReportGenerator (mixinDefinition, _assemblyIdentifierGenerator, _remotionReflector, _outputFormatter).GenerateXml ());
      }

      return mixinElement;
    }

    private string GetRelationName (ReflectedObject mixinContext)
    {
      if (mixinContext.GetProperty ("MixinKind").ToString ().Equals ("Extending"))
        return "Extends";
      return "Used by";
    }
  }
}