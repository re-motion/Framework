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
using Remotion.Mixins.Utilities;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  [ImplementationFor (typeof (IObjectFactoryImplementation), Lifetime = LifetimeKind.Singleton)]
  public class ObjectFactoryImplementation : IObjectFactoryImplementation
  {
    private readonly IPipelineRegistry _pipelineRegistry;

    public ObjectFactoryImplementation (IPipelineRegistry pipelineRegistry)
    {
      ArgumentUtility.CheckNotNull ("pipelineRegistry", pipelineRegistry);
      _pipelineRegistry = pipelineRegistry;
    }

    public object CreateInstance (
        bool allowNonPublicConstructors,
        Type targetOrConcreteType,
        ParamList constructorParameters,
        params object[] preparedMixins)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);
      ArgumentUtility.CheckNotNull ("preparedMixins", preparedMixins);

      if (targetOrConcreteType.IsInterface)
      {
        var message = string.Format ("Cannot instantiate type '{0}', it's an interface.", targetOrConcreteType);
        throw new ArgumentException (message, "targetOrConcreteType");
      }

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);
      if (classContext == null && preparedMixins.Length > 0)
      {
          throw new ArgumentException (string.Format ("There is no mixin configuration for type {0}, so no mixin instances must be specified.",
              targetOrConcreteType.FullName), "preparedMixins");
      }

      using (new MixedObjectInstantiationScope(preparedMixins))
      {
        if (classContext != null && classContext.Type != targetOrConcreteType)
        {
          // The ClassContext doesn't match the requested type, so it must already be a concrete type. Just instantiate it.
          Assertion.DebugAssert (MixinTypeUtility.IsGeneratedConcreteMixedType (targetOrConcreteType));

          var reflectionService = _pipelineRegistry.DefaultPipeline.ReflectionService;
          return reflectionService.InstantiateAssembledType (targetOrConcreteType, constructorParameters, allowNonPublicConstructors);
        }
        return _pipelineRegistry.DefaultPipeline.Create (targetOrConcreteType, constructorParameters, allowNonPublicConstructors);
      }
    }
  }
}
