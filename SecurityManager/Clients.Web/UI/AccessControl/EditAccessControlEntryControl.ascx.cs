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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditAccessControlEntryControl : BaseControl
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlResources")]
    public enum ResourceIdentifier
    {
      SpecificGroupFieldRequiredFieldErrorMessage,
      SpecificGroupTypeFieldRequiredFieldErrorMessage,
      SpecificPositionFieldRequiredFieldErrorMessage,
      SpecificTenantFieldRequiredFieldErrorMessage,
      SpecificUserFieldRequiredFieldErrorMessage,
      SpecificGroupFieldInvalidItemErrorMessage,
      SpecificUserFieldInvalidItemErrorMessage,
      DeleteAccessControlEntryButtonText,
      CollapseAccessControlEntryButtonText,
      ExpandAccessControlEntryButtonText,
      AllPermissionsMenu_ClearAllPermissions_Text,
      AllPermissionsMenu_DenyAllPermissions_Text,
      AllPermissionsMenu_GrantAllPermissions_Text,
    }

    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object();

    // member fields

    private readonly List<EditPermissionControl> _editPermissionControls = new List<EditPermissionControl>();
    private const string c_grantAllMenuItemID = "GrantAllPermissions";
    private const string c_denyAllMenuItemID = "DenyAllPermissions";
    private const string c_clearAllMenuItemID = "ClearAllPermissions";

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string CssClass { get; set; }

    public bool IsCollapsed
    {
      get { return (bool?) ViewState["IsCollapsed"] ?? true; }
      set { ViewState["IsCollapsed"] = value; }
    }

    protected AccessControlEntry CurrentAccessControlEntry
    {
      get { return (AccessControlEntry) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);
      var resourceManager = GetResourceManager (typeof (ResourceIdentifier));

      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_clearAllMenuItemID,
              Text = resourceManager.GetString (ResourceIdentifier.AllPermissionsMenu_ClearAllPermissions_Text),
              Icon = new IconInfo (GetIconUrl ("PermissionUndefined.gif").GetUrl())
          });
      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_grantAllMenuItemID,
              Text = resourceManager.GetString (ResourceIdentifier.AllPermissionsMenu_GrantAllPermissions_Text),
              Icon = new IconInfo (GetIconUrl ("PermissionGranted.gif").GetUrl())
          });
      AllPermisionsMenu.MenuItems.Add (
          new WebMenuItem
          {
              ItemID = c_denyAllMenuItemID,
              Text = resourceManager.GetString (ResourceIdentifier.AllPermissionsMenu_DenyAllPermissions_Text),
              Icon = new IconInfo (GetIconUrl ("PermissionDenied.gif").GetUrl())
          });
      AllPermisionsMenu.EventCommandClick += AllPermisionsMenu_EventCommandClick;

      if (string.IsNullOrEmpty (SpecificTenantField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificTenantField);
 
      if (string.IsNullOrEmpty (SpecificGroupField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificGroupField);

      if (string.IsNullOrEmpty (SpecificUserField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificUserField);

      if (string.IsNullOrEmpty (SpecificGroupTypeField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificGroupTypeField);

      if (string.IsNullOrEmpty (SpecificPositionField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificPositionField);
      
      if (string.IsNullOrEmpty (SpecificAbstractRoleField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (SpecificAbstractRoleField);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager (typeof (ResourceIdentifier));
      SpecificGroupField.NullItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificGroupFieldRequiredFieldErrorMessage);
      SpecificGroupTypeField.NullItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificGroupTypeFieldRequiredFieldErrorMessage);
      SpecificPositionField.NullItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificPositionFieldRequiredFieldErrorMessage);
      SpecificTenantField.NullItemErrorMessage = resourceManager.GetString(ResourceIdentifier.SpecificTenantFieldRequiredFieldErrorMessage);
      SpecificUserField.NullItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificUserFieldRequiredFieldErrorMessage);

      SpecificGroupField.InvalidItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificGroupFieldInvalidItemErrorMessage);
      SpecificUserField.InvalidItemErrorMessage = resourceManager.GetString (ResourceIdentifier.SpecificUserFieldRequiredFieldErrorMessage);

      base.OnPreRender (e);

      if (IsCollapsed)
      {
        var collapsedRenderer = new CollapsedAccessControlConditionsRenderer (CurrentAccessControlEntry, ResourceUrlFactory, GlobalizationService);
        CollapsedTenantInformation.SetRenderMethodDelegate (
            (writer, control) => collapsedRenderer.RenderTenant (writer, new ControlWrapper (control)));
        CollapsedGroupInformation.SetRenderMethodDelegate (
            (writer, control) => collapsedRenderer.RenderGroup (writer, new ControlWrapper (control)));
        CollapsedUserInformation.SetRenderMethodDelegate (
            (writer, control) => collapsedRenderer.RenderUser (writer, new ControlWrapper (control)));
        CollapsedAbstractRoleInformation.SetRenderMethodDelegate (
            (writer, control) => collapsedRenderer.RenderAbstractRole (writer, new ControlWrapper (control)));
      }

      DetailsCell.Attributes.Add ("colspan", (4 + CurrentAccessControlEntry.AccessControlList.Class.AccessTypes.Count + 3).ToString());

      DeleteAccessControlEntryButton.Icon = new IconInfo (GetIconUrl ("DeleteItem.gif").GetUrl());

      DeleteAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString (ResourceIdentifier.DeleteAccessControlEntryButtonText);

      if (IsCollapsed)
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl ("Expand.gif").GetUrl();
        ToggleAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString (ResourceIdentifier.ExpandAccessControlEntryButtonText);
        DetailsView.Visible = false;
      }
      else
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl ("Collapse.gif").GetUrl();
        ToggleAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString (ResourceIdentifier.CollapseAccessControlEntryButtonText);
        DetailsView.Visible = true;
      }
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadPermissions (interim);
      AdjustSpecificTenantField (false);
      AdjustTenantHierarchyConditionField (false);
      AdjustSpecificGroupField (false);
      AdjustGroupHierarchyConditionField (false);
      AdjustSpecificGroupTypeField();
      AdjustSpecificUserField (false);
      AdjustSpecificPositionField();
      AdjustSpecificAbstractRoleField();
    }

    public override bool SaveValues (bool interim)
    {
      bool hasSaved;
      using (SecurityFreeSection.Activate())
      {
        hasSaved = base.SaveValues (interim);
      }

      hasSaved &= SavePermissions (interim);
      return hasSaved;
    }

    private bool SavePermissions (bool interim)
    {
      bool hasSaved = true;
      foreach (EditPermissionControl control in _editPermissionControls)
        hasSaved &= control.SaveValues (interim);
      return hasSaved;
   }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();
      isValid &= ValidatePermissions();

      return isValid;
    }

    protected void DeleteAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      EventHandler handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, e);
    }

    protected void TenantConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificTenantField (true);
      AdjustTenantHierarchyConditionField (true);
    }

    protected void SpecificTenantField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificUserField (true);
      AdjustSpecificGroupField (true);
    }

    protected void GroupConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificGroupField (false);
      AdjustGroupHierarchyConditionField (true);
      AdjustSpecificGroupTypeField();
    }

    protected void UserConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificUserField (false);
      AdjustSpecificPositionField();
    }

    private void AdjustSpecificTenantField (bool hasTenantConditionChanged)
    {
      var isSpecifciTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.SpecificTenant;
      if (hasTenantConditionChanged && !isSpecifciTenantSelected)
        SpecificTenantField.Value = null;
      SpecificTenantField.Visible = isSpecifciTenantSelected;
    }

    private void AdjustTenantHierarchyConditionField (bool hasTenantConditionChanged)
    {
      bool isSpecificTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.SpecificTenant;
      bool isOwningTenantSelected = (TenantCondition?) TenantConditionField.Value == TenantCondition.OwningTenant;

      if (hasTenantConditionChanged)
      {
        if (isOwningTenantSelected)
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.ThisAndParent;
        else if (isSpecificTenantSelected)
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.This;
        else
          TenantHierarchyConditionField.Value = TenantHierarchyCondition.Undefined;
      }

      TenantHierarchyConditionField.Visible = isSpecificTenantSelected || isOwningTenantSelected;
    }

    private void AdjustSpecificGroupField (bool resetValue)
    {
      if (resetValue)
        SpecificGroupField.Value = null;

      SpecificGroupField.Args = SpecificTenantField.BusinessObjectUniqueIdentifier ?? CurrentFunction.TenantHandle.AsArgument();

      SpecificGroupField.Visible = (GroupCondition?) GroupConditionField.Value == GroupCondition.SpecificGroup;
    }

    private void AdjustGroupHierarchyConditionField (bool hasGroupConditionChanged)
    {
      bool isSpecificGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.SpecificGroup;
      bool isOwningGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.OwningGroup;

      if (hasGroupConditionChanged)
      {
        if (isSpecificGroupSelected || isOwningGroupSelected)
          GroupHierarchyConditionField.Value = GroupHierarchyCondition.ThisAndParent;
        else
          GroupHierarchyConditionField.Value = GroupHierarchyCondition.Undefined;
      }

      GroupHierarchyConditionField.Visible = isSpecificGroupSelected || isOwningGroupSelected;
    }

    private void AdjustSpecificGroupTypeField ()
    {
      bool isSpecificGroupTypeSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.AnyGroupWithSpecificGroupType;
      bool isBranchOfOwningGroupSelected = (GroupCondition?) GroupConditionField.Value == GroupCondition.BranchOfOwningGroup;
      SpecificGroupTypeField.Visible = isSpecificGroupTypeSelected || isBranchOfOwningGroupSelected;
    }

    private void AdjustSpecificUserField (bool resetValue)
    {
      if (resetValue)
        SpecificUserField.Value = null;

      SpecificUserField.Args = SpecificTenantField.BusinessObjectUniqueIdentifier ?? CurrentFunction.TenantHandle.AsArgument();

      SpecificUserField.Visible = (UserCondition?) UserConditionField.Value == UserCondition.SpecificUser;
    }

    private void AdjustSpecificPositionField ()
    {
      bool isPositionSelected = (UserCondition?) UserConditionField.Value == UserCondition.SpecificPosition;
      SpecificPositionField.Visible = isPositionSelected;
    }

    private void AdjustSpecificAbstractRoleField ()
    {
      SpecificAbstractRoleField.Visible = CurrentAccessControlEntry.AccessControlList is StatefulAccessControlList;
    }

    private void LoadPermissions (bool interim)
    {
      CreateEditPermissionControls (CurrentAccessControlEntry.GetPermissions());
      foreach (var control in _editPermissionControls)
        control.LoadValues (interim);
    }

    private void CreateEditPermissionControls (IList<Permission> permissions)
    {
      PermissionsPlaceHolder.Controls.Clear();
      _editPermissionControls.Clear();

      for (int i = 0; i < permissions.Count; i++)
      {
        var permission = permissions[i];

        var control = (EditPermissionControl) LoadControl ("EditPermissionControl.ascx");
        control.ID = "P_" + i;
        control.BusinessObject = permission;

        var td = new HtmlGenericControl ("td");
        td.Attributes.Add ("class", "permissionCell");
        PermissionsPlaceHolder.Controls.Add (td);
        td.Controls.Add (control);

        _editPermissionControls.Add (control);
      }
    }

    private bool ValidatePermissions ()
    {
      bool isValid = true;
      foreach (var control in _editPermissionControls)
        isValid &= control.Validate();

      return isValid;
    }

    private void AllPermisionsMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
    {
      bool? isAllowed;
      switch (e.Item.ItemID)
      {
        case c_grantAllMenuItemID:
          isAllowed = true;
          break;
        case c_denyAllMenuItemID:
          isAllowed = false;
          break;
        case c_clearAllMenuItemID:
          isAllowed = null;
          break;
        default:
          throw new InvalidOperationException (string.Format ("The menu item '{0}' is not defined.", e.Item.ItemID));
      }

      foreach (var control in _editPermissionControls)
        control.SetPermissionValue (isAllowed);
    }

    protected void ToggleAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      if (IsCollapsed)
      {
        IsCollapsed = false;
      }
      else if (Validate())
      {
        SaveValues (false);
        LoadValues (false);
        IsCollapsed = true;
      }

    }

    private IResourceUrl GetIconUrl (string url)
    {
      return ResourceUrlFactory.CreateThemedResourceUrl (typeof (EditAccessControlEntryControl), ResourceType.Image, url);
    }
  }
}
