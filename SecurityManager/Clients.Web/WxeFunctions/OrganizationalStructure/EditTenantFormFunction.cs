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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditTenantFormFunction : FormFunction<Tenant>
  {
    public EditTenantFormFunction ()
    {
    }

    protected EditTenantFormFunction (ITransactionMode transactionMode, params object[] args)
        : base(transactionMode, args)
    {
    }

    public EditTenantFormFunction (ITransactionMode transactionMode, [CanBeNull] IDomainObjectHandle<Tenant>? currentObjectHandle)
        : base(transactionMode, currentObjectHandle)
    {
    }

    private void Step1 ()
    {
      if (CurrentObject == null)
        CurrentObject = SafeServiceLocator.Current.GetInstance<IOrganizationalStructureFactory>().CreateTenant();
    }

    private WxeResourcePageStep Step2 = new WxeResourcePageStep(typeof(EditTenantForm), "UI/OrganizationalStructure/EditTenantForm.aspx");
  }
}
