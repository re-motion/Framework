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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// The exception that is thrown when properties or methods of an object are accessed in a transaction where that object is invalid.
  /// </summary>
  /// <remarks>
  /// A <see cref="DomainObject"/> is invalid in the following situations:
  /// <list type="buttons">
  ///   <description>
  ///     A new <see cref="DomainObject"/> has been created and the <see cref="ClientTransaction"/> has been rolled back.
  ///   </description>
  ///   <description>
  ///     A new <see cref="DomainObject"/> has been created and deleted.
  ///   </description>
  ///   <description>
  ///     An existing <see cref="DomainObject"/> has been deleted and the <see cref="ClientTransaction"/> has been committed.
  ///   </description>
  ///   <description>
  ///     A new <see cref="DomainObject"/> has been created in a sub-transaction of the <see cref="ClientTransaction"/> and the sub-transaction
  ///     has not yet been committed. When the sub-transaction is committed, the object becomes valid in the parent, but not in the parent's parent and
  ///     above.
  ///   </description>
  /// </list>
  /// The <see cref="DataContainer"/> that was associated with the invalid <see cref="DomainObject"/> is discarded.
  /// </remarks>
  public class ObjectInvalidException : DomainObjectException
  {
    private readonly ObjectID? _id;

    public ObjectInvalidException (string message)
        : base(message)
    {
    }

    public ObjectInvalidException (string message, Exception inner)
        : base(message, inner)
    {
    }

    public ObjectInvalidException (ObjectID id)
        : this(string.Format("Object '{0}' is invalid in this transaction.", id), id)
    {
    }

    public ObjectInvalidException (string message, ObjectID id)
        : base(message)
    {
      ArgumentUtility.CheckNotNull("id", id);

      _id = id;
    }

    /// <summary>
    /// The <see cref="ObjectID"/> of the object that caused the exception.
    /// </summary>
    public ObjectID? ID
    {
      get { return _id; }
    }
  }
}
