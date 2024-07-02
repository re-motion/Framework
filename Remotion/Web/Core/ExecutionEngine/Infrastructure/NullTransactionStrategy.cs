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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class NullTransactionStrategy : TransactionStrategyBase
  {
    public static readonly NullTransactionStrategy Null = new NullTransactionStrategy();

    private NullTransactionStrategy ()
    {
    }

    public override bool IsNull
    {
      get { return true; }
    }

    public override TransactionStrategyBase? OuterTransactionStrategy
    {
      // TODO RM-8108: This property should return the NullTransactionStrategy instance.
      get { return null; }
    }

    public override TTransaction? GetNativeTransaction<TTransaction> () where TTransaction : default
    {
      return default(TTransaction);
    }

    public override TransactionStrategyBase? CreateChildTransactionStrategy (bool autoCommit, IWxeFunctionExecutionContext executionContext, WxeContext wxeContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);
      return null;
    }

    public override void UnregisterChildTransactionStrategy (TransactionStrategyBase childTransactionStrategy)
    {
      ArgumentUtility.CheckNotNull("childTransactionStrategy", childTransactionStrategy);
      //NOP
    }

    public override IWxeFunctionExecutionListener CreateExecutionListener (IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull("innerListener", innerListener);

      return innerListener;
    }

    public override void EnsureCompatibility (IEnumerable objects)
    {
      //NOP
    }

    public override bool EvaluateDirtyState ()
    {
      return false;
    }

    public override void Commit ()
    {
      //NOP
    }

    public override void Rollback ()
    {
      //NOP
    }

    public override void Reset ()
    {
      //NOP
    }

    public override void OnExecutionPlay (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("listener", listener);

      listener.OnExecutionPlay(context);
    }

    public override void OnExecutionStop (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("listener", listener);

      listener.OnExecutionStop(context);
    }

    public override void OnExecutionPause (WxeContext context, IWxeFunctionExecutionListener listener)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("listener", listener);

      listener.OnExecutionPause(context);
    }

    public override void OnExecutionFail (WxeContext context, IWxeFunctionExecutionListener listener, Exception exception)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("listener", listener);

      listener.OnExecutionFail(context, exception);
    }
  }
}
