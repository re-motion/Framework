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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ImageButtonControlObjectTest : IntegrationTest
  {
    [Test]
    [TestCaseSource(typeof(DisabledTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    public void GenericTests (GenericSelectorTestAction<ImageButtonSelector, ImageButtonControlObject> testAction)
    {
      testAction(Helper, e => e.ImageButtons(), "imageButton");
    }

    [Test]
    [TestCaseSource(typeof(HtmlIDControlSelectorTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    [TestCaseSource(typeof(IndexControlSelectorTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    [TestCaseSource(typeof(LocalIDControlSelectorTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    [TestCaseSource(typeof(FirstControlSelectorTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    [TestCaseSource(typeof(SingleControlSelectorTestCaseFactory<ImageButtonSelector, ImageButtonControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<ImageButtonSelector, ImageButtonControlObject> testAction)
    {
      testAction(Helper, e => e.ImageButtons(), "imageButton");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.ImageButtons().GetByLocalID("ImageButtonDisabled");

      Assert.That(control.IsDisabled(), Is.True);
      Assert.That(() => control.Click(), Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "Click").Message));
    }

    [Test]
    public void TestGetImageSourceUrl ()
    {
      var home = Start();

      var imageButton = home.ImageButtons().GetByLocalID("MyImageButton");
      Assert.That(imageButton.GetImageSourceUrl(), Does.EndWith("/Image/SampleIcon.gif"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      {
        var imageButton2 = home.ImageButtons().GetByLocalID("MyImageButton2");
        var completionDetection = new CompletionDetectionStrategyTestHelper(imageButton2);
        home = imageButton2.Click().Expect<WxePageObject>();
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxePostBackCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.EqualTo("MyImageButton2|MyImageButton2Command"));
      }

      {
        var imageButton3 = home.ImageButtons().GetByLocalID("MyImageButton3");
        var completionDetection = new CompletionDetectionStrategyTestHelper(imageButton3);
        home = imageButton3.Click().Expect<WxePageObject>();
        Assert.That(completionDetection.GetAndReset(), Is.TypeOf<WxeResetCompletionDetectionStrategy>());
        Assert.That(home.Scope.FindId("TestOutputLabel").Text, Is.Empty);
      }
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("ImageButtonTest.wxe");
    }
  }
}
