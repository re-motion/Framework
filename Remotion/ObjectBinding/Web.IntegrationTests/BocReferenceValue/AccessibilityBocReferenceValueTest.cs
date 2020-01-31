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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocReferenceValue
{
  [TestFixture]
  public class AccessibilityBocReferenceValueTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void PartnerFieldWithoutSelectedValueRequired_WithValidationErrors ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_WithoutSelectedValue_Required");
      var validateButton = home.WebButtons().GetByLocalID ("ValidateButton");
      validateButton.Click();
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (bocReferenceValue.GetValidationErrors(), Is.Not.Empty);
      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void NormalAlternativeRendering ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_Normal_AlternativeRendering");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnly ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_ReadOnly");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnlyWithoutSelectedValue ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_ReadOnlyWithoutSelectedValue");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void ReadOnly_AlternativeRendering ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_ReadOnly_AlternativeRendering");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void Disabled ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_Disabled");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    [Test]
    public void Required ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID ("PartnerField_Required");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocReferenceValue.Analyze (analyzer);

      Assert.That (result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start ("BocReferenceValue");
    }
  }
}