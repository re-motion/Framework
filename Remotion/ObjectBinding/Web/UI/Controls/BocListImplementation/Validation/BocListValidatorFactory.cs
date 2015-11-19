using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  public class BocListValidatorFactory : IBocListValidatorFactory
  {
    public IEnumerable<BaseValidator> CreateValidators (IBocList control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return control.EditModeController.CreateValidators (isReadOnly, control.GetResourceManager());
    }
  }
}