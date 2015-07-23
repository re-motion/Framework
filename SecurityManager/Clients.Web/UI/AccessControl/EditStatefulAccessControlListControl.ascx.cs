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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditStatefulAccessControlListControl : EditAccessControlListControlBase<StatefulAccessControlList>
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlResources")]
    public enum StatefulAccessResourceIdentifier
    {
      MissingStateCombinationsValidatorErrorMessage,
      NewStateCombinationButtonText,
    }

    // types

    // static members and constants

    // member fields
    
    private readonly List<EditStateCombinationControl> _editStateCombinationControls = new List<EditStateCombinationControl> ();
    private bool _isCreatingNewStateCombination;

    // construction and disposing

    // methods and properties

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override ControlCollection GetAccessControlEntryControls ()
    {
      return AccessControlEntryControls.Controls;
    }

    protected override void OnPreRender (EventArgs e)
    {
      { // Base
        var resourceManager = GetResourceManager (typeof (ResourceIdentifier));
        DeleteAccessControlListButton.Text = resourceManager.GetString (ResourceIdentifier.DeleteAccessControlListButtonText);
        NewAccessControlEntryButton.Text = resourceManager.GetString (ResourceIdentifier.NewAccessControlEntryButtonText);
      }

      {// This
        var resourceManager = GetResourceManager (typeof (StatefulAccessResourceIdentifier));
        MissingStateCombinationsValidator.ErrorMessage =
            resourceManager.GetString (StatefulAccessResourceIdentifier.MissingStateCombinationsValidatorErrorMessage);
        NewStateCombinationButton.Text = resourceManager.GetString (StatefulAccessResourceIdentifier.NewStateCombinationButtonText);
      }

      base.OnPreRender (e);

      EnableNewStateCombinationButton ();
    }

    private void EnableNewStateCombinationButton ()
    {
      NewStateCombinationButton.Enabled = CurrentAccessControlList.Class.AreStateCombinationsComplete();
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      LoadStateCombinations (interim);
    }

    private void LoadStateCombinations (bool interim)
    {
      CreateEditStateCombinationControls (CurrentAccessControlList.StateCombinations);
      foreach (var control in _editStateCombinationControls)
        control.LoadValues (interim);
    }

    private void CreateEditStateCombinationControls (IList<StateCombination> stateCombinations)
    {
      StateCombinationControls.Controls.Clear ();
      _editStateCombinationControls.Clear ();

      for (int i = 0; i < stateCombinations.Count; i++)
      {
        StateCombination stateCombination = stateCombinations[i];

        EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) LoadControl ("EditStateCombinationControl.ascx");
        editStateCombinationControl.ID = "SC_" + i;
        editStateCombinationControl.BusinessObject = stateCombination;
        editStateCombinationControl.Delete += EditStateCombinationControl_Delete;

        StateCombinationControls.Controls.Add (editStateCombinationControl);

        _editStateCombinationControls.Add (editStateCombinationControl);
      }
    }

    public override bool SaveValues (bool interim)
    {
      var hasSaved = base.SaveValues (interim);

      hasSaved &= SaveStateCombinations (interim);
      return hasSaved;
    }

    private bool SaveStateCombinations (bool interim)
    {
      bool hasSaved = true;
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
        hasSaved &= control.SaveValues (interim);
      return hasSaved;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateStateCombinations ();

      return isValid;
    }

    private bool ValidateStateCombinations (params EditStateCombinationControl[] excludedControls)
    {
      List<EditStateCombinationControl> excludedControlList = new List<EditStateCombinationControl> (excludedControls);

      bool isValid = true;
      foreach (EditStateCombinationControl control in _editStateCombinationControls)
      {
        if (!excludedControlList.Contains (control))
          isValid &= control.Validate ();
      }

      if (!_isCreatingNewStateCombination)
      {
        MissingStateCombinationsValidator.Validate ();
        isValid &= MissingStateCombinationsValidator.IsValid;
      }

      return isValid;
    }

    protected void MissingStateCombinationsValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = CurrentAccessControlList.StateCombinations.Count > 0;
    }

    protected void NewStateCombinationButton_Click (object sender, EventArgs e)
    {
      _isCreatingNewStateCombination = true;
      Page.PrepareValidation ();
      bool isValid = Validate ();
      if (!isValid)
      {
        return;
      }
      SaveValues (false);
      Page.IsDirty = true;

      CurrentAccessControlList.CreateStateCombination ();

      LoadStateCombinations (false);
      _isCreatingNewStateCombination = false;
    }

    void EditStateCombinationControl_Delete (object sender, EventArgs e)
    {
      EditStateCombinationControl editStateCombinationControl = (EditStateCombinationControl) sender;
      Page.PrepareValidation ();
      bool isValid = ValidateStateCombinations (editStateCombinationControl);
      if (!isValid)
        return;

      _editStateCombinationControls.Remove (editStateCombinationControl);
      StateCombination accessControlEntry = (StateCombination) editStateCombinationControl.DataSource.BusinessObject;
      accessControlEntry.Delete ();

      SaveStateCombinations (false);
      Page.IsDirty = true;

      LoadStateCombinations (false);
    }
  }
}
