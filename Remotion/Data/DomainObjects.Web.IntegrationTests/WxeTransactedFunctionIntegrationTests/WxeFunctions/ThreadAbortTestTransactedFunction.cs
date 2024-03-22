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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions
{
  [Serializable]
  public class ThreadAbortTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ThreadAbortTestTransactedFunction ()
      : base(WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit)
    {
    }

    public bool FirstStepExecuted;
    public bool SecondStepExecuted;
    public bool ThreadAborted;

    public ClientTransactionScope TransactionScopeInFirstStep;
    public ClientTransactionScope TransactionScopeInSecondStepBeforeException;
    public ClientTransactionScope TransactionScopeInSecondStepAfterException;

    // methods and properties

    private void Step1 ()
    {
      Assert.That(FirstStepExecuted, Is.False);
      Assert.That(SecondStepExecuted, Is.False);
      Assert.That(ThreadAborted, Is.False);
      FirstStepExecuted = true;
      TransactionScopeInFirstStep = ClientTransactionScope.ActiveScope;
    }

    private void Step2 ()
    {
      Assert.That(FirstStepExecuted, Is.True);
      Assert.That(SecondStepExecuted, Is.False);

      if (!ThreadAborted)
      {
        TransactionScopeInSecondStepBeforeException = ClientTransactionScope.ActiveScope;
        Assert.That(TransactionScopeInSecondStepBeforeException, Is.SameAs(TransactionScopeInFirstStep));
        ThreadAborted = true;
        WxeThreadAbortHelper.Abort();
      }
      TransactionScopeInSecondStepAfterException = ClientTransactionScope.ActiveScope;
      Assert.That(TransactionScopeInSecondStepAfterException, Is.Not.SameAs(TransactionScopeInFirstStep));
      Assert.That(TransactionScopeInSecondStepAfterException.ScopedTransaction, Is.SameAs(TransactionScopeInFirstStep.ScopedTransaction));
      SecondStepExecuted = true;
    }
  }
}
