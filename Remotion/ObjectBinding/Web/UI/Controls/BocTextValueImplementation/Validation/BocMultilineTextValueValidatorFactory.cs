using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation
{
  [ImplementationFor (typeof (IBocMultilineTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class BocMultilineTextValueValidatorFactory : IBocMultilineTextValueValidatorFactory
  {
    public IEnumerable<BaseValidator> CreateValidators (IBocMultilineTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      IResourceManager resourceManager = control.GetResourceManager();
      if (control.IsRequired)
        yield return CreateRequiredFieldValidator (control, resourceManager);

      if (control.TextBoxStyle.MaxLength.HasValue)
        yield return CreateLengthValidator (control, resourceManager);
    }

    private RequiredFieldValidator CreateRequiredFieldValidator (IBocMultilineTextValue control, IResourceManager resourceManager)
    {
      RequiredFieldValidator requiredValidator = new RequiredFieldValidator ();
      requiredValidator.ID = control.ID + "_ValidatorRequired";
      requiredValidator.ControlToValidate = control.TargetControl.ID;
      requiredValidator.ErrorMessage = resourceManager.GetString (BocMultilineTextValue.ResourceIdentifier.RequiredValidationMessage);
      return requiredValidator;
    }

    private LengthValidator CreateLengthValidator (IBocMultilineTextValue control, IResourceManager resourceManager)
    {
      var maxLength = control.TextBoxStyle.MaxLength;
      Assertion.IsTrue (maxLength.HasValue);

      LengthValidator lengthValidator = new LengthValidator ();
      lengthValidator.ID = control.ID + "_ValidatorMaxLength";
      lengthValidator.ControlToValidate = control.TargetControl.ID;
      lengthValidator.MaximumLength = maxLength.Value;
      lengthValidator.ErrorMessage = string.Format (resourceManager.GetString (BocMultilineTextValue.ResourceIdentifier.MaxLengthValidationMessage), maxLength.Value);
      return lengthValidator;
    }
  }
}