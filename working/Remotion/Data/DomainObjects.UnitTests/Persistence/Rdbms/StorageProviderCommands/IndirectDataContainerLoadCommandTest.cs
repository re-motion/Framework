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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class IndirectDataContainerLoadCommandTest : StandardMappingTest
  {
    private IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext> _objectIDLoadCommandStub;
    private IStorageProviderCommand<ObjectLookupResult<DataContainer>[], IRdbmsProviderCommandExecutionContext> _dataContainerLoadCommandStub;
    private IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> _storageProviderFactoryStub;

    private IndirectDataContainerLoadCommand _loadCommand;
    private ObjectID _objectID1;
    private ObjectID _objectID2;
    private ObjectLookupResult<DataContainer>[] _fakeResult;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    public override void SetUp ()
    {
      base.SetUp();

      _fakeResult = new ObjectLookupResult<DataContainer>[0];
      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order3;
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();
      _commandExecutionContextStub = MockRepository.GenerateStub<IRdbmsProviderCommandExecutionContext>();

      _objectIDLoadCommandStub = MockRepository.GenerateStub<IStorageProviderCommand<IEnumerable<ObjectID>, IRdbmsProviderCommandExecutionContext>>();
      _objectIDLoadCommandStub.Stub (stub => stub.Execute (_commandExecutionContextStub)).Return (new[] { _objectID1, _objectID2 });

      _dataContainerLoadCommandStub =
          MockRepository.GenerateStub<IStorageProviderCommand<ObjectLookupResult<DataContainer>[], IRdbmsProviderCommandExecutionContext>>();
      _dataContainerLoadCommandStub.Stub (stub => stub.Execute (_commandExecutionContextStub)).Return (_fakeResult);

      _storageProviderFactoryStub = MockRepository.GenerateStub<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();
      _storageProviderFactoryStub
          .Stub (stub => stub.CreateForSortedMultiIDLookup (Arg<ObjectID[]>.List.Equal (new[] { _objectID1, _objectID2 })))
          .Return (_dataContainerLoadCommandStub);

      _loadCommand = new IndirectDataContainerLoadCommand (_objectIDLoadCommandStub, _storageProviderFactoryStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_loadCommand.ObjectIDLoadCommand, Is.SameAs (_objectIDLoadCommandStub));
      Assert.That (_loadCommand.StorageProviderCommandFactory, Is.SameAs (_storageProviderFactoryStub));
    }

    [Test]
    public void Execute ()
    {
      var result = _loadCommand.Execute (_commandExecutionContextStub);

      Assert.That (result, Is.SameAs (_fakeResult));
    }
  }
}