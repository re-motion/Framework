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
using System.Threading;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class ThreadAbortExceptionTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void ThreadAbortException ()
    {
      var function = new ThreadAbortTestTransactedFunction();
      Assert.That(() => function.Execute(Context), Throws.TypeOf<ThreadAbortException>());

      Assert.That(function.FirstStepExecuted, Is.True);
      Assert.That(function.SecondStepExecuted, Is.False);
      Assert.That(function.ThreadAborted, Is.True);

      function.Execute(Context);

      Assert.That(function.FirstStepExecuted, Is.True);
      Assert.That(function.SecondStepExecuted, Is.True);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction();
      ClientTransactionScope originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction(ClientTransactionScope.CurrentTransaction, nestedFunction);

      Assert.That(() => parentFunction.Execute(Context), Throws.TypeOf<ThreadAbortException>());

      Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(originalScope));

      Assert.That(nestedFunction.FirstStepExecuted, Is.True);
      Assert.That(nestedFunction.SecondStepExecuted, Is.False);
      Assert.That(nestedFunction.ThreadAborted, Is.True);

      parentFunction.Execute(Context);

      Assert.That(nestedFunction.FirstStepExecuted, Is.True);
      Assert.That(nestedFunction.SecondStepExecuted, Is.True);

      Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(originalScope));
      originalScope.Leave();
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunctionWithThreadMigration ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction();
      var originalScope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction(ClientTransactionScope.CurrentTransaction, nestedFunction);

      Assert.That(() => parentFunction.Execute(Context), Throws.TypeOf<ThreadAbortException>());

      Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(originalScope));

      ThreadRunner.Run(
          delegate
          {
            using (SafeContext.Instance.OpenSafeContextBoundary())
            {
              Assert.That(ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null before execute.");
              Assert.That(nestedFunction.FirstStepExecuted, Is.True);
              Assert.That(nestedFunction.SecondStepExecuted, Is.False);
              Assert.That(nestedFunction.ThreadAborted, Is.True);

              parentFunction.Execute(Context);

              Assert.That(nestedFunction.FirstStepExecuted, Is.True);
              Assert.That(nestedFunction.SecondStepExecuted, Is.True);
              Assert.That(ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null after execute.");
              //TODO: Before there was a transaction, now there isn't                           
              //Assert.That ( ClientTransactionScope.CurrentTransaction, Is.SameAs (originalScope.ScopedTransaction)); // but same transaction as on old thread
            }
          });

      originalScope.Leave();
    }
  }
}
