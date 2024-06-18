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
using System.IO;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.XRef.RemotionReflector;

namespace Remotion.Mixins.XRef
{
  [ReflectorSupport("Remotion", "1.11.20", "Remotion.Interfaces.dll")]
  public class DefaultReflector : RemotionReflectorBase
  {
    private string _assemblyDirectory;
    private Assembly _remotionAssembly;
    private Assembly _remotionInterfaceAssembly;

    private Assembly RemotionAssembly
    {
      get { return _remotionAssembly ?? (_remotionAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(_assemblyDirectory, "Remotion.dll")))); }
    }

    private Assembly RemotionInterfaceAssembly
    {
      get
      {
        return _remotionInterfaceAssembly
               ?? (_remotionInterfaceAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(_assemblyDirectory, "Remotion.Interfaces.dll"))));
      }
    }

    public override IRemotionReflector Initialize (string assemblyDirectory)
    {
      Utilities.ArgumentUtility.CheckNotNull("assemblyDirectory", assemblyDirectory);

      _assemblyDirectory = assemblyDirectory;

      return this;
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return assembly.GetReferencedAssemblies().Any(r => r.FullName == RemotionInterfaceAssembly.GetName().FullName);
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      Utilities.ArgumentUtility.CheckNotNull("assembly", assembly);

      return assembly.GetCustomAttributes(false).Any(attribute => attribute.GetType().Name == "NonApplicationAssemblyAttribute");
    }

    public override bool IsConfigurationException (Exception exception)
    {
      Utilities.ArgumentUtility.CheckNotNull("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public override bool IsValidationException (Exception exception)
    {
      Utilities.ArgumentUtility.CheckNotNull("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public override bool IsInfrastructureType (Type type)
    {
      Utilities.ArgumentUtility.CheckNotNull("type", type);

      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      Utilities.ArgumentUtility.CheckNotNull("type", type);

      var mixinBaseType = RemotionInterfaceAssembly.GetType("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom(type);
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      Utilities.ArgumentUtility.CheckNotNull("targetType", targetType);
      Utilities.ArgumentUtility.CheckNotNull("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = RemotionAssembly.GetType("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod(targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      Utilities.ArgumentUtility.CheckNotNull("assemblies", assemblies);

      var declarativeConfigurationBuilderType = RemotionAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod(declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      Utilities.ArgumentUtility.CheckNotNull("validationException", validationException);

      return new ReflectedObject(new ValidationLogNullObject());
    }

    public override ICollection<Type> GetComposedInterfaces (ReflectedObject classContext)
    {
      return classContext.GetProperty("CompleteInterfaces").To<ICollection<Type>>();
    }
  }
}
