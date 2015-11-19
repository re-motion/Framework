using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class BocReferenceValueValidatorFactory: IBocReferenceValueValidatorFactory
  {
    public IEnumerable<BaseValidator> CreateValidators (IBocReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly || !control.IsRequired)
        yield break;

      var resourceManager = control.GetResourceManager ();
      yield return CreateRequiredFieldValidator (control, resourceManager);
    }

    private RequiredFieldValidator CreateRequiredFieldValidator (IBocReferenceValue control, IResourceManager resourceManage)
    {
      var requiredFieldValidator = new RequiredFieldValidator ();
      requiredFieldValidator.ID = control.ID + "_ValidatorNotNullItem";
      requiredFieldValidator.ControlToValidate = control.ID;
      requiredFieldValidator.ErrorMessage = resourceManage.GetString (BocReferenceValue.ResourceIdentifier.NullItemErrorMessage);
      return requiredFieldValidator;
    }
  }
}