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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class EnsureCompatibility : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test ()
    {
      var object1 = new object();
      var object2 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, object2 });
        TransactionMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, () => TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void Test_WithIncompatibleInParameters ()
    {
      var object1 = new object();
      var invalidOperationException = new InvalidOperationException ("Objects no good!");

      ExecutionContextMock.Stub (mock => mock.GetInParameters()).Return (new[] { object1 });
      TransactionMock
          .Stub (mock => mock.EnsureCompatibility (Arg<IEnumerable>.List.ContainsAll (new[] { object1 })))
          .Throw (invalidOperationException);
      MockRepository.ReplayAll();

      Assert.That (
          () => new RootTransactionStrategy (false, () => TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock),
          Throws
              .TypeOf<WxeException>()
              .With.Message.EqualTo (
                  "One or more of the input parameters passed to the WxeFunction are incompatible with the function's transaction. Objects no good!")
              .And.InnerException.SameAs (invalidOperationException));
    }

    [Test]
    public void Test_WithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, null, object2 });
        TransactionMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, () => TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void Test_Recursively ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, new[] { object2, object3 } });
        TransactionMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2, object3 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, () => TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }

    [Test]
    public void Test_RecursivelyWithNullValue ()
    {
      var object1 = new object();
      var object2 = new object();
      var object3 = new object();

      using (MockRepository.Ordered())
      {
        ExecutionContextMock.Expect (mock => mock.GetInParameters()).Return (new[] { object1, new[] { object2, null, object3 } });
        TransactionMock.Expect (mock => mock.EnsureCompatibility (Arg<IEnumerable>.List.ContainsAll (new[] { object1, object2, object3 })));
      }
      MockRepository.ReplayAll();

      new RootTransactionStrategy (false, () => TransactionMock, NullTransactionStrategy.Null, ExecutionContextMock);

      MockRepository.VerifyAll();
    }
  }
}
