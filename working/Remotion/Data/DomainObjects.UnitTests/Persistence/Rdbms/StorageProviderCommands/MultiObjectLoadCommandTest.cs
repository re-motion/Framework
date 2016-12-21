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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands
{
  [TestFixture]
  public class MultiObjectLoadCommandTest
  {
    private MockRepository _repository;

    private IDataReader _dataReaderMock;
    private IRdbmsProviderCommandExecutionContext _commandExecutionContextStub;

    private IDbCommand _dbCommandMock1;
    private IDbCommand _dbCommandMock2;
    private IDbCommandBuilder _dbCommandBuilderMock1;
    private IDbCommandBuilder _dbCommandBuilderMock2;

    private IObjectReader<object> _objectReaderStub1;
    private IObjectReader<object> _objectReaderStub2;

    private object _fakeResult1;
    private object _fakeResult2;
    
    [SetUp]
    public void SetUp ()
    {
      _repository = new MockRepository();

      _dataReaderMock = _repository.StrictMock<IDataReader>();
      _commandExecutionContextStub = _repository.Stub<IRdbmsProviderCommandExecutionContext>();

      _dbCommandMock1 = _repository.StrictMock<IDbCommand>();
      _dbCommandMock2 = _repository.StrictMock<IDbCommand>();
      _dbCommandBuilderMock1 = _repository.StrictMock<IDbCommandBuilder>();
      _dbCommandBuilderMock2 = _repository.StrictMock<IDbCommandBuilder>();

      _objectReaderStub1 = _repository.Stub<IObjectReader<object>> ();
      _objectReaderStub2 = _repository.Stub<IObjectReader<object>> ();

      _fakeResult1 = new object ();
      _fakeResult2 = new object ();
    }

    [Test]
    public void Initialization ()
    {
      var command = new MultiObjectLoadCommand<object> (
          new[]
          {
              Tuple.Create (_dbCommandBuilderMock1, _objectReaderStub1), 
              Tuple.Create (_dbCommandBuilderMock2, _objectReaderStub2)
          });

      Assert.That (
          command.DbCommandBuildersAndReaders,
          Is.EqualTo (
              new[]
              {
                  Tuple.Create (_dbCommandBuilderMock1, _objectReaderStub1),
                  Tuple.Create (_dbCommandBuilderMock2, _objectReaderStub2)
              }));
    }

    [Test]
    public void Execute_OneCommandBuilder ()
    {
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _dbCommandBuilderMock1.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dataReaderMock.Expect (mock => mock.Dispose());
      _repository.ReplayAll();

      var command = new MultiObjectLoadCommand<object> (new[] { Tuple.Create (_dbCommandBuilderMock1, _objectReaderStub1) });

      _objectReaderStub1.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _fakeResult1 });
      var result = command.Execute (_commandExecutionContextStub).ToArray();

      Assert.That (result, Is.EqualTo (new[] { _fakeResult1 }));
      _repository.VerifyAll();
    }

    [Test]
    public void Execute_SeveralCommandBuilders ()
    {
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock2, CommandBehavior.SingleResult)).Return (_dataReaderMock);
      _dbCommandBuilderMock1.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
      _dbCommandBuilderMock2.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock2);
      _dbCommandMock1.Expect (mock => mock.Dispose());
      _dbCommandMock2.Expect (mock => mock.Dispose());
      _dataReaderMock.Expect (mock => mock.Dispose()).Repeat.Twice();
      _repository.ReplayAll();

      _objectReaderStub1.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _fakeResult1 });
      _objectReaderStub2.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (new[] { _fakeResult2 });

      var command = new MultiObjectLoadCommand<object> (
          new[] 
          { 
            Tuple.Create (_dbCommandBuilderMock1, _objectReaderStub1), 
            Tuple.Create (_dbCommandBuilderMock2, _objectReaderStub2) 
          });

      var result = command.Execute (_commandExecutionContextStub).ToArray();

      _repository.VerifyAll();
      Assert.That (result, Is.EqualTo (new[] { _fakeResult1, _fakeResult2 }));
    }

    [Test]
    public void Execute_EnsuresCorrectDisposeOrder ()
    {
      var enumerableStub = _repository.Stub<IEnumerable<object>> ();
      var enumeratorMock = _repository.StrictMock<IEnumerator<object>>();

      using (_repository.Ordered())
      {
        _dbCommandBuilderMock1.Expect (mock => mock.Create (_commandExecutionContextStub)).Return (_dbCommandMock1);
        _commandExecutionContextStub.Stub (stub => stub.ExecuteReader (_dbCommandMock1, CommandBehavior.SingleResult)).Return (_dataReaderMock);
        _objectReaderStub1.Stub (stub => stub.ReadSequence (_dataReaderMock)).Return (enumerableStub);
        enumerableStub.Stub (stub => stub.GetEnumerator()).Return (enumeratorMock);
        enumeratorMock.Expect (mock => mock.MoveNext()).Return (false);
        enumeratorMock.Expect (mock => mock.Dispose());
        _dataReaderMock.Expect (mock => mock.Dispose());
        _dbCommandMock1.Expect (mock => mock.Dispose());
      }

      _repository.ReplayAll();

      var command = new MultiObjectLoadCommand<object> (new[] { Tuple.Create (_dbCommandBuilderMock1, _objectReaderStub1) });

      var result = command.Execute(_commandExecutionContextStub);
      result.ToArray();

      _repository.VerifyAll();
    }
  }
}