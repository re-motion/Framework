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

namespace Remotion.Web.Test.ErrorHandling
{
  public partial class ErrorForm : Page
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      var exception = GetLastError();
      if (exception != null)
        ErrorDetails.Text = exception.ToString().Replace ("\r\n", "\r\n<br/>");
      else
        ErrorDetails.Text = "Error";
      Server.ClearError();
    }

    private Exception GetLastError ()
    {
      var exception = Server.GetLastError();
      if (exception is HttpException)
        return exception.InnerException;
      return exception;
    }

    protected override void Render (HtmlTextWriter writer)
    {
      base.Render (writer);
    }
  }
}