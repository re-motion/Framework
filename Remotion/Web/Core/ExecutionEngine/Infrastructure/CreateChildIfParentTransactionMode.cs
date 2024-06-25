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
  public class CreateChildIfParentTransactionMode : CreateRootTransactionMode
  {
    public CreateChildIfParentTransactionMode (bool autoCommit, ITransactionFactory transactionFactory)
      :base(autoCommit, transactionFactory)
    {
    }

    public override TransactionStrategyBase CreateTransactionStrategy (WxeFunction function, WxeContext context)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("context", context);

      if (function.ParentFunction != null)
      {
        var childTransactionStrategy = function.ParentFunction.TransactionStrategy.CreateChildTransactionStrategy(AutoCommit, function, context);
        if (childTransactionStrategy != null)
          return childTransactionStrategy;
      }

      return base.CreateTransactionStrategy(function, context);
    }
  }
}
