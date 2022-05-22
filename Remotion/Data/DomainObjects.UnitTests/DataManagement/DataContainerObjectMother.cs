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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  public static class DataContainerObjectMother
  {
    public static DataContainer Create (ObjectID objectID = null)
    {
      return CreateNew(objectID);
    }

    public static DataContainer Create (IDomainObject domainObject)
    {
      var dataContainer = Create(domainObject.ID);
      dataContainer.SetDomainObject(domainObject);
      return dataContainer;
    }

    public static DataContainer CreateNew (ObjectID objectID = null)
    {
      var dataContainer = DataContainer.CreateNew(objectID ?? new ObjectID(typeof(Order), Guid.NewGuid()));
      return dataContainer;
    }

    public static DataContainer CreateNew (IDomainObject domainObject)
    {
      var dataContainer = CreateNew(domainObject.ID);
      dataContainer.SetDomainObject(domainObject);
      return dataContainer;
    }

    public static DataContainer CreateExisting (ObjectID objectID = null)
    {
      var dataContainer = DataContainer.CreateForExisting(objectID ?? new ObjectID(typeof(Order), Guid.NewGuid()), 4711, pd => pd.DefaultValue);
      return dataContainer;
    }

    public static DataContainer CreateExisting (IDomainObject domainObject)
    {
      var dataContainer = CreateExisting(domainObject.ID);
      dataContainer.SetDomainObject(domainObject);
      return dataContainer;
    }

    public static DataContainer CreateDeleted (ObjectID objectID = null)
    {
      var dataContainer = CreateExisting(objectID);
      dataContainer.Delete();
      return dataContainer;
    }

    public static DataContainer CreateDeleted (IDomainObject domainObject)
    {
      var dataContainer = CreateDeleted(domainObject.ID);
      dataContainer.SetDomainObject(domainObject);
      return dataContainer;
    }
  }
}
