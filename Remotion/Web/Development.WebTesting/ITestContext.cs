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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting;

/// <summary>
/// Minimalist abstraction of a test context in order to provide necessary functionality while remaining independent of any specific testing framework. 
/// </summary>
/// <remarks>
/// We provide no default implementation, but you can use the example code with NUnit.
/// </remarks>
/// <example>
/// <code>
/// public class NUnitTestContext (TestContext testContext, TestExecutionContext testExecutionContext) : ITestContext
/// {
///   private static IReadOnlyDictionary&lt;string, object&gt; GetPropertiesFromAttributes (TestExecutionContext testExecutionContext)
///   {
///     var testMethod = testExecutionContext.CurrentTest.Method;
///     if (testMethod == null)
///       throw new InvalidOperationException($"Current test '{testExecutionContext.CurrentTest.FullName}' does not have a method associated with it.");
///
///     return WebTestAttribute.CreatePropertiesFromAttributes(testMethod.MethodInfo);
///   }
///
///   public string TestName => testContext.Test.Type?.Name + "_" + testContext.Test.Name;
///
///   public bool IsSuccessful => testExecutionContext.CurrentResult.ResultState != ResultState.Failure;
///
///   public void SetFailure (string message) => testExecutionContext.CurrentResult.SetResult(ResultState.Failure, message);
///
///   public IReadOnlyDictionary&lt;string, object&gt; Properties { get; } = GetPropertiesFromAttributes(testExecutionContext);
/// }
/// </code>
/// </example>
public interface ITestContext
{
  /// <summary>
  /// The name of the currently executing test.
  /// </summary>
  /// <remarks>
  /// This is used for logging purposes.
  /// </remarks>
  [NotNull]
  string TestName { get; }

  /// <summary>
  /// Generic container for context information. 
  /// </summary>
  /// <remarks>
  /// Annotate a test method or class with a <see cref="WebTestAttribute"/> to populate this <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
  /// </remarks>
  [NotNull] IReadOnlyDictionary<string, object> Properties { get; }

  /// <summary>
  /// Indicates whether the current test has been successful so far. Returns <see langword="false"/> only if the test has failed.
  /// </summary>
  bool IsSuccessful { get; }

  /// <summary>
  /// Sets the current test's status to failed, so that <see cref="IsSuccessful"/> is <see langword="false"/>.
  /// Indicate the reason for the failure in <paramref name="message"/>. 
  /// </summary>
  void SetFailure ([NotNull] string message);
}
