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

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class FileDownloadTest : Page
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      DownloadButton.Click += DownloadButtonOnClick;
    }

    private void DownloadButtonOnClick (object sender, EventArgs eventArgs)
    {
      const string file = "SampleFile.txt";
      var fullFilePath = Server.MapPath ("~/SampleFile.txt");

      Response.Clear();
      Response.ClearHeaders();
      Response.ClearContent();
      Response.AddHeader ("Content-Disposition", "attachment; filename=" + file);
      Response.AddHeader ("Content-Length", file.Length.ToString());
      Response.ContentType = "text/plain";
      Response.Flush();
      Response.TransmitFile (fullFilePath);
      Response.End();
    }
  }
}