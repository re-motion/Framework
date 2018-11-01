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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class ChildTransactionExecutionListener : IWxeFunctionExecutionListener
  {
    private readonly ChildTransactionStrategy _transactionStrategy;
    private readonly IWxeFunctionExecutionListener _innerListener;

    public ChildTransactionExecutionListener (ChildTransactionStrategy transactionStrategy, IWxeFunctionExecutionListener innerListener)
    {
      ArgumentUtility.CheckNotNull ("transactionStrategy", transactionStrategy);
      ArgumentUtility.CheckNotNull ("innerListener", innerListener);

      _transactionStrategy = transactionStrategy;
      _innerListener = innerListener;
    }

    public ChildTransactionStrategy TransactionStrategy
    {
      get { return _transactionStrategy; }
    }

    public IWxeFunctionExecutionListener InnerListener
    {
      get { return _innerListener; }
    }

    public bool IsNull
    {
      get { return false; }
    }

    public void OnExecutionPlay (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      //_transactionStrategy.State == Started
      Assertion.IsNotNull (_transactionStrategy.Scope);
      _innerListener.OnExecutionPlay (context);
    }

    public void OnExecutionStop (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      //_transactionStrategy.State == Playing
      Assertion.IsNotNull (_transactionStrategy.Scope);
      _transactionStrategy.OnExecutionStop (context, _innerListener);
    }

    public void OnExecutionPause (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      //_transactionStrategy.State == Playing
      Assertion.IsNotNull (_transactionStrategy.Scope);
      _innerListener.OnExecutionPause (context);
    }

    public void OnExecutionFail (WxeContext context, Exception exception)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      //_transactionStrategy.State == Started
      Assertion.IsNotNull (_transactionStrategy.Scope);
      _transactionStrategy.OnExecutionFail (context, _innerListener, exception);
    }
  }
}
