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
using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ExecutionEngine.CompletionDetectionStrategies;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ElementScopeTest : IntegrationTest
  {

    [Test]
    public void TestFocus_IsDisabled ()
    {
      var home = Start();

      var disabledButton = home.Scope.FindId("DisabledButton");

      Assert.That(disabledButton.Disabled, Is.True);
      Assert.That(
          () => disabledButton.Focus(NullLogger.Instance),
          Throws.Exception.With.Message.EqualTo(AssertionExceptionUtility.CreateControlDisabledException(Driver, "Focus").Message));
    }

    [Test]
    public void TestFocus_IsNotDisabled ()
    {
      var home = Start();

      var normalButton = home.Scope.FindId("NormalButton");

      Assert.That(normalButton.Disabled, Is.False);
      Assert.That(() => normalButton.Focus(NullLogger.Instance), Throws.Nothing);
    }

    [Test]
    public void ExistsWorkaround_ShouldFailImmediately ()
    {
      var home = Start();

      var webButton = home.Scope.FindId("DoNotFind");
      webButton.ElementFinder.Options.Timeout = TimeSpan.FromMinutes(5);

      var stopwatch = Stopwatch.StartNew();
      var exists = webButton.ExistsWorkaround();

      Assert.That(exists, Is.EqualTo(false));
      // 1 Minute should account for any slow timing issue while still assuring that not the full Timeout of 5 Minute was used.
      Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromMinutes(1)));
    }

    [Test]
    public void ExistsWorkaround_ElementExists_ShouldNotResetTimeout ()
    {
      var home = Start();

      var webButton = home.Scope.FindId("body_NormalButton");
      var customTimeout = TimeSpan.FromSeconds(42);

      webButton.ElementFinder.Options.Timeout = customTimeout;

      webButton.ExistsWorkaround();

      Assert.That(webButton.ElementFinder.Options.Timeout, Is.EqualTo(customTimeout));
    }

    [Test]
    public void ExistsWorkaround_ElementDoesNotExist_ShouldNotResetTimeout ()
    {
      var home = Start();

      var webButton = home.Scope.FindId("DoNotFind");
      var customTimeout = TimeSpan.FromSeconds(42);

      webButton.ElementFinder.Options.Timeout = customTimeout;

      webButton.ExistsWorkaround();

      Assert.That(webButton.ElementFinder.Options.Timeout, Is.EqualTo(customTimeout));
    }

    [Test]
    public void ExistsWorkaround_Toggle ()
    {
      var home = Start();

      var toggleVisibilityButton = home.WebButtons().GetByLocalID("ToggleVisibilityButton");
      var visibilityButton = home.Anchors().GetByLocalID("VisibilityButton");
      Assert.That(visibilityButton.Scope.ExistsWorkaround(), Is.True);

      toggleVisibilityButton.Click(Opt.ContinueWhen(Wxe.PostBackCompleted));
      Assert.That(visibilityButton.Scope.ExistsWorkaround(), Is.False);

      toggleVisibilityButton.Click(Opt.ContinueWhen(Wxe.PostBackCompleted));
      Assert.That(visibilityButton.Scope.ExistsWorkaround(), Is.True);

      toggleVisibilityButton.Click(Opt.ContinueWhen(Wxe.PostBackCompleted));
      Assert.That(visibilityButton.Scope.ExistsWorkaround(), Is.False);
    }

    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject>("ElementScopeTest.wxe");
    }
  }
}
