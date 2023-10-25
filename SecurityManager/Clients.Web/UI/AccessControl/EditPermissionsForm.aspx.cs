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
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Globalization;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditPermissionsForm : BaseEditPage<SecurableClassDefinition>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlResources")]
    public enum ResourceIdentifier
    {
      StatefulAccessControlListsTitle,
      StatelessAccessControlListTitle,
      DuplicateStateCombinationsValidatorErrorMessage,
      NewStatefulAccessControlListButtonText,
      NewStatelessAccessControlListButtonText,
      Title,
    }

    private readonly List<EditAccessControlListControlBase> _editAccessControlListControls = new List<EditAccessControlListControlBase>();

    protected override void OnPreRenderComplete (EventArgs e)
    {
      var title = PlainTextString.CreateFromText(
          string.Format(
              GlobalizationService.GetResourceManager(typeof(ResourceIdentifier)).GetString(ResourceIdentifier.Title),
              CurrentSecurableClassDefinition.DisplayName));
      TitleLabel.InnerHtml = title.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      HtmlHeadAppender.Current.SetTitle(title);
      base.OnPreRenderComplete(e);
    }

    private SecurableClassDefinition CurrentSecurableClassDefinition
    {
      get { return Assertion.IsNotNull(CurrentFunction.CurrentObject, "CurrentSecurableClassDefinition has not been set."); }
    }

    protected override void LoadValues (bool interim)
    {
      base.LoadValues(interim);

      LoadAccessControlLists(interim);

      CurrentObjectHeaderControls.BusinessObject = CurrentSecurableClassDefinition;
      CurrentObjectHeaderControls.LoadValues(interim);

      CurrentObject.BusinessObject = CurrentSecurableClassDefinition;
      CurrentObject.LoadValues(interim);
    }

    private void LoadAccessControlLists (bool interim)
    {
      var accessControlLists =
          new List<AccessControlList>(CurrentSecurableClassDefinition.StatefulAccessControlLists.Cast<AccessControlList>());
      if (CurrentSecurableClassDefinition.StatelessAccessControlList != null)
        accessControlLists.Insert(0, CurrentSecurableClassDefinition.StatelessAccessControlList);

      CreateEditAccessControlListControls(accessControlLists.ToArray());
      foreach (var control in _editAccessControlListControls)
        control.LoadValues(interim);
    }

    private void CreateEditAccessControlListControls (AccessControlList[] accessControlLists)
    {
      var resourceManager = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier));

      AccessControlListsPlaceHolder.Controls.Clear();
      PlaceHolder statelessAccessControlListsPlaceHolder = new PlaceHolder();
      AccessControlListsPlaceHolder.Controls.Add(statelessAccessControlListsPlaceHolder);

      PlaceHolder statefulAccessControlListsPlaceHolder = new PlaceHolder();
      AccessControlListsPlaceHolder.Controls.Add(statefulAccessControlListsPlaceHolder);

      _editAccessControlListControls.Clear();
      for (int i = 0; i < accessControlLists.Length; i++)
      {
        var accessControlList = accessControlLists[i];

        string controlName = string.Format("Edit{0}Control.ascx", accessControlList.GetPublicDomainObjectType().Name);

        var editAccessControlListControlBase = (EditAccessControlListControlBase)LoadControl(controlName);
        editAccessControlListControlBase.ID = "Acl_" + i;
        editAccessControlListControlBase.BusinessObject = accessControlList;
        editAccessControlListControlBase.Delete += EditAccessControlListControl_Delete;

        UpdatePanel updatePanel = new UpdatePanel();
        updatePanel.ID = "UpdatePanel_" + i;
        updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

        var div = new HtmlGenericControl("div");
        div.Attributes.Add("class", "accessControlListContainer");
        div.Controls.Add(editAccessControlListControlBase);
        updatePanel.ContentTemplateContainer.Controls.Add(div);

        if (editAccessControlListControlBase is EditStatelessAccessControlListControl)
        {
          if (statelessAccessControlListsPlaceHolder.Controls.Count == 0)
          {
            statelessAccessControlListsPlaceHolder.Controls.Add(
                CreateAccessControlListTitle(resourceManager.GetText(ResourceIdentifier.StatelessAccessControlListTitle)));
          }
          statelessAccessControlListsPlaceHolder.Controls.Add(updatePanel);
        }
        else if (editAccessControlListControlBase is EditStatefulAccessControlListControl)
        {
          if (statefulAccessControlListsPlaceHolder.Controls.Count == 0)
          {
            statefulAccessControlListsPlaceHolder.Controls.Add(
                CreateAccessControlListTitle(resourceManager.GetText(ResourceIdentifier.StatefulAccessControlListsTitle)));
          }
          statefulAccessControlListsPlaceHolder.Controls.Add(updatePanel);
        }
        else
        {
          throw new InvalidOperationException(string.Format("Control-type '{0}' is not supported.", editAccessControlListControlBase.GetType()));
        }

        _editAccessControlListControls.Add(editAccessControlListControlBase);
      }
    }

    protected override void SaveValues (bool interim)
    {
      base.SaveValues(interim);

      SaveAccessControlLists(interim);

      CurrentObjectHeaderControls.SaveValues(interim);
      CurrentObject.SaveValues(interim);
    }

    private void SaveAccessControlLists (bool interim)
    {
      foreach (var control in _editAccessControlListControls)
        control.SaveValues(interim);
    }

    protected override bool ValidatePage ()
    {
      bool isValid = true;
      isValid &= base.ValidatePage();
      isValid &= ValidateAccessControlLists();
      isValid &= CurrentObjectHeaderControls.Validate();
      isValid &= CurrentObject.Validate();

      return isValid;
    }

    protected override bool ValidatePagePostSaveValues ()
    {
      bool isValid = true;
      isValid &= base.ValidatePagePostSaveValues();
      DuplicateStateCombinationsValidator.Validate();
      isValid &= DuplicateStateCombinationsValidator.IsValid;

      return isValid;
    }

    private bool ValidateAccessControlLists (params EditAccessControlListControlBase[] excludedControls)
    {
      var excludedControlList = new List<EditAccessControlListControlBase>(excludedControls);

      bool isValid = true;
      foreach (var control in _editAccessControlListControls)
      {
        if (!excludedControlList.Contains(control))
          isValid &= control.Validate();
      }

      return isValid;
    }

    protected void DuplicateStateCombinationsValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      if (CurrentSecurableClassDefinition.StateProperties.Count > 1)
        throw new NotSupportedException("Only classes with a zero or one StatePropertyDefinition are supported.");

      var usedStates = new HashSet<StateDefinition>();
      foreach (var accessControlList in CurrentSecurableClassDefinition.StatefulAccessControlLists)
      {
        foreach (var stateCombination in accessControlList.StateCombinations)
        {
          var state = stateCombination.GetStates().SingleOrDefault();
          if (state != null)
          {
            if (usedStates.Contains(state))
              args.IsValid = false;
            else
              usedStates.Add(state);
          }

          if (!args.IsValid)
            break;
        }

        if (!args.IsValid)
          break;
      }
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      CurrentFunction.Transaction.Rollback();
      throw new WxeUserCancelException();
    }

    protected void NewStatelessAccessControlListButton_Click (object sender, EventArgs e)
    {
      PrepareValidation();
      bool isValid = ValidateAccessControlLists();
      if (!isValid)
        return;

      SaveAccessControlLists(false);

      var accessControlList = CurrentSecurableClassDefinition.CreateStatelessAccessControlList();

      LoadAccessControlLists(false);
      _editAccessControlListControls.Where(o => o.BusinessObject == accessControlList).Single().ExpandAllAccessControlEntries();
    }

    protected void NewStatefulAccessControlListButton_Click (object sender, EventArgs e)
    {
      PrepareValidation();
      bool isValid = ValidateAccessControlLists();
      if (!isValid)
        return;

      SaveAccessControlLists(false);

      var accessControlList = CurrentSecurableClassDefinition.CreateStatefulAccessControlList();

      LoadAccessControlLists(false);
      _editAccessControlListControls.Where(o => o.BusinessObject == accessControlList).Single().ExpandAllAccessControlEntries();
    }

    private void EditAccessControlListControl_Delete (object? sender, EventArgs e)
    {
      var accessControlListControl = ArgumentUtility.CheckNotNullAndType<EditAccessControlListControlBase>("sender", sender!);

      PrepareValidation();
      bool isValid = ValidateAccessControlLists(accessControlListControl);
      if (!isValid)
        return;

      _editAccessControlListControls.Remove(accessControlListControl);
      var accessControlList = accessControlListControl.CurrentAccessControlList;
      accessControlList.Delete();

      SaveAccessControlLists(false);

      LoadAccessControlLists(false);
    }

    protected override void OnPreRender (EventArgs e)
    {
      var resourceManager = GlobalizationService.GetResourceManager(typeof(ResourceIdentifier));
      DuplicateStateCombinationsValidator.ErrorMessage = resourceManager.GetString(
          ResourceIdentifier.DuplicateStateCombinationsValidatorErrorMessage);

      NewStatefulAccessControlListButton.Text = resourceManager.GetText(ResourceIdentifier.NewStatefulAccessControlListButtonText);

      NewStatelessAccessControlListButton.Text = resourceManager.GetText(ResourceIdentifier.NewStatelessAccessControlListButtonText);

      SaveButton.Text = GlobalResourcesHelper.GetText(GlobalResources.Save);
      CancelButton.Text = GlobalResourcesHelper.GetText(GlobalResources.Cancel);

      base.OnPreRender(e);

      EnableNewAccessControlListButton();

      HtmlHeadAppender.Current.RegisterWebClientScriptInclude();
      var url = ResourceUrlFactory.CreateResourceUrl(typeof(EditPermissionsForm), ResourceType.Html, "EditPermissionsForm.js");
      HtmlHeadAppender.Current.RegisterJavaScriptInclude(GetType().GetFullNameChecked() + "_script", url);
    }

    private void EnableNewAccessControlListButton ()
    {
      NewStatefulAccessControlListButton.Enabled = CurrentSecurableClassDefinition.AreStateCombinationsComplete();
      NewStatelessAccessControlListButton.Enabled = CurrentSecurableClassDefinition.StatelessAccessControlList == null;
    }

    private HtmlGenericControl CreateAccessControlListTitle (WebString title)
    {
      var control = new HtmlGenericControl("h2");
      control.InnerHtml = title.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      control.Attributes["class"] = "accessControlListTitle";
      return control;
    }
  }
}
