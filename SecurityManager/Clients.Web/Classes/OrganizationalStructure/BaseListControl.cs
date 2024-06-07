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
    protected abstract IReadOnlyList<T> GetValues ();

    protected abstract FormFunction<T> CreateEditFunction (ITransactionMode transactionMode, [CanBeNull] IDomainObjectHandle<T>? editedObject);

    protected new BaseListTransactedFunction CurrentFunction
    {
      get { return (BaseListTransactedFunction)base.CurrentFunction; }
    }

    protected void HandleEditItemClick (BocList sender, BocListItemCommandClickEventArgs e)
    {
      ArgumentUtility.CheckNotNull("sender", sender);
      Assertion.IsNotNull(Page, "Page != null when processing page life cycle events.");

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction(WxeTransactionMode.CreateRootWithAutoCommit, ((T)e.BusinessObject).GetHandle());
        Page.ExecuteFunction(editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction<T>)Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Reset();
          sender.LoadUnboundValue(GetValues(), false);
        }
      }
    }

    protected void HandleNewButtonClick (BocList sender)
    {
      ArgumentUtility.CheckNotNull("sender", sender);
      Assertion.IsNotNull(Page, "Page != null when processing page life cycle events.");

      if (!Page.IsReturningPostBack)
      {
        var editUserFormFunction = CreateEditFunction(WxeTransactionMode.CreateRootWithAutoCommit, null);
        Page.ExecuteFunction(editUserFormFunction, WxeCallArguments.Default);
      }
      else
      {
        if (!((FormFunction<T>)Page.ReturningFunction).HasUserCancelled)
        {
          CurrentFunction.Reset();
          sender.LoadUnboundValue(GetValues(), false);
        }
      }
    }

    protected void ResetListOnTenantChange (BocList list)
    {
      ArgumentUtility.CheckNotNull("list", list);

      if (HasTenantChanged)
      {
        CurrentFunction.Reset();
        list.LoadUnboundValue(GetValues(), false);
      }
    }
  }
}
