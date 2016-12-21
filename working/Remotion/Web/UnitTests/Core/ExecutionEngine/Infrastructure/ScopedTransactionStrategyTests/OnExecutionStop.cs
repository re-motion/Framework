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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop : ScopedTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        TransactionMock.Expect (mock => mock.Commit ());
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateScopedTransactionStrategy (true, OuterTransactionStrategyMock);
      var expectedObjects = new[] { new object() };

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        TransactionMock.Expect (mock => mock.Commit ());

        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (expectedObjects);
        OuterTransactionStrategyMock.Expect (mock => mock.EnsureCompatibility (expectedObjects));

        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Release());
      }

      MockRepository.ReplayAll();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithCommitTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        strategy.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "CommitTransaction"));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithReleaseTransactionOverride ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ScopeMock.Expect (mock => mock.Leave ());
        strategy.Expect (mock => PrivateInvoke.InvokeNonPublicMethod ( mock, "ReleaseTransaction"));
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context, ExecutionListenerStub);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);

      Assert.That (strategy.Scope, Is.Null);

      strategy.OnExecutionStop (Context, ExecutionListenerStub);
    }

    [Test]
    public void Test_ChildStrategyThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ChildStrategyThrowsFatalException ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new WxeFatalExecutionException (new Exception ("ChildStrategy Exception"), null);

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub)).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_EnsureCompatibility_ThrowsBecauseOfIncompatibleOutParameters ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var invalidOperationException = new InvalidOperationException ("Completely bad objects!");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));

      ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Return (new object[0]);
      OuterTransactionStrategyMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.Is.Anything)).Throw (invalidOperationException);

      MockRepository.ReplayAll();

      Assert.That (
          () => strategy.OnExecutionStop (Context, ExecutionListenerStub),
          Throws
              .TypeOf<WxeException> ()
              .With.Message.EqualTo (
                  "One or more of the output parameters returned from the WxeFunction are incompatible with the function's parent transaction. "
                  + "Completely bad objects!")
              .And.InnerException.SameAs (invalidOperationException));
      
      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_EnsureCompatibilityThrowsUnexpected_GetsBubbledOut ()
    {
      var strategy = CreateScopedTransactionStrategy (false, OuterTransactionStrategyMock);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));

      ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
      OuterTransactionStrategyMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.Is.Anything)).Throw (innerException);

      MockRepository.ReplayAll ();

      Assert.That (
          () => strategy.OnExecutionStop (Context, ExecutionListenerStub),
          Throws.Exception.SameAs (innerException));


      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
      ScopeMock.Expect (mock => mock.Leave()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateScopedTransactionStrategy (false, NullTransactionStrategy.Null);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      ChildTransactionStrategyMock.Expect (mock => mock.OnExecutionStop (Context, ExecutionListenerStub));
      ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
      ScopeMock.Expect (mock => mock.Leave());
      TransactionMock.Expect (mock => mock.Release()).Throw (innerException);

      MockRepository.ReplayAll();

      try
      {
        strategy.OnExecutionStop (Context, ExecutionListenerStub);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll();
      Assert.That (strategy.Scope, Is.Null);
    }
  }
}
