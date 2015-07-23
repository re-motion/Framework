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
using Remotion.Development.Web.UnitTesting.Configuration;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebButtonTests
{

[TestFixture]
public class WcagTest : BaseTest
{
  private TestWebButton _webButton;

  protected override void SetUpPage()
  {
    base.SetUpPage();
    _webButton = new TestWebButton();
    _webButton.ID = "WebButton";
  }


	[Test]
  public void EvaluateWaiConformityDebugLevelUndefined()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelUndefined();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }

	[Test]
  public void EvaluateWaiConformityLevelA()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();

	  Assert.That (WcagHelperMock.HasWarning, Is.False);
	  Assert.That (WcagHelperMock.HasError, Is.False);
  }

	[Test]
  public void EvaluateWaiConformityDebugLevelAWithUseLegacyButtonIsFalse()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetDebugExceptionLevelA();
    _webButton.UseLegacyButton = false;
    _webButton.EvaluateWaiConformity();

	  Assert.That (WcagHelperMock.HasError, Is.True);
	  Assert.That (WcagHelperMock.Priority, Is.EqualTo (1));
	  Assert.That (WcagHelperMock.Control, Is.SameAs (_webButton));
	  Assert.That (WcagHelperMock.Property, Is.EqualTo ("UseLegacyButton"));
  }


  [Test]
  public void IsLegacyButtonEnabledWithWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelA();
    _webButton.UseLegacyButton = false;
    Assert.That (_webButton.IsLegacyButtonEnabled, Is.True);
  }

  [Test]
  public void IsLegacyButtonEnabledWithoutWcagOverride()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetLevelUndefined();
    _webButton.UseLegacyButton = false;
    Assert.That (_webButton.IsLegacyButtonEnabled, Is.False);
  }
}

}
