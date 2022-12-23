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
using System.Runtime.ExceptionServices;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner
{
  /// <summary>
  /// Serializes exceptions to strings for cross process communication.
  /// </summary>
  /// <see cref="IsolatedCodeRunner"/>
  public static class ExceptionSerializer
  {
    public static string SerializeException (Exception exception)
    {
      ArgumentUtility.CheckNotNull("exception", exception);

      var exceptionTypeName = exception.GetType().AssemblyQualifiedName!;
      var exceptionMessage = exception.Message;
      var exceptionStackTrace = exception.StackTrace ?? string.Empty;

      return string.Join(
          ";",
          EncodeBase64(exceptionTypeName),
          EncodeBase64(exceptionMessage),
          EncodeBase64(exceptionStackTrace));
    }

    public static Exception DeserializeException (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("value", value);

      var parts = value.Split(';');
      if (parts.Length != 3)
        throw new ArgumentException("The specified value is not a valid serialized exception", nameof(value));

      var exceptionTypeName = DecodeBase64(parts[0]);
      var exceptionMessage = DecodeBase64(parts[1]);
      var exceptionStackTrace = DecodeBase64(parts[2]);

      Exception? remoteException = null;
      try
      {
        var type = Type.GetType(exceptionTypeName, true)!;

        // Try to recreate the exception using the (string message) constructor
        var messageConstructor = type.GetConstructor(new[] { typeof(string) });
        if (messageConstructor != null)
        {
          remoteException = (Exception)messageConstructor.Invoke(new object[] { exceptionMessage });
        }
        else
        {
          // Try to recreate the exception using the (string message, Exception? ex) constructor as fallback
          var messageAndInnerExceptionConstructor = type.GetConstructor(new[] { typeof(string), typeof(Exception) });
          if (messageAndInnerExceptionConstructor != null)
            remoteException = (Exception)messageAndInnerExceptionConstructor.Invoke(new object?[] { exceptionMessage, null });
        }
      }
      catch
      {
        // Could not recreate the exception -> will return generic IsolatedCodeException instead
      }

      if (remoteException != null && !string.IsNullOrEmpty(exceptionStackTrace))
        ExceptionDispatchInfo.SetRemoteStackTrace(remoteException, exceptionStackTrace);

      return remoteException ?? new IsolatedCodeException(exceptionTypeName, exceptionMessage, exceptionStackTrace);
    }

    private static string DecodeBase64 (string value) => Encoding.UTF8.GetString(Convert.FromBase64String(value));

    private static string EncodeBase64 (string? value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? string.Empty));
  }
}
