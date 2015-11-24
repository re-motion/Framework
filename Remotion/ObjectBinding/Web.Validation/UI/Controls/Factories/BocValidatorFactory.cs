using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories
{
  [ImplementationFor (typeof (IBocReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocReferenceValueValidatorFactory)]
  [ImplementationFor (typeof (IBocAutoCompleteReferenceValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocAutoCompleteReferenceValueValidatorFactory)]
  [ImplementationFor (typeof (IBocTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocTextValueValidatorFactory)]
  [ImplementationFor (typeof (IBocBooleanValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocBooleanValueValidatorFactory)]
  [ImplementationFor (typeof (IBocCheckBoxValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocCheckBoxValidatorFactory)]
  [ImplementationFor (typeof (IBocDateTimeValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocDateTimeValueValidatorFactory)]
  [ImplementationFor (typeof (IBocEnumValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocEnumValueValidatorFactory)]
  [ImplementationFor (typeof (IBocMultilineTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position_BocMultilineTextValueValidatorFactory)]
  public class BocValidatorFactory
      : IBocTextValueValidatorFactory,
        IBocReferenceValueValidatorFactory,
        IBocAutoCompleteReferenceValueValidatorFactory,
        IBocBooleanValueValidatorFactory,
        IBocCheckBoxValidatorFactory,
        IBocDateTimeValueValidatorFactory,
        IBocEnumValueValidatorFactory,
        IBocMultilineTextValueValidatorFactory
  {
    public const int Position_BocTextValueValidatorFactory = BocTextValueValidatorFactory.Position + 1;
    public const int Position_BocReferenceValueValidatorFactory = BocReferenceValueValidatorFactory.Position + 1;
    public const int Position_BocAutoCompleteReferenceValueValidatorFactory = BocAutoCompleteReferenceValueValidatorFactory.Position + 1;
    public const int Position_BocBooleanValueValidatorFactory = BocBooleanValueValidatorFactory.Position + 1;
    public const int Position_BocCheckBoxValidatorFactory = 0;
    public const int Position_BocDateTimeValueValidatorFactory = BocDateTimeValueValidatorFactory.Position + 1;
    public const int Position_BocEnumValueValidatorFactory = BocEnumValueValidatorFactory.Position + 1;
    public const int Position_BocMultilineTextValueValidatorFactory = BocMultilineTextValueValidatorFactory.Position + 1;

    public IEnumerable<BaseValidator> CreateValidators (IBocTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocAutoCompleteReferenceValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocBooleanValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocCheckBox control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocDateTimeValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocEnumValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocMultilineTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return CreateBocValidator (control.ID, isReadOnly);
    }

    private IEnumerable<BocValidator> CreateBocValidator (string id, bool isReadonly)
    {
      if (isReadonly)
        yield break;

      var bocValidator = new BocValidator();
      bocValidator.ControlToValidate = id;
      bocValidator.ID = id + "_BocValidator";
      yield return bocValidator;
    }
  }
}