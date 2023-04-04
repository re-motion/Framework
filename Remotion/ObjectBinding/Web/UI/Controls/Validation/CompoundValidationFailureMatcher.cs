using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public class CompoundValidationFailureMatcher : IBocColumnValidationFailureMatcher
  {
    private readonly IBocColumnValidationFailureMatcher[] _failureMatchers;

    public CompoundValidationFailureMatcher (params IBocColumnValidationFailureMatcher[] failureMatchers)
    {
      _failureMatchers = failureMatchers;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (IBusinessObject rowObject, IBusinessObjectValidationResult validationResult)
    {
      if (_failureMatchers.Length == 0)
        return Array.Empty<BusinessObjectValidationFailure>();

      if (_failureMatchers.Length == 1)
        return _failureMatchers[0].GetMatchingValidationFailures(rowObject, validationResult);

      return _failureMatchers
          .SelectMany(e => e.GetMatchingValidationFailures(rowObject, validationResult))
          .ToList();
    }
  }
}
