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
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions
{
  public class DelegateExecutingTransactedFunction : WxeFunction
  {
    public DelegateExecutingTransactedFunction (
        ITransactionMode transactionMode, Action<WxeContext, DelegateExecutingTransactedFunction> testDelegate, params object[] actualParameters)
        : base (transactionMode, actualParameters)
    {
      Assertion.IsFalse (TransactionMode.AutoCommit);

      CurrentDelegateIndex = 0;
      DelegateBatch = new List<Action<WxeContext, DelegateExecutingTransactedFunction>> ();
      Add (new WxeMethodStep (() =>
      {
        CurrentDelegateIndex = 0;
      }));

      AddDelegate (testDelegate);
    }

    public bool DelegatesExecuted
    {
      get { return CurrentDelegateIndex >= DelegateBatch.Count; }
    }

    private List<Action<WxeContext, DelegateExecutingTransactedFunction>> DelegateBatch
    {
      get { return (List<Action<WxeContext, DelegateExecutingTransactedFunction>>) Variables["DelegateBatch"]; }
      set { Variables["DelegateBatch"] = value; }
    }

    private int CurrentDelegateIndex
    {
      get { return (int) Variables["CurrentDelegateIndex"]; }
      set { Variables["CurrentDelegateIndex"] = value; }
    }

    public void AddDelegate (Action<WxeContext, DelegateExecutingTransactedFunction> action)
    {
      DelegateBatch.Add (action);
      Add (new WxeMethodStep (ctx => DelegateBatch[CurrentDelegateIndex++] (ctx, this)));
    }

    public void Reset ()
    {
      TransactionStrategy.Reset();
    }
  }
}