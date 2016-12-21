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
  public class WebButtonControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByID ("body_MyWebButton1Sync");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton1Sync"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByIndex (2);
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton2Async"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByLocalID ("MyWebButton3Href");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton3Href"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var webButton = home.GetWebButton().First();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton1Sync"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var webButton = scope.GetWebButton().Single();
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton3Href"));

      try
      {
        home.GetWebButton().Single();
        Assert.Fail ("Should not be able to unambigously find a button.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_Text ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByTextContent ("AsyncButton");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton2Async"));
    }

    [Test]
    public void TestSelection_ItemID ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByItemID ("MyWebButton2Async");
      Assert.That (webButton.Scope.Id, Is.EqualTo ("body_MyWebButton2Async"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByLocalID ("MyWebButton1Sync");
      Assert.That (webButton.GetText(), Is.EqualTo ("SyncButton"));
    }

    [Test]
    public void TestIsEnabled ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByLocalID ("MyWebButton1Sync");
      Assert.That (webButton.IsEnabled(), Is.True);

      var disabledWebButton = home.GetWebButton().ByLocalID ("MyDisabledWebButton");
      Assert.That (disabledWebButton.IsEnabled(), Is.False);
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var syncWebButton = home.GetWebButton().ByLocalID ("MyWebButton1Sync");
      home = syncWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Sync"));

      var asyncWebButton = home.GetWebButton().ByLocalID ("MyWebButton2Async");
      home = asyncWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Async"));

      var hrefWebButton = home.GetWebButton().ByTextContent ("HrefButton");
      home = hrefWebButton.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    [Test]
    public void TestHasClass ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.HasCssClass ("buttonBody"), Is.True);
      Assert.That (webButton.StyleInfo.HasCssClass ("doesNotHaveThisClass"), Is.False);
    }

    [Test]
    public void TestGetBackgroundColor ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.GetBackgroundColor(), Is.EqualTo (WebColor.FromRgb (221, 221, 221)));
    }

    [Test]
    public void TestGetTextColor ()
    {
      var home = Start();

      var webButton = home.GetWebButton().ByID ("body_MyWebButton1Sync");
      Assert.That (webButton.StyleInfo.GetTextColor(), Is.EqualTo (WebColor.Black));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("WebButtonTest.wxe");
    }
  }
}