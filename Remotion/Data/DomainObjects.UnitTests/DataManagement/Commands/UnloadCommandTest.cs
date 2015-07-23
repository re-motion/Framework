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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class UnloadCommandTest : StandardMappingTest
  {
    private MockRepository _mockRepository;

    private Order _domainObject1;
    private Order _domainObject2;

    private IClientTransactionEventSink _transactionEventSinkWithMock;

    private IDataManagementCommand _unloadDataCommandMock;

    private UnloadCommand _unloadCommand;
    private Exception _exception1;
    private Exception _exception2;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order> ();

      _transactionEventSinkWithMock = MockRepository.GenerateStrictMock<IClientTransactionEventSink>();

      _unloadDataCommandMock = _mockRepository.StrictMock<IDataManagementCommand>();

      _unloadCommand = new UnloadCommand (
          new[] { _domainObject1, _domainObject2 },
          _unloadDataCommandMock,
          _transactionEventSinkWithMock);

      _exception1 = new Exception ("1");
      _exception2 = new Exception ("2");
    }

    [Test]
    public void GetAllExceptions ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new[] { _exception1, _exception2 });
      _unloadDataCommandMock.Replay();

      Assert.That (_unloadCommand.GetAllExceptions(), Is.EqualTo (new[] { _exception1, _exception2 }));
    }

    [Test]
    public void Begin ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);

      using (_mockRepository.Ordered())
      {
        _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectsUnloadingEvent (
            Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _domainObject1, _domainObject2 })));
        _unloadDataCommandMock.Expect (mock => mock.Begin());
      }
      _mockRepository.ReplayAll();

      _unloadCommand.Begin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Begin_NonExecutable ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions()).Return (new[] { _exception1 });

      _mockRepository.ReplayAll();

      var exception = Assert.Throws<Exception> (_unloadCommand.Begin);
      Assert.That (exception, Is.SameAs (_exception1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Perform ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      _unloadDataCommandMock.Expect (mock => mock.Perform ());
      _mockRepository.ReplayAll();

      _unloadCommand.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Perform_NonExecutable ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new[] { _exception1 });
      _mockRepository.ReplayAll ();

      var exception = Assert.Throws<Exception> (_unloadCommand.Perform);
      Assert.That (exception, Is.SameAs (_exception1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);

      using (_mockRepository.Ordered ())
      {
        _unloadDataCommandMock.Expect (mock => mock.End ());
        _transactionEventSinkWithMock.Expect (mock => mock.RaiseObjectsUnloadedEvent (
            Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { _domainObject1, _domainObject2 })));
      }
      _mockRepository.ReplayAll ();

      _unloadCommand.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End_NonExecutable ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new[] { _exception1 });

      _mockRepository.ReplayAll ();

      var exception = Assert.Throws<Exception> (_unloadCommand.End);
      Assert.That (exception, Is.SameAs (_exception1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      _unloadDataCommandMock.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      _mockRepository.ReplayAll();

      var result = _unloadCommand.ExpandToAllRelatedObjects();

      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _unloadCommand }));
    }
  }
}