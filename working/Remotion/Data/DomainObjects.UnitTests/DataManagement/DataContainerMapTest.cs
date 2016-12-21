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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerMapTest : StandardMappingTest
  {
    private IClientTransactionEventSink _eventSinkWithDynamicMock;
    private DataContainerMap _map;

    public override void SetUp ()
    {
      base.SetUp();

      _eventSinkWithDynamicMock = MockRepository.GenerateMock<IClientTransactionEventSink>();
      _map = new DataContainerMap (_eventSinkWithDynamicMock);
    }

    [Test]
    public void CommitAllDataContainers_CommitsDataContainers ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      _map.Register (dataContainer);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.New));

      _map.CommitAllDataContainers();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void RollbackAllDataContainers_RollsbackDataContainers ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      dataContainer.Delete();
      _map.Register (dataContainer);
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Deleted));

      _map.RollbackAllDataContainers();

      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Remove_RemovesDataContainer ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _map.Register (dataContainer);
      Assert.That (_map[DomainObjectIDs.Order1], Is.Not.Null);

      _map.Remove (dataContainer.ID);

      Assert.That (_map[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void Remove_RaisesNotification_BeforeRemoving ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      _map.Register (dataContainer);

      Assert.That (_map[dataContainer.ID], Is.Not.Null);

      _eventSinkWithDynamicMock.Expect (mock => mock.RaiseDataContainerMapUnregisteringEvent ( dataContainer))
          .WhenCalled (mi => Assert.That (_map[dataContainer.ID], Is.Not.Null));

      _map.Remove (dataContainer.ID);

      _eventSinkWithDynamicMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Data container 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not part of this map.\r\nParameter name: id")]
    public void Remove_NonExistingDataContainer ()
    {
      _map.Remove (DomainObjectIDs.Order1);
    }
  }
}