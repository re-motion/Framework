// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Runtime.Serialization;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  public class AccessControlException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AccessControlException () : this("An access control exception occurred.") { }
    public AccessControlException (string message) : base(message) { }
    public AccessControlException (string message, Exception inner) : base(message, inner) { }

#if NET8_0_OR_GREATER
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
#endif
    protected AccessControlException (SerializationInfo info, StreamingContext context) : base(info, context) { }

    // methods and properties

  }
}
