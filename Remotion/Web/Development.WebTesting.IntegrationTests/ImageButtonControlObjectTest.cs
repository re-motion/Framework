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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ImageButtonControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().ByID ("body_MyImageButton");
      Assert.That (imageButton.Scope.Id, Is.EqualTo ("body_MyImageButton"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().ByIndex (2);
      Assert.That (imageButton.Scope.Id, Is.EqualTo ("body_MyImageButton2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().ByLocalID ("MyImageButton");
      Assert.That (imageButton.Scope.Id, Is.EqualTo ("body_MyImageButton"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().First();
      Assert.That (imageButton.Scope.Id, Is.EqualTo ("body_MyImageButton"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var imageButton = scope.GetImageButton().Single();
      Assert.That (imageButton.Scope.Id, Is.EqualTo ("body_MyImageButton2"));

      try
      {
        home.GetImageButton().Single();
        Assert.Fail ("Should not be able to unambigously find an image button.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetImageSourceUrl ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().ByLocalID ("MyImageButton");
      Assert.That (imageButton.GetImageSourceUrl(), Is.StringEnding ("/Images/SampleIcon.gif"));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var imageButton = home.GetImageButton().ByLocalID ("MyImageButton2");
      home = imageButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("MyImageButton2|MyImageButton2Command"));

      imageButton = home.GetImageButton().ByLocalID ("MyImageButton3");
      home = imageButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("ImageButtonTest.wxe");
    }
  }
}