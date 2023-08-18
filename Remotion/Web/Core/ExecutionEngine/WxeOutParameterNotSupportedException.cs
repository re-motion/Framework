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

/// <summary>
/// This exception occurs when a WxeFunction constructor with one or more <c>out</c> parameters is called.
/// </summary>
[Serializable]
public class WxeOutParameterNotSupportedException: NotSupportedException
{
  public new const string Message = "Cannot use WxeFunction constructors with one or more 'out' parameters. These constructors are provided for intellisense only.";

  public WxeOutParameterNotSupportedException ()
    : base(Message)
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  public WxeOutParameterNotSupportedException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

}
