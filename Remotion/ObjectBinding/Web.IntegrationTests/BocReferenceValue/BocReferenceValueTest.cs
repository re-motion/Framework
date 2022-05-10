// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocReferenceValue
{
  [TestFixture]
  public class DropDownListTest : IntegrationTest
  {
    [Test]
    public void ResetBusinessObjectList_AllowsListToBeRepopulated ()
    {
      var home = Start();
      var bocReferenceValue = home.ReferenceValues().GetByLocalID("PartnerField_Normal");
      Assert.That(bocReferenceValue.GetSelectedOption().Text, Is.EqualTo("D, A"));
      Assert.That(bocReferenceValue.GetOptionDefinitions().Count, Is.GreaterThan(2));

      home.WebButtons().GetByLocalID("SetEmptyBusinessObjectListButton").Click();

      Assert.That(bocReferenceValue.GetSelectedOption().Text, Is.EqualTo("D, A"));
      Assert.That(bocReferenceValue.GetOptionDefinitions().Count, Is.EqualTo(2));

      home.WebButtons().GetByLocalID("ResetBusinessObjectListButton").Click();

      Assert.That(bocReferenceValue.GetSelectedOption().Text, Is.EqualTo("D, A"));
      Assert.That(bocReferenceValue.GetOptionDefinitions().Count, Is.GreaterThan(2));
    }

    private WxePageObject Start ()
    {
      return Start("BocReferenceValue");
    }
  }
}
