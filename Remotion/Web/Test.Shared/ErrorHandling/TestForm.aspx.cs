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
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Web.UI;

namespace Remotion.Web.Test.Shared.ErrorHandling
{
  public partial class TestForm : SmartPage
  {
    private readonly IResourceUrlFactory _resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      ScriptManager.AsyncPostBackError += ScriptManager_AsyncPostBackError;
    }

    void ScriptManager_AsyncPostBackError (object sender, AsyncPostBackErrorEventArgs e)
    {

    }
    protected void SynchronousPostbackErrorButton_Click (object sender, EventArgs e)
    {
      var f = new ErrorPageHandlerFactory();
      var errorFormVirtualPath = _resourceUrlFactory.CreateResourceUrl(typeof(ErrorForm), TestResourceType.Root, "ErrorHandling/ErrorForm.aspx").GetUrl();
      var p = f.GetHandler(Context, "GET", errorFormVirtualPath, Server.MapPath(errorFormVirtualPath));
      var stringBuilder = new StringBuilder();
      TextWriter output = new StringWriter(stringBuilder);
      var context = new HttpContext(new SimpleWorkerRequest(errorFormVirtualPath, "", output));
      context.AddError(new ApplicationException("temp error"));
      p.ProcessRequest(context);
      context.Response.Flush();
      throw new ErrorHandlingException("Synchronous Error" + stringBuilder, new ApplicationException("Inner Exception"));
    }

    protected void AsynchronousPostbackErrorButton_Click (object sender, EventArgs e)
    {
      throw new ErrorHandlingException("Asynchronous Error", new ApplicationException("Inner Exception"));
    }
  }

  internal class ErrorPageHandlerFactory : PageHandlerFactory
  {
    public ErrorPageHandlerFactory ()
    {
    }
  }
}
