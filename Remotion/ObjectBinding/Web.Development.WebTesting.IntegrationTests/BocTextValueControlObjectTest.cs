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
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByID ("body_DataEditControl_LastNameField_Normal");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByIndex (2);
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocText = home.GetTextValue().First();
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetTextValue().Single();
        Assert.Fail ("Should not be able to unambigously find a BOC text.");
      }
      catch (AmbiguousException)
      {
      }
    }

    [Test]
    public void TestSelection_DisplayName ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByDisplayName ("LastName");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByDomainProperty ("LastName");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByDomainProperty ("LastName", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocText.Scope.Id, Is.EqualTo ("body_DataEditControl_LastNameField_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      Assert.That (bocText.IsReadOnly(), Is.False);

      bocText = home.GetTextValue().ByLocalID ("LastNameField_ReadOnly");
      Assert.That (bocText.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_ReadOnly");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_Disabled");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_NoAutoPostBack");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_PasswordNoRender");
      Assert.That (bocText.GetText(), Is.Empty);

      bocText = home.GetTextValue().ByLocalID ("LastNameField_PasswordRenderMasked");
      Assert.That (bocText.GetText(), Is.EqualTo ("Doe"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      var bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_NoAutoPostBack");
      bocText.FillWith ("Blubba"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Doe"));

      bocText = home.GetTextValue().ByLocalID ("LastNameField_Normal");
      bocText.FillWith ("Doe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocTextValue");
    }
  }
}