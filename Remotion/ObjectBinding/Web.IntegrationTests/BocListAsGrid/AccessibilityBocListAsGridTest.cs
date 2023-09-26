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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocListAsGrid
{
  [TestFixture]
  public class AccessibilityBocListAsGridTest : IntegrationTest
  {
    [Test]
    public void Normal ()
    {
      var home = Start();
      var bocListAsGrid = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var analyzer = Helper.CreateAccessibilityAnalyzer();

      var result = bocListAsGrid.Analyze(analyzer);

      //RM-8997 This will be removed again when the accessibility problems have been fixed
      var violationResult = result.Violations.IgnoreKnownIssues();
      Assert.That(violationResult, Is.Empty);
    }

    [Test]
    public void AlwaysInvalid_WithValidationErrors ()
    {
      var home = Start();
      var bocList = home.Lists().GetByLocalID("JobList_AlwaysInvalid");
      var analyzer = Helper.CreateAccessibilityAnalyzer();
      var validateButton = home.GetValidateButton();
      validateButton.Click();
      var result = bocList.Analyze(analyzer);

      Assert.That(bocList.GetValidationErrors(), Is.Not.Empty);
      Assert.That(result.Violations, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start("BocListAsGrid");
    }
  }
}
