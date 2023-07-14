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

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocTextValue
{
  [TestFixture]
  public class ReadOnlyBocTextValueTest : IntegrationTest
  {
    [Test]
    public void Validate_OnReadOnlyControl_CreatesValidationErrors ()
    {
      var home = Start();
      home.GetValidateButton().Click();

      var readOnlyControl = home.TextValues().GetByLocalID("LastNameField_ReadOnly");
      var validationErrors = readOnlyControl.GetValidationErrorsForReadOnly();

      Assert.That(validationErrors, Is.EqualTo(new[] { "Localized invalid last name." }));

      readOnlyControl = home.TextValues().GetByLocalID("CVString_ReadOnly");
      validationErrors = readOnlyControl.GetValidationErrorsForReadOnly();

      Assert.That(validationErrors, Is.EqualTo(new[] { "Localized invalid CV string." }));
    }

    private WxePageObject Start ()
    {
      return Start("BocTextValue");
    }
  }
}
