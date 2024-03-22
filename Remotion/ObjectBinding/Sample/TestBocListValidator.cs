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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class TestBocListValidator : CustomValidator
  {
    protected override bool EvaluateIsValid ()
    {
      Control control = this.NamingContainer.FindControl(ControlToValidate);
      BocList bocList = (BocList)control;
      if (! bocList.IsRequired)
        return true;
      return bocList.Value != null && bocList.Value.Count > 0;
    }

    /// <summary>
    ///   Helper function that determines whether the control specified by the 
    ///   <see cref="ControlToValidate"/> property is a valid control.
    /// </summary>
    /// <returns> 
    ///   <see langword="true"/> if the control specified by the <see cref="ControlToValidate"/>
    ///   property is a valid control; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="HttpException"> 
    ///   Thrown if the <see cref="ControlToValidate"/> is not of type <see cref="TestBocList"/>.
    /// </exception>
    protected override bool ControlPropertiesValid ()
    {
      Control control = this.NamingContainer.FindControl(ControlToValidate);

      if (! (control is TestBocList))
      {
        throw new HttpException("Control '" + ControlToValidate + "' is not of type '" + typeof(TestBocList) + "'");
      }

      return true;
    }

    /// <summary> Gets or sets the input control to validate. </summary>
    public new string ControlToValidate
    {
      get { return base.ControlToValidate; }
      set { base.ControlToValidate = value; }
    }

  }
}
