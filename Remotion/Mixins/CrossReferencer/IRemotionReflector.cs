using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.CrossReferencer;

public interface IRemotionReflector
{
  bool IsRelevantAssemblyForConfiguration (Assembly assembly);
  bool IsNonApplicationAssembly (Assembly assembly);
  bool IsConfigurationException (Exception exception);
  bool IsValidationException (Exception exception);
  bool IsInfrastructureType (Type type);
  bool IsInheritedFromMixin (Type type);
  TargetClassDefinition GetTargetClassDefinition (Type targetType, MixinConfiguration mixinConfiguration, ClassContext classContext);
  MixinConfiguration BuildConfigurationFromAssemblies (Assembly[] assemblies);
  SerializableValidationLogData GetValidationLogFromValidationException (ValidationException validationException);
  UniqueDefinitionCollection<Type,NextCallDependencyDefinition> GetNextCallDependencies (MixinDefinition mixinDefinition);
  UniqueDefinitionCollection<Type,TargetCallDependencyDefinition> GetTargetCallDependencies (MixinDefinition mixinDefinition);
  IReadOnlyCollection<Type> GetComposedInterfaces (ClassContext classContext);
  ITypeDiscoveryService GetTypeDiscoveryService ();
}
