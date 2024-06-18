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
  [ReflectorSupport("Remotion", "1.13.141", "Remotion.Mixins.dll")]
  public class MixinAssemblyReflector : RemotionReflectorBase
  {
    private string _assemblyDirectory;
    private Assembly _mixinsAssembly;

    private Assembly RemotionAssembly
    {
      get { return _mixinsAssembly ?? (_mixinsAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(_assemblyDirectory, "Remotion.Mixins.dll")))); }
    }

    public override IRemotionReflector Initialize (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull("assemblyDirectory", assemblyDirectory);

      _assemblyDirectory = assemblyDirectory;

      return this;
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return assembly.GetReferencedAssemblies().Any(r => r.FullName == RemotionAssembly.GetName().FullName);
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return type.Assembly.GetName().Name == "Remotion.Mixins";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var mixinBaseType = RemotionAssembly.GetType("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom(type);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull("assemblies", assemblies);

      var declarativeConfigurationBuilderType = RemotionAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod(declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull("targetType", targetType);
      ArgumentUtility.CheckNotNull("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = RemotionAssembly.GetType("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
      return ReflectedObject.CallMethod(targetClassDefinitionFactoryType, "CreateTargetClassDefinition", classContext);
    }
  }
}
