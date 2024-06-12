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

namespace Remotion.Web.UI
{
  public sealed class AsyncUnhandledException : Exception
  {
    public AsyncUnhandledException (Exception innerException)
        : base(
            "ASP.NET asynchronous postback exception. This exception can be detected in the HttpApplication's Error handler and based on the requirements, "
            + "the error can be logged, reset (via Server.ClearError()), "
            + "and the user can be client-side redirected to a specific error page via Response.Redirect(...).",
            innerException)
    {
    }
  }
}
