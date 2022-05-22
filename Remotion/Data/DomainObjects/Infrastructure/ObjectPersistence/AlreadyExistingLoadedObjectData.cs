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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents a loaded object whose data already exists in the target <see cref="ClientTransaction"/>.
  /// </summary>
  public class AlreadyExistingLoadedObjectData : ILoadedObjectData
  {
    private readonly DataContainer _existingDataContainer;

    public AlreadyExistingLoadedObjectData (DataContainer existingDataContainer)
    {
      ArgumentUtility.CheckNotNull("existingDataContainer", existingDataContainer);

      if (!existingDataContainer.IsRegistered)
        throw new ArgumentException("The DataContainer must have been registered with a ClientTransaction.", "existingDataContainer");

      Assertion.IsTrue(existingDataContainer.HasDomainObject, "ClientTransaction only accepts DataContainers with DomainObjects.");

      _existingDataContainer = existingDataContainer;
    }

    public ObjectID ObjectID
    {
      get { return _existingDataContainer.ID; }
    }

    public DataContainer ExistingDataContainer
    {
      get { return _existingDataContainer; }
    }

    public IDomainObject? GetDomainObjectReference ()
    {
      Assertion.IsTrue(_existingDataContainer.HasDomainObject, "Checked by ctor.");
      return _existingDataContainer.DomainObject;
    }

    public void Accept (ILoadedObjectVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);
      visitor.VisitAlreadyExistingLoadedObject(this);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }


}
