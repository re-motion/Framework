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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class ObjectLoaderTest : StandardMappingTest
  {
    private MockRepository _mockRepository;
    
    private IPersistenceStrategy _persistenceStrategyMock;
    private ILoadedObjectDataRegistrationAgent _loadedObjectDataRegistrationAgentMock;
    private ILoadedObjectDataProvider _loadedObjectDataProviderStub;

    private ObjectLoader _objectLoader;

    private IQuery _fakeQuery;
    private ILoadedObjectData _loadedObjectDataStub1;
    private ILoadedObjectData _loadedObjectDataStub2;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository ();

      _persistenceStrategyMock = _mockRepository.StrictMock<IPersistenceStrategy> ();
      _loadedObjectDataRegistrationAgentMock = _mockRepository.StrictMock<ILoadedObjectDataRegistrationAgent>();
      _loadedObjectDataProviderStub = _mockRepository.Stub<ILoadedObjectDataProvider>();

      _objectLoader = new ObjectLoader (_persistenceStrategyMock, _loadedObjectDataRegistrationAgentMock, _loadedObjectDataProviderStub);

      _fakeQuery = CreateFakeQuery();

      _loadedObjectDataStub1 = MockRepository.GenerateStub<ILoadedObjectData> ();
      _loadedObjectDataStub2 = MockRepository.GenerateStub<ILoadedObjectData> ();
    }

    [Test]
    public void LoadObject ()
    {
      _persistenceStrategyMock.Expect (mock => mock.LoadObjectData (DomainObjectIDs.Order1)).Return (_loadedObjectDataStub1);
      var throwOnNotFound = BooleanObjectMother.GetRandomBoolean();
      _loadedObjectDataRegistrationAgentMock.Expect (mock => mock.RegisterIfRequired (Arg.Is (new[] { _loadedObjectDataStub1 }), Arg.Is (throwOnNotFound)));

      _mockRepository.ReplayAll();

      var result = _objectLoader.LoadObject (DomainObjectIDs.Order1, throwOnNotFound);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_loadedObjectDataStub1));
    }

    [Test]
    public void LoadObjects ()
    {
      _loadedObjectDataStub1.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order1);
      _loadedObjectDataStub2.Stub (stub => stub.ObjectID).Return (DomainObjectIDs.Order3);

      _persistenceStrategyMock
          .Expect (mock => mock.LoadObjectData (
              Arg<ICollection<ObjectID>>.List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 })))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }),
              Arg.Is (true)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.LoadObjects (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }, true);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    [Test]
    public void GetOrLoadRelatedObject ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order4, "OrderTicket");

      _persistenceStrategyMock
          .Expect (mock => mock.ResolveObjectRelationData (endPointID, _loadedObjectDataProviderStub))
          .Return (_loadedObjectDataStub1);
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (new[] { _loadedObjectDataStub1 }, true));
      _mockRepository.ReplayAll();

      var result = _objectLoader.GetOrLoadRelatedObject (endPointID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.SameAs (_loadedObjectDataStub1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "GetOrLoadRelatedObject can only be used with virtual end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_NonVirtualID ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _objectLoader.GetOrLoadRelatedObject (endPointID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObject can only be used with one-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObject_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderItems");
      _objectLoader.GetOrLoadRelatedObject (endPointID);
    }

    [Test]
    public void GetOrLoadRelatedObjects ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Customer1, "Orders");

      _persistenceStrategyMock
          .Expect (mock => mock.ResolveCollectionRelationData (endPointID, _loadedObjectDataProviderStub))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (
              mock =>
              mock.RegisterIfRequired (
                  Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }), 
                  Arg.Is (true)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadRelatedObjects (endPointID);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "GetOrLoadRelatedObjects can only be used with many-valued end points.\r\nParameter name: relationEndPointID")]
    public void GetOrLoadRelatedObjects_WrongCardinality ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      _objectLoader.GetOrLoadRelatedObjects (endPointID);
    }

    [Test]
    public void GetOrLoadCollectionQueryResult ()
    {
      _persistenceStrategyMock
          .Expect (mock => mock.ExecuteCollectionQuery (_fakeQuery, _loadedObjectDataProviderStub))
          .Return (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 });
      _loadedObjectDataRegistrationAgentMock
          .Expect (mock => mock.RegisterIfRequired (
              Arg<IEnumerable<ILoadedObjectData>>.List.Equal (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }),
              Arg.Is (true)));

      _mockRepository.ReplayAll ();

      var result = _objectLoader.GetOrLoadCollectionQueryResult (_fakeQuery);

      _mockRepository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _loadedObjectDataStub1, _loadedObjectDataStub2 }));
    }

    private IQuery CreateFakeQuery ()
    {
      return QueryFactory.CreateCollectionQuery (
          "test",
          UnitTestStorageProviderDefinition,
          "TEST",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));
    }
  }
}