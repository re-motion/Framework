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
#if NETFRAMEWORK
using System.Runtime.Serialization;
#else
using System.Runtime.CompilerServices;
#endif
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Creates new domain object instances via an instance of <see cref="IPipeline"/>.
  /// </summary>
  [ImplementationFor(typeof(IDomainObjectCreator), Lifetime = LifetimeKind.Singleton)]
  public class DomainObjectCreator : IDomainObjectCreator
  {
    private readonly IPipelineRegistry _pipelineRegistry;

    public DomainObjectCreator (IPipelineRegistry pipelineRegistry)
    {
      ArgumentUtility.CheckNotNull("pipelineRegistry", pipelineRegistry);

      _pipelineRegistry = pipelineRegistry;
    }

    public IPipelineRegistry PipelineRegistry
    {
      get { return _pipelineRegistry; }
    }

    private IPipeline Pipeline
    {
      get { return _pipelineRegistry.DefaultPipeline; }
    }

    public DomainObject CreateObjectReference (IObjectInitializationContext objectInitializationContext, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("objectInitializationContext", objectInitializationContext);
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

      var objectID = objectInitializationContext.ObjectID;
      CheckDomainTypeAndClassDefinition(objectID.ClassDefinition.Type);
      objectID.ClassDefinition.ValidateCurrentMixinConfiguration();

      var concreteType = Pipeline.ReflectionService.GetAssembledType(objectID.ClassDefinition.Type);
#if NETFRAMEWORK
      var instance = (DomainObject)FormatterServices.GetSafeUninitializedObject(concreteType);
#else
      var instance = (DomainObject)RuntimeHelpers.GetUninitializedObject(concreteType);
#endif

      Pipeline.ReflectionService.PrepareExternalUninitializedObject(instance, InitializationSemantics.Construction);

      // These calls are normally performed by DomainObject's ctor
      instance.Initialize(objectID, objectInitializationContext.RootTransaction);
      objectInitializationContext.RegisterObject(instance);

      using (clientTransaction.EnterNonDiscardingScope())
      {
        instance.RaiseReferenceInitializatingEvent();
      }

      return instance;
    }

    public DomainObject CreateNewObject (IObjectInitializationContext objectInitializationContext, ParamList constructorParameters, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull("objectInitializationContext", objectInitializationContext);
      ArgumentUtility.CheckNotNull("constructorParameters", constructorParameters);
      ArgumentUtility.CheckNotNull("clientTransaction", clientTransaction);

      var domainObjectType = objectInitializationContext.ObjectID.ClassDefinition.Type;
      CheckDomainTypeAndClassDefinition(domainObjectType);
      var classDefinition = MappingConfiguration.Current.GetClassDefinition(domainObjectType);
      classDefinition.ValidateCurrentMixinConfiguration();

      using (clientTransaction.EnterNonDiscardingScope())
      {
        using (new ObjectInititalizationContextScope(objectInitializationContext))
        {
          var instance = (DomainObject)Pipeline.Create(domainObjectType, constructorParameters, allowNonPublicConstructor: true);
          DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated(instance);
          return instance;
        }
      }
    }

    private void CheckDomainTypeAndClassDefinition (Type domainObjectType)
    {
      if (domainObjectType.IsSealed)
      {
        var message = string.Format("Cannot instantiate type '{0}' as it is sealed.", domainObjectType.GetFullNameSafe());
        throw new NonInterceptableTypeException(message, domainObjectType);
      }

      var classDefinition = MappingConfiguration.Current.GetClassDefinition(domainObjectType);
      if (classDefinition.IsAbstract)
      {
        var message1 = string.Format(
            "Cannot instantiate type '{0}' as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
            classDefinition.Type.GetFullNameSafe());
        throw new NonInterceptableTypeException(message1, classDefinition.Type);
      }
    }
  }
}
