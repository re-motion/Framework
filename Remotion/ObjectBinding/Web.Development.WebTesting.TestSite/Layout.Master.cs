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
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class Layout : MasterPage
  {
    protected override void OnInit (EventArgs e)
    {
      var requestUrl = Request.Url;

      var query = HttpUtility.ParseQueryString (requestUrl.Query);
      query["GuaranteeRefresh"] = Guid.NewGuid().ToString();

      RefreshButton.NavigateUrl = requestUrl.GetLeftPart (UriPartial.Path) + "?" + query;

      base.OnInit (e);
    }

    protected override void OnPreRender (EventArgs e)
    {
      HtmlHeadAppender.Current.RegisterPageStylesheetLink();

      base.OnPreRender (e);
    }

    public UserControl GetTestOutputControl ()
    {
      return (UserControl) testOutput.Controls[1].Controls[0];
    }
  }
}