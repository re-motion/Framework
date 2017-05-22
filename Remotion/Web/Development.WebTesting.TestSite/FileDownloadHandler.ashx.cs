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
using System.ComponentModel;
using System.Numerics;
using System.Threading;
using System.Web;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  /// <summary>
  /// File Download Handler serving files based on the value of the 'testMode' parameter. Used by the <see cref="FileDownloadTest"/>.
  /// </summary>
  public class FileDownloadHandler : IHttpHandler
  {
    public void ProcessRequest (HttpContext context)
    {
      switch (context.Request.Params["testMode"])
      {
        case null:
          throw new HttpException (404, "FileDownloadHandler only returns files if the Parameter 'testMode' is set.");
        case "xml":
          AddFileToResponse (context, "SampleXmlFile.xml");
          break;
        case "txt":
          AddFileToResponse (context, "SampleFile.txt");
          break;
        case "withoutExtension":
          AddFileToResponse (context, "SampleFile.txt", "SampleFile");
          break;
        case "longRunning":
          LongRunningResponse (context);
          break;
        default:
          throw new HttpException (404, "Parameter 'testMode' only supports 'txt', 'xml', 'withoutExtension', and 'longRunning'.");
      }
    }

    private static void AddFileToResponse (HttpContext context, string file)
    {
      AddFileToResponse (context, file, file);
    }

    private static void AddFileToResponse (HttpContext context, string file, string fileName)
    {
      var response = context.Response;
      var fullFilePath = context.Server.MapPath ("~/" + file);

      response.Clear();
      response.ClearHeaders();
      response.ClearContent();
      response.AddHeader ("Content-Disposition", "attachment; filename=" + fileName);
      response.AddHeader ("Content-Length", file.Length.ToString());
      response.ContentType = "text/plain";
      response.Flush();
      response.TransmitFile (fullFilePath);
      response.End();
    }

    private void LongRunningResponse (HttpContext context)
    {
      var response = context.Response;
      response.Buffer = false;
      response.Clear();
      response.ClearHeaders();
      response.ClearContent();
      response.AddHeader ("Content-Disposition", "attachment; filename=SampleFile.txt");
      BigInteger length = 1024 * 500;
      length = length * 2;
      response.AddHeader ("Content-Length", length.ToString());  //Some high number so browser does not think we are finished
      response.ContentType = "text/plain";
      response.Flush();
       
      byte[] someMagicNumber = new byte[1024 * 500];
      new Random().NextBytes (someMagicNumber);
      response.OutputStream.Write (someMagicNumber, 0, someMagicNumber.Length);
      response.OutputStream.Flush();
      Thread.Sleep (TimeSpan.FromSeconds(5));
    }

    public bool IsReusable
    {
      get
      {
        return false;
      }
    }
  }
}