using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public static class BusinessObjectDataSourceValidationResultDispatcher
  {
    public static void DispatchValidationResultForBoundControls (IBusinessObjectDataSourceControl dataSource, IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull ("validationResult", validationResult);

      var allControlsInsideNamingContainer = EnumerableUtility.SelectRecursiveDepthFirst (
          dataSource.NamingContainer,
          child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer) && (item != dataSource)));
      var validators = allControlsInsideNamingContainer.OfType<IBusinessObjectBoundEditableWebControlValidationResultDispatcher>();

      var controlsWithValidBinding = dataSource.GetBoundControlsWithValidBinding().Cast<Control>();

      var validatorsMatchingToControls = controlsWithValidBinding.Join (
          validators,
          c => c.ID,
          v => ((BaseValidator) v).ControlToValidate,
          (c, v) => v);

      foreach (var validator in validatorsMatchingToControls)
        validator.DispatchValidationFailures (validationResult);
    }
  }
}