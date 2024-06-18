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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Reflectors
{
  /// <summary>
  ///   restores support for assemblies using re-motion >= 1.15.0 by delegating to the apporopriate reflectors
  /// </summary>
  [ReflectorSupport("Remotion", "1.15.0.0", "Remotion.Mixins.dll")]
  public class Net4_5SupportReflector : RemotionReflectorBase
  {
    private string _assemblyDirectory;
    private Assembly _remotionAssembly;
    private readonly ComposedInterfacesReflector _composedInterfacesReflector;
    private readonly CreateAndValidateReflector _createAndValidateReflector;
    private readonly DefaultReflector _defaultReflector;
    private readonly MixinAssemblyReflector _mixinAssemblyReflector;
    private readonly NewMixinDependenciesReflector _newMixinDependenciesReflector;
    private readonly OldMixinDependenciesReflector _oldMixinDependenciesReflector;
    private readonly TargetClassDefinitionFactoryReflector _targetClassDefinitionFactoryReflector;

    public Net4_5SupportReflector ()
    {
      _defaultReflector = new DefaultReflector();
      _oldMixinDependenciesReflector = new OldMixinDependenciesReflector();
      _targetClassDefinitionFactoryReflector = new TargetClassDefinitionFactoryReflector();
      _newMixinDependenciesReflector = new NewMixinDependenciesReflector();
      _mixinAssemblyReflector = new MixinAssemblyReflector();
      _createAndValidateReflector = new CreateAndValidateReflector();
      _composedInterfacesReflector = new ComposedInterfacesReflector();
    }

    private Assembly RemotionAssembly
    {
      get { return _remotionAssembly ?? (_remotionAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(_assemblyDirectory, "Remotion.dll")))); }
    }

    public override IRemotionReflector Initialize (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull("assemblyDirectory", assemblyDirectory);

      _assemblyDirectory = assemblyDirectory;

      _defaultReflector.Initialize(assemblyDirectory);
      _oldMixinDependenciesReflector.Initialize(assemblyDirectory);
      _targetClassDefinitionFactoryReflector.Initialize(assemblyDirectory);
      _newMixinDependenciesReflector.Initialize(assemblyDirectory);
      _mixinAssemblyReflector.Initialize(assemblyDirectory);
      _createAndValidateReflector.Initialize(assemblyDirectory);
      _composedInterfacesReflector.Initialize(assemblyDirectory);

      return this;
    }

    public override void InitializeLogging ()
    {
      var type = RemotionAssembly.GetType("Remotion.Logging.LogManager", true);
      object logger = ReflectedObject.CallMethod(type, "GetLogger", "LoggerName");
      if (logger == null)
        throw new InvalidOperationException("Failed to initialize log4net.");
    }

    public override ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      var type = RemotionAssembly.GetType("Remotion.Reflection.TypeDiscovery.ContextAwareTypeDiscoveryUtility", true);
      return ReflectedObject.CallMethod(type, "GetTypeDiscoveryService").To<ITypeDiscoveryService>();
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return _mixinAssemblyReflector.IsRelevantAssemblyForConfiguration(assembly);
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      return _targetClassDefinitionFactoryReflector.IsNonApplicationAssembly(assembly);
    }

    public override bool IsConfigurationException (Exception exception)
    {
      return _defaultReflector.IsConfigurationException(exception);
    }

    public override bool IsValidationException (Exception exception)
    {
      return _defaultReflector.IsValidationException(exception);
    }

    public override bool IsInfrastructureType (Type type)
    {
      return _mixinAssemblyReflector.IsInfrastructureType(type);
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      return _mixinAssemblyReflector.IsInheritedFromMixin(type);
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      return _createAndValidateReflector.GetTargetClassDefinition(targetType, mixinConfiguration, classContext);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      return _mixinAssemblyReflector.BuildConfigurationFromAssemblies(assemblies);
    }

    public override ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      return new ReflectedObject(new ValidationLogNullObject());
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return _newMixinDependenciesReflector.GetTargetCallDependencies(mixinDefinition);
    }

    public override ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      return _composedInterfacesReflector.GetComposedInterfaces(classContext);
    }

    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return _newMixinDependenciesReflector.GetNextCallDependencies(mixinDefinition);
    }
  }
}
