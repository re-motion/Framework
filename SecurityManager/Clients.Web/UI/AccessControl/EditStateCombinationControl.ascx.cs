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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;
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

    private static readonly object s_deleteEvent = new object();

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public StateCombination CurrentStateCombination
    {
      get { return Assertion.IsNotNull((StateCombination?)CurrentObject.BusinessObject, "CurrentStateCombination has not been set."); }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      DeleteStateDefinitionButton.Icon = new IconInfo(
          ResourceUrlFactory.CreateThemedResourceUrl(typeof(EditStateCombinationControl), ResourceType.Image, "sprite.svg#DeleteItem").GetUrl());
      DeleteStateDefinitionButton.Icon.AlternateText =
          GetResourceManager(typeof(ResourceIdentifier)).GetString(ResourceIdentifier.DeleteStateCombinationButtonText);
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues(interim);

      Assertion.IsNotNull(CurrentStateCombination.Class, "CurrentStateCombination.Class != null");
      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        if (!interim)
          FillStateDefinitionField();

        var currentStateDefinition = GetStateDefinition(CurrentStateCombination);
        StateDefinitionField.LoadUnboundValue(currentStateDefinition, interim);
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
          GetResourceManager(typeof(ResourceIdentifier)).GetString(ResourceIdentifier.RequiredStateCombinationValidatorErrorMessage);

      base.OnPreRender(e);
    }

    private void FillStateDefinitionField ()
    {
      Assertion.IsNotNull(CurrentStateCombination.Class, "CurrentStateCombination.Class != null");
      var stateProperties = CurrentStateCombination.Class.StateProperties;
      if (stateProperties.Count > 1)
        throw new NotSupportedException("Only classes with a zero or one StatePropertyDefinition are supported.");

      var possibleStateDefinitions = new List<StateDefinition>();
      if (stateProperties.Count > 0)
        possibleStateDefinitions.AddRange(stateProperties[0].DefinedStates);
      StateDefinitionField.SetBusinessObjectList(possibleStateDefinitions);
    }

    private StateDefinition? GetStateDefinition (StateCombination stateCombination)
    {
      return stateCombination.GetStates().SingleOrDefault();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= ValidateStateCombination();

      return isValid;
    }

    private bool ValidateStateCombination ()
    {
      bool isValid = true;
      Assertion.IsNotNull(CurrentStateCombination.Class, "CurrentStateCombination.Class != null");
      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        RequiredStateCombinationValidator.Validate();
        isValid &= RequiredStateCombinationValidator.IsValid;
      }
      return isValid;
    }

    public override bool SaveValues (bool interim)
    {
      var hasSaved = base.SaveValues(interim);

      Assertion.IsNotNull(CurrentStateCombination.Class, "CurrentStateCombination.Class != null");
      if (CurrentStateCombination.Class.StateProperties.Count == 1)
      {
        var stateDefinition = (StateDefinition?)StateDefinitionField.Value;
        StateDefinitionField.IsDirty = false;
        CurrentStateCombination.ClearStates();
        if (stateDefinition != null)
          CurrentStateCombination.AttachState(stateDefinition);
      }

      return hasSaved;
    }

    protected void DeleteStateDefinitionButton_Click (object sender, EventArgs e)
    {
      var handler = (EventHandler?)Events[s_deleteEvent];
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public event EventHandler Delete
    {
      add { Events.AddHandler(s_deleteEvent, value); }
      remove { Events.RemoveHandler(s_deleteEvent, value); }
    }

    protected void RequiredStateCombinationValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = !string.IsNullOrEmpty(StateDefinitionField.BusinessObjectUniqueIdentifier);
    }
  }
}
