// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions
{
  [Serializable]
  public abstract class FormFunction<T> : BaseTransactedFunction
      where T : BaseSecurityManagerObject, ISupportsGetObject
  {
    protected FormFunction ()
    {
    }

    protected FormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    protected FormFunction (ITransactionMode transactionMode, IDomainObjectHandle<T> currentObjectHandle)
      : base (transactionMode, currentObjectHandle)
    {
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public IDomainObjectHandle<T> CurrentObjectHandle
    {
      get { return (IDomainObjectHandle<T>) Variables["CurrentObjectHandle"]; }
      set { Variables["CurrentObjectHandle"] = value; }
    }

    public T CurrentObject
    {
      get
      {
        if (CurrentObjectHandle != null)
          return CurrentObjectHandle.GetObject();

        return null;
      }
      set
      {
        CurrentObjectHandle = value.GetHandle();
      }
    }
  }
}
