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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerAssociateWithIDsCommandTest : StandardMappingTest
  {
    private Mock<IStorageProviderCommand<IEnumerable<DataContainer>>> _commandStub;
    private Mock<IRdbmsProviderCommandExecutionContext> _executionContext;
    private DataContainer _order1Container;
    private DataContainer _order2Container;
    private DataContainer _order3Container;

    public override void SetUp ()
    {
      base.SetUp();

      _commandStub = new Mock<IStorageProviderCommand<IEnumerable<DataContainer>>>();
      _executionContext = new Mock<IRdbmsProviderCommandExecutionContext>();

      _order1Container = DataContainerObjectMother.Create(DomainObjectIDs.Order1);
      _order2Container = DataContainerObjectMother.Create(DomainObjectIDs.Order3);
      _order3Container = DataContainerObjectMother.Create(DomainObjectIDs.Order4);
    }

    [Test]
    public void Initialize_NullObjectID ()
    {
      Assert.That(
          () => new MultiDataContainerAssociateWithIDsCommand(new[] { DomainObjectIDs.Order1, null }, _commandStub.Object),
          Throws.ArgumentNullException.With.ArgumentExceptionMessageWithParameterNameEqualTo("objectIDs[1]"));
    }

    [Test]
    public void Execute ()
    {
      var command = new MultiDataContainerAssociateWithIDsCommand(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1 }, _commandStub.Object);
      _commandStub.Setup(stub => stub.Execute(_executionContext.Object)).Returns(new[] { _order2Container, _order1Container });

      var result = command.Execute(_executionContext.Object).ToList();

      Assert.That(result.Count, Is.EqualTo(3));
      Assert.That(result[0].LocatedObject, Is.SameAs(_order1Container));
      Assert.That(result[0].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(result[1].LocatedObject, Is.SameAs(_order2Container));
      Assert.That(result[1].ObjectID, Is.EqualTo(DomainObjectIDs.Order3));
      Assert.That(result[2].LocatedObject, Is.Null);
      Assert.That(result[2].ObjectID, Is.EqualTo(DomainObjectIDs.OrderItem1));
    }

    [Test]
    public void Execute_DuplicatedObjectID ()
    {
      var command = new MultiDataContainerAssociateWithIDsCommand(new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order1 }, _commandStub.Object);

      _commandStub.Setup(stub => stub.Execute(_executionContext.Object)).Returns(new[] { _order1Container });

      var result = command.Execute(_executionContext.Object).ToList();

      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].LocatedObject, Is.SameAs(_order1Container));
      Assert.That(result[0].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
      Assert.That(result[1].LocatedObject, Is.SameAs(_order1Container));
      Assert.That(result[1].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void Execute_DuplicatedDataContainer ()
    {
      var command = new MultiDataContainerAssociateWithIDsCommand(new[] { DomainObjectIDs.Order1 }, _commandStub.Object);

      var otherOrder1DataContainer = DataContainerObjectMother.Create(_order1Container.ID);

      _commandStub.Setup(stub => stub.Execute(_executionContext.Object)).Returns(new[] { _order1Container, otherOrder1DataContainer });

      var result = command.Execute(_executionContext.Object).ToList();

      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].LocatedObject, Is.SameAs(otherOrder1DataContainer));
      Assert.That(result[0].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void Execute_NullDataContainer ()
    {
      var command = new MultiDataContainerAssociateWithIDsCommand(new[] { DomainObjectIDs.Order1 }, _commandStub.Object);

      _commandStub.Setup(stub => stub.Execute(_executionContext.Object)).Returns(new[] { _order1Container, null });

      var result = command.Execute(_executionContext.Object).ToList();

      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].LocatedObject, Is.SameAs(_order1Container));
      Assert.That(result[0].ObjectID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void Execute_DataContainersNotMatchingAnyID ()
    {
      var wrongID1 = new ObjectID(typeof(OrderItem), _order1Container.ID.Value);
      var wrongID2 = new ObjectID(typeof(OrderTicket), _order1Container.ID.Value);

      var command = new MultiDataContainerAssociateWithIDsCommand(new[] { wrongID1, wrongID1, wrongID2, _order3Container.ID }, _commandStub.Object);

      _commandStub.Setup(stub => stub.Execute(_executionContext.Object)).Returns(new[] { _order1Container, _order2Container, _order3Container });

      Assert.That(
          () => command.Execute(_executionContext.Object).ToList(),
          Throws.TypeOf<PersistenceException>().With.Message.EqualTo(
              "The ObjectID of one or more loaded DataContainers does not match the expected ObjectIDs:\r\n"
              + "Loaded DataContainer ID: Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid, expected ObjectID(s): "
              + "OrderItem|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid, OrderTicket|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid\r\n"
              + "Loaded DataContainer ID: Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid, expected ObjectID(s): none"));
    }
  }
}
