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
using System.Reflection;
using Remotion.Mixins.XRef.RemotionReflector;

namespace Remotion.Mixins.XRef
{
  [ReflectorSupport("Remotion", "1.13.186.0", "Remotion.Mixins.dll")]
  public class Net2_0SupportReflector : RemotionReflectorBase
  {
    public Net2_0SupportReflector ()
    {
    }

    private NotSupportedException CreateException ()
    {
      return new NotSupportedException("Re-motion versions between 1.13.186.0 and 1.15.0.0 are not supported by the .net 4.5 version of XRef.");
    }

    public override IRemotionReflector Initialize (string assemblyDirectory)
    {
      return this;
    }

    public override void InitializeLogging ()
    {
      throw CreateException();
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      throw CreateException();
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      throw CreateException();
    }

    public override bool IsConfigurationException (Exception exception)
    {
      throw CreateException();
    }

    public override bool IsValidationException (Exception exception)
    {
      throw CreateException();
    }

    public override bool IsInfrastructureType (Type type)
    {
      throw CreateException();
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      throw CreateException();
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      throw CreateException();
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      throw CreateException();
    }

    public override ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      throw CreateException();
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      throw CreateException();
    }

    public override ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      throw CreateException();
    }

    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      throw CreateException();
    }
  }
}
