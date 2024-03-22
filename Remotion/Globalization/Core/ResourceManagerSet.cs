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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>Combines one or more <see cref="IResourceManager"/> instances to a set that can be accessed using a single interface.</summary>
  /// <threadsafety static="true" instance="true" />
  public class ResourceManagerSet : IResourceManager
  {
    private static readonly ILog s_log = LogManager.GetLogger(typeof(ResourceManagerSet));

    private readonly IResourceManager[] _resourceManagers;
    private readonly string _name;

    /// <summary>
    ///   Combines several <see cref="IResourceManager"/> instances to a single <see cref="ResourceManagerSet"/>.
    /// </summary>
    /// <remarks>
    ///   For parameters that are <see cref="ResourceManagerSet"/> instances, the contained <see cref="IResourceManager"/>s are added directly.
    /// </remarks>
    /// <example>
    ///   <para>
    ///     Given the following parameter list of resource managers (rm) and resource manager sets (rmset):
    ///   </para><para>
    ///     rm1, rm2, rmset (rm3, rm4, rm5), rm6, rmset (rm7, rm8)
    ///   </para><para>
    ///     The following resource manager set is created:
    ///   </para><para>
    ///     rmset (rm1, rm2, rm3, rm4, rm5, rm6, rm7, rm8)
    ///   </para>
    /// </example>
    /// <param name="resourceManagers"> The resource managers, starting with the most specific. </param>
    public static ResourceManagerSet Create (params IResourceManager[] resourceManagers)
    {
      ArgumentUtility.CheckNotNull("resourceManagers", resourceManagers);

      return new ResourceManagerSet(resourceManagers.AsEnumerable());
    }

    /// <summary>
    ///   Creates a <see cref="ResourceManagerSet"/> from a sequence of <see cref="IResourceManager"/>s.
    /// </summary>
    /// <remarks>
    ///   For parameters that are <see cref="ResourceManagerSet"/> instances, the contained <see cref="IResourceManager"/>s are added directly.
    /// </remarks>
    /// <param name="resourceManagers"> The resource managers, starting with the most specific. </param>
    public ResourceManagerSet (IEnumerable<IResourceManager> resourceManagers)
    {
      ArgumentUtility.CheckNotNull("resourceManagers", resourceManagers);

      _resourceManagers = CreateFlatList(resourceManagers).ToArray();
      _name = _resourceManagers.Any() ? string.Join(", ", _resourceManagers.Select(rm=> rm.Name)) : "Empty ResourceManagerSet";
    }

    public ReadOnlyCollection<IResourceManager> ResourceManagers
    {
      get { return new ReadOnlyCollection<IResourceManager>(_resourceManagers); }
    }

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed with a matching tag.
    /// </summary>
    public IReadOnlyDictionary<string, string> GetAllStrings (string? prefix)
    {
      var result = new Dictionary<string, string>();
      foreach (var resourceManager in _resourceManagers)
      {
        var strings = resourceManager.GetAllStrings(prefix);
        foreach (var resourceEntry in strings)
        {
          var key = resourceEntry.Key;
          if (!result.ContainsKey(key))
            result.Add(key, resourceEntry.Value);
        }
      }
      return result;
    }

    /// <summary>
    ///   Tries to get the value of the specified string resource. If the resource is not found, <see langword="false" /> is returned.
    /// </summary>
    public bool TryGetString (string id, [MaybeNullWhen(false)] out string value)
    {
      //FOR-loop for performance reasons
      // ReSharper disable ForCanBeConvertedToForeach
      for (var i = 0; i < _resourceManagers.Length; i++)
      {
        if (_resourceManagers[i].TryGetString(id, out value))
          return true;
      }
      // ReSharper restore ForCanBeConvertedToForeach

      s_log.DebugFormat("Could not find resource with ID '{0}' in any of the following resource containers '{1}'.", id, _name);

      value = null;
      return false;
    }

    private IEnumerable<IResourceManager> CreateFlatList (IEnumerable<IResourceManager> resourceManagers)
    {
      foreach (var resourceManager in resourceManagers)
      {
        var rmset = resourceManager as ResourceManagerSet;
        if (rmset != null)
        {
          foreach (var rm in rmset.ResourceManagers)
            yield return rm;
        }
        else if (resourceManager != null && !resourceManager.IsNull)
          yield return resourceManager;
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public IReadOnlyDictionary<CultureInfo, string> GetAvailableStrings (string id)
    {
      Dictionary<CultureInfo, string> result = new Dictionary<CultureInfo, string>();

      foreach (var resourceManager in _resourceManagers)
      {
        foreach(var localization in resourceManager.GetAvailableStrings(id))
        {
          if (!result.ContainsKey(localization.Key))
            result.Add(localization.Key, localization.Value);
        }
      }

      return result;
    }

    bool INullObject.IsNull
    {
      get { return !_resourceManagers.Any(); }
    }
  }
}
