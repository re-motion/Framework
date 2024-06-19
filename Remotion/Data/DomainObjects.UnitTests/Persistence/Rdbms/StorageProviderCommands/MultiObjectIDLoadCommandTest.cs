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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiObjectIDLoadCommandTest : StandardMappingTest
  {
    private Mock<IDbCommandBuilder> _dbCommandBuilder1Mock;
    private Mock<IDbCommandBuilder> _dbCommandBuilder2Mock;
    private MultiObjectIDLoadCommand _command;
    private Mock<IDbCommand> _dbCommandMock1;
    private Mock<IDbCommand> _dbCommandMock2;
    private Mock<IDataReader> _dataReaderMock;
    private Mock<IObjectReader<ObjectID>> _objectIDReaderStub;
    private ObjectID[] _fakeResult;
    private ObjectID _objectID1;

    public override void SetUp ()
    {
      base.SetUp();

      _dbCommandBuilder1Mock = new Mock<IDbCommandBuilder>(MockBehavior.Strict);
      _dbCommandBuilder2Mock = new Mock<IDbCommandBuilder>(MockBehavior.Strict);

      _objectIDReaderStub = new Mock<IObjectReader<ObjectID>>();

      _dbCommandMock1 = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandMock2 = new Mock<IDbCommand>(MockBehavior.Strict);
      _dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);

      _command = new MultiObjectIDLoadCommand(new[] { _dbCommandBuilder1Mock.Object, _dbCommandBuilder2Mock.Object }, _objectIDReaderStub.Object);

      _objectID1 = new ObjectID("Order", Guid.NewGuid());
      _fakeResult = new[] { _objectID1 };
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_command.DbCommandBuilders, Is.EqualTo(new[] { _dbCommandBuilder1Mock.Object, _dbCommandBuilder2Mock.Object }));
      Assert.That(_command.ObjectIDReader, Is.SameAs(_objectIDReaderStub.Object));
    }

    [Test]
    public void Execute_OneDbCommandBuilder ()
    {
      var sequence = new VerifiableSequence();

      var executionContextStub = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>();
      _dbCommandBuilder1Mock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(executionContextStub.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      executionContextStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult))
          .Returns(_dataReaderMock.Object);

      _objectIDReaderStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ReadSequence(_dataReaderMock.Object))
          .Returns(_fakeResult);

      _dataReaderMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      _dbCommandMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      var command = new MultiObjectIDLoadCommand(new[] { _dbCommandBuilder1Mock.Object }, _objectIDReaderStub.Object);

      var result = command.Execute(executionContextStub.Object).ToArray();

      _dbCommandBuilder1Mock.Verify();
      _dbCommandBuilder2Mock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dataReaderMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new[] { _objectID1 }));
    }

    [Test]
    public void Execute_SeveralDbCommandBuilders ()
    {
       var executionContextStub = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>();

      _dbCommandBuilder1Mock.Setup(mock => mock.Create(executionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandBuilder2Mock.Setup(mock => mock.Create(executionContextStub.Object)).Returns(_dbCommandMock2.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock2.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      executionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      executionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock2.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _objectIDReaderStub.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(_fakeResult);

      var command = new MultiObjectIDLoadCommand(
          new[] { _dbCommandBuilder1Mock.Object, _dbCommandBuilder2Mock.Object }, _objectIDReaderStub.Object);

      var result = command.Execute(executionContextStub.Object).ToArray();

      _dbCommandBuilder1Mock.Verify();
      _dbCommandBuilder2Mock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dataReaderMock.Verify(mock => mock.Dispose(), Times.Exactly(2));
      Assert.That(result, Is.EqualTo(new[] { _objectID1, _objectID1 }));
    }

    [Test]
    public void ExecuteReadOnly_OneDbCommandBuilder ()
    {
      var sequence = new VerifiableSequence();

      var executionContextStub = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>();

      _dbCommandBuilder1Mock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(executionContextStub.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      executionContextStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult))
          .Returns(_dataReaderMock.Object);

      _objectIDReaderStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ReadSequence(_dataReaderMock.Object))
          .Returns(_fakeResult);

      _dataReaderMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      _dbCommandMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      var command = new MultiObjectIDLoadCommand(new[] { _dbCommandBuilder1Mock.Object }, _objectIDReaderStub.Object);

      var result = command.Execute(executionContextStub.Object).ToArray();

      _dbCommandBuilder1Mock.Verify();
      _dbCommandBuilder2Mock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dataReaderMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.EqualTo(new[] { _objectID1 }));
    }

    [Test]
    public void ExecuteReadOnly_SeveralDbCommandBuilders ()
    {
      var executionContextStub = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>();

      _dbCommandBuilder1Mock.Setup(mock => mock.Create(executionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandBuilder2Mock.Setup(mock => mock.Create(executionContextStub.Object)).Returns(_dbCommandMock2.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock2.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      executionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      executionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock2.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _objectIDReaderStub.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(_fakeResult);

      var command = new MultiObjectIDLoadCommand(
          new[] { _dbCommandBuilder1Mock.Object, _dbCommandBuilder2Mock.Object }, _objectIDReaderStub.Object);

      var result = command.Execute(executionContextStub.Object).ToArray();

      _dbCommandBuilder1Mock.Verify();
      _dbCommandBuilder2Mock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dataReaderMock.Verify(mock => mock.Dispose(), Times.Exactly(2));
      Assert.That(result, Is.EqualTo(new[] { _objectID1, _objectID1 }));
    }
  }
}
