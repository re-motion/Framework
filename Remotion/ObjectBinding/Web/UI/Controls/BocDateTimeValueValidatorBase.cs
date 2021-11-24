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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> Validates a <see cref="BocDateTimeValue"/> control. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocDateTimeValueValidatorBase.xml' path='BocDateTimeValueValidatorBase/Class/*' />
  [ToolboxItem (false)]
  public abstract class BocDateTimeValueValidatorBase : BaseValidator
  {
    protected abstract void RefreshBaseErrorMessage ();

    /// <summary>
    ///   Helper function that determines whether the control specified by the 
    ///   <see cref="ControlToValidate"/> property is a valid control.
    /// </summary>
    /// <returns> 
    ///   <see langword="true"/> if the control specified by the <see cref="ControlToValidate"/>
    ///   property is a valid control; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="HttpException"> 
    ///   Thrown if the <see cref="ControlToValidate"/> is not of type <see cref="BocDateTimeValue"/>.
    /// </exception>
    protected override bool ControlPropertiesValid ()
    {
      if (!base.ControlPropertiesValid())
        return false;

      Control? control = NamingContainer.FindControl (ControlToValidate);

      if (!(control is BocDateTimeValue))
        throw new HttpException ("Control '" + ControlToValidate + "' is not of type '" + typeof (BocDateTimeValue) + "'");

      return true;
    }

    /// <summary> Gets or sets the input control to validate. </summary>
    [AllowNull]
    public new string ControlToValidate
    {
      get { return base.ControlToValidate; }
      set { base.ControlToValidate = value; }
    }

    /// <summary> Gets or sets the text for the error message. </summary>
    /// <remarks> Will be set to one of the more specific error messages, if they are provided. </remarks>
    [Browsable (false)]
    [AllowNull]
    public new string ErrorMessage
    {
      get
      {
        RefreshBaseErrorMessage();
        return base.ErrorMessage;
      }
      set { base.ErrorMessage = value; }
    }
  }
}