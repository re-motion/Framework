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
using System.Web;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web.Script.Serialization;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure
{
  /// <summary>
  /// Represents a class which contains test for a <see cref="IControlSelector"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  /// <remarks>
  /// The parameters when using <see cref="TestCaseSourceAttribute"/> are: <see cref="TestCaseFactoryBase.TestSetupAction{TControlSelector,TControl}"/>.
  /// </remarks>
  public abstract class ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, TTestParameters> : TestCaseFactoryBase
      where TControlSelector : IControlSelector
      where TControl : ControlObject
      where TTestParameters : ITestParameters, new()
  {
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ClassNeverInstantiated.Local
#pragma warning disable 649
    private class TestInformation
    {
      public string Status;

      public Parameter[] Parameters;
      
      public class Parameter
      {
        public string Name;

        public string[] Arguments;
      }
    }
#pragma warning restore 649
    // ReSharper enable FieldCanBeMadeReadOnly.Local
    // ReSharper enable ClassNeverInstantiated.Local

    private IFluentControlSelector<TControlSelector, TControl> _controlSelector;
    private TTestParameters _testParameters;
    private ElementScope _frameRootElement;

    protected ControlSelectorTestCaseFactoryBase ()
    {
    }

    protected IFluentControlSelector<TControlSelector, TControl> Selector
    {
      get { return _controlSelector; }
    }

    protected TTestParameters Parameters
    {
      get { return _testParameters; }
    }

    protected void SwitchToIFrame ()
    {
      _frameRootElement.Now();
    }

    /// <summary>
    /// Browses the generic page using the specified control and page type as parameters.
    /// </summary>
    protected IControlHost BrowseGenericPage (WebTestHelper webTestHelper, string controlName, GenericPageTypes pageType)
    {
      var url = string.Concat (
          webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot,
          string.Format (TestConstants.TestUrlTemplate, controlName, (int) pageType));

      webTestHelper.MainBrowserSession.Window.Visit (url);

      var host = webTestHelper.CreateInitialPageObject<HtmlPageObject> (webTestHelper.MainBrowserSession);

      var informationText = host.Scope.FindId (TestConstants.TestInformationOutputID).Text;

      if (string.IsNullOrWhiteSpace (informationText))
        throw new InvalidOperationException ("The web page does not provide test information.");

      var serializer = new JavaScriptSerializer();
      var information = (TestInformation) serializer.Deserialize (informationText, typeof (TestInformation));

      if (information == null)
        throw new InvalidOperationException ("The generic web page provides test information in an invalid format.");

      if (information.Status == TestConstants.Fail)
        throw new InvalidOperationException (string.Format ("The generic web page does not support the control '{0}'.", controlName));
      if (information.Status != TestConstants.Ok)
        throw new InvalidOperationException (string.Format ("The generic web page provided an unknown status code '{0}'.", information.Status));

      var parameter = new TTestParameters();
      var concreteInformation = information.Parameters.FirstOrDefault (p => p.Name == parameter.Name);
      if (concreteInformation == null)
        throw new InvalidOperationException (string.Format ("The generic web page does not support the selector '{0}'.", typeof (TControlSelector).Name));
      
      parameter.Apply (concreteInformation.Arguments);

      _testParameters = parameter;

      return host;
    }

    /// <inheritdoc />
    protected override TestCaseData SetupTestCaseData (TestCaseOptions options)
    {
      return new TestCaseData ((TestSetupAction<TControlSelector, TControl>) ((a, b, c) => StartTest (a, b, c, options)));
    }

    private void StartTest (
        WebTestHelper webTestHelper,
        Func<IControlHost, IFluentControlSelector<TControlSelector, TControl>> controlSelectorFactory,
        string controlObjectName,
        TestCaseOptions testCaseOptions)
    {
      var host = BrowseGenericPage (webTestHelper, controlObjectName, testCaseOptions.PageType);

      _controlSelector = controlSelectorFactory (host);

      var frame = webTestHelper.MainBrowserSession.Window.FindFrame ("frame");
      _frameRootElement = frame.FindCss ("html");

      try
      {
        testCaseOptions.TargetMethod.Invoke (this, new object[0]);
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
          ExceptionDispatchInfo.Capture (ex.InnerException).Throw();

        throw;
      }
    }
  }
}