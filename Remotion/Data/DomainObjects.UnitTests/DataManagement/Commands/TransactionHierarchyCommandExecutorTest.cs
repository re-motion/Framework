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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.Commands
{
  [TestFixture]
  public class TransactionHierarchyCommandExecutorTest : StandardMappingTest
  {
    public interface ICommandFactory
    {
      IDataManagementCommand Create (ClientTransaction tx);
    }

    private ClientTransaction _leafRootTransaction;

    private ClientTransaction _rootTransactionWithSub;
    private ClientTransaction _leafSubTransaction;

    private TestableClientTransaction _readOnlyTransaction;
    private Mock<ICommandFactory> _commandFactoryMock;
    private Mock<IDataManagementCommand> _commandMock1;
    private Mock<IDataManagementCommand> _commandMock2;
    private TransactionHierarchyCommandExecutor _executor;

    public override void SetUp ()
    {
      base.SetUp();

      _leafRootTransaction = ClientTransaction.CreateRootTransaction();

      _rootTransactionWithSub = ClientTransaction.CreateRootTransaction();
      _leafSubTransaction = _rootTransactionWithSub.CreateSubTransaction();

      _readOnlyTransaction = new TestableClientTransaction();
      ClientTransactionTestHelper.SetIsWriteable(_readOnlyTransaction, false);

      _commandFactoryMock = new Mock<ICommandFactory> (MockBehavior.Strict);
      _commandMock1 = new Mock<IDataManagementCommand> (MockBehavior.Strict);
      _commandMock2 = new Mock<IDataManagementCommand> (MockBehavior.Strict);

      _executor = new TransactionHierarchyCommandExecutor(_commandFactoryMock.Object.Create);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_LeafRootTransaction_True ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafRootTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      var result = _executor.TryExecuteCommandForTransactionHierarchy(_leafRootTransaction);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
      Assert.That(result, Is.True);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_LeafRootTransaction_False ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafRootTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new[] { new Exception() });

      _mockRepository.ReplayAll();

      var result = _executor.TryExecuteCommandForTransactionHierarchy(_leafRootTransaction);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToRoot ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.End()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      _executor.TryExecuteCommandForTransactionHierarchy(_rootTransactionWithSub);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToLeaf ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.End()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      _executor.TryExecuteCommandForTransactionHierarchy(_leafSubTransaction);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_NoBeginIfException_InLeaf ()
    {
      var exception = new Exception("Oh no!");
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new[] { exception });

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var result = _executor.TryExecuteCommandForTransactionHierarchy(_rootTransactionWithSub);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_NoBeginIfException_InRoot ()
    {
      var exception = new Exception("Oh no!");
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new[] { exception });

      var result = _executor.TryExecuteCommandForTransactionHierarchy(_rootTransactionWithSub);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
      Assert.That(result, Is.False);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_UnlocksReadOnlyTransaction ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_readOnlyTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.False)).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.True)).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.False)).Verifiable();

      Assert.That(_readOnlyTransaction.IsWriteable, Is.False);
      _executor.TryExecuteCommandForTransactionHierarchy(_readOnlyTransaction);
      Assert.That(_readOnlyTransaction.IsWriteable, Is.False);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_LeafRootTransaction ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafRootTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      _mockRepository.ReplayAll();

      _executor.ExecuteCommandForTransactionHierarchy(_leafRootTransaction);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToRoot ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.End()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      _executor.ExecuteCommandForTransactionHierarchy(_rootTransactionWithSub);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToLeaf ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Begin()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.Perform()).Verifiable();
      _commandMock2.InSequence (sequence).Setup (mock => mock.End()).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Verifiable();

      _executor.ExecuteCommandForTransactionHierarchy(_leafSubTransaction);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_NoBeginIfException_InLeaf ()
    {
      var exception = new Exception("Oh no!");
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new[] { exception });

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      Assert.That(() => _executor.ExecuteCommandForTransactionHierarchy(_rootTransactionWithSub), Throws.Exception.SameAs(exception));

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_NoBeginIfException_InSub ()
    {
      var exception = new Exception("Oh no!");
      _commandFactoryMock
          .Setup(mock => mock.Create(_leafSubTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      _commandFactoryMock
          .Setup(mock => mock.Create(_rootTransactionWithSub))
          .Returns(_commandMock2.Object)
          .Verifiable();
      _commandMock2.Setup (stub => stub.GetAllExceptions()).Returns (new[] { exception });

      Assert.That(() => _executor.ExecuteCommandForTransactionHierarchy(_rootTransactionWithSub), Throws.Exception.SameAs(exception));

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_UnlocksReadOnlyTransaction ()
    {
      _commandFactoryMock
          .Setup(mock => mock.Create(_readOnlyTransaction))
          .Returns(_commandMock1.Object)
          .Verifiable();
      _commandMock1.Setup (stub => stub.GetAllExceptions()).Returns (new Exception[0]);

      var sequence = new MockSequence();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Begin()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.False)).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.Perform()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.True)).Verifiable();
      _commandMock1.InSequence (sequence).Setup (mock => mock.End()).Callback (() => Assert.That (_readOnlyTransaction.IsWriteable, Is.False)).Verifiable();

      Assert.That(_readOnlyTransaction.IsWriteable, Is.False);
      _executor.ExecuteCommandForTransactionHierarchy(_readOnlyTransaction);
      Assert.That(_readOnlyTransaction.IsWriteable, Is.False);

      _commandFactoryMock.Verify();
      _commandMock1.Verify();
      _commandMock2.Verify();
    }
  }
}
