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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{

/// <summary>
///   Calls <see cref="ISupportsPostLoadControl.OnPostLoad"/> on all controls that support the interface.
/// </summary>
/// <remarks>
///   Children are called after their parents.
/// </remarks>
public class PostLoadInvoker
{
  public static void InvokePostLoad (Control control)
  {
    if (control is ISupportsPostLoadControl)
      ((ISupportsPostLoadControl)control).OnPostLoad ();

    ControlCollection controls = control.Controls;
    for (int i = 0; i < controls.Count; ++i)
    {
      Control childControl = controls[i];
      InvokePostLoad (childControl);
    }
  }

  private Control _control;
  private bool _invoked;

  public PostLoadInvoker (Control control)
  {
    _control = control;
    _invoked = false;
  }

  public void EnsurePostLoadInvoked ()
  {
    if (! _invoked)
    {
      InvokePostLoad (_control);
      _invoked = true;
    }
  }

}

}
