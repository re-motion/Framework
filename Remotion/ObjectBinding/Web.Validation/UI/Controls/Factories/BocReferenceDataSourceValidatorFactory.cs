using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories
{
  [ImplementationFor (typeof (IBusinessObjectReferenceDataSourceControlValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocReferenceDataSourceValidatorFactory : IBusinessObjectReferenceDataSourceControlValidatorFactory
  {
    public const int Position = 0;

    public IEnumerable<BaseValidator> CreateValidators (BusinessObjectReferenceDataSourceControl control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      yield return CreateBocReferenceDataSourceValidator (control);
    }

    private BocReferenceDataSourceValidator CreateBocReferenceDataSourceValidator (BusinessObjectReferenceDataSourceControl control)
    {
      var bocValidator = new BocReferenceDataSourceValidator();
      bocValidator.ControlToValidate = control.ID;
      bocValidator.ID = control.ID + "_BocReferenceDataSourceValidator";
      return bocValidator;
    }
  }
}