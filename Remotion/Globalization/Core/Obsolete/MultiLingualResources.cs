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

// ReSharper disable once CheckNamespace
namespace Remotion.Globalization
{
  /// <summary>
  /// Provides the public API for classes working with and analyzing instances of <see cref="MultiLingualResourcesAttribute"/>.
  /// </summary>
  public static class MultiLingualResources
  {
    /// <summary>
    ///   Loads a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container and use IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).GetString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)", true)]
    public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
    {
      throw new NotSupportedException ("Retrieve IGlobalizationService from IoC container and use IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).GetString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)");
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container and test for IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).ContainsString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)", true)]
    public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
    {
      throw new NotSupportedException ("Retrieve IGlobalizationService from IoC container and test for IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).ContainsString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)");
    }
  }
}