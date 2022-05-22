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
  /// Represents an object that was freshly loaded from the data source.
  /// </summary>
  public class FreshlyLoadedObjectData : ILoadedObjectData
  {
    private readonly DataContainer _freshlyLoadedDataContainer;

    public FreshlyLoadedObjectData (DataContainer freshlyLoadedDataContainer)
    {
      ArgumentUtility.CheckNotNull("freshlyLoadedDataContainer", freshlyLoadedDataContainer);

      if (freshlyLoadedDataContainer.IsRegistered)
        throw new ArgumentException("The DataContainer must not have been registered with a ClientTransaction.", "freshlyLoadedDataContainer");

      if (freshlyLoadedDataContainer.HasDomainObject)
        throw new ArgumentException("The DataContainer must not have been registered with a DomainObject.", "freshlyLoadedDataContainer");

      _freshlyLoadedDataContainer = freshlyLoadedDataContainer;
    }

    public ObjectID ObjectID
    {
      get { return _freshlyLoadedDataContainer.ID; }
    }

    public DataContainer FreshlyLoadedDataContainer
    {
      get { return _freshlyLoadedDataContainer; }
    }

    public IDomainObject? GetDomainObjectReference ()
    {
      if (!_freshlyLoadedDataContainer.HasDomainObject)
        throw new InvalidOperationException("Cannot obtain a DomainObject reference for a freshly loaded object that has not yet been registered.");

      return _freshlyLoadedDataContainer.DomainObject;
    }

    public void Accept (ILoadedObjectVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);
      visitor.VisitFreshlyLoadedObject(this);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
