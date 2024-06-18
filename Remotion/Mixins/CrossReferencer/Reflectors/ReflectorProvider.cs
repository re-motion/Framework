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
using System.Runtime.InteropServices;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.Reflectors
{
  public abstract class ReflectorProvider
  {
    private readonly string _component;
    private readonly Version _version;
    private readonly string _assemblyDirectory;
    private readonly IReadOnlyCollection<Type> _reflectorTypes;
    private readonly IDictionary<MethodBase, IRemotionReflector> _reflectorInstances = new Dictionary<MethodBase, IRemotionReflector>();

    protected ReflectorProvider (string component, Version version, IEnumerable<_Assembly> assemblies, string assemblyDirectory)
    {
      _component = component;
      _version = version;
      _assemblyDirectory = assemblyDirectory;

      _reflectorTypes = assemblies.SelectMany(a => a.GetExportedTypes()).Where(IsValidReflector).ToArray();

      if (!_reflectorTypes.Any())
        throw new ArgumentException("There are no valid reflectors in the given assemblies", "assemblies");

      CheckAssemblyRequirements(
          _reflectorTypes.OrderByDescending(t => t.GetAttribute<ReflectorSupportAttribute>().MinVersion).First(),
          assemblyDirectory);
    }

    private void CheckAssemblyRequirements (Type reflectorType, string assemblyDirectory)
    {
      foreach (var requiredAssembly in reflectorType.GetAttribute<ReflectorSupportAttribute>().RequiredAssemblies)
        if (!File.Exists(Path.Combine(assemblyDirectory, requiredAssembly)))
          throw new MissingRequirementException(requiredAssembly);
    }

    protected IRemotionReflector GetCompatibleReflector (MethodBase methodBase)
    {
      IRemotionReflector reflector;
      if (!_reflectorInstances.TryGetValue(methodBase, out reflector))
        _reflectorInstances.Add(methodBase, reflector = FindCompatibleReflector(methodBase));

      return reflector;
    }

    private bool IsValidReflector (Type type)
    {
      var attribute = type.GetAttribute<ReflectorSupportAttribute>();
      return attribute != null &&
             typeof(IRemotionReflector).IsAssignableFrom(type) &&
             attribute.Component == _component &&
             _version >= attribute.MinVersion;
    }

    private IRemotionReflector FindCompatibleReflector (MethodBase methodBase)
    {
      var parameterTypes = methodBase.GetParameters().Select(p => p.ParameterType).ToArray();
      var methods = _reflectorTypes
          .Select(
              t =>
                  t.GetMethod(methodBase.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, null, parameterTypes, null))
          .Where(m => m != null)
          .OrderByDescending(m => m.DeclaringType.GetAttribute<ReflectorSupportAttribute>().MinVersion).ToArray();

      if (!methods.Any())
        throw new NotSupportedException(
            string.Format(
                "There is no reflector that supports {0} in version {1} for {2}",
                methodBase,
                _version,
                _component));

      if (methods.Length > 1 &&
          methods[0].DeclaringType.GetAttribute<ReflectorSupportAttribute>().MinVersion ==
          methods[1].DeclaringType.GetAttribute<ReflectorSupportAttribute>().MinVersion)
        throw new AmbiguousMatchException(
            string.Format(
                "There are two or more implementations of {0} with MinVersion {1}",
                methods[0],
                methods[0].DeclaringType.GetAttribute<ReflectorSupportAttribute>().MinVersion));

      var reflector = methods[0].DeclaringType;
      return CreateInstanceOf(reflector, _assemblyDirectory);
    }

    private static IRemotionReflector CreateInstanceOf (Type type, string assemblyDirectory)
    {
      return ((IRemotionReflector)Activator.CreateInstance(type)).Initialize(assemblyDirectory);
    }
  }
}
