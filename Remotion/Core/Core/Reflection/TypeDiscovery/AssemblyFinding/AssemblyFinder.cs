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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Finds assemblies using an <see cref="IRootAssemblyFinder"/> and an <see cref="IAssemblyLoader"/>. The <see cref="IRootAssemblyFinder"/> is
  /// used to find a set of root assemblies, the <see cref="AssemblyFinder"/> automatically traverses the assembly references to (transitively)
  /// find all referenced assemblies as well. The root assemblies and referenced assemblies are loaded with the <see cref="IAssemblyLoader"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class AssemblyFinder : IAssemblyFinder
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<AssemblyFinder>();

    private readonly IRootAssemblyFinder _rootAssemblyFinder;
    private readonly IAssemblyLoader _assemblyLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyFinder"/> class.
    /// </summary>
    /// <param name="rootAssemblyFinder">The <see cref="IRootAssemblyFinder"/> to use for finding the root assemblies.</param>
    /// <param name="assemblyLoader">The <see cref="IAssemblyLoader"/> to use for loading the assemblies found.</param>
    public AssemblyFinder (IRootAssemblyFinder rootAssemblyFinder, IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull("rootAssemblyFinder", rootAssemblyFinder);
      ArgumentUtility.CheckNotNull("assemblyLoader", assemblyLoader);

      _rootAssemblyFinder = rootAssemblyFinder;
      _assemblyLoader = assemblyLoader;
    }

    public IRootAssemblyFinder RootAssemblyFinder
    {
      get { return _rootAssemblyFinder; }
    }

    public IAssemblyLoader AssemblyLoader
    {
      get { return _assemblyLoader; }
    }

    /// <summary>
    /// Uses the <see cref="RootAssemblyFinder"/> to find root assemblies and returns them together with all directly or indirectly referenced 
    /// assemblies. The assemblies are loaded via the <see cref="AssemblyLoader"/>.
    /// </summary>
    /// <returns>The root assemblies and their referenced assemblies.</returns>
    public virtual IEnumerable<Assembly> FindAssemblies ()
    {
      s_logger.LogDebug("Finding assemblies...");
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Information, "Time spent for finding and loading assemblies: {elapsed}."))
      {
        var rootAssemblies = FindRootAssemblies();
        var resultSet = new HashSet<Assembly>(rootAssemblies.Select(root => root.Assembly));
        resultSet.UnionWith(FindReferencedAssemblies(rootAssemblies));

        // Forcing the enumeration at this point does not have a measurable impact on performance.
        // Instead, decoupling the assembly loading from the rest of the system is actually helpful for concurrency.
        return resultSet.LogAndReturnItems(s_logger, LogLevel.Information, count => string.Format("Found {0} assemblies.", count))
            .ToList();
      }
    }

    private ICollection<RootAssembly> FindRootAssemblies ()
    {
      s_logger.LogDebug("Finding root assemblies...");
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Debug, "Time spent for finding and loading root assemblies: {elapsed}."))
      {
        return _rootAssemblyFinder.FindRootAssemblies()
            .LogAndReturnItems(s_logger, LogLevel.Debug, count => string.Format("Found {0} root assemblies.", count))
            .ToList();
      }
    }

    private ICollection<Assembly> FindReferencedAssemblies (ICollection<RootAssembly> rootAssemblies)
    {
      s_logger.LogDebug("Finding referenced assemblies...");
      using (StopwatchScope.CreateScope(s_logger, LogLevel.Debug, "Time spent for finding and loading referenced assemblies: {elapsed}."))
      {
        // referenced assemblies are added later in order to get their references as well
        var referenceRoots = new ConcurrentQueue<Assembly>(
            rootAssemblies.Where(r => r.FollowReferences).Select(r => r.Assembly).Distinct());

        // used to prevent analyzing an assembly twice 
        // and to prevent analysis of root-assemblies marked as do-not-follow references
        var processedAssemblies = new ConcurrentDictionary<Assembly, object?>(
            rootAssemblies.Select(r=>r.Assembly).Distinct().Select(a => new KeyValuePair<Assembly, object?>(a, null)));

        // used to avoid loading assemblies twice.
        var processedAssemblyNames = new HashSet<string>(processedAssemblies.Keys.Select(a => a.GetFullNameChecked()));

        var result = new ConcurrentBag<Assembly>();

        while (referenceRoots.TryDequeue(out var currentRoot)) // Sequential loop because of 'processedAssemblyNames'-HashSet
        {
          var nonprocessedAssemblyNames = currentRoot.GetReferencedAssemblies().Where(a => !processedAssemblyNames.Contains(a.FullName)).ToList();
          processedAssemblyNames.UnionWith(nonprocessedAssemblyNames.Select(a => a.FullName));

          foreach (var referencedAssemblyName in nonprocessedAssemblyNames)
          {
            var referencedAssembly = _assemblyLoader.TryLoadAssembly(referencedAssemblyName, currentRoot.GetFullNameChecked());
            if (referencedAssembly != null) // might return null if filtered by the loader
            {
              if (processedAssemblies.TryAdd(referencedAssembly, null))
              {
                // store as a root in order to process references transitively
                referenceRoots.Enqueue(referencedAssembly);
                result.Add(referencedAssembly);
              }
            }
          }
        }

        return result
            .LogAndReturnItems(s_logger, LogLevel.Debug, count => string.Format("Found {0} referenced assemblies.", count))
            .ToList();
      }
    }
  }
}
