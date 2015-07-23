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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiDataContainerSaveCommandTest : StandardMappingTest
  {
    private ObjectID _objectID1;
    private ObjectID _objectID2;

    private MockRepository _mockRepository;

    private IDbCommandBuilder _dbCommandBuilderMock1;
    private IDbCommandBuilder _dbCommandBuilderMock2;

    private IDbCommand _dbCommandMock1;
    private IDbCommand _dbCommandMock2;

    private IRdbmsProviderCommandExecutionContext _rdbmsExecutionContextStrictMock;

    private Tuple<ObjectID, IDbCommandBuilder> _tuple1;
    private Tuple<ObjectID, IDbCommandBuilder> _tuple2;

    public override void SetUp ()
    {
      base.SetUp ();

      _objectID1 = DomainObjectIDs.Order1;
      _objectID2 = DomainObjectIDs.Order3;

      _mockRepository = new MockRepository();

      _dbCommandBuilderMock1 = _mockRepository.StrictMock<IDbCommandBuilder>();
      _dbCommandBuilderMock2 = _mockRepository.StrictMock<IDbCommandBuilder> ();

      _dbCommandMock1 = _mockRepository.StrictMock<IDbCommand>();
      _dbCommandMock2 = _mockRepository.StrictMock<IDbCommand> ();

      _rdbmsExecutionContextStrictMock = _mockRepository.StrictMock<IRdbmsProviderCommandExecutionContext>();

      _tuple1 = Tuple.Create (_objectID1, _dbCommandBuilderMock1);
      _tuple2 = Tuple.Create (_objectID2, _dbCommandBuilderMock2);
    }

    [Test]
    public void Execute_NullCommand ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilderMock1.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (null);

      _mockRepository.ReplayAll();

      command.Execute (_rdbmsExecutionContextStrictMock);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_NoAffectedRecords ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      using (_mockRepository.Ordered ())
      {
        _dbCommandBuilderMock1.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (_dbCommandMock1);
        _rdbmsExecutionContextStrictMock.Expect (mock => mock.ExecuteNonQuery (_dbCommandMock1)).Return (0);
        _dbCommandMock1.Expect (mock => mock.Dispose());
      }

      _mockRepository.ReplayAll();

      Assert.That (
          () => command.Execute (_rdbmsExecutionContextStrictMock),
          Throws.Exception.TypeOf<ConcurrencyViolationException>().With.Message.EqualTo (
              "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else."));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_ExceptionOccurs ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      _dbCommandBuilderMock1.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (_dbCommandMock1);
      var rdbmsProviderException = new RdbmsProviderException ("Text");
      _rdbmsExecutionContextStrictMock
          .Expect (mock => mock.ExecuteNonQuery (_dbCommandMock1))
          .Throw (rdbmsProviderException);
      _dbCommandMock1.Expect (mock => mock.Dispose());

      _mockRepository.ReplayAll();

      Assert.That (
          () => command.Execute (_rdbmsExecutionContextStrictMock),
          Throws.Exception.TypeOf<RdbmsProviderException>()
              .With.Message.EqualTo ("Error while saving object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'. Text")
              .And.InnerException.SameAs (rdbmsProviderException));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_OneTuple ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1 });

      using (_mockRepository.Ordered ())
      {
        _dbCommandBuilderMock1.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (_dbCommandMock1);
        _rdbmsExecutionContextStrictMock.Expect (mock => mock.ExecuteNonQuery (_dbCommandMock1)).Return (1);
        _dbCommandMock1.Dispose();
      }

      _mockRepository.ReplayAll();

      command.Execute (_rdbmsExecutionContextStrictMock);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Execute_SeveralTuples ()
    {
      var command = new MultiDataContainerSaveCommand (new[] { _tuple1, _tuple2 });

      using (_mockRepository.Ordered ())
      {
        _dbCommandBuilderMock1.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (_dbCommandMock1);
        _rdbmsExecutionContextStrictMock.Expect (mock => mock.ExecuteNonQuery (_dbCommandMock1)).Return (1);
        _dbCommandMock1.Expect (mock => mock.Dispose());

        _dbCommandBuilderMock2.Expect (mock => mock.Create (_rdbmsExecutionContextStrictMock)).Return (_dbCommandMock2);
        _rdbmsExecutionContextStrictMock.Expect (mock => mock.ExecuteNonQuery (_dbCommandMock2)).Return (1);
        _dbCommandMock2.Expect (mock => mock.Dispose());
      }

      _mockRepository.ReplayAll();

      command.Execute (_rdbmsExecutionContextStrictMock);

      _mockRepository.VerifyAll();
    }
  }
}