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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocList
{
  [TestFixture]
  public class BocListTest : IntegrationTest
  {
    [Test]
    public void BocListPreservesCurrentPageWhenDeletingARow ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      bocList.GoToNextPage();
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(2));

      bocList.GetRow(1).Select();
      home.WebButtons().GetByLocalID("DeleteSelectedRowTestCaseRowButton").Click();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));
    }

    [Test]
    public void BocListPreservesCurrentPageWhenDeletingTheLastRowInAPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      bocList.GoToNextPage();
      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(2));

      bocList.GetRow(1).Select();
      home.WebButtons().GetByLocalID("DeleteSelectedRowTestCaseRowButton").Click();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(2));
    }

    [Test]
    public void BocListJumpsToPreviousPageWhenDeletingAllRowsOnTheLastPage ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Normal");

      bocList.GoToLastPage();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(4));
      Assert.That(bocList.GetNumberOfRows(), Is.EqualTo(2));

      bocList.GetRow(1).Select();
      bocList.GetRow(2).Select();
      home.WebButtons().GetByLocalID("DeleteSelectedRowTestCaseRowButton").Click();

      Assert.That(bocList.GetCurrentPage(), Is.EqualTo(3));
    }

    [Test]
    public void InlineValidation_ErrorInVisibleRow ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      home.WebButtons().GetByLocalID("ValidationTestCaseRowButton").Click();

      var expectedValidationErrors = new[]
                                     {
                                         "Invalid input in at least one row. Check the rows for more details."
                                     };

      Assert.That(bocList.GetValidationErrors(), Is.EqualTo(expectedValidationErrors));

      // .GetValidationErrors() uses screen reader info, but does not check that the text is actually visible so we also test the form grid message
      var formGridValidationMessage = bocList.Scope.FindXPath("ancestor::td//*[@class='formGridValidationMessage']").Text;
      Assert.That(formGridValidationMessage, Is.EqualTo("Invalid input in at least one row. Check the rows for more details."));
    }

    [Test]
    public void InlineValidation_ErrorInNonVisibleRow ()
    {
      var home = Start();

      var bocList = home.Lists().GetByLocalID("JobList_Validation");

      bocList.GoToNextPage();

      home.WebButtons().GetByLocalID("ValidationTestCaseRowButton").Click();

      var expectedValidationErrors = new[]
                                     {
                                         "Invalid input on at least one other page."
                                     };

      Assert.That(bocList.GetValidationErrors(), Is.EqualTo(expectedValidationErrors));

      // .GetValidationErrors() uses screen reader info, but does not check that the text is actually visible so we also test the form grid message
      var formGridValidationMessage = bocList.Scope.FindXPath("ancestor::td//*[@class='formGridValidationMessage']").Text;
      Assert.That(formGridValidationMessage, Is.EqualTo("Invalid input on at least one other page."));
    }

    private WxePageObject Start ()
    {
      return Start("BocList");
    }
  }
}
