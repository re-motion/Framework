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
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Globalization;

namespace Remotion.Web.UI.Globalization
{
/// <summary>
///   Functionality for working with <see cref="IResourceManager"/> in Controls.
/// </summary>
public static class ResourceManagerUtility
{
  private const string c_globalResourceKeyPrefix = "$res:";

  public static bool IsGlobalResourceKey (string elementValue)
  {
    if (string.IsNullOrEmpty (elementValue))
      return false;
    return elementValue.StartsWith (c_globalResourceKeyPrefix);
  }

  public static string GetGlobalResourceKey (string elementValue)
  {
    if (IsGlobalResourceKey (elementValue))
      return elementValue.Substring (c_globalResourceKeyPrefix.Length);
    else
      return null;
  }

  /// <summary>
  ///   Get resource managers of all controls impementing <see cref="IObjectWithResources"/> in the 
  ///   current control's hierarchy (parents last).
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented. If more than
  ///   one resource manager is found, an <see cref="ResourceManagerSet"/> is returned.
  /// </returns>
  /// <remarks> Uses a cache for the individual <see cref="IResourceManager"/> instances. </remarks>
  public static IResourceManager GetResourceManager (Control control)
  {
    return GetResourceManager (control, true);
  }

  /// <summary>
  ///   Get resource managers of all controls impementing <see cref="IObjectWithResources"/> in the 
  ///   current control's hierarchy (parents last).
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <param name="alwaysIncludeParents">
  ///   If true, parent controls' resource managers are included even if a resource manager has already 
  ///   been found in a child control. Default is true.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented. If more than
  ///   one resource manager is found, an <see cref="ResourceManagerSet"/> is returned.
  /// </returns>
  /// <remarks> Uses a cache for the individual <see cref="IResourceManager"/> instances. </remarks>
  [NotNull]
  public static IResourceManager GetResourceManager ([CanBeNull]Control control, bool alwaysIncludeParents)
  {
    if (control == null)
      return NullResourceManager.Instance;

    var resourceManagers = new List<IResourceManager>();
    GetResourceManagersRecursive (control, resourceManagers, alwaysIncludeParents);

    if (resourceManagers.Count == 0)
      return NullResourceManager.Instance;
    else if (resourceManagers.Count == 1)
      return resourceManagers[0];
    else
      return new ResourceManagerSet (resourceManagers);
  }

  private static void GetResourceManagersRecursive ([CanBeNull]Control control, List<IResourceManager> resourceManagers, bool alwaysIncludeParents)
  {
    if (control == null)
      return;

    var objectWithResources = control as IObjectWithResources;

    if (objectWithResources == null || alwaysIncludeParents)
      GetResourceManagersRecursive (control.Parent, resourceManagers, alwaysIncludeParents);

    if (objectWithResources != null)
      resourceManagers.Add (objectWithResources.GetResourceManager() ?? NullResourceManager.Instance);
  }
}
}
