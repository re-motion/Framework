using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [ImplementationFor(typeof(IBocDateTimeValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocDateTimeValueValidatorFactory : IBocDateTimeValueValidatorFactory
  {
    public const int Position = 0;

    public IEnumerable<BaseValidator> CreateValidators (IBocDateTimeValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      var resourceManager = control.GetResourceManager();
      yield return CreateDateTimeValidator (control, resourceManager);
    }

    private BocDateTimeValueValidator CreateDateTimeValidator (IBocDateTimeValue control, IResourceManager resourceManager)
    {
      var dateTimeValueValidator = new BocDateTimeValueValidator();
      dateTimeValueValidator.ID = control.ID + "_ValidatorDateTime";
      dateTimeValueValidator.ControlToValidate = control.ID;
      dateTimeValueValidator.MissingDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateAndTimeErrorMessage);
      dateTimeValueValidator.MissingDateOrTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateOrTimeErrorMessage);
      dateTimeValueValidator.MissingDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateErrorMessage);
      dateTimeValueValidator.MissingTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingTimeErrorMessage);
      dateTimeValueValidator.InvalidDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      dateTimeValueValidator.InvalidDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateErrorMessage);
      dateTimeValueValidator.InvalidTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidTimeErrorMessage);

      return dateTimeValueValidator;
    }
  }
}