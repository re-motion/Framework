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
using NUnit.Framework;

namespace Remotion.Development.UnitTesting.IsolatedCodeRunner.UnitTests
{
  [TestFixture]
  public class ExceptionSerializerTest
  {
    private class WithMessageConstructorException : Exception
    {
      public WithMessageConstructorException (string message)
          : base(message)
      {
      }
    }

    private class WithMessageAndInnerExceptionException : Exception
    {
      public WithMessageAndInnerExceptionException (string message, Exception exception)
          : base(message, exception)
      {
      }
    }

    private class WithInvalidConstructorException : Exception
    {
      public WithInvalidConstructorException (int value)
      {
      }
    }

    [Test]
    public void SerializeException ()
    {
      var exception = ExceptionDispatchInfo.SetRemoteStackTrace(new Exception("message😂"), "stackTrace");

      Assert.That(
          ExceptionSerializer.SerializeException(exception),
          Is.EqualTo(
              $"{EncodeBase64(typeof(Exception).AssemblyQualifiedName)};{EncodeBase64("message😂")};{EncodeBase64("stackTrace\r\n--- End of stack trace from previous location ---\r\n")}"));
    }

    [Test]
    public void Deserialize_WithInvalidValue_Throws ()
    {
      Assert.That(
          () => ExceptionSerializer.DeserializeException($"{EncodeBase64("abc")};{EncodeBase64("abc")}"),
          Throws.ArgumentException
              .With.Message.EqualTo("The specified value is not a valid serialized exception (Parameter 'value')"));
    }

    [Test]
    public void Deserialize_WithMessageConstructor_ReconstructsException ()
    {
      var exception = ExceptionSerializer.DeserializeException(
          $"{EncodeBase64(typeof(WithMessageConstructorException).AssemblyQualifiedName)};{EncodeBase64("message")};{EncodeBase64("stackTrace")}");

      Assert.That(exception, Is.TypeOf<WithMessageConstructorException>());
      Assert.That(exception.Message, Is.EqualTo("message"));
      Assert.That(exception.StackTrace, Is.EqualTo("stackTrace\r\n--- End of stack trace from previous location ---\r\n"));
    }

    [Test]
    public void Deserialize_WithMessageAndExceptionConstructor_ReconstructsException ()
    {
      var exception = ExceptionSerializer.DeserializeException(
          $"{EncodeBase64(typeof(WithMessageAndInnerExceptionException).AssemblyQualifiedName)};{EncodeBase64("message")};{EncodeBase64("stackTrace")}");

      Assert.That(exception, Is.TypeOf<WithMessageAndInnerExceptionException>());
      Assert.That(exception.Message, Is.EqualTo("message"));
      Assert.That(exception.StackTrace, Is.EqualTo("stackTrace\r\n--- End of stack trace from previous location ---\r\n"));
    }

    [Test]
    public void Deserialize_WithInvalidExceptionConstructor_CreatesIsolatedCodeException ()
    {
      var exception = ExceptionSerializer.DeserializeException(
          $"{EncodeBase64(typeof(WithInvalidConstructorException).AssemblyQualifiedName)};{EncodeBase64("message")};{EncodeBase64("stackTrace")}");

      Assert.That(exception, Is.TypeOf<IsolatedCodeException>());
      Assert.That(
          exception.Message,
          Is.EqualTo($"Isolated code threw exception of type '{typeof(WithInvalidConstructorException).AssemblyQualifiedName}' with message 'message'"));
      Assert.That(exception.StackTrace, Is.Null);

      var isolatedCodeException = (IsolatedCodeException)exception;
      Assert.That(isolatedCodeException.ExceptionTypeName, Is.EqualTo(typeof(WithInvalidConstructorException).AssemblyQualifiedName));
      Assert.That(isolatedCodeException.ExceptionMessage, Is.EqualTo("message"));
      Assert.That(isolatedCodeException.ExceptionStackTrace, Is.EqualTo("stackTrace"));
    }

    private static string EncodeBase64 (string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value ?? string.Empty));
  }
}
