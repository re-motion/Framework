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
using System.Threading;
using System.Web;
using System.Web.Services;

namespace Remotion.Web.Test
{
  /// <summary>
  /// Summary description for $codebehindclassname$
  /// </summary>
  [WebService (Namespace = "http://tempuri.org/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  public class CssHandler : IHttpHandler
  {
    public void ProcessRequest (HttpContext context)
    {
      var cssClass = context.Request.QueryString["Class"];
      if (!cssClass.EndsWith ("a"))
        context.Response.Write ("@import url('CssHandler.ashx?Class=" + cssClass + "a');\r\n");
      context.Response.Write ("." + cssClass + "{color:Red}\r\n");
      Thread.Sleep (100);
    }

    public bool IsReusable
    {
      get { return false; }
    }
  }
}