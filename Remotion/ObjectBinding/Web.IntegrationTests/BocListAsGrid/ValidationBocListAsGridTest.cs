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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocListAsGrid
{
  [TestFixture]
  public class ValidationBocListAsGridTest : IntegrationTest
  {
    [Test]
    public void BocListAsGridWithoutInlineValidation_DomainValidationFailuresAreDispatchedToEditableControls ()
    {
      var home = Start();

      home.Scope.FindId("body_DataEditControl_ValidationTestCaseStartDate").Check();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Normal");
      var editableRow = bocList.GetRow(1);
      var startDateCell = editableRow.GetCell().WithColumnTitle("StartDate");
      var startDateControl = startDateCell.DateTimeValues().First();
      startDateControl.SetDate("asd");

      home.WebButtons().GetByLocalID("ValidateButton").Click();

      var expectedValidationErrors = new [] { "Unknown date format.", "Localized start date has a failure" };
      Assert.That(startDateControl.GetValidationErrors(), Is.EqualTo(expectedValidationErrors));
    }

    [Test]
    public void BocListAsGridWithInlineValidation_DomainValidationFailuresAreDispatchedToEditableControls ()
    {
      var home = Start();

      home.Scope.FindId("body_DataEditControl_ValidationTestCaseStartDate").Check();

      var bocList = home.ListAsGrids().GetByLocalID("JobList_Validation");
      var editableRow = bocList.GetRow(1);
      var startDateCell = editableRow.GetCell().WithColumnTitle("StartDate");
      var startDateControl = startDateCell.DateTimeValues().First();
      startDateControl.SetDate("asd");

      home.WebButtons().GetByLocalID("ValidateButton").Click();

      var expectedValidationErrors = new [] { "Unknown date format.", "Localized start date has a failure" };
      Assert.That(startDateControl.GetValidationErrors(), Is.EqualTo(expectedValidationErrors));
    }

    private WxePageObject Start ()
    {
      return Start("BocListAsGrid");
    }
  }
}
