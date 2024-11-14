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
namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// The search result of a <see cref="IBocAutoCompleteReferenceValueWebService"/>.<see cref="IBocAutoCompleteReferenceValueWebService.Search"/> call.
  /// </summary>
  public abstract class BocAutoCompleteReferenceValueSearchResult
  {
    /// <summary>
    /// Creates a new <see cref="BocAutoCompleteReferenceValueSearchResult"/> based on the <paramref name="values"/>.
    /// </summary>
    public static BocAutoCompleteReferenceValueSearchResult CreateForValueList (
        BusinessObjectWithIdentityProxy[] values,
        bool hasMoreSearchResults,
        string? context = null)
    {
      return new BocAutoCompleteReferenceValueSearchResultWithValueList(values, hasMoreSearchResults, context);
    }

    /// <summary>
    /// The discriminator for different <see cref="BocAutoCompleteReferenceValueSearchResult"/> variants, used during Javascript deserialization.
    /// </summary>
    public abstract string Type { get; }

    internal BocAutoCompleteReferenceValueSearchResult ()
    {
    }
  }
}
