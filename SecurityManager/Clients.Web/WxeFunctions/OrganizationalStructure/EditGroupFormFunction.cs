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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditGroupFormFunction : FormFunction<Group>
  {
    public EditGroupFormFunction ()
    {
    }

    protected EditGroupFormFunction (ITransactionMode transactionMode, params object[] args)
      : base(transactionMode, args)
    {
    }

    public EditGroupFormFunction (ITransactionMode transactionMode, [CanBeNull] IDomainObjectHandle<Group>? currentObjectHandle)
      : base(transactionMode, currentObjectHandle)
    {
    }

    private void Step1 ()
    {
      if (CurrentObject == null)
      {
        CurrentObject = SafeServiceLocator.Current.GetInstance<IOrganizationalStructureFactory>().CreateGroup();
        CurrentObject.Tenant = TenantHandle.GetObject();
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep(typeof(EditGroupForm), "UI/OrganizationalStructure/EditGroupForm.aspx");
  }
}
