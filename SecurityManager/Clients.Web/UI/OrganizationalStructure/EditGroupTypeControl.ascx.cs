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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditGroupTypeControl : BaseEditControl<EditGroupTypeControl>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      GroupTypeLabelText,
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override FormGridManager GetFormGridManager ()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      var bocListInlineEditingConfigurator = ServiceLocator.GetInstance<BocListInlineEditingConfigurator>();

      PositionsList.EditModeControlFactory = ServiceLocator.GetInstance<EditableRowAutoCompleteControlFactory>();
      bocListInlineEditingConfigurator.Configure(PositionsList, GroupTypePosition.NewObject);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      if (!IsPostBack)
      {
        PositionsList.SetSortingOrder(
            new BocListSortingOrderEntry((IBocSortableColumnDefinition)PositionsList.FixedColumns.FindMandatory("Position"), SortingDirection.Ascending));
      }

      if (PositionsList.IsReadOnly)
        PositionsList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      GroupTypeLabel.Text = GetResourceManager(typeof(ResourceIdentifier)).GetText(ResourceIdentifier.GroupTypeLabelText);

      base.OnPreRender(e);
    }
  }
}
