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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Resources;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  ///   A wrapper for the .net Framework <see cref="System.Resources.ResourceManager"/> implementation.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Limited to accessing string resources.
  ///     Limited on resources for the current UI culture and its less specific cultures.
  ///   </para><para>
  ///     If multiple Resource Managers are added which belonging to derived types, 
  ///     make sure to sort the resource managers in the order of inheritance before wrapping them.
  ///   </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public sealed class ResourceManagerWrapper : IResourceManager
  {
    //  static members

    private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerWrapper));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceManagers">
    ///   The <see cref="System.Resources.ResourceManager"/> array to be wrapped.
    ///   The <paramref name="resourceManagers"/> must not be emtpy and no instance may be <see langname="null"/>.
    /// </param>
    /// <returns></returns>
    public static ResourceManagerSet CreateWrapperSet (IEnumerable<ResourceManager> resourceManagers)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);
      
      return new ResourceManagerSet (resourceManagers.Select (rm => (IResourceManager) new ResourceManagerWrapper (rm)));
    }

    // member fields

    private readonly ResourceManager _resourceManager;

    private readonly LockingCacheDecorator<Tuple<CultureInfo, string>, NameValueCollection> _cachedResourceSet =
        CacheFactory.CreateWithLocking<Tuple<CultureInfo, string>, NameValueCollection>();

    // construction and disposing

    /// <summary>
    ///   Constructor for wrapping multiple resource managers
    /// </summary>
    /// <param name="resourceManager">
    ///   The <see cref="System.Resources.ResourceManager"/> to be wrapped. Must not be <see langname="null"/>.
    /// </param>
    public ResourceManagerWrapper (ResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      _resourceManager = resourceManager;
    }

    // methods and properties

    /// <summary>
    ///   Gets the wrapped <see cref="System.Resources.ResourceManager"/> instance. 
    /// </summary>
    public ResourceManager ResourceManager
    {
      get { return _resourceManager; }
    }

    /// <summary>
    ///   Gets the root names of the resource files that the <c>IResourceManager</c>
    ///   searches for resources. Multiple roots are separated by a comma.
    /// </summary>
    string IResourceManager.Name
    {
      get { return _resourceManager.BaseName; }
    }

    /// <summary>
    ///   Searches for all string resources inside the <see cref="System.Resources.ResourceManager"/> whose name is prefixed with a matching tag.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   If a duplicate ID is found, the ID from the last ResourceManager wins.
    /// </para><para>
    ///   Fallback:
    ///     <list type="number">
    ///       <item>
    ///         Resource hierarchy for current culture.
    ///       </item>
    ///       <item>
    ///         Resource hierarchies for less specific cultures
    ///       </item>
    ///     </list>
    ///   </para>
    /// </remarks>
    public NameValueCollection GetAllStrings (string prefix)
    {
      return _cachedResourceSet.GetOrCreateValue (
          Tuple.Create (CultureInfo.CurrentUICulture, prefix ?? string.Empty),
          key =>
          {
            //  Loop through all entries in the resource managers
            CultureInfo[] cultureHierarchy = GetCultureHierarchy (key.Item1);

            // Loop from most neutral to current UICulture
            // Copy the resources into a collection
            NameValueCollection result = new NameValueCollection ();
            for (int i = 0; i < cultureHierarchy.Length; i++)
            {
              CultureInfo culture = cultureHierarchy[i];
              ResourceSet resourceSet = _resourceManager.GetResourceSet (culture, true, false);
              if (resourceSet != null)
              {
                foreach (DictionaryEntry entry in resourceSet)
                {
                  string entryKey = (string) entry.Key;
                  if (entryKey.StartsWith (key.Item2))
                    result[entryKey] = (string) entry.Value;
                }
              }
            }

            return result;
          });
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    public bool TryGetString (string id, out string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      string result = _resourceManager.GetString (id);
      if (result != null)
      {
        value = result;
        return true;
      }

      s_log.DebugFormat ("Could not find resource with ID '{0}' in resource container '{1}'.", id, _resourceManager.BaseName);
      value = null;
      return false;
    }

    /// <summary>
    ///   Returns the culture hierarchy, starting with the most specialized culture.
    /// </summary>
    /// <param name="mostSpecialized">
    ///   The starting point for walking the culture tree upwards. Must not be <see langame="null"/>.
    /// </param>
    /// <returns>
    ///   The cultures, starting with the invariant culture, ending with the most specialized culture.
    /// </returns>
    public static CultureInfo[] GetCultureHierarchy (CultureInfo mostSpecialized)
    {
      ArrayList hierarchyTopDown = new ArrayList();

      CultureInfo currentLevel = mostSpecialized;

      do
      {
        hierarchyTopDown.Add (currentLevel);
        currentLevel = currentLevel.Parent;
      } while (currentLevel != CultureInfo.InvariantCulture);

      if (mostSpecialized != CultureInfo.InvariantCulture)
        hierarchyTopDown.Add (currentLevel);

      hierarchyTopDown.Reverse();

      return (CultureInfo[]) hierarchyTopDown.ToArray (typeof (CultureInfo));
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
