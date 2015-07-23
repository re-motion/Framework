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
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Development.Web.ResourceHosting
{
  /// <summary>
  /// Renders directory content as an HTML page.
  /// </summary>
  public class DirectoryListingHandler : IHttpHandler
  {
    public DirectoryListingHandler ()
    {
    }

    public void ProcessRequest (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      context.Response.ContentType = "text/html";
      context.Response.Charset = "utf-8";

      var responseWriter = new HtmlTextWriter (context.Response.Output);
      responseWriter.RenderBeginTag (HtmlTextWriterTag.Html);

      RenderHead (context, responseWriter);
      RenderBody (context, responseWriter);

      responseWriter.RenderEndTag();
      responseWriter.Flush();
    }

    private void RenderBody (HttpContext context, HtmlTextWriter responseWriter)
    {
      responseWriter.RenderBeginTag (HtmlTextWriterTag.Body);
      responseWriter.RenderBeginTag (HtmlTextWriterTag.H1);
      responseWriter.WriteEncodedText (string.Format ("{0} - {1}", context.Request.Url.Host, context.Request.AppRelativeCurrentExecutionFilePath));
      responseWriter.RenderEndTag();

      responseWriter.RenderBeginTag (HtmlTextWriterTag.Hr);
      responseWriter.RenderEndTag();

      responseWriter.AddAttribute (HtmlTextWriterAttribute.Href, "../");
      responseWriter.RenderBeginTag (HtmlTextWriterTag.A);
      responseWriter.WriteEncodedText ("[To Parent Directory]");
      responseWriter.RenderEndTag();

      responseWriter.WriteBreak();
      responseWriter.WriteBreak();

      var virtualDirectory = HostingEnvironment.VirtualPathProvider.GetDirectory (context.Request.AppRelativeCurrentExecutionFilePath);

      foreach (var item in virtualDirectory.Children.Cast<VirtualFileBase>().OrderBy (v => v.Name))
      {
        responseWriter.AddAttribute (HtmlTextWriterAttribute.Style, "display: table-row;");
        responseWriter.RenderBeginTag (HtmlTextWriterTag.Li);
        
        if (item is ResourceVirtualDirectory)
        {
          var info = new DirectoryInfo (((ResourceVirtualDirectory) item).PhysicalPath);
          RenderCell(responseWriter, info.LastWriteTime.ToString());
          RenderCell(responseWriter, "<dir> ");

          responseWriter.AddAttribute (HtmlTextWriterAttribute.Href, item.VirtualPath);
          responseWriter.RenderBeginTag (HtmlTextWriterTag.A);
          responseWriter.WriteEncodedText (item.Name);
          responseWriter.RenderEndTag();
        }
        else
        {
          var info = new FileInfo (((ResourceVirtualFile) item).PhysicalPath);

          RenderCell(responseWriter, info.LastWriteTime.ToString());
          RenderCell(responseWriter, info.Length.ToString());

          responseWriter.AddAttribute (HtmlTextWriterAttribute.Href, item.VirtualPath);
          responseWriter.RenderBeginTag (HtmlTextWriterTag.A);
          responseWriter.WriteEncodedText (item.Name);
          responseWriter.RenderEndTag();
        }
      }

      responseWriter.RenderEndTag();
      responseWriter.RenderEndTag();
    }

    private void RenderCell (HtmlTextWriter responseWriter, string text)
    {
      responseWriter.AddAttribute (HtmlTextWriterAttribute.Style, "display: table-cell; padding-right: 1em;");
      responseWriter.RenderBeginTag (HtmlTextWriterTag.Span);
      responseWriter.WriteEncodedText (text);
      responseWriter.RenderEndTag();
    }

    private void RenderHead (HttpContext context, HtmlTextWriter responseWriter)
    {
      responseWriter.RenderBeginTag (HtmlTextWriterTag.Head);

      responseWriter.RenderBeginTag (HtmlTextWriterTag.Title);
      responseWriter.WriteEncodedText (string.Format ("{0} - {1}", context.Request.Url.Host, context.Request.AppRelativeCurrentExecutionFilePath));
      responseWriter.RenderEndTag();
      responseWriter.RenderEndTag();
    }

    bool IHttpHandler.IsReusable
    {
      get { return true; }
    }
  }
}