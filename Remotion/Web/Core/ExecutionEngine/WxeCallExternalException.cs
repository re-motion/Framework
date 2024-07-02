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

namespace Remotion.Web.ExecutionEngine
{

  /// <summary> 
  ///   This exception is thrown to cancel the processing of an event handler that called a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   When this exception is thrown in an event handler, further execution of that event handler should stop, but the 
  ///   event handler should then return normally.
  ///   </para><para>
  ///   Usually, calls to <see cref="WxeFunction"/>s eventually end up with a <see cref="System.Threading.ThreadAbortException"/> 
  ///   thrown by ASP.NET via <see cref="HttpResponse.Redirect(string)"/> or <see cref="HttpServerUtility.Transfer(string)"/>, so there
  ///   is no need to prevent code below that call from being executed immediately after the call.
  ///   </para><para>
  ///   However, calling them externally results in the current request being completed normally, with a JavaScript 
  ///   window.open() rendered out to be invoked on the client. In this case, this exception is thrown, and should be 
  ///   caught and ignored by the event handler. (Consider catching its base type <see cref="WxeIgnorableException"/>
  ///   instead.)
  ///   </para>
  /// </remarks>
  public class WxeCallExternalException : WxeIgnorableException
  {
    public WxeCallExternalException ()
      : base("This exception does not indicate an error. It is used to cancel the processing of an event handler that used "
              + "WxeCallOptionsExternal. Please catch this exception when invoking a page's Call method.")
    {
    }
  }

}
