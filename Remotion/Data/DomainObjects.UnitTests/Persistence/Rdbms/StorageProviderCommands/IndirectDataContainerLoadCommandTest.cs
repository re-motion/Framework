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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class IndirectDataContainerLoadCommandTest : StandardMappingTest
  {
    private Mock<IStorageProviderCommand<IEnumerable<ObjectID>>> _objectIDLoadCommandStub;
    private Mock<IStorageProviderCommand<ObjectLookupResult<DataContainer>[]>> _dataContainerLoadCommandStub;
    private Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>> _storageProviderFactoryStub;

    private IndirectDataContainerLoadCommand _loadCommand;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectLookupResult<DataContainer>[] _fakeResult;
    private Mock<IRdbmsProviderCommandExecutionContext> _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _fakeResult = new ObjectLookupResult<DataContainer>[0];
      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order3;
      _commandExecutionContextStub = new Mock<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub = new Mock<IRdbmsProviderCommandExecutionContext>();

      _objectIDLoadCommandStub = new Mock<IStorageProviderCommand<IEnumerable<ObjectID>>>();
      _objectIDLoadCommandStub.Setup(stub => stub.Execute(_commandExecutionContextStub.Object)).Returns(new[] { _objectID1, _objectID2 });

      _dataContainerLoadCommandStub =
          new Mock<IStorageProviderCommand<ObjectLookupResult<DataContainer>[]>>();
      _dataContainerLoadCommandStub.Setup(stub => stub.Execute(_commandExecutionContextStub.Object)).Returns(_fakeResult);

      _storageProviderFactoryStub = new Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();
      _storageProviderFactoryStub
          .Setup(stub => stub.CreateForSortedMultiIDLookup(new[] { _objectID1, _objectID2 }))
          .Returns(_dataContainerLoadCommandStub.Object);

      _loadCommand = new IndirectDataContainerLoadCommand(_objectIDLoadCommandStub.Object, _storageProviderFactoryStub.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_loadCommand.ObjectIDLoadCommand, Is.SameAs(_objectIDLoadCommandStub.Object));
      Assert.That(_loadCommand.StorageProviderCommandFactory, Is.SameAs(_storageProviderFactoryStub.Object));
    }

    [Test]
    public void Execute ()
    {
      var result = _loadCommand.Execute(_commandExecutionContextStub.Object);

      Assert.That(result, Is.SameAs(_fakeResult));
    }
  }
}
