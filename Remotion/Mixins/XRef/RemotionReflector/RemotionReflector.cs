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
using System.Reflection;

namespace Remotion.Mixins.XRef.RemotionReflector
{
  public class RemotionReflector : ReflectorProvider, IRemotionReflector
  {
    public RemotionReflector (string component, Version version, IEnumerable<_Assembly> assemblies, string assemblyDirectory)
        : base(component, version, assemblies, assemblyDirectory)
    {
    }

    public IRemotionReflector Initialize (string assemblyDirectory)
    {
      return this;
    }

    public void InitializeLogging ()
    {
      GetCompatibleReflector(MethodBase.GetCurrentMethod()).InitializeLogging();
    }

    public ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetTypeDiscoveryService();
    }

    public bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsRelevantAssemblyForConfiguration(assembly);
    }

    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsNonApplicationAssembly(assembly);
    }

    public bool IsConfigurationException (Exception exception)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsConfigurationException(exception);
    }

    public bool IsValidationException (Exception exception)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsValidationException(exception);
    }

    public bool IsInfrastructureType (Type type)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsInfrastructureType(type);
    }

    public bool IsInheritedFromMixin (Type type)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).IsInheritedFromMixin(type);
    }

    public ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetTargetClassDefinition(targetType, mixinConfiguration, classContext);
    }

    public ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).BuildConfigurationFromAssemblies(assemblies);
    }

    public ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetValidationLogFromValidationException(validationException);
    }

    public ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetTargetCallDependencies(mixinDefinition);
    }

    public ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetComposedInterfaces(classContext);
    }

    public ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return GetCompatibleReflector(MethodBase.GetCurrentMethod()).GetNextCallDependencies(mixinDefinition);
    }
  }
}
