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

/// <summary> This exception indicates an attempt to resubmit a page cached in the browser's history. </summary>
[Serializable]
public class WxePostbackOutOfSequenceException: WxeException
{
  public WxePostbackOutOfSequenceException ()
    : base("The server has received a post back from a page that has already been submitted. "
        + "The page's state is no longer valid. Please navigate to the start page to restart the web application.")
  {
  }

#if NET8_0_OR_GREATER
  [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
  protected WxePostbackOutOfSequenceException (SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}

}
