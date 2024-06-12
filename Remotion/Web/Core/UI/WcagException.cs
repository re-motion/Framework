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

namespace Remotion.Web.UI
{

public class WcagException: Exception
{
  public WcagException ()
    : this("An element on the page is not WCAG conform.", null)
  {
  }

  public WcagException (int priority)
    : base(string.Format("An element on the page does comply with a priority {0} checkpoint.", priority))
  {
  }

  public WcagException (int priority, Control control)
    : base(string.Format(
       "{0} '{1}' does not comply with a priority {2} checkpoint.",
        control.GetType().Name, control.ID, priority))
  {
  }

  public WcagException (int priority, Control control, string property)
    : base(string.Format(
        "The value of property '{0}' for {1} '{2}' does not comply with a priority {3} checkpoint.",
        property, control.GetType().Name, control.ID, priority))
  {
  }

  public WcagException (string message, Exception? innerException)
    : base(message, innerException)
  {
  }
}

}
