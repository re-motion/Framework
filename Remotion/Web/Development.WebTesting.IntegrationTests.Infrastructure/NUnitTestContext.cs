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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

public class NUnitTestContext (TestContext testContext, TestExecutionContext testExecutionContext) : ITestContext
{
  private static IReadOnlyDictionary<string, object> GetPropertiesFromAttributes (TestExecutionContext testExecutionContext)
  {
    var testMethod = testExecutionContext.CurrentTest.Method;
    if (testMethod == null)
      throw new InvalidOperationException($"Current test '{testExecutionContext.CurrentTest.FullName}' does not have a method associated with it.");

    return WebTestAttribute.CreatePropertiesFromAttributes(testMethod.MethodInfo);
  }

  public string TestName => testContext.Test.Type?.Name + "_" + testContext.Test.Name;

  public bool IsSuccessful => testExecutionContext.CurrentResult.ResultState != ResultState.Failure;

  public void SetFailure (string message) => testExecutionContext.CurrentResult.SetResult(ResultState.Failure, message);

  public IReadOnlyDictionary<string, object> Properties { get; } = GetPropertiesFromAttributes(testExecutionContext);
}
