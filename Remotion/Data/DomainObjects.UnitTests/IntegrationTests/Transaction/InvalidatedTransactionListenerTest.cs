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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class InvalidatedTransactionListenerTest
  {
    [Test]
    public void AllMethodsMustThrow ()
    {
      var listener = new InvalidatedTransactionListener();
      MethodInfo[] methods =
          typeof(InvalidatedTransactionListener).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      Assert.That(methods.Length, Is.EqualTo(38));

      foreach (var method in methods)
      {
        var concreteMethod =
            method.Name == "FilterQueryResult" || method.Name == "FilterCustomQueryResult"
            ? method.MakeGenericMethod(typeof(Order))
            : method;

        object[] arguments = Array.ConvertAll(concreteMethod.GetParameters(), p => GetDefaultValue(p.ParameterType));

        ExpectException(
            typeof(InvalidOperationException),
            "The transaction can no longer be used because it has been discarded.",
            listener,
            concreteMethod,
            arguments);
      }
    }

    private void ExpectException (
        Type expectedExceptionType,
        string expectedMessage,
        InvalidatedTransactionListener listener,
        MethodInfo method,
        object[] arguments)
    {
      try
      {
        method.Invoke(listener, arguments);
        Assert.Fail(BuildErrorMessage(expectedExceptionType, method, arguments, "the call succeeded."));
      }
      catch (TargetInvocationException tex)
      {
        Exception ex = tex.InnerException;
        if (ex.GetType() == expectedExceptionType)
        {
          if (ex.Message == expectedMessage)
            return;
          else
          {
            Assert.Fail(
                BuildErrorMessage(
                    expectedExceptionType,
                    method,
                    arguments,
                    string.Format("the message was incorrect.\nExpected: {0}\nWas:      {1}", expectedMessage, ex.Message)));
          }
        }
        else
          Assert.Fail(BuildErrorMessage(expectedExceptionType, method, arguments, "the exception type was " + ex.GetType() + ".\n" + ex));
      }
    }

    private string BuildErrorMessage (Type expectedExceptionType, MethodInfo method, object[] arguments, string problem)
    {
      return string.Format(
          "Expected {0} when calling {1}({2}), but {3}",
          expectedExceptionType,
          method.Name,
          ReflectionUtility.GetSignatureForArguments(arguments),
          problem);
    }

    private object GetDefaultValue (Type t)
    {
      if (t.IsValueType)
        return Activator.CreateInstance(t);
      else
        return null;
    }
  }
}
