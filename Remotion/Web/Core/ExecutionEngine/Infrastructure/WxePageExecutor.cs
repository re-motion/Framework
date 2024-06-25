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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class WxePageExecutor : IWxePageExecutor
  {
    private const int HttpStatusCode_NotFound = 404;

    public WxePageExecutor ()
    {
    }

    public void ExecutePage (WxeContext context, string page, bool isPostBack)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("page", page);

      string url = page;
      string queryString = context.HttpContext.Request.Url.Query;
      if (!string.IsNullOrEmpty(queryString))
      {
        queryString = queryString.Replace(":", HttpUtility.UrlEncode(":"));
        if (url.Contains("?"))
          url = url + "&" + queryString.TrimStart('?');
        else
          url = url + queryString;
      }

      WxeHandler? wxeHandlerBackUp = context.HttpContext.Handler as WxeHandler;
      Assertion.IsNotNull(wxeHandlerBackUp, "The HttpHandler must be of type WxeHandler.");
      try
      {
        context.HttpContext.Server.Transfer(url, isPostBack);
      }
      catch (HttpException httpException)
      {
        if (httpException.InnerException is WxeExecutionControlException)
          return;

        if (httpException.GetHttpCode() == HttpStatusCode_NotFound)
          throw new WxeResourceNotFoundException(string.Format("The page '{0}' does not exist.", page), httpException);

        throw;
      }
      finally
      {
        context.HttpContext.Handler = wxeHandlerBackUp;
      }
    }
  }
}
