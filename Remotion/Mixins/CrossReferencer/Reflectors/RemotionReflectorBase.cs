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
  public abstract class RemotionReflectorBase : IRemotionReflector
  {
    public virtual IRemotionReflector Initialize (string assemblyDirectory)
    {
      return this;
    }

    public virtual void InitializeLogging ()
    {
      throw new NotSupportedException();
    }

    public virtual ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      throw new NotSupportedException();
    }

    public virtual bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsNonApplicationAssembly (Assembly assembly)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsConfigurationException (Exception exception)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsValidationException (Exception exception)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsInfrastructureType (Type type)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsInheritedFromMixin (Type type)
    {
      throw new NotSupportedException();
    }

    public virtual ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      throw new NotSupportedException();
    }

    public virtual ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      throw new NotSupportedException();
    }

    public virtual ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      throw new NotSupportedException();
    }

    public virtual ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      throw new NotSupportedException();
    }

    public virtual ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      throw new NotSupportedException();
    }

    public virtual ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      throw new NotSupportedException();
    }
  }
}
