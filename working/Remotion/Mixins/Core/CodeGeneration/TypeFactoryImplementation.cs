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
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [ImplementationFor (typeof (ITypeFactoryImplementation), Lifetime = LifetimeKind.Singleton)]
  public class TypeFactoryImplementation : ITypeFactoryImplementation
  {
    private readonly IPipelineRegistry _pipelineRegistry;

    public TypeFactoryImplementation (IPipelineRegistry pipelineRegistry)
    {
      ArgumentUtility.CheckNotNull ("pipelineRegistry", pipelineRegistry);

      _pipelineRegistry = pipelineRegistry;
    }

    public Type GetConcreteType (Type targetOrConcreteType)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);
      if (classContext == null)
        return targetOrConcreteType;

      if (classContext.Type != targetOrConcreteType)
      {
        // The ClassContext doesn't match the requested type, so it must already be a concrete type. Just return it.
        Assertion.DebugAssert (MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType));
        return targetOrConcreteType;
      }

      return _pipelineRegistry.DefaultPipeline.ReflectionService.GetAssembledType (targetOrConcreteType);
    }

    public void InitializeUnconstructedInstance (object mixinTarget, InitializationSemantics initializationSemantics)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ArgumentUtility.CheckType<IMixinTarget> ("mixinTarget", mixinTarget);

      _pipelineRegistry.DefaultPipeline.ReflectionService.PrepareExternalUninitializedObject (mixinTarget, initializationSemantics);
    }
  }
}
