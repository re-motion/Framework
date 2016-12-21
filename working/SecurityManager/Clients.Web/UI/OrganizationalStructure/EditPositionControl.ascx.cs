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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  public partial class EditPositionControl : BaseEditControl<EditPositionControl>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure.OrganizationalStructureResources")]
    public enum ResourceIdentifier
    {
      PositionLabelText,
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override FormGridManager GetFormGridManager()
    {
      return FormGridManager;
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var bocListInlineEditingConfigurator = ServiceLocator.GetInstance<BocListInlineEditingConfigurator>();

      GroupTypesList.EditModeControlFactory = ServiceLocator.GetInstance<EditableRowAutoCompleteControlFactory>();
      bocListInlineEditingConfigurator.Configure (GroupTypesList, GroupTypePosition.NewObject);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupTypesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupTypesList.FixedColumns.Find ("GroupType"), SortingDirection.Ascending));
      }

      if (GroupTypesList.IsReadOnly)
        GroupTypesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      PositionLabel.Text = GetResourceManager (typeof (ResourceIdentifier)).GetString (ResourceIdentifier.PositionLabelText);

      base.OnPreRender (e);
    }
  }
}