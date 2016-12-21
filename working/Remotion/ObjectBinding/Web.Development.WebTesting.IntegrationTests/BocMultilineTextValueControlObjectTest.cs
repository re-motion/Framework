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
  public class BocMultilineTextValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByID ("body_DataEditControl_CVField_Normal");
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    public void TestSelection_ByIndex ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByIndex (2);
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_ReadOnly"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    public void TestSelection_First ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().First();
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    [Category ("LongRunning")]
    public void TestSelection_Single ()
    {
      var home = Start();

      try
      {
        home.GetMultilineTextValue().Single();
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

      var bocMultilineText = home.GetMultilineTextValue().ByDisplayName ("CV");
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    public void TestSelection_DomainProperty ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByDomainProperty ("CV");
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    public void TestSelection_DomainPropertyAndClass ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue()
          .ByDomainProperty ("CV", "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample");
      Assert.That (bocMultilineText.Scope.Id, Is.EqualTo ("body_DataEditControl_CVField_Normal"));
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.IsReadOnly(), Is.False);

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_ReadOnly");
      Assert.That (bocMultilineText.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_ReadOnly");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Disabled");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_NoAutoPostBack");
      Assert.That (bocMultilineText.GetText(), Is.EqualTo ("<Test 1>" + Environment.NewLine + "Test 2" + Environment.NewLine + "Test 3"));
    }

    [Test]
    public void TestFillWith ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Blubba");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Blubba"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_NoAutoPostBack");
      bocMultilineText.FillWith ("Blubba"); // no auto post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Blubba", Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("<Test 1> NL Test 2 NL Test 3"));

      bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      bocMultilineText.FillWith ("Doe" + Environment.NewLine + "SecondLineDoe");
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Doe NL SecondLineDoe"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("Blubba"));
    }

    [Test]
    public void TestFillWithLines ()
    {
      var home = Start();

      var bocMultilineText = home.GetMultilineTextValue().ByLocalID ("CVField_Normal");
      bocMultilineText.FillWith (new[] { "Line1", "Line2", "Line3", "Line4", "Line5" });
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("Line1 NL Line2 NL Line3 NL Line4 NL Line5"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocMultilineTextValue");
    }
  }
}