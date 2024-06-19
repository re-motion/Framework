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
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class SingleObjectLoadCommandTest
  {
    private Mock<IDbCommandBuilder> _dbCommandBuilderMock;
    private Mock<IDbCommand> _dbCommandMock;
    private Mock<IDataReader> _dataReaderMock;
    private SingleObjectLoadCommand<object> _command;
    private Mock<IObjectReader<object>> _objectReaderMock;
    private object _fakeResult;
    private Mock<IRdbmsProviderReadWriteCommandExecutionContext> _readWriteExecutionContextMock;
    private Mock<IRdbmsProviderReadOnlyCommandExecutionContext> _readOnlyExecutionContextMock;

    [SetUp]
    public void SetUp ()
    {
      _fakeResult = new object();

      _dataReaderMock = new Mock<IDataReader>();
      _readWriteExecutionContextMock = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>(MockBehavior.Strict);
      _readOnlyExecutionContextMock = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>(MockBehavior.Strict);
      _dbCommandMock = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandBuilderMock = new Mock<IDbCommandBuilder>(MockBehavior.Strict);
      _objectReaderMock = new Mock<IObjectReader<object>>(MockBehavior.Strict);

      _command = new SingleObjectLoadCommand<object>(_dbCommandBuilderMock.Object, _objectReaderMock.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_command.DbCommandBuilder, Is.SameAs(_dbCommandBuilderMock.Object));
      Assert.That(_command.ObjectReader, Is.SameAs(_objectReaderMock.Object));
    }

    [Test]
    public void Execute ()
    {
      _readWriteExecutionContextMock
          .Setup(mock => mock.ExecuteReader(_dbCommandMock.Object, CommandBehavior.SingleRow))
          .Returns(_dataReaderMock.Object)
          .Verifiable();

      _objectReaderMock.Setup(mock => mock.Read(_dataReaderMock.Object)).Returns(_fakeResult).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(_readWriteExecutionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(_readWriteExecutionContextMock.Object);

      _readWriteExecutionContextMock.Verify();
      _objectReaderMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.SameAs(_fakeResult));
    }

    [Test]
    public void Execute_NullContainer ()
    {
      _readWriteExecutionContextMock
          .Setup(mock => mock.ExecuteReader(_dbCommandMock.Object, CommandBehavior.SingleRow))
          .Returns(_dataReaderMock.Object)
          .Verifiable();

      _objectReaderMock.Setup(mock => mock.Read(_dataReaderMock.Object)).Returns((object)null).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(_readWriteExecutionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(_readWriteExecutionContextMock.Object);

      _readWriteExecutionContextMock.Verify();
      _objectReaderMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.Null);
    }

    [Test]
    public void ExecuteReadOnly ()
    {
      _readOnlyExecutionContextMock
          .Setup(mock => mock.ExecuteReader(_dbCommandMock.Object, CommandBehavior.SingleRow))
          .Returns(_dataReaderMock.Object)
          .Verifiable();

      _objectReaderMock.Setup(mock => mock.Read(_dataReaderMock.Object)).Returns(_fakeResult).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(_readOnlyExecutionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(_readOnlyExecutionContextMock.Object);

      _readOnlyExecutionContextMock.Verify();
      _objectReaderMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.SameAs(_fakeResult));
    }

    [Test]
    public void ExecuteReadOnly_NullContainer ()
    {
      _readOnlyExecutionContextMock
          .Setup(mock => mock.ExecuteReader(_dbCommandMock.Object, CommandBehavior.SingleRow))
          .Returns(_dataReaderMock.Object)
          .Verifiable();

      _objectReaderMock.Setup(mock => mock.Read(_dataReaderMock.Object)).Returns((object)null).Verifiable();

      _dbCommandBuilderMock.Setup(mock => mock.Create(_readOnlyExecutionContextMock.Object)).Returns(_dbCommandMock.Object).Verifiable();

      _dbCommandMock.Setup(mock => mock.Dispose()).Verifiable();

      var result = _command.Execute(_readOnlyExecutionContextMock.Object);

      _readOnlyExecutionContextMock.Verify();
      _objectReaderMock.Verify();
      _dbCommandBuilderMock.Verify();
      _dbCommandMock.Verify();
      Assert.That(result, Is.Null);
    }
  }
}
