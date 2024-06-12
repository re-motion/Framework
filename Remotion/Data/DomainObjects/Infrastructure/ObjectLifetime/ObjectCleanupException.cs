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

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime
{
  /// <summary>
  /// Thrown when the cleanup of a <see cref="DomainObject"/> whose constructor threw an exception fails.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a <see cref="DomainObject"/>'s constructor throws an exception, the object is automatically deleted from the transaction that caused its 
  /// creation. When that cleanup process again causes an exception, e.g., because the Delete operation is canceled, an 
  /// <see cref="ObjectCleanupException"/> is thrown that contains both the original constructor exception and the exception that canceled the
  /// cleanup process.
  /// </para>
  /// <para>
  /// When this exception occurs, the partially constructed <see cref="DomainObject"/> remains within the <see cref="ClientTransaction"/>.
  /// Rollback the <see cref="ClientTransaction"/> to get rid of the partially constructed instance.
  /// </para>
  /// </remarks>
  public class ObjectCleanupException : DomainObjectException
  {
    private readonly ObjectID _objectID;
    private readonly Exception _cleanupException;

    public ObjectCleanupException (string message, ObjectID objectID, Exception innerException, Exception cleanupException)
        : base(message, ArgumentUtility.CheckNotNull("innerException", innerException))
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      ArgumentUtility.CheckNotNull("cleanupException", cleanupException);

      _objectID = objectID;
      _cleanupException = cleanupException;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public Exception CleanupException
    {
      get { return _cleanupException; }
    }
  }
}
