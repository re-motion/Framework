using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  [ImplementationFor (typeof (IBocListValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocListValidatorFactory : IBocListValidatorFactory
  {
    public const int Position = 0;

    public IEnumerable<BaseValidator> CreateValidators (IBocList control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      if ((control.EditModeController.IsListEditModeActive || control.EditModeController.IsRowEditModeActive) && control.EditModeController.EnableEditModeValidator)
       yield return CreateEditModeValidator (control, control.GetResourceManager());
    }

    private EditModeValidator CreateEditModeValidator (IBocList control, IResourceManager resourceManager)
    {
      EditModeValidator editModeValidator = new EditModeValidator (control.EditModeController);
      editModeValidator.ID = control.ID + "_ValidatorEditMode";
      editModeValidator.ControlToValidate = control.ID;
      if (control.EditModeController.IsRowEditModeActive)
        editModeValidator.ErrorMessage = resourceManager.GetString (BocList.ResourceIdentifier.RowEditModeErrorMessage);
      else if (control.EditModeController.IsListEditModeActive)
        editModeValidator.ErrorMessage = resourceManager.GetString (BocList.ResourceIdentifier.ListEditModeErrorMessage);

      return editModeValidator;
    }
  }
}