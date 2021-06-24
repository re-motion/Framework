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
using System.Collections;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;
using MockRepository = Rhino.Mocks.MockRepository;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Reset : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;
    private Mock<ITransaction> _newTransactionMock;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      _newTransactionMock = new Mock<ITransaction> (MockBehavior.Strict);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      var object1 = new object ();
      var object2 = new object ();

      TransactionMock.BackToRecord();
      TransactionFactoryMock.BackToRecord ();
      var sequence = new MockSequence();
      TransactionMock.Setup (mock => mock.Release()).Verifiable();
      TransactionFactoryMock.Setup (mock => mock.Create ()).Returns (_newTransactionMock.Object).Verifiable();
      ExecutionContextMock.Setup (mock => mock.GetVariables()).Returns (new[] { object1, object2 }).Verifiable();
      _newTransactionMock.InSequence (sequence).Setup (mock => mock.EnsureCompatibility (It.Is<IEnumerable> (_ => new[] { object1, object2 }.All (_.Contains)))).Verifiable();
      Assert.That (_strategy.Scope, Is.Null);

      _strategy.Reset();

      _newTransactionMock.Verify();
      Assert.That (_strategy.Scope, Is.Null);
      Assert.That (_strategy.Transaction, Is.SameAs (_newTransactionMock.Object));
    }

    [Test]
    public void Test_WithoutScope_And_CreateThrows ()
    {
      var exception = new ApplicationException ("Reset Exception");

      TransactionMock.BackToRecord ();
      TransactionMock.Setup (mock => mock.Release ()).Verifiable();
      TransactionFactoryMock.BackToRecord();
      TransactionFactoryMock.Setup (mock => mock.Create ()).Throws (exception).Verifiable();

      Assert.That (_strategy.Scope, Is.Null);

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      _newTransactionMock.Verify();
      Assert.That (_strategy.Scope, Is.Null);
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock), "Transaction just released is retained.");
    }

    [Test]
    public void Test_WithScope ()
    {
      var object1 = new object ();
      var object2 = new object ();

      InvokeOnExecutionPlay (_strategy);
      TransactionMock.BackToRecord ();
      TransactionFactoryMock.BackToRecord();
      var newScopeMock = new Mock<ITransactionScope> (MockBehavior.Strict);

      var sequence = new MockSequence();

      ScopeMock.Setup (mock => mock.Leave()).Verifiable();

      TransactionMock.Setup (mock => mock.Release()).Verifiable();

      TransactionFactoryMock.Setup (mock => mock.Create ()).Returns (_newTransactionMock.Object).Verifiable();
      _newTransactionMock.InSequence (sequence).Setup (mock => mock.EnterScope ()).Returns (newScopeMock.Object).Verifiable();

      ExecutionContextMock.Setup (mock => mock.GetVariables()).Returns (new[] { object1, object2 }).Verifiable();
      _newTransactionMock.InSequence (sequence).Setup (mock => mock.EnsureCompatibility (It.Is<IEnumerable> (_ => new[] { object1, object2 }.All (_.Contains)))).Verifiable();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));

      _strategy.Reset();

      _newTransactionMock.Verify();
      newScopeMock.Verify();
      Assert.That (_strategy.Scope, Is.SameAs (newScopeMock.Object));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Leave Exception");
      var sequence = new MockSequence();
      ScopeMock.Setup (mock => mock.Leave ()).Throws (exception).Verifiable();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      _newTransactionMock.Verify();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_ResetThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Reset Exception");
      TransactionFactoryMock.BackToRecord ();
      TransactionMock.BackToRecord ();
      var sequence = new MockSequence();
      ScopeMock.Setup (mock => mock.Leave()).Verifiable();
      TransactionMock.Setup (mock => mock.Release ()).Verifiable();
      TransactionFactoryMock.Setup (mock => mock.Create ()).Throws (exception).Verifiable();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      _newTransactionMock.Verify();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock), "Transaction just released is retained.");
    }

    [Test]
    public void Test_WithIncompatibleVariables ()
    {
      var object1 = new object ();
      var object2 = new object ();
      var invalidOperationException = new InvalidOperationException ("Oh nos!");

      TransactionMock.BackToRecord ();
      TransactionFactoryMock.BackToRecord ();
      TransactionMock.Setup (mock => mock.Release());
      TransactionFactoryMock.Setup (mock => mock.Create()).Returns (_newTransactionMock.Object);

      ExecutionContextMock.Setup (mock => mock.GetVariables()).Returns (new[] { object1, object2 });
      _newTransactionMock
          .Setup (mock => mock.EnsureCompatibility (It.Is<IEnumerable> (_ => new[] { object1, object2 }.All (_.Contains))))
          .Throws (invalidOperationException);

      Assert.That (
          () => _strategy.Reset(),
          Throws
              .TypeOf<WxeException>()
              .With.Message.EqualTo (
                "One or more of the variables of the WxeFunction are incompatible with the new transaction after the Reset. Oh nos! "
                + "(To avoid this exception, clear the Variables collection from incompatible objects before calling Reset and repopulate it "
                + "afterwards.)")
              .And.InnerException.SameAs (invalidOperationException));

      Assert.That (_strategy.Scope, Is.Null);
      Assert.That (_strategy.Transaction, Is.SameAs (_newTransactionMock.Object));
    }
  }
}
