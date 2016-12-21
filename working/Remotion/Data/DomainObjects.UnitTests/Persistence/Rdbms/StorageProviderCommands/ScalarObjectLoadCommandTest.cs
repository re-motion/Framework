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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class ScalarValueLoadCommandTest
  {
    private IDbCommandBuilder _dbCommandBuilderMock;
    private IDbCommand _dbCommandMock;
    private ScalarValueLoadCommand _command;
    private object _fakeResult;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextMock;

    [SetUp]
    public void SetUp ()
    {
      _fakeResult = new object();
     
      _commandExecutionContextMock = MockRepository.GenerateStrictMock<IRdbmsProviderCommandExecutionContext>();
      _dbCommandMock = MockRepository.GenerateStrictMock<IDbCommand>();
      _dbCommandBuilderMock = MockRepository.GenerateStrictMock<IDbCommandBuilder>();

      _command = new ScalarValueLoadCommand (_dbCommandBuilderMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.DbCommandBuilder, Is.SameAs (_dbCommandBuilderMock));
    }

    [Test]
    public void Execute ()
    {
      _commandExecutionContextMock.Expect (mock => mock.ExecuteScalar (_dbCommandMock)).Return (_fakeResult);
      _commandExecutionContextMock.Replay();

      _dbCommandBuilderMock.Expect (mock => mock.Create (_commandExecutionContextMock)).Return (_dbCommandMock);
      _dbCommandBuilderMock.Replay();

      _dbCommandMock.Expect (mock => mock.Dispose());
      _dbCommandMock.Replay();
      
      var result = _command.Execute(_commandExecutionContextMock);

      _commandExecutionContextMock.VerifyAllExpectations();
      _dbCommandBuilderMock.VerifyAllExpectations();
      _dbCommandMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeResult));
    }
  }
}