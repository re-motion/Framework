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

namespace Remotion.ObjectBinding.Web.Services
{
  /// <summary>
  /// Represents a <see cref="BocAutoCompleteReferenceValueSearchResult"/> containing an array of <see cref="BusinessObjectWithIdentityProxy"/> instances.
  /// </summary>
  public class BocAutoCompleteReferenceValueSearchResultWithValueList : BocAutoCompleteReferenceValueSearchResult
  {
    /// <inheritdoc/>
    public override string Type => "ValueList";

    /// <summary>
    /// Gets the <see cref="BusinessObjectWithIdentityProxy"/> instances contained in this result.
    /// </summary>
    public BusinessObjectWithIdentityProxy[] Values { get; }

    /// <summary>
    /// Indicates that more search results are available to be queried on-demand by the client.
    /// </summary>
    public bool HasMoreSearchResults { get; }

    internal BocAutoCompleteReferenceValueSearchResultWithValueList (BusinessObjectWithIdentityProxy[] values, bool hasMoreSearchResults)
    {
      Values = values;
      HasMoreSearchResults = hasMoreSearchResults;
    }
  }
}
