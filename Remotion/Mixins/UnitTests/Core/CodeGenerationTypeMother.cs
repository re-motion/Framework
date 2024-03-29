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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core
{
  /// <summary>
  /// Contains utility methods to retrieve the types generated by the mixin engine.
  /// </summary>
  public static class CodeGenerationTypeMother
  {
    public static Type GetGeneratedMixinTypeInActiveConfiguration (Type targetType, Type mixinType)
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext(targetType);
      return GetGeneratedMixinType(requestingClass, mixinType);
    }

    public static Type GetGeneratedMixinType (Type targetType, Type mixinType)
    {
      var requestingClass = ClassContextObjectMother.Create(targetType, mixinType);
      return GetGeneratedMixinType(requestingClass, mixinType);
    }

    public static Type GetGeneratedMixinType (ClassContext requestingClass, Type mixinType)
    {
      ConcreteMixinType concreteMixinType = GetGeneratedMixinTypeAndMetadata(requestingClass, mixinType);
      return concreteMixinType.GeneratedType;
    }

    public static ConcreteMixinType GetGeneratedMixinTypeAndMetadata (Type targetType, Type mixinType)
    {
      var requestingClass = ClassContextObjectMother.Create(targetType, mixinType);
      return GetGeneratedMixinTypeAndMetadata(requestingClass, mixinType);
    }

    private static ConcreteMixinType GetGeneratedMixinTypeAndMetadata (ClassContext requestingClass, Type mixinType)
    {
      MixinDefinition mixinDefinition = TargetClassDefinitionFactory
          .CreateAndValidate(requestingClass)
          .GetMixinByConfiguredType(mixinType);
      Assert.That(mixinDefinition, Is.Not.Null);

      var mixinTypeIdentifier = mixinDefinition.GetConcreteMixinTypeIdentifier();

      var pipeline = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().DefaultPipeline;
      var generatedMixinType = pipeline.ReflectionService.GetAdditionalType(mixinTypeIdentifier);
      return new AttributeBasedMetadataImporter().GetMetadataForMixinType(generatedMixinType);
    }
  }
}
