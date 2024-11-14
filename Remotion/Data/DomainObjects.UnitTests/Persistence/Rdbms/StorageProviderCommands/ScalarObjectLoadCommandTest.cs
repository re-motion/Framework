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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class ScalarValueLoadCommandTest
  {
    private Mock<IDbCommandBuilder> _dbCommandBuilderMock;
    private Mock<IDbCommand> _dbCommandMock;
    private ScalarValueLoadCommand _command;
    private object _fakeResult;

    [SetUp]
    public void SetUp ()
    {
      _fakeResult = new object();

      _dbCommandMock = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandBuilderMock = new Mock<IDbCommandBuilder>(MockBehavior.Strict);

      _command = new ScalarValueLoadCommand(_dbCommandBuilderMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_command.DbCommandBuilder, Is.SameAs(_dbCommandBuilderMock.Object));
    }

    [Test]
    public void Execute ()
    {
      var executionContextMock = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>(MockBehavior.Strict);
      executionContextMock.Setup(mock => mock.ExecuteScalar(_dbCommandMock.Object)).Returns(_fakeResult).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(executionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(executionContextMock.Object);

      executionContextMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.SameAs(_fakeResult));
    }

    [Test]
    public void ExecuteReadOnly ()
    {
      var executionContextMock = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>(MockBehavior.Strict);
      executionContextMock.Setup(mock => mock.ExecuteScalar(_dbCommandMock.Object)).Returns(_fakeResult).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(executionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(executionContextMock.Object);

      executionContextMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.SameAs(_fakeResult));
    }
  }
}
