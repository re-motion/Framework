using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Validation.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public class BocListValidator : BaseValidator, IBocValidator
  {
    private List<ValidationFailure> _validationFailures = new List<ValidationFailure> ();

    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var bocListControl = control as BocList;
      if (bocListControl == null)
        throw new InvalidOperationException ("BocListValidator may only be applied to controls of type BocList");

      _validationFailures = new List<ValidationFailure> ();
      foreach (var failure in failures)
      {
        if (IsMatchingControl (failure, bocListControl))
          _validationFailures.Add (failure);
        else
          yield return failure;
      }

      if (_validationFailures.Any ())
        Validate ();

      ErrorMessage = string.Join ("\r\n", _validationFailures.Select (f => f.ErrorMessage));
    }

    private bool IsMatchingControl (ValidationFailure failure, BocList bocControl)
    {
      var validatedInstance = failure.GetValidatedInstance();
      //bocControl.DataSource != null;
      //bocControl.Property != null;

      
      //bocControl.Property.Identifier == failure.PropertyName;

      //bool isObject = bocControl.DataSource.BusinessObject == validatedInstance;

      if (failure.PropertyName == bocControl.PropertyIdentifier)
        return true;
      if (GetShortPropertyName (failure) == bocControl.PropertyIdentifier)
        return true;
      return false;
    }

    private string GetShortPropertyName (ValidationFailure failure)
    {
      return failure.PropertyName.Split ('.').Last ();
    }

    protected override bool EvaluateIsValid ()
    {
      return !_validationFailures.Any ();
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty (controlToValidate))
        return base.ControlPropertiesValid ();
      else
        return NamingContainer.FindControl (controlToValidate) != null;
    }
  }
}