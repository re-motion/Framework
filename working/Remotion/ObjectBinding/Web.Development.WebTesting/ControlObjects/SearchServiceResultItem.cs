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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Represents an item of an auto complete search service query result (an item of the auto completion set).
  /// </summary>
  /// <seealso cref="T:Remotion.ObjectBinding.Web.Services.BusinessObjectWithIdentityProxy"/>
  public class SearchServiceResultItem
  {
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;
    private readonly string _iconUrl;

    public SearchServiceResultItem ([NotNull] string uniqueIdentifier, [NotNull] string displayName, [NotNull] string iconUrl)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);
      ArgumentUtility.CheckNotNullOrEmpty ("iconUrl", iconUrl);

      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
      _iconUrl = iconUrl;
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public string DisplayName
    {
      get { return _displayName; }
    }

    public string IconUrl
    {
      get { return _iconUrl; }
    }
  }
}