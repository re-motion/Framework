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

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Defines an interface for objects reacting on objects being registered within a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface ILoadedObjectDataRegistrationListener
  {
    void OnBeforeObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs);
    // Calls to OnAfterObjectRegistration must be exactly matched with OnBeforeObjectRegistration; they must not be swallowed in case of exceptions.
    void OnAfterObjectRegistration (IReadOnlyList<ObjectID> loadedObjectIDs, IReadOnlyList<IDomainObject> actuallyLoadedDomainObjects);
    void OnObjectsNotFound (IReadOnlyList<ObjectID> notFoundObjectIDs);
  }
}
