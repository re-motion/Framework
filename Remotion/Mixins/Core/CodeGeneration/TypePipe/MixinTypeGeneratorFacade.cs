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
using Remotion.Mixins.Definitions;
using Remotion.ServiceLocation;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Generates concrete mixin types and meta data by calling the methods on <see cref="MixinTypeGenerator"/> in proper order.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IMixinTypeProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class MixinTypeGeneratorFacade : IMixinTypeProvider
  {
    public MixinTypeGeneratorFacade ()
    {
    }

    public IMixinInfo GetMixinInfo (IProxyTypeAssemblyContext context, MixinDefinition mixin)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("mixin", mixin);

      if (!mixin.NeedsDerivedMixinType())
        return new RegularMixinInfo(mixin.Type);

      var concreteMixinTypeIdentifier = mixin.GetConcreteMixinTypeIdentifier();
      return GetOrGenerateConcreteMixinType(context, concreteMixinTypeIdentifier);
    }

    public ConcreteMixinType GetOrGenerateConcreteMixinType (ITypeAssemblyContext context, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);

      var concreteMixinTypeCache = GetOrCreateConcreteMixinTypeCache(context.ParticipantState);

      ConcreteMixinType? concreteMixinType;
      if (!concreteMixinTypeCache.TryGetValue(concreteMixinTypeIdentifier, out concreteMixinType))
      {
        concreteMixinType = GenerateConcreteMixinType(context, concreteMixinTypeIdentifier);

        context.GenerationCompleted += generatedTypeContext =>
        {
          var completedConcreteMixinType = concreteMixinType.SubstituteMutableReflectionObjects(generatedTypeContext);
          concreteMixinTypeCache.Add(concreteMixinTypeIdentifier, completedConcreteMixinType);
        };
      }

      return concreteMixinType;
    }

    private ConcreteMixinType GenerateConcreteMixinType (ITypeAssemblyContext context, ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      var mixinProxyType = context.CreateAddtionalProxyType(concreteMixinTypeIdentifier, concreteMixinTypeIdentifier.MixinType);

      var generator = new MixinTypeGenerator(concreteMixinTypeIdentifier, mixinProxyType, new AttributeGenerator(), context.ParticipantConfigurationID);
      generator.AddInterfaces();
      generator.AddFields();
      generator.AddTypeInitializer();

      generator.AddMixinTypeAttribute();
      generator.AddDebuggerAttributes();

      var overrideInterface = generator.GenerateOverrides();
      var methodWrappers = generator.GenerateMethodWrappers();

      return new ConcreteMixinType(
          concreteMixinTypeIdentifier, mixinProxyType, overrideInterface.Type, overrideInterface.InterfaceMethodsByOverriddenMethods, methodWrappers);
    }

    private IDictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType> GetOrCreateConcreteMixinTypeCache (
        IParticipantState participantState)
    {
      const string key = "ConcreteMixinTypes";
      var concreteMixinTypeCache = (Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>)participantState.GetState(key);
      if (concreteMixinTypeCache == null)
      {
        concreteMixinTypeCache = new Dictionary<ConcreteMixinTypeIdentifier, ConcreteMixinType>();
        participantState.AddState(key, concreteMixinTypeCache);
      }

      return concreteMixinTypeCache;
    }
  }
}
