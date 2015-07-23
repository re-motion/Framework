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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  public class LazyInitializationContainer
  {
    private bool _isEnsured;
    private readonly PlaceHolder _placeHolder;

    public LazyInitializationContainer ()
    {
      _placeHolder = new PlaceHolder ();
    }

    public bool IsInitialized
    {
      get { return _isEnsured; }
    }

    public ControlCollection GetControls (ControlCollection baseControls)
    {
      ArgumentUtility.CheckNotNull ("baseControls", baseControls);
        if (_isEnsured)
          return baseControls;
        else
          return _placeHolder.Controls;
    }

    public void Ensure (ControlCollection baseControls)
    {
      ArgumentUtility.CheckNotNull ("baseControls", baseControls);

      if (_isEnsured)
        return;

      _isEnsured = true;

      List<Control> controls = new List<Control> (_placeHolder.Controls.Cast<Control> ());
      foreach (Control control in controls)
        baseControls.Add (control);
    }
  }
}
