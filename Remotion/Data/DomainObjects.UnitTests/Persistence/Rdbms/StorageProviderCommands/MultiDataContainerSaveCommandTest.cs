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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerSaveCommandTest : StandardMappingTest
  {
    private ObjectID _objectID1;
    private ObjectID _objectID2;

    private Mock<IDbCommandBuilder> _dbCommandBuilderMock1;
    private Mock<IDbCommandBuilder> _dbCommandBuilderMock2;

    private Mock<IDbCommand> _dbCommandMock1;
    private Mock<IDbCommand> _dbCommandMock2;

    private Mock<IRdbmsProviderReadWriteCommandExecutionContext> _rdbmsExecutionContextStrictMock;

    private Tuple<ObjectID, IDbCommandBuilder> _tuple1;
    private Tuple<ObjectID, IDbCommandBuilder> _tuple2;

    public override void SetUp ()
    {
      base.SetUp();

      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order3;

      _dbCommandBuilderMock1 = new Mock<IDbCommandBuilder>(MockBehavior.Strict);
      _dbCommandBuilderMock2 = new Mock<IDbCommandBuilder>(MockBehavior.Strict);

      _dbCommandMock1 = new Mock<IDbCommand>(MockBehavior.Strict);
      _dbCommandMock2 = new Mock<IDbCommand>(MockBehavior.Strict);

      _rdbmsExecutionContextStrictMock = new Mock<IRdbmsProviderReadWriteCommandExecutionContext>(MockBehavior.Strict);

      _tuple1 = Tuple.Create(_objectID1, _dbCommandBuilderMock1.Object);
      _tuple2 = Tuple.Create(_objectID2, _dbCommandBuilderMock2.Object);
    }

    [Test]
    public void Execute_NullCommand ()
    {
      var command = new MultiDataContainerSaveCommand(new[] { _tuple1 });

      _dbCommandBuilderMock1.Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object)).Returns((IDbCommand)null).Verifiable();

      command.Execute(_rdbmsExecutionContextStrictMock.Object);

      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _rdbmsExecutionContextStrictMock.Verify();
    }

    [Test]
    public void Execute_NoAffectedRecords ()
    {
      var command = new MultiDataContainerSaveCommand(new[] { _tuple1 });

      var sequence = new VerifiableSequence();

      _dbCommandBuilderMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      _rdbmsExecutionContextStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecuteNonQuery(_dbCommandMock1.Object))
          .Returns(0)
          .Verifiable();
      _dbCommandMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      var exception = Assert.Throws<ConcurrencyViolationException>(() => command.Execute(_rdbmsExecutionContextStrictMock.Object));
      Assert.That(exception.IDs, Is.EqualTo(new ObjectID[] { _tuple1.Item1 }));

      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _rdbmsExecutionContextStrictMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Execute_ExceptionOccurs ()
    {
      var command = new MultiDataContainerSaveCommand(new[] { _tuple1 });

      _dbCommandBuilderMock1
          .Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();
      var rdbmsProviderException = new RdbmsProviderException("Text");
      _rdbmsExecutionContextStrictMock
          .Setup(mock => mock.ExecuteNonQuery(_dbCommandMock1.Object))
          .Throws(rdbmsProviderException)
          .Verifiable();
      _dbCommandMock1.Setup(mock => mock.Dispose()).Verifiable();

      Assert.That(
          () => command.Execute(_rdbmsExecutionContextStrictMock.Object),
          Throws.Exception.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while saving object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. Text")
              .And.InnerException.SameAs(rdbmsProviderException));

      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _rdbmsExecutionContextStrictMock.Verify();
    }

    [Test]
    public void Execute_OneTuple ()
    {
      var command = new MultiDataContainerSaveCommand(new[] { _tuple1 });

      var sequence = new VerifiableSequence();

      _dbCommandBuilderMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object))
          .Returns(_dbCommandMock1.Object)
          .Verifiable();

      _rdbmsExecutionContextStrictMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.ExecuteNonQuery(_dbCommandMock1.Object))
          .Returns(1)
          .Verifiable();

      _dbCommandMock1
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.Dispose())
          .Verifiable();

      command.Execute(_rdbmsExecutionContextStrictMock.Object);

      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _rdbmsExecutionContextStrictMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Execute_SeveralTuples ()
    {
      var command = new MultiDataContainerSaveCommand(new[] { _tuple1, _tuple2 });

      var sequence = new VerifiableSequence();

      _dbCommandBuilderMock1.Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object)).Returns(_dbCommandMock1.Object).Verifiable();

      _rdbmsExecutionContextStrictMock.Setup(mock => mock.ExecuteNonQuery(_dbCommandMock1.Object)).Returns(1).Verifiable();
      _dbCommandMock1.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();

      _dbCommandBuilderMock2.Setup(mock => mock.Create(_rdbmsExecutionContextStrictMock.Object)).Returns(_dbCommandMock2.Object).Verifiable();

      _rdbmsExecutionContextStrictMock.Setup(mock => mock.ExecuteNonQuery(_dbCommandMock2.Object)).Returns(1).Verifiable();
      _dbCommandMock2.InVerifiableSequence(sequence).Setup(mock => mock.Dispose()).Verifiable();

      command.Execute(_rdbmsExecutionContextStrictMock.Object);

      _dbCommandBuilderMock1.Verify();
      _dbCommandBuilderMock2.Verify();
      _dbCommandMock1.Verify();
      _dbCommandMock2.Verify();
      _rdbmsExecutionContextStrictMock.Verify();
      sequence.Verify();
    }
  }
}
