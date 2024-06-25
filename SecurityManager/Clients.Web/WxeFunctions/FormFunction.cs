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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions
{
  public abstract class FormFunction<T> : BaseTransactedFunction
      where T : BaseSecurityManagerObject, ISupportsGetObject
  {
    protected FormFunction ()
    {
    }

    protected FormFunction (ITransactionMode transactionMode, params object[] args)
      : base(transactionMode, args)
    {
    }

    protected FormFunction (ITransactionMode transactionMode, IDomainObjectHandle<T>? currentObjectHandle)
      : base(transactionMode, currentObjectHandle)
    {
    }

    [WxeParameter(1, false, WxeParameterDirection.In)]
    public IDomainObjectHandle<T>? CurrentObjectHandle
    {
      get { return (IDomainObjectHandle<T>?)Variables["CurrentObjectHandle"]; }
      set { Variables["CurrentObjectHandle"] = value; }
    }

    public T? CurrentObject
    {
      get
      {
        if (CurrentObjectHandle != null)
          return CurrentObjectHandle.GetObject();

        return null;
      }
      set
      {
        CurrentObjectHandle = value.GetSafeHandle();
      }
    }
  }
}
