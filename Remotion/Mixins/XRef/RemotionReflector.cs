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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins.XRef;

public class RemotionReflector
{
  private Assembly _assemblyToCheck;
  private Assembly _mixinsAssembly;

  public RemotionReflector (Assembly? assembly = null)
  {
    _assemblyToCheck = assembly ?? Assembly.GetAssembly(typeof(INullObject));
    _mixinsAssembly = Assembly.GetAssembly(typeof(Mixin));
  }

  public bool IsRelevantAssemblyForConfiguration (Assembly assembly)
  {
    return assembly.GetReferencedAssemblies().Any(r => r.FullName == _assemblyToCheck.GetName().FullName);
  }

  public bool IsNonApplicationAssembly (Assembly assembly)
  {
    ArgumentUtility.CheckNotNull("assembly", assembly);

    return assembly.GetCustomAttributes(false).Any(
        attribute => attribute.GetType().FullName == "Remotion.Reflection.TypeDiscovery.NonApplicationAssemblyAttribute");
  }

  public bool IsConfigurationException (Exception exception)
  {
    ArgumentUtility.CheckNotNull("exception", exception);

    return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
  }

  public bool IsValidationException (Exception exception)
  {
    ArgumentUtility.CheckNotNull("exception", exception);

    return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
  }

  public bool IsInfrastructureType (Type type)
  {
    ArgumentUtility.CheckNotNull("type", type);

    return type.Assembly.GetName().Name == "Remotion.Mixins";
  }

  public bool IsInheritedFromMixin (Type type)
  {
    ArgumentUtility.CheckNotNull("type", type);

    var mixinBaseType = typeof(IInitializableMixin);
    return mixinBaseType!.IsAssignableFrom(type);
  }

  public TargetClassDefinition GetTargetClassDefinition (Type targetType, MixinConfiguration mixinConfiguration, ClassContext classContext)
  {
    ArgumentUtility.CheckNotNull("targetType", targetType);
    ArgumentUtility.CheckNotNull("mixinConfiguration", mixinConfiguration);

    var targetClassDefinitionFactoryType = _mixinsAssembly.GetType("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
    return (TargetClassDefinition)ReflectedObject.CallMethod(targetClassDefinitionFactoryType!, "CreateAndValidate", classContext).WrappedObject;
  }

  public MixinConfiguration BuildConfigurationFromAssemblies (Assembly[] assemblies)
  {
    ArgumentUtility.CheckNotNull("assemblies", assemblies);

    var declarativeConfigurationBuilderType = _mixinsAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
    return (MixinConfiguration)ReflectedObject.CallMethod(declarativeConfigurationBuilderType!, "BuildConfigurationFromAssemblies", new object[] { assemblies }).WrappedObject;
  }

  public SerializableValidationLogData GetValidationLogFromValidationException (ValidationException validationException)
  {
    return validationException.ValidationLogData;
  }

  public UniqueDefinitionCollection<Type,NextCallDependencyDefinition> GetNextCallDependencies (MixinDefinition mixinDefinition)
  {
    ArgumentUtility.CheckNotNull("mixinDefinition", mixinDefinition);

    return mixinDefinition.NextCallDependencies;
  }

  public UniqueDefinitionCollection<Type,TargetCallDependencyDefinition> GetTargetCallDependencies (MixinDefinition mixinDefinition)
  {
    ArgumentUtility.CheckNotNull("mixinDefinition", mixinDefinition);

    return mixinDefinition.TargetCallDependencies;
  }

  public IReadOnlyCollection<Type> GetComposedInterfaces (ClassContext classContext)
  {
    return classContext.ComposedInterfaces;
  }

  public ITypeDiscoveryService GetTypeDiscoveryService ()
  {
    var type = _assemblyToCheck.GetType("Remotion.Reflection.TypeDiscovery.ContextAwareTypeDiscoveryUtility", true);
    return ReflectedObject.CallMethod(type!, "GetTypeDiscoveryService").To<ITypeDiscoveryService>();
  }
}
