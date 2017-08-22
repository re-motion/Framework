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
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ImageControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<ImageSelector, ImageControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<ImageSelector, ImageControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<ImageSelector, ImageControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<ImageSelector, ImageControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<ImageSelector, ImageControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<ImageSelector, ImageControlObject> testSetupAction)
    {
      testSetupAction (Helper, e => e.Images(), "image");
    }

    [Test]
    public void TestGetSourceUrl ()
    {
      var home = Start();

      var image = home.Images().GetByLocalID ("MyImage");
      Assert.That (image.GetSourceUrl(), Is.StringEnding ("/Images/SampleIcon.gif"));

      var image3 = home.Images().GetByLocalID ("MyImage3");
      Assert.That (image3.GetSourceUrl(), Is.Null);
    }

    [Test]
    public void TestGetAltText ()
    {
      var home = Start();

      var image = home.Images().GetByLocalID ("MyImage");
      Assert.That (image.GetAltText(), Is.StringEnding ("My alternative text"));

      var image2 = home.Images().GetByLocalID ("MyImage2");
      Assert.That (image2.GetAltText(), Is.Empty);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("ImageTest.aspx");
    }
  }
}