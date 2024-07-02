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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public abstract class TransactionStrategyBase : ITransactionStrategy
  {
    protected TransactionStrategyBase ()
    {
    }

    public abstract TTransaction? GetNativeTransaction<TTransaction> ();
    public abstract bool EvaluateDirtyState ();
    public abstract void Commit ();
    public abstract void Rollback ();
    public abstract void Reset ();
    public abstract void EnsureCompatibility (IEnumerable objects);
    public abstract bool IsNull { get; }

    public abstract TransactionStrategyBase? OuterTransactionStrategy { get; }
    public abstract TransactionStrategyBase? CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext, WxeContext wxeContext);
    public abstract IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener);
    public abstract void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener);
    public abstract void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception);
    public abstract void UnregisterChildTransactionStrategy (TransactionStrategyBase childTransactionStrategy);
  }
}
