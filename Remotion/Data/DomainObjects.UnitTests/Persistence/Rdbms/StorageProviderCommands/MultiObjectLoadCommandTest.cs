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
  public class MultiObjectLoadCommandTest
  {

    private Mock<IDataReader> _dataReaderMock;
    private Mock<IRdbmsProviderReadWriteCommandExecutionContext> _readWriteExecutionContextStub;
    private Mock<IRdbmsProviderReadOnlyCommandExecutionContext> _readOnlyExecutionContextStub;

    private Mock<IDbCommand> _dbCommandMock1;
    private Mock<IDbCommand> _dbCommandMock2;
    private Mock<IDbCommandBuilder> _dbCommandBuilderMock1;
    private Mock<IDbCommandBuilder> _dbCommandBuilderMock2;

    private Mock<IObjectReader<object>> _objectReaderStub1;
    private Mock<IObjectReader<object>> _objectReaderStub2;

    private object _fakeResult1;
    private object _fakeResult2;

    [SetUp]
    public void SetUp ()
    {
      _dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);
      _readWriteExecutionContextStub = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>();
      _readOnlyExecutionContextStub = new Mock<IRdbmsProviderReadOnlyCommandExecutionContext>();

      _dbCommandMock1 = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandMock2 = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandBuilderMock1 = new Mock<IDbCommandBuilder>(MockBehavior.Strict);
      _dbCommandBuilderMock2 = new Mock<IDbCommandBuilder>(MockBehavior.Strict);

      _objectReaderStub1 = new Mock<IObjectReader<object>>();
      _objectReaderStub2 = new Mock<IObjectReader<object>>();

      _fakeResult1 = new object();
      _fakeResult2 = new object();
    }

    [Test]
    public void Initialization ()
    {
      var command = new MultiObjectLoadCommand<object>(
          new[]
          {
              Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object),
              Tuple.Create(_dbCommandBuilderMock2.Object, _objectReaderStub2.Object)
          });

      Assert.That(
          command.DbCommandBuildersAndReaders,
          Is.EqualTo(
              new[]
              {
                  Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object),
                  Tuple.Create(_dbCommandBuilderMock2.Object, _objectReaderStub2.Object)
              }));
    }

    [Test]
    public void Execute_OneCommandBuilder ()
    {
      _readWriteExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _dbCommandBuilderMock1.Setup(mock => mock.Create(_readWriteExecutionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      var command = new MultiObjectLoadCommand<object>(new[] { Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object) });

      _objectReaderStub1.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult1 });
      var result = command.Execute(_readWriteExecutionContextStub.Object).ToArray();

      Assert.That(result, Is.EqualTo(new[] { _fakeResult1 }));
      _dataReaderMock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
    }

    [Test]
    public void Execute_SeveralCommandBuilders ()
    {
      _readWriteExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _readWriteExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock2.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _dbCommandBuilderMock1.Setup(mock => mock.Create(_readWriteExecutionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandBuilderMock2.Setup(mock => mock.Create(_readWriteExecutionContextStub.Object)).Returns(_dbCommandMock2.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock2.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      _objectReaderStub1.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult1 });
      _objectReaderStub2.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult2 });

      var command = new MultiObjectLoadCommand<object>(
          new[]
          {
            Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object),
            Tuple.Create(_dbCommandBuilderMock2.Object, _objectReaderStub2.Object)
          });

      var result = command.Execute(_readWriteExecutionContextStub.Object).ToArray();

      _dataReaderMock.Verify(mock => mock.Dispose(), Times.Exactly(2));
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeResult1, _fakeResult2 }));
    }

    [Test]
    public void Execute_EnsuresCorrectDisposeOrder ()
    {
      var enumerableStub = new Mock<IEnumerable<object>>();
      var enumeratorMock = new Mock<IEnumerator<object>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();

      _dbCommandBuilderMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(_readWriteExecutionContextStub.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      _readWriteExecutionContextStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult))
          .Returns(_dataReaderMock.Object);

      _objectReaderStub1
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ReadSequence(_dataReaderMock.Object))
          .Returns(enumerableStub.Object);

      enumerableStub.Setup(stub => stub.GetEnumerator()).Returns(enumeratorMock.Object);
      enumeratorMock.InVerifiableSequence(sequence).Setup(mock => mock.MoveNext()).Returns(false).Verifiable();
      enumeratorMock.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      var command = new MultiObjectLoadCommand<object>(new[] { Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object) });

      var result = command.Execute(_readWriteExecutionContextStub.Object);
      result.ToArray();

      _dataReaderMock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      enumeratorMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void ExecuteReadOnly_OneCommandBuilder ()
    {
      _readOnlyExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _dbCommandBuilderMock1.Setup(mock => mock.Create(_readOnlyExecutionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      var command = new MultiObjectLoadCommand<object>(new[] { Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object) });

      _objectReaderStub1.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult1 });
      var result = command.Execute(_readOnlyExecutionContextStub.Object).ToArray();

      Assert.That(result, Is.EqualTo(new[] { _fakeResult1 }));
      _dataReaderMock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
    }

    [Test]
    public void ExecuteReadOnly_SeveralCommandBuilders ()
    {
      _readOnlyExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _readOnlyExecutionContextStub.Setup(stub => stub.ExecuteReader(_dbCommandMock2.Object, CommandBehavior.SingleResult)).Returns(_dataReaderMock.Object);
      _dbCommandBuilderMock1.Setup(mock => mock.Create(_readOnlyExecutionContextStub.Object)).Returns(_dbCommandMock1.Object).Verifiable();
      _dbCommandBuilderMock2.Setup(mock => mock.Create(_readOnlyExecutionContextStub.Object)).Returns(_dbCommandMock2.Object).Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock2.Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.Setup(mock => mock.Dispose()).Verifiable();

      _objectReaderStub1.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult1 });
      _objectReaderStub2.Setup(stub => stub.ReadSequence(_dataReaderMock.Object)).Returns(new[] { _fakeResult2 });

      var command = new MultiObjectLoadCommand<object>(
          new[]
          {
            Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object),
            Tuple.Create(_dbCommandBuilderMock2.Object, _objectReaderStub2.Object)
          });

      var result = command.Execute(_readOnlyExecutionContextStub.Object).ToArray();

      _dataReaderMock.Verify(mock => mock.Dispose(), Times.Exactly(2));
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeResult1, _fakeResult2 }));
    }

    [Test]
    public void ExecuteReadOnly_EnsuresCorrectDisposeOrder ()
    {
      var enumerableStub = new Mock<IEnumerable<object>>();
      var enumeratorMock = new Mock<IEnumerator<object>>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();

      _dbCommandBuilderMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(_readOnlyExecutionContextStub.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      _readOnlyExecutionContextStub
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ExecuteReader(_dbCommandMock1.Object, CommandBehavior.SingleResult))
          .Returns(_dataReaderMock.Object);

      _objectReaderStub1
          .InVerifiableSequence(sequence)
          .Setup(stub => stub.ReadSequence(_dataReaderMock.Object))
          .Returns(enumerableStub.Object);

      enumerableStub.Setup(stub => stub.GetEnumerator()).Returns(enumeratorMock.Object);
      enumeratorMock.InVerifiableSequence(sequence).Setup(mock => mock.MoveNext()).Returns(false).Verifiable();
      enumeratorMock.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      _dataReaderMock.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      _dbCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();
      var command = new MultiObjectLoadCommand<object>(new[] { Tuple.Create(_dbCommandBuilderMock1.Object, _objectReaderStub1.Object) });

      var result = command.Execute(_readOnlyExecutionContextStub.Object);
      result.ToArray();

      _dataReaderMock.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      enumeratorMock.Verify();
      sequence.Verify();
    }
  }
}
