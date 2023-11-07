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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Accessibility;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocTextValue
{
  [TestFixture]
  public class AccessibilityBocTextValueTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnly ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Disabled ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void PasswordRenderMasked ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_PasswordRenderMasked");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void PasswordNoRender ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_PasswordNoRender");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void CVStringNormal ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("CVString_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void CVStringReadOnly ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("CVString_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void CVStringDisabled ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("CVString_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void NormalRequired_WithValidationErrors ()
    {
      var home = Start();
      var bocTextValue = home.TextValues().GetByLocalID("LastNameField_Normal_Required");
      bocTextValue.FillWith("");
      var validateButton = home.GetValidateButton();
      validateButton.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocTextValue.Analyze(analyzer);

      Assert.That(bocTextValue.GetValidationErrors(), Is.Not.Empty);
      Assert.That(result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start("BocTextValue");
    }
  }
}
