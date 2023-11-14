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
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.BrowserSession;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Simplifies the execution of JavaScript.
  /// </summary>
  public static class JavaScriptExecutor
  {
    /// <summary>
    /// Executes the specified <paramref name="statement"/> and ensures that no value is returned.
    /// </summary>
    /// <exception cref="InvalidOperationException">The JavaScript statement returned a value.</exception>
    public static void ExecuteVoidStatement ([NotNull] IJavaScriptExecutor executor, [NotNull] string statement, [NotNull] params object[] args)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNullOrEmpty("statement", statement);
      ArgumentUtility.CheckNotNull("args", args);

      var result = executor.ExecuteScript(statement, args);
      if (result != null)
        throw new InvalidOperationException("JavaScript void statement returned a value.");
    }

    /// <summary>
    /// Executes the specified <paramref name="statement"/> and returns result converted to <typeparamref name="T"/>.
    /// Ensures that the result is not <see langword="null" />.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static T? ExecuteStatement<T> ([NotNull] IJavaScriptExecutor executor, [NotNull] string statement, [NotNull] params object[] args)
    {
      // TODO RM-8107: Improve null safety.

      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNullOrEmpty("statement", statement);
      ArgumentUtility.CheckNotNull("args", args);

      var result = executor.ExecuteScript(statement, args);
      if (result == null && !NullableTypeUtility.IsNullableType(typeof(T)))
      {
        throw new InvalidOperationException(
            string.Format("The JavaScript statement returned null which is incompatible with the specified type '{0}'.", typeof(T).Name));
      }

      return (T?)result;
    }

    /// <summary>
    /// Returns the <see cref="IJavaScriptExecutor"/> associated with the specified <paramref name="browserSession"/>.
    /// </summary>
    public static IJavaScriptExecutor GetJavaScriptExecutor ([NotNull] IBrowserSession browserSession)
    {
      ArgumentUtility.CheckNotNull("browserSession", browserSession);

      return (IJavaScriptExecutor)browserSession.Driver.Native;
    }

    /// <summary>
    /// Returns the <see cref="IJavaScriptExecutor"/> associated with the specified <paramref name="controlObject"/>.
    /// </summary>
    public static IJavaScriptExecutor GetJavaScriptExecutor ([NotNull] ControlObject controlObject)
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);

      return (IJavaScriptExecutor)((IWrapsDriver)controlObject.Scope.Native).WrappedDriver;
    }

    /// <summary>
    /// Returns the <see cref="IJavaScriptExecutor"/> associated with the specified <paramref name="element"/>.
    /// </summary>
    public static IJavaScriptExecutor GetJavaScriptExecutor ([NotNull] ElementScope element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      return (IJavaScriptExecutor)((IWrapsDriver)element.Native).WrappedDriver;
    }

    /// <summary>
    /// Returns the <see cref="IJavaScriptExecutor"/> associated with the specified <paramref name="webElement"/>.
    /// </summary>
    public static IJavaScriptExecutor GetJavaScriptExecutor ([NotNull] IWebElement webElement)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      return (IJavaScriptExecutor)((IWrapsDriver)webElement).WrappedDriver;
    }
  }
}
