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
      ArgumentUtility.CheckNotNull("newObjectFactory", newObjectFactory);
      _newObjectFactory = newObjectFactory;
      Show = CommandShow.EditMode;
    }

    public override void OnClick (BocMenuItem menuItem)
    {
      base.OnClick(menuItem);

      Assertion.IsNotNull(OwnerControl, "OwnerControl != null when processing page lifecycle events.");
      ((BocList)OwnerControl).AddAndEditRow(_newObjectFactory());
    }
  }
}
