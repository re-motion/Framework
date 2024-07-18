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
using System.Collections.Generic;
using System.Linq;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Finds the root assemblies by looking up and loading all DLL and EXE files in the assembly search path.
  /// </summary>
  public class SearchPathRootAssemblyFinder : IRootAssemblyFinder
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPathRootAssemblyFinder"/> type to look for assemblies within the current
    /// <see cref="AppDomain"/>'s <see cref="AppDomain.BaseDirectory"/> as well as its <see cref="AppDomain.RelativeSearchPath"/>
    /// (and, optionally, dynamic directory).
    /// </summary>
    /// <param name="considerDynamicDirectory">Specifies whether to search the <see cref="AppDomain.DynamicDirectory"/> as well as the base
    /// directory.</param>
    /// <param name="assemblyLoader">
    /// The <see cref="IAssemblyLoader"/> to use for loading the root assemblies. This object determines any filtering made on the assemblies
    /// to be loaded.
    /// </param>
    /// <returns>An instance of the <see cref="SearchPathRootAssemblyFinder"/> type looking for assemblies within the current
    /// <see cref="AppDomain"/>'s <see cref="AppDomain.BaseDirectory"/> as well as its <see cref="AppDomain.RelativeSearchPath"/>.
    /// </returns>
    public static SearchPathRootAssemblyFinder CreateForCurrentAppDomain (bool considerDynamicDirectory, IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull("assemblyLoader", assemblyLoader);

      string? relativeSearchPath = null;
      string? dynamicDirectory = null;

      var searchPathRootAssemblyFinder = new SearchPathRootAssemblyFinder(
          AppContext.BaseDirectory,
          relativeSearchPath,
          considerDynamicDirectory,
          dynamicDirectory,
          assemblyLoader);
      return searchPathRootAssemblyFinder;
    }

    private readonly string _baseDirectory;
    private readonly string? _relativeSearchPath;
    private readonly bool _considerDynamicDirectory;
    private readonly string? _dynamicDirectory;
    private readonly IAssemblyLoader _assemblyLoader;

    public SearchPathRootAssemblyFinder (
        string baseDirectory,
        string? relativeSearchPath,
        bool considerDynamicDirectory,
        string? dynamicDirectory,
        IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNullOrEmpty("baseDirectory", baseDirectory);
      ArgumentUtility.CheckNotNull("assemblyLoader", assemblyLoader);

      _baseDirectory = baseDirectory;
      _relativeSearchPath = relativeSearchPath;
      _considerDynamicDirectory = considerDynamicDirectory;
      _dynamicDirectory = dynamicDirectory;
      _assemblyLoader = assemblyLoader;
    }

    public string BaseDirectory
    {
      get { return _baseDirectory; }
    }

    public string? RelativeSearchPath
    {
      get { return _relativeSearchPath; }
    }

    public bool ConsiderDynamicDirectory
    {
      get { return _considerDynamicDirectory; }
    }

    public string? DynamicDirectory
    {
      get { return _dynamicDirectory; }
    }

    public IAssemblyLoader AssemblyLoader
    {
      get { return _assemblyLoader; }
    }

    public IEnumerable<RootAssembly> FindRootAssemblies ()
    {
      var combinedFinder = CreateCombinedFinder();
      return combinedFinder.FindRootAssemblies();
    }

    public virtual CompositeRootAssemblyFinder CreateCombinedFinder ()
    {
      var fileSearchService = new FileSystemSearchService();
      var specifications = new[]
                           {
                             new FilePatternSpecification("*.dll", FilePatternSpecificationKind.IncludeFollowReferences)
                           };

      var finders = new List<IRootAssemblyFinder> { new FilePatternRootAssemblyFinder(_baseDirectory, specifications, fileSearchService, _assemblyLoader) };

      if (!string.IsNullOrEmpty(_relativeSearchPath))
      {
        var privateBinPaths = _relativeSearchPath.Split(';');
        var rootAssemblyFinders = privateBinPaths
            .Select(privateBinPath => new FilePatternRootAssemblyFinder(privateBinPath, specifications, fileSearchService, _assemblyLoader));
        finders.AddRange(rootAssemblyFinders);
      }

      if (_considerDynamicDirectory && !string.IsNullOrEmpty(_dynamicDirectory))
        finders.Add(new FilePatternRootAssemblyFinder(_dynamicDirectory, specifications, fileSearchService, _assemblyLoader));

      return new CompositeRootAssemblyFinder(finders);
    }
  }
}
