using System;
using System.Collections.Generic;
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public interface IBocColumnValidationFailureMatcher
  {
    IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (IBusinessObject rowObject, IBusinessObjectValidationResult validationResult);
  }
}
