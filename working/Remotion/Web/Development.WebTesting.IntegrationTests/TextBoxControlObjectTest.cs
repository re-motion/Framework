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
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class TextBoxControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var editableTextBox = home.GetTextBox().ByID ("body_MyEditableTextBox");
      Assert.That (editableTextBox.Scope.Id, Is.EqualTo ("body_MyEditableTextBox"));

      var aspTextBox = home.GetTextBox().ByID ("body_MyAspTextBox");
      Assert.That (aspTextBox.Scope.Id, Is.EqualTo ("body_MyAspTextBox"));

      var htmlTextBox = home.GetTextBox().ByID ("body_MyHtmlTextBox");
      Assert.That (htmlTextBox.Scope.Id, Is.EqualTo ("body_MyHtmlTextBox"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var htmlAnchor = home.GetTextBox().ByIndex (2);
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyAspTextBox"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var editableTextBox = home.GetTextBox().ByLocalID ("MyEditableTextBox");
      Assert.That (editableTextBox.Scope.Id, Is.EqualTo ("body_MyEditableTextBox"));

      var aspTextBox = home.GetTextBox().ByLocalID ("MyAspTextBox");
      Assert.That (aspTextBox.Scope.Id, Is.EqualTo ("body_MyAspTextBox"));

      var htmlTextBox = home.GetTextBox().ByLocalID ("MyHtmlTextBox");
      Assert.That (htmlTextBox.Scope.Id, Is.EqualTo ("body_MyHtmlTextBox"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var htmlAnchor = home.GetTextBox().First();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyEditableTextBox"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();
      var scope = home.GetScope().ByID ("scope");

      var htmlAnchor = scope.GetTextBox().Single();
      Assert.That (htmlAnchor.Scope.Id, Is.EqualTo ("body_MyAspTextBoxNoAutoPostBack"));

      try
      {
        home.GetTextBox().Single();
        Assert.Fail ("Should not be able to unambigously find a text box.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var editableTextBox = home.GetTextBox().ByLocalID ("MyEditableTextBox");
      Assert.That (editableTextBox.GetText(), Is.EqualTo ("MyEditableTextBoxValue"));

      var aspTextBox = home.GetTextBox().ByLocalID ("MyAspTextBox");
      Assert.That (aspTextBox.GetText(), Is.EqualTo ("MyAspTextBoxValue"));

      var htmlTextBox = home.GetTextBox().ByLocalID ("MyHtmlTextBox");
      Assert.That (htmlTextBox.GetText(), Is.EqualTo ("MyHtmlTextBoxValue"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      // Check WaitFor.Nothing once before default behavior usage...

      var aspTextBoxNoAutoPostback = home.GetTextBox().ByLocalID ("MyAspTextBoxNoAutoPostBack");
      aspTextBoxNoAutoPostback.FillWith ("Blubba4", Opt.ContinueImmediately());
      Assert.That (aspTextBoxNoAutoPostback.GetText(), Is.EqualTo ("Blubba4"));

      var editableTextBox = home.GetTextBox().ByLocalID ("MyEditableTextBox");
      editableTextBox.FillWith ("Blubba1");
      Assert.That (editableTextBox.GetText(), Is.EqualTo ("Blubba1"));

      // ...and once afterwards

      var aspTextBox = home.GetTextBox().ByLocalID ("MyAspTextBox");
      aspTextBox.FillWith ("Blubba2");
      Assert.That (aspTextBox.GetText(), Is.EqualTo ("Blubba2"));

      var htmlTextBox = home.GetTextBox().ByLocalID ("MyHtmlTextBox");
      htmlTextBox.FillWith ("Blubba3", Opt.ContinueImmediately());
      Assert.That (htmlTextBox.GetText(), Is.EqualTo ("Blubba3"));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("TextBoxTest.wxe");
    }
  }
}