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
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ImageControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var image = home.GetImage().ByID ("body_MyImage");
      Assert.That (image.Scope.Id, Is.EqualTo ("body_MyImage"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var image = home.GetImage().ByIndex (2);
      Assert.That (image.Scope.Id, Is.EqualTo ("body_MyImage2"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var image = home.GetImage().ByLocalID ("MyImage");
      Assert.That (image.Scope.Id, Is.EqualTo ("body_MyImage"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var image = home.GetImage().First();
      Assert.That (image.Scope.Id, Is.EqualTo ("body_MyImage"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var image = scope.GetImage().Single();
      Assert.That (image.Scope.Id, Is.EqualTo ("body_MyImage2"));

      try
      {
        home.GetImage().Single();
        Assert.Fail ("Should not be able to unambigously find an image button.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetSourceUrl ()
    {
      var home = Start();

      var image = home.GetImage().ByLocalID ("MyImage");
      Assert.That (image.GetSourceUrl(), Is.StringEnding ("/Images/SampleIcon.gif"));

      var image3 = home.GetImage().ByLocalID ("MyImage3");
      Assert.That (image3.GetSourceUrl(), Is.Null);
    }

    [Test]
    public void TestGetAltText ()
    {
      var home = Start();

      var image = home.GetImage().ByLocalID ("MyImage");
      Assert.That (image.GetAltText(), Is.StringEnding ("My alternative text"));

      var image2 = home.GetImage().ByLocalID ("MyImage2");
      Assert.That (image2.GetAltText(), Is.Empty);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("ImageTest.aspx");
    }
  }
}