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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class EditPermissionsFormFunction : FormFunction<SecurableClassDefinition>
  {
    public EditPermissionsFormFunction ()
    {
    }

    protected EditPermissionsFormFunction (ITransactionMode transactionMode, params object[] args)
        : base (transactionMode, args)
    {
    }

    public EditPermissionsFormFunction (ITransactionMode transactionMode, [NotNull] IDomainObjectHandle<SecurableClassDefinition> currentObjectHandle)
        : base (transactionMode, currentObjectHandle)
    {
    }

    private void Step1 ()
    {
      QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                  .Where (cd => cd == CurrentObject).Select (cd => cd)
                  .FetchDetails()
                  .ToArray();
    }

    private WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditPermissionsForm), "UI/AccessControl/EditPermissionsForm.aspx");
  }
}