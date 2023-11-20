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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine
{

[Serializable]
public class WxeTimeoutException: WxeException
{
  private string? _functionToken;

  public WxeTimeoutException (string message, string functionToken)
    : this(message, functionToken, null)
  {
  }

  public WxeTimeoutException (string message, string functionToken, Exception? innerException)
    : base(message, innerException)
  {
    _functionToken = functionToken;
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  public WxeTimeoutException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public string? FunctionToken
  {
    get { return _functionToken; }
  }
}

}
