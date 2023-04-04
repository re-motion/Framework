using System;
using System.Collections.Generic;
using Remotion.ObjectBinding.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public class PropertyPathValidationFailureMatcher : IBocColumnValidationFailureMatcher
  {
    private readonly IBusinessObjectPropertyPath _propertyPath;

    public PropertyPathValidationFailureMatcher (IBusinessObjectPropertyPath propertyPath)
    {
      _propertyPath = propertyPath;
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (IBusinessObject rowObject, IBusinessObjectValidationResult validationResult)
    {
      var result = _propertyPath.GetResult(
          rowObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);

      var businessObject = result.ResultObject;
      var businessObjectProperty = result.ResultProperty;
      if (businessObject == null || businessObjectProperty == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      return validationResult.GetValidationFailures(businessObject, businessObjectProperty, true);
    }
  }
}
