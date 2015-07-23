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

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class LabelControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestSelection_ByHtmlID ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByID ("body_MySmartLabel");
      Assert.That (smartLabel.Scope.Id, Is.EqualTo ("body_MySmartLabel"));

      var formGridLabel = home.GetLabel().ByID ("body_MyFormGridLabel");
      Assert.That (formGridLabel.Scope.Id, Is.EqualTo ("body_MyFormGridLabel"));

      var aspLabel = home.GetLabel().ByID ("body_MyAspLabel");
      Assert.That (aspLabel.Scope.Id, Is.EqualTo ("body_MyAspLabel"));

      var htmlLabel = home.GetLabel().ByID ("body_MyHtmlLabel");
      Assert.That (htmlLabel.Scope.Id, Is.EqualTo ("body_MyHtmlLabel"));
    }

    [Test]
    public void TestSelection_ByLocalID ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (smartLabel.Scope.Id, Is.EqualTo ("body_MySmartLabel"));

      var formGridLabel = home.GetLabel().ByLocalID ("MyFormGridLabel");
      Assert.That (formGridLabel.Scope.Id, Is.EqualTo ("body_MyFormGridLabel"));

      var aspLabel = home.GetLabel().ByLocalID ("MyAspLabel");
      Assert.That (aspLabel.Scope.Id, Is.EqualTo ("body_MyAspLabel"));

      var htmlLabel = home.GetLabel().ByLocalID ("MyHtmlLabel");
      Assert.That (htmlLabel.Scope.Id, Is.EqualTo ("body_MyHtmlLabel"));
    }

    [Test]
    public void TestGetText ()
    {
      var home = Start();

      var smartLabel = home.GetLabel().ByLocalID ("MySmartLabel");
      Assert.That (smartLabel.GetText(), Is.EqualTo ("MySmartLabelContent"));

      var formGridLabel = home.GetLabel().ByLocalID ("MyFormGridLabel");
      Assert.That (formGridLabel.GetText(), Is.EqualTo ("MyFormGridLabelContent"));

      var aspLabel = home.GetLabel().ByLocalID ("MyAspLabel");
      Assert.That (aspLabel.GetText(), Is.EqualTo ("MyAspLabelContent"));

      var htmlLabel = home.GetLabel().ByLocalID ("MyHtmlLabel");
      Assert.That (htmlLabel.GetText(), Is.EqualTo ("MyHtmlLabelContent"));
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("LabelTest.aspx");
    }
  }
}