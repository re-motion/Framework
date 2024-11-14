// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Provides the ability to register validators with their target control and call validate on the web controls themselves.
/// </summary>
/// <remarks>
///   Use <see cref="ValidatableControlInitializer"/> to register all validators with their validatable controls.
/// </remarks>
public interface IValidatableControl: IControl
{
  /// <summary>
  ///   Registers a validator that references this control.
  /// </summary>
  /// <remarks>
  ///   The control may choose to ignore this call.
  /// </remarks>
  void RegisterValidator (BaseValidator validator);

  /// <summary>
  ///   Calls <see cref="BaseValidator.Validate"/> on all registered validators.
  /// </summary>
  /// <returns> True, if all validators validated. </returns>
  bool Validate ();

  /// <summary>
  /// This method puts the control into a state in which validation can be performed. 
  /// This may include populating control values from the view state explicitly.
  /// </summary>
  void PrepareValidation ();

  /// <summary>
  ///   Gets the input control that can be referenced by the <see cref="BaseValidator"/>.
  /// </summary>
  Control TargetControl { get; }
}

/// <summary>
///   Initializes validators for <see cref="IValidatableControl"/>.
/// </summary>
public class ValidatableControlInitializer
{
  /// <summary>
  ///   Registers validators with their <see cref="IValidatableControl"/> web controls.
  /// </summary>
  /// <remarks>
  ///   All <see cref="BaseValidator"/> controls within <paramref name="page"/> that validate a 
  ///   <see cref="IValidatableControl"/> control are registered. This method is best called
  ///   from a postback event handler.
  /// </remarks>
  public static void InitializeValidatableControls (Page page)
  {
    foreach (IValidator ivalidator in page.Validators)
    {
      BaseValidator? validator = ivalidator as BaseValidator;
      if (validator == null)
        continue;

      Control? validatedControl = validator.NamingContainer.FindControl(validator.ControlToValidate);

      if (validatedControl is IValidatableControl)
      {
        // register validator with parent
        ((IValidatableControl)validatedControl).RegisterValidator(validator);
      }
      else
      {
        // try to find a parent control that supports IValidatableControl and has validatedControl as TargetControl
        // (the validator may point to a child control of the control that should actually be validated)
        for (Control? parentControl = validatedControl;
            parentControl != null;
            parentControl = parentControl.Parent)
        {
          if (   parentControl is IValidatableControl
              && parentControl is ISmartControl
              && ((IValidatableControl)parentControl).TargetControl == validatedControl)
          {
            ((IValidatableControl)parentControl).RegisterValidator(validator);
          }
          continue;
        }
      }
    }
  }

  private Page _page;
  private bool _initialized;

  /// <summary>
  ///   Creates a new initializer for <c>page</c> and all sub-controls.
  /// </summary>
  public ValidatableControlInitializer (Page page)
  {
    _page = page;
    _initialized = false;
  }

  /// <summary>
  ///   When called the first time, registers validators with their controls. Call this method before validating.
  /// </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    if (! _initialized)
    {
      ValidatableControlInitializer.InitializeValidatableControls(_page);
      _initialized = true;
    }
  }
}

}
