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
using Moq;
using NUnit.Framework;
using Remotion.Data;
using Remotion.FunctionalProgramming;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

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
      _strategy = CreateScopedTransactionStrategy(true, NullTransactionStrategy.Null).Object;
      _newTransactionMock = new Mock<ITransaction>(MockBehavior.Strict);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      var object1 = new object();
      var object2 = new object();

      TransactionMock.Reset();
      TransactionFactoryMock.Reset();

      var sequence = new VerifiableSequence();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      TransactionFactoryMock.InVerifiableSequence(sequence).Setup(mock => mock.Create()).Returns(_newTransactionMock.Object).Verifiable();

      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetVariables()).Returns(new[] { object1, object2 }).Verifiable();
      _newTransactionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2 }))))
          .Verifiable();
      Assert.That(_strategy.Scope, Is.Null);

      _strategy.Reset();

      VerifyAll();
      _newTransactionMock.Verify();
      sequence.Verify();
      Assert.That(_strategy.Scope, Is.Null);
      Assert.That(_strategy.Transaction, Is.SameAs(_newTransactionMock.Object));
    }

    [Test]
    public void Test_WithoutScope_And_CreateThrows ()
    {
      var exception = new ApplicationException("Reset Exception");

      TransactionMock.Reset();
      TransactionMock.Setup(mock => mock.Release()).Verifiable();
      TransactionFactoryMock.Reset();
      TransactionFactoryMock.Setup(mock => mock.Create()).Throws(exception).Verifiable();

      Assert.That(_strategy.Scope, Is.Null);

      try
      {
        _strategy.Reset();
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(exception));
      }
      VerifyAll();
      _newTransactionMock.Verify();
      Assert.That(_strategy.Scope, Is.Null);
      Assert.That(_strategy.Transaction, Is.SameAs(TransactionMock.Object), "Transaction just released is retained.");
    }

    [Test]
    public void Test_WithScope ()
    {
      var object1 = new object();
      var object2 = new object();

      InvokeOnExecutionPlay(_strategy);
      TransactionMock.Reset();
      TransactionFactoryMock.Reset();
      var newScopeMock = new Mock<ITransactionScope>(MockBehavior.Strict);

      var sequence = new VerifiableSequence();
      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      TransactionFactoryMock.InVerifiableSequence(sequence).Setup(mock => mock.Create()).Returns(_newTransactionMock.Object).Verifiable();
      _newTransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.EnterScope()).Returns(newScopeMock.Object).Verifiable();

      ExecutionContextMock.InVerifiableSequence(sequence).Setup(mock => mock.GetVariables()).Returns(new[] { object1, object2 }).Verifiable();
      _newTransactionMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2 }))))
          .Verifiable();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));

      _strategy.Reset();

      VerifyAll();
      _newTransactionMock.Verify();
      newScopeMock.Verify();
      sequence.Verify();
      Assert.That(_strategy.Scope, Is.SameAs(newScopeMock.Object));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay(_strategy);
      var exception = new ApplicationException("Leave Exception");
      var sequence = new VerifiableSequence();
      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Throws(exception).Verifiable();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));

      try
      {
        _strategy.Reset();
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(exception));
      }
      VerifyAll();
      sequence.Verify();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));
    }

    [Test]
    public void Test_WithScope_And_ResetThrows ()
    {
      InvokeOnExecutionPlay(_strategy);
      var exception = new ApplicationException("Reset Exception");
      TransactionFactoryMock.Reset();
      TransactionMock.Reset();
      var sequence = new VerifiableSequence();
      ScopeMock.InVerifiableSequence(sequence).Setup(mock => mock.Leave()).Verifiable();
      TransactionMock.InVerifiableSequence(sequence).Setup(mock => mock.Release()).Verifiable();
      TransactionFactoryMock.InVerifiableSequence(sequence).Setup(mock => mock.Create()).Throws(exception).Verifiable();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));

      try
      {
        _strategy.Reset();
        Assert.Fail("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That(actualException, Is.SameAs(exception));
      }
      VerifyAll();
      sequence.Verify();
      Assert.That(_strategy.Scope, Is.SameAs(ScopeMock.Object));
      Assert.That(_strategy.Transaction, Is.SameAs(TransactionMock.Object), "Transaction just released is retained.");
    }

    [Test]
    public void Test_WithIncompatibleVariables ()
    {
      var object1 = new object();
      var object2 = new object();
      var invalidOperationException = new InvalidOperationException("Oh nos!");

      TransactionMock.Reset();
      TransactionFactoryMock.Reset();
      TransactionMock.Setup(mock => mock.Release());
      TransactionFactoryMock.Setup(mock => mock.Create()).Returns(_newTransactionMock.Object);

      ExecutionContextMock.Setup(mock => mock.GetVariables()).Returns(new[] { object1, object2 });
      _newTransactionMock
          .Setup(mock => mock.EnsureCompatibility(It.Is<IEnumerable<object>>(p => p.SetEquals(new[] { object1, object2 }))))
          .Throws(invalidOperationException);

      Assert.That(
          () => _strategy.Reset(),
          Throws
              .TypeOf<WxeException>()
              .With.Message.EqualTo(
                "One or more of the variables of the WxeFunction are incompatible with the new transaction after the Reset. Oh nos! "
                + "(To avoid this exception, clear the Variables collection from incompatible objects before calling Reset and repopulate it "
                + "afterwards.)")
              .And.InnerException.SameAs(invalidOperationException));

      Assert.That(_strategy.Scope, Is.Null);
      Assert.That(_strategy.Transaction, Is.SameAs(_newTransactionMock.Object));
    }
  }
}
