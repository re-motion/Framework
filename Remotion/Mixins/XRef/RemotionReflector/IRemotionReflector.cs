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
  public interface IRemotionReflector
  {
    IRemotionReflector Initialize (string assemblyDirectory);

    bool IsRelevantAssemblyForConfiguration (Assembly assembly);
    bool IsNonApplicationAssembly (Assembly assembly);
    bool IsConfigurationException (Exception exception);
    bool IsValidationException (Exception exception);
    bool IsInfrastructureType (Type type);
    bool IsInheritedFromMixin (Type type);

    ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext);
    ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies);
    ReflectedObject GetValidationLogFromValidationException (Exception validationException);
    ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition);
    ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition);

    ICollection<Type> GetComposedInterfaces (ReflectedObject classContext);
    void InitializeLogging ();
    ITypeDiscoveryService GetTypeDiscoveryService ();
  }
}
