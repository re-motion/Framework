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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
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
      PermissionDeniedText,
      PermissionGrantedText,
      PermissionUndefinedText,
    }

    // types

    // static members and constants

    private static readonly object s_deleteEvent = new object();

    // member fields

    private readonly List<Tuple<Permission, PermissionBooleanValue>> _permissionControls = new List<Tuple<Permission, PermissionBooleanValue>>();
    private const string c_grantAllMenuItemID = "GrantAllPermissions";
    private const string c_denyAllMenuItemID = "DenyAllPermissions";
    private const string c_clearAllMenuItemID = "ClearAllPermissions";

    private string? _cssClass;
    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler(s_deleteEvent, value); }
      remove { Events.RemoveHandler(s_deleteEvent, value); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [AllowNull]
    public string CssClass
    {
      get { return _cssClass ?? string.Empty; }
      set { _cssClass = value; }
    }

    public bool IsCollapsed
    {
      get { return (bool?)ViewState["IsCollapsed"] ?? true; }
      set { ViewState["IsCollapsed"] = value; }
    }

    public AccessControlEntry CurrentAccessControlEntry
    {
      get { return Assertion.IsNotNull((AccessControlEntry?)CurrentObject.BusinessObject, "CurrentAccessControlEntry has not been set."); }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));

      AllPermisionsMenu.MenuItems.Add(
          new WebMenuItem
          {
              ItemID = c_clearAllMenuItemID,
              Text = resourceManager.GetText(ResourceIdentifier.AllPermissionsMenu_ClearAllPermissions_Text),
              Icon = new IconInfo(GetIconUrl("sprite.svg#PermissionUndefined").GetUrl())
          });
      AllPermisionsMenu.MenuItems.Add(
          new WebMenuItem
          {
              ItemID = c_grantAllMenuItemID,
              Text = resourceManager.GetText(ResourceIdentifier.AllPermissionsMenu_GrantAllPermissions_Text),
              Icon = new IconInfo(GetIconUrl("sprite.svg#PermissionGranted").GetUrl())
          });
      AllPermisionsMenu.MenuItems.Add(
          new WebMenuItem
          {
              ItemID = c_denyAllMenuItemID,
              Text = resourceManager.GetText(ResourceIdentifier.AllPermissionsMenu_DenyAllPermissions_Text),
              Icon = new IconInfo(GetIconUrl("sprite.svg#PermissionDenied").GetUrl())
          });
      AllPermisionsMenu.EventCommandClick += AllPermisionsMenu_EventCommandClick;

      if (string.IsNullOrEmpty(SpecificTenantField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificTenantField);

      if (string.IsNullOrEmpty(SpecificGroupField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificGroupField);

      if (string.IsNullOrEmpty(SpecificUserField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificUserField);

      if (string.IsNullOrEmpty(SpecificGroupTypeField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificGroupTypeField);

      if (string.IsNullOrEmpty(SpecificPositionField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificPositionField);

      if (string.IsNullOrEmpty(SpecificAbstractRoleField.ControlServicePath))
        SecurityManagerAutoCompleteReferenceValueWebService.BindServiceToControl(SpecificAbstractRoleField);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      SpecificGroupField.NullItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificGroupFieldRequiredFieldErrorMessage);
      SpecificGroupTypeField.NullItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificGroupTypeFieldRequiredFieldErrorMessage);
      SpecificPositionField.NullItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificPositionFieldRequiredFieldErrorMessage);
      SpecificTenantField.NullItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificTenantFieldRequiredFieldErrorMessage);
      SpecificUserField.NullItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificUserFieldRequiredFieldErrorMessage);

      SpecificGroupField.InvalidItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificGroupFieldInvalidItemErrorMessage);
      SpecificUserField.InvalidItemErrorMessage = resourceManager.GetText(ResourceIdentifier.SpecificUserFieldInvalidItemErrorMessage);

      base.OnPreRender(e);

      if (IsCollapsed)
      {
        var collapsedRenderer = new CollapsedAccessControlConditionsRenderer(CurrentAccessControlEntry, ResourceUrlFactory, GlobalizationService);
        CollapsedTenantInformation.SetRenderMethodDelegate(
            (writer, control) => collapsedRenderer.RenderTenant(writer, new ControlWrapper(control)));
        CollapsedGroupInformation.SetRenderMethodDelegate(
            (writer, control) => collapsedRenderer.RenderGroup(writer, new ControlWrapper(control)));
        CollapsedUserInformation.SetRenderMethodDelegate(
            (writer, control) => collapsedRenderer.RenderUser(writer, new ControlWrapper(control)));
        CollapsedAbstractRoleInformation.SetRenderMethodDelegate(
            (writer, control) => collapsedRenderer.RenderAbstractRole(writer, new ControlWrapper(control)));
      }

      DetailsCell.Attributes.Add("colspan", (4 + _permissionControls.Count + 3).ToString());

      DeleteAccessControlEntryButton.Icon = new IconInfo(GetIconUrl("sprite.svg#DeleteItem").GetUrl());

      DeleteAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString(ResourceIdentifier.DeleteAccessControlEntryButtonText);

      if (IsCollapsed)
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl("sprite.svg#Expand").GetUrl();
        ToggleAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString(ResourceIdentifier.ExpandAccessControlEntryButtonText);
        DetailsView.Visible = false;
      }
      else
      {
        ToggleAccessControlEntryButton.Icon.Url = GetIconUrl("sprite.svg#Collapse").GetUrl();
        ToggleAccessControlEntryButton.Icon.AlternateText = resourceManager.GetString(ResourceIdentifier.CollapseAccessControlEntryButtonText);
        DetailsView.Visible = true;
      }

      PermissionsPlaceHolder.SetRenderMethodDelegate(RenderPermissions);
    }

    private void RenderPermissions (HtmlTextWriter writer, Control container)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("container", container);

      // Text is not needed before rendering phase. 
      // By moving the evaluation into the Render-method, UpdatePanel-postbacks will not cause a hit for unaffected rows.
      var resourceManager = GetResourceManager(typeof(ResourceIdentifier));
      foreach (var tuple in _permissionControls)
      {
        var permission = tuple.Item1;
        var control = tuple.Item2;
        string accessTypeName = permission.AccessType.DisplayName;
        control.TrueDescription = PlainTextString.CreateFromText(string.Format(resourceManager.GetString(ResourceIdentifier.PermissionGrantedText), accessTypeName));
        control.FalseDescription = PlainTextString.CreateFromText(string.Format(resourceManager.GetString(ResourceIdentifier.PermissionDeniedText), accessTypeName));
        control.NullDescription = PlainTextString.CreateFromText(string.Format(resourceManager.GetString(ResourceIdentifier.PermissionUndefinedText), accessTypeName));
      }

      container.SetRenderMethodDelegate(null);
      container.RenderControl(writer);
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues(interim);

      LoadPermissions(interim);
      AdjustSpecificTenantField(false);
      AdjustTenantHierarchyConditionField(false);
      AdjustSpecificGroupField(false);
      AdjustGroupHierarchyConditionField(false);
      AdjustSpecificGroupTypeField();
      AdjustSpecificUserField(false);
      AdjustSpecificPositionField();
      AdjustSpecificAbstractRoleField();
    }

    public override bool SaveValues (bool interim)
    {
      bool hasSaved;
      using (SecurityFreeSection.Activate())
      {
        hasSaved = base.SaveValues(interim);
      }

      hasSaved &= SavePermissions(interim);
      return hasSaved;
    }

    private bool SavePermissions (bool interim)
    {
      if (interim)
        return false;

      foreach (var tuple in _permissionControls)
      {
        var permission = tuple.Item1;
        var control = tuple.Item2;
        permission.Allowed = control.Value;
        control.IsDirty = false;
      }
      return true;
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
      var handler = (EventHandler?)Events[s_deleteEvent];
      if (handler != null)
        handler(this, e);
    }

    protected void TenantConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificTenantField(true);
      AdjustTenantHierarchyConditionField(true);
    }

    protected void SpecificTenantField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificUserField(true);
      AdjustSpecificGroupField(true);
    }

    protected void GroupConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificGroupField(false);
      AdjustGroupHierarchyConditionField(true);
      AdjustSpecificGroupTypeField();
    }

    protected void UserConditionField_SelectionChanged (object sender, EventArgs e)
    {
      AdjustSpecificUserField(false);
      AdjustSpecificPositionField();
    }

    private void AdjustSpecificTenantField (bool hasTenantConditionChanged)
    {
      var isSpecifciTenantSelected = (TenantCondition?)TenantConditionField.Value == TenantCondition.SpecificTenant;
      if (hasTenantConditionChanged && !isSpecifciTenantSelected)
        SpecificTenantField.Value = null;
      SpecificTenantField.Visible = isSpecifciTenantSelected;
    }

    private void AdjustTenantHierarchyConditionField (bool hasTenantConditionChanged)
    {
      bool isSpecificTenantSelected = (TenantCondition?)TenantConditionField.Value == TenantCondition.SpecificTenant;
      bool isOwningTenantSelected = (TenantCondition?)TenantConditionField.Value == TenantCondition.OwningTenant;

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

      SpecificGroupField.ControlServiceArguments = SpecificTenantField.BusinessObjectUniqueIdentifier ?? CurrentFunction.TenantHandle.AsArgument();

      SpecificGroupField.Visible = (GroupCondition?)GroupConditionField.Value == GroupCondition.SpecificGroup;
    }

    private void AdjustGroupHierarchyConditionField (bool hasGroupConditionChanged)
    {
      bool isSpecificGroupSelected = (GroupCondition?)GroupConditionField.Value == GroupCondition.SpecificGroup;
      bool isOwningGroupSelected = (GroupCondition?)GroupConditionField.Value == GroupCondition.OwningGroup;

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
      bool isSpecificGroupTypeSelected = (GroupCondition?)GroupConditionField.Value == GroupCondition.AnyGroupWithSpecificGroupType;
      bool isBranchOfOwningGroupSelected = (GroupCondition?)GroupConditionField.Value == GroupCondition.BranchOfOwningGroup;
      SpecificGroupTypeField.Visible = isSpecificGroupTypeSelected || isBranchOfOwningGroupSelected;
    }

    private void AdjustSpecificUserField (bool resetValue)
    {
      if (resetValue)
        SpecificUserField.Value = null;

      SpecificUserField.ControlServiceArguments = SpecificTenantField.BusinessObjectUniqueIdentifier ?? CurrentFunction.TenantHandle.AsArgument();

      SpecificUserField.Visible = (UserCondition?)UserConditionField.Value == UserCondition.SpecificUser;
    }

    private void AdjustSpecificPositionField ()
    {
      bool isPositionSelected = (UserCondition?)UserConditionField.Value == UserCondition.SpecificPosition;
      SpecificPositionField.Visible = isPositionSelected;
    }

    private void AdjustSpecificAbstractRoleField ()
    {
      SpecificAbstractRoleField.Visible = CurrentAccessControlEntry.AccessControlList is StatefulAccessControlList;
    }

    private void LoadPermissions (bool interim)
    {
      CreateEditPermissionControls(CurrentAccessControlEntry.GetPermissions());
      foreach (var tuple in _permissionControls)
      {
        var permission = tuple.Item1;
        var control = tuple.Item2;
        control.LoadUnboundValue(permission.Allowed, interim);
      }
    }

    private void CreateEditPermissionControls (IList<Permission> permissions)
    {
      PermissionsPlaceHolder.Controls.Clear();
      _permissionControls.Clear();

      for (int i = 0; i < permissions.Count; i++)
      {
        var permission = permissions[i];

        // Control needs to be created inline instead of as DataEditUserControl to optimize performance.
        var control = new PermissionBooleanValue();
        control.ID = "P_" + i;
        control.ShowDescription = false;
        control.Width = Unit.Pixel(16);

        var td = new HtmlGenericControl("td");
        td.Attributes.Add("class", "permissionCell");
        PermissionsPlaceHolder.Controls.Add(td);
        td.Controls.Add(control);

        _permissionControls.Add(Tuple.Create(permission, control));
      }
    }
    private bool ValidatePermissions ()
    {
      return true;
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
          throw new InvalidOperationException(string.Format("The menu item '{0}' is not defined.", e.Item.ItemID));
      }

      foreach (var tuple in _permissionControls)
      {
        var control = tuple.Item2;
        control.Value = isAllowed;
      }
    }

    protected void ToggleAccessControlEntryButton_Click (object sender, EventArgs e)
    {
      if (IsCollapsed)
      {
        IsCollapsed = false;
      }
      else if (Validate())
      {
        SaveValues(false);
        LoadValues(false);
        IsCollapsed = true;
      }

    }

    private IResourceUrl GetIconUrl (string url)
    {
      return ResourceUrlFactory.CreateThemedResourceUrl(typeof(EditAccessControlEntryControl), ResourceType.Image, url);
    }
  }
}
