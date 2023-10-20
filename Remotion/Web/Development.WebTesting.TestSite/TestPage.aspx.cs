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
using System.IO;
using System.Security.Cryptography;
using System.Web.UI;
using Remotion.Web.UI;


namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class TestPage : SmartPage
  {
    private static string GenerateRandomNonceValue ()
    {
      using var rng = RandomNumberGenerator.Create();
      return Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    }

    protected void Page_Load (object sender, EventArgs e)
    {
      string nonceValue;
      if (this.ScriptManager1.IsInAsyncPostBack)
      {
        nonceValue = (string)this.ViewState["nonce"];
      }
      else
      {
        nonceValue = GenerateRandomNonceValue();
        ViewState["nonce"] = nonceValue;
      }
      Response.Headers.Add("Content-Security-Policy",  $"default-src 'self'; script-src 'unsafe-eval' 'self' 'nonce-{nonceValue}'; script-src-attr 'unsafe-inline';");
    }

    protected void UpdateButton_OnClick (object sender, EventArgs e)
    {
      var myCount = Convert.ToInt32(InnerResult.Text);
      myCount += 1;
      InnerResult.Text = myCount.ToString();
      OuterResult.Text = myCount.ToString();
    }

    protected override HtmlTextWriter CreateHtmlTextWriter (TextWriter tw)
    {
      return new CustomHtmlTextWriter(this, tw, (string)ViewState["nonce"]);
    }
  }
}
