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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NonExistingDataContainerEndPointsRegistrationAgentTest : StandardMappingTest
  {
    private Mock<IRelationEndPointFactory> _endPointFactoryStub;
    private Mock<IRelationEndPointRegistrationAgent> _registrationAgentStub;
    private RelationEndPointMap _map;

    private NonExistingDataContainerEndPointsRegistrationAgent _agent;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryStub = new Mock<IRelationEndPointFactory>();
      _registrationAgentStub = new Mock<IRelationEndPointRegistrationAgent>();
      _map = new RelationEndPointMap(new Mock<IClientTransactionEventSink>().Object);

      _agent = new NonExistingDataContainerEndPointsRegistrationAgent(_endPointFactoryStub.Object, _registrationAgentStub.Object);
    }

    [Test]
    public void GetOwnedEndPointIDs ()
    {
      var dataContainer = DataContainer.CreateNew(DomainObjectIDs.Order1);

      var result = (IEnumerable<RelationEndPointID>)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetOwnedEndPointIDs", dataContainer);

      Assert.That(
          result,
          Is.EquivalentTo(
              new[]
              {
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket"),
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems"),
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Official"),
                  RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer")
              }));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_CurrentReferencesNonNull ()
    {
      var endPointStub = new Mock<IObjectEndPoint>();
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Customer1);
      endPointStub.Setup(stub => stub.OriginalOppositeObjectID).Returns((ObjectID)null);
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_OriginalReferencesNonNull ()
    {
      var endPointStub = new Mock<IObjectEndPoint>();
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns((ObjectID)null);
      endPointStub.Setup(stub => stub.OriginalOppositeObjectID).Returns(DomainObjectIDs.Customer1);
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_ObjectEndPoint_NonDangling ()
    {
      var endPointStub = new Mock<IObjectEndPoint>();
      endPointStub.Setup(stub => stub.OppositeObjectID).Returns((ObjectID)null);
      endPointStub.Setup(stub => stub.OriginalOppositeObjectID).Returns((ObjectID)null);
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_DomainObjectCollectionEndPoint_CurrentReferencesNonNull ()
    {
      var item = DomainObjectMother.CreateFakeObject<Customer>();
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();
      endPointStub.Setup(stub => stub.GetData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData(new[] { item })));
      endPointStub.Setup(stub => stub.GetOriginalData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData()));
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_DomainObjectCollectionEndPoint_OriginalReferencesNonNull ()
    {
      var item = DomainObjectMother.CreateFakeObject<Customer>();
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();
      endPointStub.Setup(stub => stub.GetData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData()));
      endPointStub.Setup(stub => stub.GetOriginalData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData(new[] { item })));
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.EqualTo(
          "Relation end-point 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' "
          + "would leave a dangling reference."));
    }

    [Test]
    public void GetUnregisterProblem_DomainObjectCollectionEndPoint_NonDangling ()
    {
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();
      endPointStub.Setup(stub => stub.GetData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData()));
      endPointStub.Setup(stub => stub.GetOriginalData()).Returns(new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData()));
      endPointStub.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetUnregisterProblem_VirtualCollectionEndPoint ()
    {
      var endPointStub = new Mock<IVirtualCollectionEndPoint>();
      endPointStub.Setup(stub => stub.GetData()).Throws(new AssertionException("GetData() should not be called."));
      endPointStub.Setup(stub => stub.GetOriginalData()).Throws(new AssertionException("GetOriginalData() should not be called."));

      var result = (string)PrivateInvoke.InvokeNonPublicMethod(_agent, "GetUnregisterProblem", endPointStub.Object, _map);

      Assert.That(result, Is.Null);
    }
  }
}
