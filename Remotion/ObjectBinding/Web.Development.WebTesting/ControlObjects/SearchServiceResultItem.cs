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
    public static bool operator == (SearchServiceResultItem? left, SearchServiceResultItem? right)
    {
      return Equals(left, right);
    }

    public static bool operator != (SearchServiceResultItem? left, SearchServiceResultItem? right)
    {
      return !Equals(left, right);
    }

    private readonly string _uniqueIdentifier;
    private readonly string _displayName;
    private readonly string? _iconUrl;

    public SearchServiceResultItem ([NotNull] string uniqueIdentifier, [NotNull] string displayName, [CanBeNull] string? iconUrl)
    {
      ArgumentUtility.CheckNotNullOrEmpty("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty("displayName", displayName);

      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
      _iconUrl = iconUrl == string.Empty ? null : iconUrl;
    }

    [NotNull]
    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    [NotNull]
    public string DisplayName
    {
      get { return _displayName; }
    }

    [CanBeNull]
    public string? IconUrl
    {
      get { return _iconUrl; }
    }

    protected bool Equals (SearchServiceResultItem other)
    {
      return _uniqueIdentifier == other._uniqueIdentifier && _displayName == other._displayName && _iconUrl == other._iconUrl;
    }

    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return Equals((SearchServiceResultItem)obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = _uniqueIdentifier.GetHashCode();
        hashCode = (hashCode * 397) ^ _displayName.GetHashCode();
        hashCode = (hashCode * 397) ^ (_iconUrl != null ? _iconUrl.GetHashCode() : 0);
        return hashCode;
      }
    }
  }
}
