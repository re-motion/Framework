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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  public static class LoadedObjectDataObjectMother
  {
    public static ILoadedObjectData CreateLoadedObjectDataStub (DomainObject domainObjectReference = null)
    {
      domainObjectReference = domainObjectReference ?? DomainObjectMother.CreateFakeObject<Order>();
      var loadedObjectDataStub = CreateLoadedObjectDataStub (domainObjectReference.ID);
      loadedObjectDataStub.Stub (stub => stub.GetDomainObjectReference ()).Return (domainObjectReference);
      return loadedObjectDataStub;
    }

    public static ILoadedObjectData CreateLoadedObjectDataStub (ObjectID objectID)
    {
      var loadedObjectDataStub = MockRepository.GenerateStub<ILoadedObjectData> ();
      loadedObjectDataStub.Stub (stub => stub.ObjectID).Return (objectID);
      return loadedObjectDataStub;
    }

    public static LoadedObjectDataWithDataSourceData CreateLoadedObjectDataWithDataSourceData (DomainObject domainObjectReference)
    {
      var loadedObjectDataStub = CreateLoadedObjectDataStub (domainObjectReference);
      return CreateLoadedObjectDataWithDataSourceData (loadedObjectDataStub);
    }

    public static LoadedObjectDataWithDataSourceData CreateLoadedObjectDataWithDataSourceData (ILoadedObjectData loadedObjectData)
    {
      var dataContainer = DataContainer.CreateForExisting (loadedObjectData.ObjectID, null, pd => pd.DefaultValue);
      return new LoadedObjectDataWithDataSourceData (loadedObjectData, dataContainer);
    }

    public static LoadedObjectDataWithDataSourceData CreateLoadedObjectDataWithDataSourceData ()
    {
      return CreateLoadedObjectDataWithDataSourceData (DomainObjectMother.CreateFakeObject<Order>());
    }

    public static LoadedObjectDataWithDataSourceData CreateLoadedObjectDataWithDataSourceData (ObjectID objectID)
    {
      return CreateLoadedObjectDataWithDataSourceData (DomainObjectMother.CreateFakeObject (objectID));
    }

    public static LoadedObjectDataWithDataSourceData CreateNullLoadedObjectDataWithDataSourceData ()
    {
      return new LoadedObjectDataWithDataSourceData (new NullLoadedObjectData(), null);
    }

    public static FreshlyLoadedObjectData CreateFreshlyLoadedObjectData (ObjectID objectID)
    {
      return new FreshlyLoadedObjectData (DataContainerObjectMother.Create (objectID));
    }
  }
}