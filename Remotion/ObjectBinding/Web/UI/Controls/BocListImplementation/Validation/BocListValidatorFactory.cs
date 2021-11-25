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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocListValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocListValidatorFactory"/>
  [ImplementationFor (typeof(IBocListValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocListValidatorFactory : IBocListValidatorFactory
  {
    public const int Position = 0;

    public BocListValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocList control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (isReadOnly)
        yield break;

      if ((control.EditModeController.IsListEditModeActive || control.EditModeController.IsRowEditModeActive) && control.EditModeController.EnableEditModeValidator)
       yield return CreateEditModeValidator(control, control.GetResourceManager());
    }

    private EditModeValidator CreateEditModeValidator (IBocList control, IResourceManager resourceManager)
    {
      EditModeValidator editModeValidator = new EditModeValidator(control.EditModeController);
      editModeValidator.ID = control.ID + "_ValidatorEditMode";
      editModeValidator.ControlToValidate = control.ID;
      if (control.EditModeController.IsRowEditModeActive)
        editModeValidator.ErrorMessage = resourceManager.GetString(BocList.ResourceIdentifier.RowEditModeErrorMessage);
      else if (control.EditModeController.IsListEditModeActive)
        editModeValidator.ErrorMessage = resourceManager.GetString(BocList.ResourceIdentifier.ListEditModeErrorMessage);
      editModeValidator.EnableViewState = false;

      return editModeValidator;
    }
  }
}
