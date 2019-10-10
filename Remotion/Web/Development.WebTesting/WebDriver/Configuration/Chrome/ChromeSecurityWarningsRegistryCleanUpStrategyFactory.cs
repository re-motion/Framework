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
using Microsoft.Win32;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium;

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome
{
  /// <summary>
  /// Responsible for interacting with the Registry (reading or manipulating the <c>CommandLineFlagSecurityWarningsEnabled</c> entry for Chrome).
  /// </summary>
  public class ChromeSecurityWarningsRegistryCleanUpStrategyFactory
  {
    private class RestoreSecurityWarningsRegistryCleanUpStrategy : IBrowserSessionCleanUpStrategy
    {
      public void CleanUp ()
      {
        EnsureWriteAccessToChromePolicyKey();

        using (var key = GetOrCreateChromePoliciesKey())
        {
          key.DeleteValue (c_securityWarningsPolicyName);
        }
      }
    }

    private class EmptyCleanUpStrategy : IBrowserSessionCleanUpStrategy
    {
      public void CleanUp ()
      {
      }
    }

    private const string c_chromePoliciesPath = @"SOFTWARE\Policies\Google\Chrome";
    private const string c_securityWarningsPolicyName = "CommandLineFlagSecurityWarningsEnabled";

    private readonly ChromiumDisableSecurityWarningsBehavior _disableSecurityWarningsBehavior;

    public ChromeSecurityWarningsRegistryCleanUpStrategyFactory (ChromiumDisableSecurityWarningsBehavior disableSecurityWarningsBehavior)
    {
      _disableSecurityWarningsBehavior = disableSecurityWarningsBehavior;
    }

    /// <summary>
    /// Depending on the set behavior, either throws an exception if <c>CommandLineFlagSecurityWarningsEnabled</c> is not set to false in the registry, attempts to set it,
    /// or does nothing.
    /// </summary>
    /// <returns>
    /// A <see cref="IBrowserSessionCleanUpStrategy"/>, cleaning up the registry flag if set.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// If the behavior <see cref="ChromiumDisableSecurityWarningsBehavior.Require"/> is set, and the needed registry key is not set.
    /// </exception>
    [NotNull]
    public IBrowserSessionCleanUpStrategy CreateCleanUpStrategy ()
    {
      if (!SecurityWarningsEnabled())
        return new EmptyCleanUpStrategy();

      switch (_disableSecurityWarningsBehavior)
      {
        case ChromiumDisableSecurityWarningsBehavior.Require:
        {
          throw new InvalidOperationException ($"The '{c_securityWarningsPolicyName}' Chrome policy needs to be set to 0.");
        }
        case ChromiumDisableSecurityWarningsBehavior.Automatic:
        {
          DisableSecurityWarningsViaRegistry();
          return new RestoreSecurityWarningsRegistryCleanUpStrategy();
        }
        default:
        {
          return new EmptyCleanUpStrategy();
        }
      }
    }

    private void DisableSecurityWarningsViaRegistry ()
    {
      EnsureWriteAccessToChromePolicyKey();

      using (var key = GetOrCreateChromePoliciesKey())
      {
        key.SetValue (c_securityWarningsPolicyName, 0);
      }
    }

    private bool SecurityWarningsEnabled ()
    {
      using (var currentUserKey = Registry.CurrentUser.OpenSubKey (c_chromePoliciesPath))
      using (var localMachineKey = Registry.LocalMachine.OpenSubKey (c_chromePoliciesPath))
      {
        var currentUserSecurityPolicyValue = currentUserKey?.GetValue (c_securityWarningsPolicyName) as int?;
        var localMachineSecurityPolicyValue = localMachineKey?.GetValue (c_securityWarningsPolicyName) as int?;

        return (currentUserSecurityPolicyValue ?? localMachineSecurityPolicyValue) != 0;
      }
    }

    private static void EnsureWriteAccessToChromePolicyKey ()
    {
      try
      {
        Registry.CurrentUser.CreateSubKey (c_chromePoliciesPath)?.Dispose();
      }
      catch (UnauthorizedAccessException)
      {
        throw new UnauthorizedAccessException ($"Cannot configure security policy for Google Chrome, write access to \"{c_chromePoliciesPath}\" was denied.");
      }
    }

    private static RegistryKey GetOrCreateChromePoliciesKey ()
    {
      return Registry.CurrentUser.CreateSubKey (c_chromePoliciesPath);
    }
  }
}