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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class SingleDataContainerAssociateWithIDCommandTest : StandardMappingTest
  {
    private ObjectID _expectedID;
    private Mock<IRdbmsProviderCommandWithReadOnlySupport<DataContainer>> _innerCommandMock;

    private SingleDataContainerAssociateWithIDCommand _associateCommand;

    private IRdbmsProviderReadWriteCommandExecutionContext _fakeReadWriteContext;
    private IRdbmsProviderReadOnlyCommandExecutionContext _fakeReadOnlyContext;

    public override void SetUp ()
    {
      base.SetUp();

      _expectedID = DomainObjectIDs.Order1;
      _innerCommandMock = new Mock<IRdbmsProviderCommandWithReadOnlySupport<DataContainer>>(MockBehavior.Strict);

      _associateCommand = new SingleDataContainerAssociateWithIDCommand(_expectedID, _innerCommandMock.Object);

      _fakeReadWriteContext = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>().Object;
      _fakeReadOnlyContext = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>().Object;
    }

    [Test]
    public void Execute_MatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create(_expectedID);
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadWriteContext)).Returns(dataContainer).Verifiable();

      var result = _associateCommand.Execute(_fakeReadWriteContext);

      _innerCommandMock.Verify();
      Assert.That(result.ObjectID, Is.EqualTo(_expectedID));
      Assert.That(result.LocatedObject, Is.SameAs(dataContainer));
    }

    [Test]
    public void Execute_NonMatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Order3);
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadWriteContext)).Returns(dataContainer).Verifiable();

      Assert.That(
          () => _associateCommand.Execute(_fakeReadWriteContext),
          Throws.TypeOf<PersistenceException>().With.Message.EqualTo(
            "The ObjectID of the loaded DataContainer 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' and the expected ObjectID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' differ."));
    }

    [Test]
    public void Execute_Null ()
    {
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadWriteContext)).Returns((DataContainer)null).Verifiable();

      var result = _associateCommand.Execute(_fakeReadWriteContext);

      _innerCommandMock.Verify();
      Assert.That(result.ObjectID, Is.EqualTo(_expectedID));
      Assert.That(result.LocatedObject, Is.Null);
    }

    [Test]
    public void ExecuteReadOnly_MatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create(_expectedID);
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadOnlyContext)).Returns(dataContainer).Verifiable();

      var result = _associateCommand.Execute(_fakeReadOnlyContext);

      _innerCommandMock.Verify();
      Assert.That(result.ObjectID, Is.EqualTo(_expectedID));
      Assert.That(result.LocatedObject, Is.SameAs(dataContainer));
    }

    [Test]
    public void ExecuteReadOnly_NonMatchingDataContainer ()
    {
      var dataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Order3);
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadOnlyContext)).Returns(dataContainer).Verifiable();

      Assert.That(
          () => _associateCommand.Execute(_fakeReadOnlyContext),
          Throws.TypeOf<PersistenceException>().With.Message.EqualTo(
            "The ObjectID of the loaded DataContainer 'Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid' and the expected ObjectID "
            + "'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' differ."));
    }

    [Test]
    public void ExecuteReadOnly_Null ()
    {
      _innerCommandMock.Setup(mock => mock.Execute(_fakeReadOnlyContext)).Returns((DataContainer)null).Verifiable();

      var result = _associateCommand.Execute(_fakeReadOnlyContext);

      _innerCommandMock.Verify();
      Assert.That(result.ObjectID, Is.EqualTo(_expectedID));
      Assert.That(result.LocatedObject, Is.Null);
    }
  }
}
