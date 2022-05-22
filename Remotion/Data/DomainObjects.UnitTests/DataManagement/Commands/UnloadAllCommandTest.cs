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
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnloadAllCommandTest : StandardMappingTest
  {
    private Mock<IRelationEndPointManager> _endPointManagerMock;
    private DataContainerMap _dataContainerMap;
    private Mock<IClientTransactionEventSink> _transactionEventSinkWithMock;
    private Mock<IInvalidDomainObjectManager> _invalidDomainObjectManagerMock;

    private DataContainer _existingDataContainer;
    private TestDomainBase _existingDomainObject;

    private DataContainer _newDataContainer;
    private TestDomainBase _newDomainObject;

    private UnloadAllCommand _unloadCommand;

    public override void SetUp ()
    {
      base.SetUp();
      _endPointManagerMock = new Mock<IRelationEndPointManager>(MockBehavior.Strict);
      _transactionEventSinkWithMock = new Mock<IClientTransactionEventSink>();
      _dataContainerMap = new DataContainerMap(_transactionEventSinkWithMock.Object);
      _invalidDomainObjectManagerMock = new Mock<IInvalidDomainObjectManager>(MockBehavior.Strict);

      _existingDataContainer = CreateExistingDataContainer();
      _existingDomainObject = (TestDomainBase)_existingDataContainer.DomainObject;

      _newDataContainer = CreateNewDataContainer();
      _newDomainObject = (TestDomainBase)_newDataContainer.DomainObject;

      _unloadCommand = new UnloadAllCommand(_endPointManagerMock.Object, _dataContainerMap, _invalidDomainObjectManagerMock.Object, _transactionEventSinkWithMock.Object);
    }

    [Test]
    public void Begin ()
    {
      _dataContainerMap.Register(_existingDataContainer);
      _dataContainerMap.Register(_newDataContainer);

      // Order of registration
      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseObjectsUnloadingEvent(new[] { _existingDomainObject, _newDomainObject }))
          .Verifiable();

      _unloadCommand.Begin();

      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void Begin_ReexecutedForNewlyRegisteredObjects ()
    {
      _dataContainerMap.Register(_existingDataContainer);

      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseObjectsUnloadingEvent(new[] { _existingDomainObject }))
          .Callback((IReadOnlyList<IDomainObject> unloadedDomainObjects) => _dataContainerMap.Register(_newDataContainer))
          .Verifiable();
      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseObjectsUnloadingEvent(new[] { _newDomainObject }))
          .Verifiable();

      _unloadCommand.Begin();

      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void Perform_ClearsAndDiscardsDataContainers_AndResetsEndPoints ()
    {
      _dataContainerMap.Register(_existingDataContainer);
      Assert.That(_dataContainerMap, Is.Not.Empty.And.Member(_existingDataContainer));

      _endPointManagerMock
          .Setup(mock => mock.Reset())
          .Callback(() => Assert.That(_dataContainerMap, Is.Not.Empty))
          .Verifiable();

      _unloadCommand.Perform();

      Assert.That(_dataContainerMap, Is.Empty);
      _endPointManagerMock.Verify();
      Assert.That(_existingDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Perform_RaisesDataContainerUnregisteringEvents ()
    {
      _dataContainerMap.Register(_existingDataContainer);
      Assert.That(_dataContainerMap, Is.Not.Empty.And.Member(_existingDataContainer));
      _endPointManagerMock.Setup(mock => mock.Reset());

      _unloadCommand.Perform();

      _transactionEventSinkWithMock.Verify(mock => mock.RaiseDataContainerMapUnregisteringEvent(_existingDataContainer), Times.AtLeastOnce());
    }

    [Test]
    public void Perform_InvalidatesAndDiscardsNewDataContainers ()
    {
      _dataContainerMap.Register(_newDataContainer);
      _endPointManagerMock.Setup(mock => mock.Reset());

      _invalidDomainObjectManagerMock.Setup(mock => mock.MarkInvalid(_newDataContainer.DomainObject)).Returns(true).Verifiable();

      _unloadCommand.Perform();

      _invalidDomainObjectManagerMock.Verify();
      Assert.That(_newDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void End_WithoutPerform ()
    {
      _dataContainerMap.Register(_existingDataContainer);
      _dataContainerMap.Register(_newDataContainer);

      _unloadCommand.End();

      _transactionEventSinkWithMock.Verify(mock => mock.RaiseObjectsUnloadedEvent(It.IsAny<ReadOnlyCollection<DomainObject>>()), Times.Never());
    }

    [Test]
    public void End_WithPerform ()
    {
      _dataContainerMap.Register(_existingDataContainer);
      _dataContainerMap.Register(_newDataContainer);

      _invalidDomainObjectManagerMock.Setup(mock => mock.MarkInvalid(It.IsAny<DomainObject>())).Returns(true);
      _endPointManagerMock.Setup(mock => mock.Reset());
      _unloadCommand.Perform();

      // Order of registration
      _transactionEventSinkWithMock
          .Setup(mock => mock.RaiseObjectsUnloadedEvent(new[] { _existingDataContainer.DomainObject, _newDataContainer.DomainObject }))
          .Verifiable();

      _unloadCommand.End();

      _transactionEventSinkWithMock.Verify();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = _unloadCommand.ExpandToAllRelatedObjects();

      Assert.That(result.GetNestedCommands(), Is.EqualTo(new[] { _unloadCommand }));
    }

    private DataContainer CreateExistingDataContainer ()
    {
      var dataContainer = DataContainer.CreateForExisting(new ObjectID(typeof(Order), Guid.NewGuid()), null, pd => pd.DefaultValue);
      dataContainer.SetDomainObject(DomainObjectMother.CreateFakeObject<Order>(dataContainer.ID));
      return dataContainer;
    }

    private DataContainer CreateNewDataContainer ()
    {
      var dataContainer = DataContainer.CreateNew(new ObjectID(typeof(Order), Guid.NewGuid()));
      dataContainer.SetDomainObject(DomainObjectMother.CreateFakeObject<Order>(dataContainer.ID));
      return dataContainer;
    }
  }
}
