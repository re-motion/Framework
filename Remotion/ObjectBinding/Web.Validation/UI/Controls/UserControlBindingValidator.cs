using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FluentValidation.Results;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Validation.Utilities;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public class UserControlBindingValidator: BaseValidator, IBocValidator
  {
    private List<ValidationFailure> _unhandledFailures = new List<ValidationFailure> ();

    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var control = NamingContainer.FindControl (ControlToValidate);
      var userControlBinding = control as UserControlBinding;
      if (userControlBinding == null)
        throw new InvalidOperationException ("UserControlBindingValidator may only be applied to controls of type UserControlBinding");
      
      userControlBinding.Validate();

      var namingContainer = userControlBinding.UserControl.DataSource.NamingContainer;
      var validator =
          EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<BocDataSourceValidator>()
              .SingleOrDefault (
                  c => c.ControlToValidate == userControlBinding.UserControl.DataSource.ID,
                  () => new InvalidOperationException ("Only zero or one BocDataSourceValidator is allowed per UserControlBinding."));
      
      _unhandledFailures.Clear();
      if (validator != null)
      {
        _unhandledFailures = validator.ApplyValidationFailures (failures).ToList();
      }
      else
      {
        _unhandledFailures = failures.ToList();
      }
      ErrorMessage = string.Join ("\r\n", _unhandledFailures.Select (f => f.ErrorMessage));

      if (_unhandledFailures.Any ())
        Validate ();
      return _unhandledFailures;
    }

    private bool IsMatchingControl (ValidationFailure failure, UserControlBinding userControlBinding)
    {
      if (!userControlBinding.HasValidBinding)
        return false;

      var validatedInstance = failure.GetValidatedInstance();
      var businessObject = userControlBinding.DataSource != null ? userControlBinding.DataSource.BusinessObject : null;

      if (validatedInstance != null && businessObject != null
          && validatedInstance != businessObject)
        return false;

      bool isMatchingProperty = failure.PropertyName == userControlBinding.Property.Identifier;
      if (!isMatchingProperty)
        isMatchingProperty = GetShortPropertyName (failure) == userControlBinding.Property.Identifier;

      bool isMatchinInstance = validatedInstance == null || validatedInstance == businessObject;

      if (isMatchingProperty && isMatchinInstance)
        return true;

      return false;
      //bocControl.DataSource != null;
      //bocControl.Property != null;

      
      //bocControl.Property.Identifier == failure.PropertyName;

      //bool isObject = bocControl.DataSource.BusinessObject == validatedInstance;

      //if (failure.PropertyName == bocControl.PropertyIdentifier)
      //  return true;
      //if (GetShortPropertyName (failure) == bocControl.PropertyIdentifier)
      //  return true;
      //return false;
    }

    private string GetShortPropertyName (ValidationFailure failure)
    {
      return failure.PropertyName.Split ('.').Last ();
    }

    protected override bool EvaluateIsValid ()
    {
      return !_unhandledFailures.Any ();
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