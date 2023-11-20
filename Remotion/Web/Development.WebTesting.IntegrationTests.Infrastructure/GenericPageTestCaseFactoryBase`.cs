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
using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <inheritdoc />
  public abstract class GenericTestPageTestCaseFactoryBase<TParameter> : GenericPageTestCaseFactoryBase
      where TParameter : IGenericTestPageParameter, new()
  {
    /// <summary>
    /// The page type of the generic page.
    /// </summary>
    public GenericTestPageType PageType { get; private set; }

    /// <summary>
    /// The parameters supplied by the generic page.
    /// </summary>
    public TParameter Parameter { get; private set; }

    private TestConstants TestConstants { get; } = new TestConstants();

    protected GenericTestPageTestCaseFactoryBase ()
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<TestCaseData> GetTests ()
    {
      return GetTests<GenericPageTestMethodAttribute>(CreateTestCaseData);
    }

    protected void PrepareTest ([NotNull] GenericPageTestMethodAttribute attribute, [NotNull] WebTestHelper helper, [NotNull] string control)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("helper", helper);
      ArgumentUtility.CheckNotNullOrEmpty("control", control);

      var url = string.Concat(
          helper.TestInfrastructureConfiguration.WebApplicationRoot,
          string.Format(TestConstants.GenericPageUrlTemplate, control, (int)attribute.PageType));

      base.PrepareTest(attribute, helper, url);

      var text = Home.Scope.FindId(TestConstants.GenericPageOutputID).Text;
      if (string.IsNullOrWhiteSpace(text))
        throw new InvalidOperationException("The generic test page did not provide any test information.");

      var serializerOptions = new JsonSerializerOptions { Converters = { GenericTestParameterConverter.Instance } };
      var dto = JsonSerializer.Deserialize<GenericTestPageParameterDto>(text, serializerOptions);
      if (dto == null)
        throw new InvalidOperationException("The generic test page output is not in a correct format.");

      switch (dto.Status)
      {
        case GenericTestPageStatus.Failed:
          throw new InvalidOperationException(string.Format("The generic test page does not support the control '{0}'", control));
        case GenericTestPageStatus.Ok:
          break;
        case GenericTestPageStatus.Unknown:
          throw new InvalidOperationException("The generic web page encountered an unknown error.");
        default:
          throw new InvalidOperationException("The generic web page provided an invalid status code.");
      }

      var parameter = new TParameter();
      var argumentCount = parameter.Count;
      if (argumentCount > 0)
      {
        if (!dto.Parameters.TryGetValue(parameter.Name, out var information))
        {
          throw new InvalidOperationException(
              string.Format("The generic web page did not provide any arguments, but {0} arguments where expected.", argumentCount));
        }

        if (information.Arguments.Count != argumentCount)
        {
          throw new InvalidOperationException(
              string.Format(
                  "The generic web page provided {0} arguments, but {1} arguments where expected.",
                  argumentCount,
                  information.Arguments.Count));
        }

        parameter.Apply(information);
      }

      PageType = attribute.PageType;
      Parameter = parameter;
    }

    private TestCaseData CreateTestCaseData ([NotNull] GenericPageTestMethodAttribute attribute, [NotNull] MethodInfo method)
    {
      ArgumentUtility.CheckNotNull("attribute", attribute);
      ArgumentUtility.CheckNotNull("method", method);

      return new TestCaseData(
          (GenericTestSetupAction)((helper, control) =>
          {
            PrepareTest(attribute, helper, control);
            RunTest(method);
          }));
    }
  }
}
