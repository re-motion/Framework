// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyLoading
{
  /// <summary>
  /// Loads assemblies and applies <see cref="IAssemblyLoaderFilter">filters</see> before returning them.
  /// Assemblies are loaded from a file path by first getting their corresponding <see cref="AssemblyName"/> and then loading the assembly with that 
  /// name.
  /// This means that only assemblies from the assembly search path (application directory, dynamic directory, GAC) can be loaded, and that GAC 
  /// assemblies are preferred. The advantage of this load mode is that assemblies are loaded exactly the same way as if loaded directly by .NET:
  /// they are always loaded into the correct context and references are resolved correctly.
  /// </summary>
  /// <remarks>
  /// The assemblies loaded by this class can be filtered by implementations of <see cref="IAssemblyLoaderFilter"/>.
  /// </remarks>
  public class FilteringAssemblyLoader : IAssemblyLoader
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<FilteringAssemblyLoader>();

    private readonly IAssemblyLoaderFilter _filter;

    public FilteringAssemblyLoader (IAssemblyLoaderFilter filter)
    {
      ArgumentUtility.CheckNotNull("filter", filter);
      _filter = filter;
    }

    public IAssemblyLoaderFilter Filter
    {
      get { return _filter; }
    }

    public virtual Assembly? TryLoadAssembly (string filePath)
    {
      ArgumentUtility.CheckNotNull("filePath", filePath);

      s_logger.LogInformation("Attempting to get assembly name for path '{0}'.", filePath);
      AssemblyName? assemblyName = PerformGuardedLoadOperation(filePath, null, () => AssemblyNameCache.GetAssemblyName(filePath));
      if (assemblyName == null)
        return null;

      s_logger.LogInformation("Assembly name for path '{0}' is '{1}'.", filePath, assemblyName.FullName);

      return TryLoadAssembly(assemblyName, filePath);
    }

    public virtual Assembly? TryLoadAssembly (AssemblyName assemblyName, string context)
    {
      ArgumentUtility.CheckNotNull("assemblyName", assemblyName);
      ArgumentUtility.CheckNotNull("context", context);

      if (PerformGuardedLoadOperation(assemblyName.FullName, context, () => _filter.ShouldConsiderAssembly(assemblyName)))
      {
        s_logger.LogInformation("Attempting to load assembly with name '{0}' in context '{1}'.", assemblyName, context);
        Assembly? loadedAssembly = PerformGuardedLoadOperation(assemblyName.FullName, context, () => Assembly.Load(assemblyName));
        s_logger.LogInformation("Success: {0}", loadedAssembly != null);

        if (loadedAssembly == null)
          return null;
        else if (PerformGuardedLoadOperation(assemblyName.FullName, context, () => _filter.ShouldIncludeAssembly(loadedAssembly)))
          return loadedAssembly;
        else
          return null;
      }
      else
        return null;
    }

    [return: MaybeNull]
    public T PerformGuardedLoadOperation<T> (string assemblyDescription, string? loadContext, Func<T> loadOperation)
    {
      ArgumentUtility.CheckNotNullOrEmpty("assemblyDescription", assemblyDescription);
      ArgumentUtility.CheckNotNull("loadOperation", loadOperation);

      var assemblyDescriptionText = "'" + assemblyDescription + "'";
      if (loadContext != null)
        assemblyDescriptionText += " (loaded in the context of '" + loadContext + "')";

      try
      {
        return loadOperation();
      }
      catch (BadImageFormatException ex)
      {
        s_logger.LogInformation(
            "The file {0} triggered a BadImageFormatException and will be ignored. Possible causes for this are:" + Environment.NewLine
            + "- The file is not a .NET assembly." + Environment.NewLine
            + "- The file was built for a newer version of .NET." + Environment.NewLine
            + "- The file was compiled for a different platform (x86, x64, etc.) than the platform this process is running on." + Environment.NewLine
            + "- The file is damaged.",
            assemblyDescriptionText);
        s_logger.LogDebug(ex, "The file {0} triggered a BadImageFormatException.", assemblyDescriptionText);

        return default(T)!;
      }
      catch (FileLoadException ex)
      {
        s_logger.LogWarning(
            ex,
            "The assembly {0} triggered a FileLoadException and will be ignored - maybe the assembly is DelaySigned, but signing has not been completed?",
            assemblyDescriptionText);
        return default(T)!;
      }
      catch (FileNotFoundException ex)
      {
        string message = string.Format("The assembly {0} triggered a FileNotFoundException - maybe the assembly does not exist or a referenced assembly "
                                        + "is missing?\r\nFileNotFoundException message: {1}", assemblyDescriptionText, ex.Message);

        // This is a workaround for an issue in Windows 8 with .NET 3.5, where System.ServiceModel references a 
        // non-existing System.IdentityModel.Selectors.dll,
        // https://www.re-motion.org/jira/browse/RM-5089
        if (assemblyDescription.Contains("System.IdentityModel.Selectors"))
        {
          s_logger.LogWarning(message, ex);
          return default(T)!;
        }

        throw new AssemblyLoaderException(message, ex);
      }
      catch (Exception ex)
      {
        string message = string.Format("The assembly {0} triggered an unexpected exception of type {1}.\r\nUnexpected exception message: {2}",
                                        assemblyDescriptionText, ex.GetType().GetFullNameSafe(), ex.Message);
        throw new AssemblyLoaderException(message, ex);
      }
    }
  }
}
