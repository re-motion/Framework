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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditUserFormFunction : FormFunction<User>
  {
    public EditUserFormFunction ()
    {
    }

    protected EditUserFormFunction (ITransactionMode transactionMode, params object[] args)
        : base (transactionMode, args)
    {
    }

    public EditUserFormFunction (ITransactionMode transactionMode, [CanBeNull] IDomainObjectHandle<User> currentObjectHandle)
        : base (transactionMode, currentObjectHandle)
    {
    }

    private void Step1 ()
    {
      if (CurrentObject == null)
      {
        CurrentObject = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser();
        CurrentObject.Tenant = TenantHandle.GetObject();
      }
    }

    private WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditUserForm), "UI/OrganizationalStructure/EditUserForm.aspx");
  }
}