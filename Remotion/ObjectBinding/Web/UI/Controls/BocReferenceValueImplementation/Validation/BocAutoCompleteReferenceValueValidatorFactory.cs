using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocAutoCompleteReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class BocAutoCompleteReferenceValueValidatorFactory :  IBocAutoCompleteReferenceValueValidatorFactory
  {
    public IEnumerable<BaseValidator> CreateValidators (IBocAutoCompleteReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      var resourceManager = control.GetResourceManager();

      if (control.IsRequired)
        yield return CreateRequiredFieldValidator (control, resourceManager);

      yield return CreateInvalidDisplayNameValidator (control, resourceManager);
    }

    private RequiredFieldValidator CreateRequiredFieldValidator (IBocAutoCompleteReferenceValue control, IResourceManager resourceManage)
    {
      var requiredFieldValidator = new RequiredFieldValidator ();
      requiredFieldValidator.ID = control.ID + "_ValidatorNotNullItem";
      requiredFieldValidator.ControlToValidate = control.ID;
      requiredFieldValidator.ErrorMessage = resourceManage.GetString (BocAutoCompleteReferenceValue.ResourceIdentifier.NullItemErrorMessage);
      return requiredFieldValidator;
    }

    private BocAutoCompleteReferenceValueInvalidDisplayNameValidator CreateInvalidDisplayNameValidator (IBocAutoCompleteReferenceValue control, IResourceManager resourceManage)
    {
      var invalidDisplayNameValidator = new BocAutoCompleteReferenceValueInvalidDisplayNameValidator ();
      invalidDisplayNameValidator.ID = control.ID + "_ValidatorValidDisplayName";
      invalidDisplayNameValidator.ControlToValidate = control.ID;
      invalidDisplayNameValidator.ErrorMessage = resourceManage.GetString (BocAutoCompleteReferenceValue.ResourceIdentifier.InvalidItemErrorMessage);
      return invalidDisplayNameValidator;
    }
  }
}