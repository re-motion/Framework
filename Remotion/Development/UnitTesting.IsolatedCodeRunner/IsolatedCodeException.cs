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
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner
{
  /// <summary>
  /// The exception that is thrown by <see cref="IsolatedCodeRunner"/> when the external process throws an unknown exception.
  /// </summary>
  /// <seealso cref="IsolatedCodeRunner"/>
  public sealed class IsolatedCodeException : Exception
  {
    public string ExceptionTypeName { get; }

    public string ExceptionMessage { get; }

    public string? ExceptionStackTrace { get; }

    public IsolatedCodeException (string exceptionTypeName, string exceptionMessage, string? exceptionStackTrace)
        : base($"Isolated code threw exception of type '{exceptionTypeName}' with message '{exceptionMessage}'")
    {
      ArgumentUtility.CheckNotNull(nameof(exceptionTypeName), exceptionTypeName);
      ArgumentUtility.CheckNotNull(nameof(exceptionMessage), exceptionMessage);

      ExceptionTypeName = exceptionTypeName;
      ExceptionMessage = exceptionMessage;
      ExceptionStackTrace = exceptionStackTrace;
    }
  }
}
