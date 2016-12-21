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
using System.Collections;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  public abstract class BaseListControl<T> : BaseControl
      where T : BaseSecurityManagerObject, ISupportsGetObject
  {
    protected abstract IList GetValues ();

    protected abstract FormFunction<T> CreateEditFunction (ITransactionMode transactionMode, [CanBeNull] IDomainObjectHandle<T> editedObject);

    protected new BaseListTransactedFunction CurrentFunction
    {
      get { return (BaseListTransactedFunction) base.CurrentFunction; }
    }

    protected void HandleEditItemClick (BocList sender, BocListItemCommandClickEventArgs e)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction (WxeTransactionMode.CreateRootWithAutoCommit, ((T) e.BusinessObject).GetHandle());
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction<T>) Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Reset();
          sender.LoadUnboundValue (GetValues(), false);
        }
      }
    }

    protected void HandleNewButtonClick (BocList sender)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction (WxeTransactionMode.CreateRootWithAutoCommit, null);
        Page.ExecuteFunction (editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction<T>) Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Reset();
          sender.LoadUnboundValue (GetValues(), false);
        }
      }
    }

    protected void ResetListOnTenantChange (BocList list)
    {
      ArgumentUtility.CheckNotNull ("list", list);

      if (HasTenantChanged)
      {
        CurrentFunction.Reset();
        list.LoadUnboundValue (GetValues(), false);
      }
    }
  }
}