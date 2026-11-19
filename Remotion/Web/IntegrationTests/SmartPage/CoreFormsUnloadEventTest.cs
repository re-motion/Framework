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
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.IntegrationTests.SmartPage
{
  /// <summary>
  /// Tests that <see cref="Remotion.Web.UI.SmartPage.EnableCoreFormsClientScriptUnloadEvent"/> correctly toggles
  /// whether CoreForms uses the (deprecated) unload event for its client script dispose logic.
  /// The basic setup is as follows:
  ///  - The test page registers a dispose script (via CoreForms/ScriptManager) that writes a marker to localStorage
  ///  - A synchronous, full postback (not an UpdatePanel async postback) is used to trigger a real page unload
  ///  - On every load, the page itself reads the marker left by the previous instance's dispose into
  ///    #unloadEventResult and clears it, so the test only ever navigates and reads the page's own output
  /// </summary>
  [TestFixture]
  public class CoreFormsUnloadEventTest : IntegrationTest
  {
    public enum UnloadEventFlag
    {
      Default,
      True,
      False
    }

    public enum PermissionsPolicyMode
    {
      Enabled,
      Disabled
    }

    private class CoreFormsUnloadEventPageObject : PageObject
    {
      public CoreFormsUnloadEventPageObject ([NotNull] PageObjectContext context)
          : base(context)
      {
      }

      public bool GetUnloadEventResult ()
      {
        return bool.Parse(Context.Scope.FindId("unloadEventResult").Text);
      }

      public void TriggerPostBack ()
      {
        var loadMarker = Context.Scope.FindId("loadMarker").Text;
        Context.Scope.FindId("PostBackButton").Click();
        RetryUntilTimeout.Run(
            Logger,
            () =>
            {
              if (Context.Scope.FindId("loadMarker").Text == loadMarker)
                throw new InvalidOperationException("Website did not update.");
            });
      }
    }

    [TestCase(UnloadEventFlag.Default, PermissionsPolicyMode.Enabled)]
    [TestCase(UnloadEventFlag.Default, PermissionsPolicyMode.Disabled)]
    [TestCase(UnloadEventFlag.False, PermissionsPolicyMode.Enabled)]
    [TestCase(UnloadEventFlag.False, PermissionsPolicyMode.Disabled)]
    [TestCase(UnloadEventFlag.True, PermissionsPolicyMode.Enabled)]
    [TestCase(UnloadEventFlag.True, PermissionsPolicyMode.Disabled)]
    [Test]
    public void UnloadEventBehavior (UnloadEventFlag unloadEventFlag, PermissionsPolicyMode permissionsPolicyMode)
    {
      var home = Start(unloadEventFlag, permissionsPolicyMode);
      Assert.That(home.GetUnloadEventResult(), Is.False);

      home.TriggerPostBack();

      var unloadFired = home.GetUnloadEventResult();

      if (unloadEventFlag != UnloadEventFlag.True)
      {
        // No override = unload disabled -> no event registers
        Assert.That(unloadFired, Is.False);
      }
      else if (Helper.BrowserConfiguration.IsFirefox())
      {
        // In Firefox, unload is not deprecated. Regardless of policy, the unload event should fire.
        Assert.That(unloadFired, Is.True);
      }
      else if (permissionsPolicyMode == PermissionsPolicyMode.Enabled)
      {
        // In Chrome/Edge we expect the unload event to only fire if the permission policy is set,
        Assert.That(unloadFired, Is.True);
      }
      else
      {
        // Otherwise, it could either work or not depending on the rollout progress of the unload deprecation
        Assert.Ignore("Chrome/Edge are phasing out the unload event without an explicit Permissions-Policy opt-in; behavior is not asserted here.");
      }
    }

    private CoreFormsUnloadEventPageObject Start (UnloadEventFlag unloadEventFlag, PermissionsPolicyMode permissionsPolicyMode)
    {
      return Start<CoreFormsUnloadEventPageObject>(
          $"CoreFormsUnloadEventTest.aspx?unloadEventFlag={unloadEventFlag}&permissionsPolicy={permissionsPolicyMode}");
    }
  }
}
