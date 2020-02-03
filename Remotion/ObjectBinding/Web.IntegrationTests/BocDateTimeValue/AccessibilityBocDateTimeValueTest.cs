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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocDateTimeValue
{
  [TestFixture]
  public class AccessibilityBocDateTimeValueTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void Normal_WithValidationErrors ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Normal");
      bocDateTimeValue.SetDate ("InvDate");
      var validateButton = home.WebButtons().GetByLocalID ("ValidateButton");
      validateButton.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (bocDateTimeValue.GetValidationErrors(), Is.Not.Empty);
      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnly ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void Disabled ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void DateOnly ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnlyDateOnly ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_ReadOnlyDateOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void DateOnly_ExpandedDatePicker ()
    {
      var home = Start();
      var bocDateTimeValue = home.DateTimeValues().GetByLocalID ("DateOfBirthField_DateOnly");
      var datePicker = bocDateTimeValue.Children.WebButtons().GetByLocalID ("DatePicker");
      datePicker.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocDateTimeValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start ("BocDateTimeValue");
    }
  }
}