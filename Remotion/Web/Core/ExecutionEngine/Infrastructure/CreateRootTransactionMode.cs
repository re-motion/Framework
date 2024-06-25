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
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  //TODO: Doc
  public class CreateRootTransactionMode : ITransactionMode
  {
    private readonly bool _autoCommit;
    private readonly ITransactionFactory _transactionFactory;

    public CreateRootTransactionMode (bool autoCommit, ITransactionFactory transactionFactory)
    {
      ArgumentUtility.CheckNotNull("transactionFactory", transactionFactory);

      _autoCommit = autoCommit;
      _transactionFactory = transactionFactory;
    }

    public virtual TransactionStrategyBase CreateTransactionStrategy (WxeFunction function, WxeContext context)
    {
      ArgumentUtility.CheckNotNull("function", function);

      var outerTransactionStrategy = function.ParentFunction != null ? function.ParentFunction.TransactionStrategy : NullTransactionStrategy.Null;
      return new RootTransactionStrategy(_autoCommit, _transactionFactory.CreateRootTransaction, outerTransactionStrategy, function);
    }

    public bool AutoCommit
    {
      get { return _autoCommit; }
    }

    public ITransactionFactory TransactionFactory
    {
      get { return _transactionFactory; }
    }
  }
}
