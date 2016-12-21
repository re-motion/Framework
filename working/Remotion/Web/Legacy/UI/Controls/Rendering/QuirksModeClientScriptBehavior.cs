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
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Implements <see cref="IClientScriptBehavior"/> to determine if the browser supports advanced client scripting in quirks mode.
  /// </summary>
  public class QuirksModeClientScriptBehavior : IClientScriptBehavior
  {
    public QuirksModeClientScriptBehavior ()
    {
      
    }

    public bool IsBrowserCapableOfScripting(HttpContextBase httpContext, IControl control)
    {
      return IsInternetExplorer55OrHigher(httpContext, control); 
    }

    private bool IsInternetExplorer55OrHigher (HttpContextBase httpContext, IControl control)
    {
      if (ControlHelper.IsDesignMode (control))
        return true;

      bool isVersionGreaterOrEqual55 =
          httpContext.Request.Browser.MajorVersion >= 6
          || httpContext.Request.Browser.MajorVersion == 5
             && httpContext.Request.Browser.MinorVersion >= 0.5;
      bool isInternetExplorer55AndHigher =
          httpContext.Request.Browser.Browser == "IE" && isVersionGreaterOrEqual55;

      return isInternetExplorer55AndHigher;
    }
  }
}