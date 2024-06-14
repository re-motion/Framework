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
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class EditPermissionsFormFunction : FormFunction<SecurableClassDefinition>
  {
    public EditPermissionsFormFunction (ITransactionMode transactionMode, [NotNull] IDomainObjectHandle<SecurableClassDefinition> currentObjectHandle)
        : base(transactionMode, ArgumentUtility.CheckNotNull("currentObjectHandle", currentObjectHandle))
    {
    }

    private void Step1 ()
    {
      QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                  .Where(cd => cd == CurrentObject).Select(cd => cd)
                  .FetchDetails()
                  .ToArray();
    }

    private WxeResourcePageStep Step2 = new WxeResourcePageStep(typeof(EditPermissionsForm), "UI/AccessControl/EditPermissionsForm.aspx");
  }
}
