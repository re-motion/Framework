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
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Reflectors
{
  [ReflectorSupport("Remotion", "1.13.23")]
  public class TargetClassDefinitionFactoryReflector : RemotionReflectorBase
  {
    private string _assemblyDirectory;
    private Assembly _remotionAssembly;

    private Assembly RemotionAssembly
    {
      get { return _remotionAssembly ?? (_remotionAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(_assemblyDirectory, "Remotion.dll")))); }
    }

    public override IRemotionReflector Initialize (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull("assemblyDirectory", assemblyDirectory);

      _assemblyDirectory = assemblyDirectory;

      return this;
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull("targetType", targetType);
      ArgumentUtility.CheckNotNull("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = RemotionAssembly.GetType("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
      return ReflectedObject.CallMethod(targetClassDefinitionFactoryType, "CreateTargetClassDefinition", classContext);
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("assembly", assembly);

      return assembly.GetCustomAttributes(false).Any(
          attribute => attribute.GetType().FullName == "Remotion.Reflection.TypeDiscovery.NonApplicationAssemblyAttribute");
    }
  }
}
