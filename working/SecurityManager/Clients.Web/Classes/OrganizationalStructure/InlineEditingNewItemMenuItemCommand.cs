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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Domain;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// <see cref="BocMenuItemCommand"/> designed for creating a <typeparamref cref="BaseSecurityManagerObject"/> when used with <see cref="BocList"/> 
  /// inline editing on <see cref="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure"/> forms.
  /// </summary>
  public class InlineEditingNewItemMenuItemCommand<TBusinessObject> : BocMenuItemCommand
      where TBusinessObject: BaseSecurityManagerObject
  {
    private readonly Func<TBusinessObject> _newObjectFactory;

    public InlineEditingNewItemMenuItemCommand (Func<TBusinessObject> newObjectFactory)
    {
      ArgumentUtility.CheckNotNull ("newObjectFactory", newObjectFactory);
      _newObjectFactory = newObjectFactory;
      Show = CommandShow.EditMode;
    }

    public override void OnClick (BocMenuItem menuItem)
    {
      base.OnClick (menuItem);

      ((BocList) OwnerControl).AddAndEditRow (_newObjectFactory());
    }
  }
}