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
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class InfrastructureTests : IntegrationTest
  {
    [Test]
    [TestCase (null, "New Input")]
    [TestCase ("TODO RM-6402: Chrome Bug", "^   ! \" § $ % & / ( ) = ? ²   { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9")]
    [TestCase ("TODO RM-6402: Chrome Bug", "^ ° ! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9")]
    [TestCase ("TODO RM-6402: Chrome Bug", "^ ° ! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9 ^ ° ! \" § $ % & / ( ) = ? ² ³ { [ ] } \\ + * ~ ' # @ < > | A Z a z 0 1 8 9")]
    public void TestCoypuElementScopeFillInWithAndSendKeysExtensions_FillWithAndWait (string ignore, string input)
    {
      if (ignore != null)
        Assert.Ignore(ignore);

      var home = Start();

      var textBox = home.TextBoxes().GetByLocalID("MyTextBox");

      textBox.FillWith(input, FinishInput.WithTab);

      Assert.That(home.Scope.FindId("dmaWxePostBackSequenceNumberField").Value, Is.EqualTo("3"));
      Assert.That(textBox.GetText(), Is.EqualTo(input));
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("InfrastructureTests.wxe");
    }
  }
}