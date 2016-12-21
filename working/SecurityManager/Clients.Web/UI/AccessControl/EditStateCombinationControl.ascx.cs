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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  public partial class EditStateCombinationControl : BaseControl
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlResources")]
    public enum ResourceIdentifier
    {
      RequiredStateCombinationValidatorErrorMessage,
      DeleteStateCombinationButtonText,
    }

    private static readonly object s_deleteEvent = new object ();

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected StateCombination CurrentStateCombination
    {
      get { return (StateCombination) CurrentObject.BusinessObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      DeleteStateDefinitionButton.Icon = new IconInfo (
          ResourceUrlFactory.CreateThemedResourceUrl (typeof (EditStateCombinationControl), ResourceType.Image, "DeleteItem.gif").GetUrl());
      DeleteStateDefinitionButton.Icon.AlternateText =
          GetResourceManager (typeof (ResourceIdentifier)).GetString (ResourceIdentifier.DeleteStateCombinationButtonText);
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        if (!interim)
          FillStateDefinitionField();

        var currentStateDefinition = GetStateDefinition (CurrentStateCombination);
        StateDefinitionField.LoadUnboundValue (currentStateDefinition, interim);
        StateDefinitionContainer.Visible = true;
      }
      else
      {
        StateDefinitionContainer.Visible = false;
      }
    }

    protected override void OnPreRender (EventArgs e)
    {
      RequiredStateCombinationValidator.ErrorMessage =
          GetResourceManager (typeof (ResourceIdentifier)).GetString (ResourceIdentifier.RequiredStateCombinationValidatorErrorMessage);
      
      base.OnPreRender (e);
    }

    private void FillStateDefinitionField ()
    {
      var stateProperties = CurrentStateCombination.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException ("Only classes with a zero or one StatePropertyDefinition are supported.");

      var possibleStateDefinitions = new List<StateDefinition> ();
      if (stateProperties.Count > 0)
        possibleStateDefinitions.AddRange (stateProperties[0].DefinedStates);
      StateDefinitionField.SetBusinessObjectList (possibleStateDefinitions);
    }

    private StateDefinition GetStateDefinition (StateCombination stateCombination)
    {
      return stateCombination.GetStates().SingleOrDefault();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= ValidateStateCombination ();

      return isValid;
    }

    private bool ValidateStateCombination ()
    {
      bool isValid = true;
      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        RequiredStateCombinationValidator.Validate();
        isValid &= RequiredStateCombinationValidator.IsValid;
      }
      return isValid;
    }

    public override bool SaveValues (bool interim)
    {
      var hasSaved = base.SaveValues (interim);

      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        var stateDefinition = (StateDefinition) StateDefinitionField.Value;
        CurrentStateCombination.ClearStates();
        CurrentStateCombination.AttachState (stateDefinition);
      }

      return hasSaved;
    }

    protected void DeleteStateDefinitionButton_Click (object sender, EventArgs e)
    {
      var handler = (EventHandler) Events[s_deleteEvent];
      if (handler != null)
        handler (this, EventArgs.Empty);
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler (s_deleteEvent, value); }
      remove { Events.RemoveHandler (s_deleteEvent, value); }
    }

    protected void RequiredStateCombinationValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = !string.IsNullOrEmpty (StateDefinitionField.BusinessObjectUniqueIdentifier);
    }
  }
}
