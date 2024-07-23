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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when an object cannot be committed to the underlying datasource because a newer version already exists.
  /// </summary>
  public class ConcurrencyViolationException : StorageProviderException
  {
    private static string BuildMessage (IEnumerable<ObjectID> ids)
    {
      return string.Format(
          "Concurrency violation encountered. One or more object(s) have already been changed by someone else: {0}",
          string.Join(", ", ids.Select(id => "'" + id + "'")));
    }

    private readonly ObjectID[] _ids;

    public ConcurrencyViolationException (IEnumerable<ObjectID> ids)
        : this(ids, null)
    {
    }

    public ConcurrencyViolationException (IEnumerable<ObjectID> ids, Exception? inner)
        : this(BuildMessage(ArgumentUtility.CheckNotNull("ids", ids)), ids, inner)
    {
    }

    public ConcurrencyViolationException (string message, IEnumerable<ObjectID> ids, Exception? inner)
        : base(message, inner)
    {
      ArgumentUtility.CheckNotNull("ids", ids);
      _ids = ids.ToArray();
    }

    public ReadOnlyCollection<ObjectID> IDs
    {
      get { return Array.AsReadOnly(_ids); }
    }
  }
}
