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

namespace Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chromium
{
  /// <summary>
  /// Responsible for interacting with the Registry (reading or manipulating the <c>CommandLineFlagSecurityWarningsEnabled</c> entry for Chromium browsers).
  /// </summary>
  public static class ChromiumSecurityWarningsRegistryCleanUpStrategyFactory
  {
    private class RestoreSecurityWarningsRegistryCleanUpStrategy : IBrowserSessionCleanUpStrategy
    {
      private readonly string _policiesPath;

      public RestoreSecurityWarningsRegistryCleanUpStrategy (string policiesPath)
      {
        _policiesPath = policiesPath;
      }

      public void CleanUp ()
      {
        EnsureWriteAccessToPolicyKey (_policiesPath);

        using (var key = GetOrCreatePoliciesKey (_policiesPath))
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

    private const string c_securityWarningsPolicyName = "CommandLineFlagSecurityWarningsEnabled";
    private const string c_chromePoliciesPath = @"SOFTWARE\Policies\Google\Chrome";
    private const string c_edgePoliciesPath = @"SOFTWARE\Policies\Microsoft\Edge";

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
    public static IBrowserSessionCleanUpStrategy CreateForChrome (ChromiumDisableSecurityWarningsBehavior disableSecurityWarningsBehavior)
    {
      return CreateCleanUpStrategy (disableSecurityWarningsBehavior, c_chromePoliciesPath);
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
    public static IBrowserSessionCleanUpStrategy CreateForEdge (ChromiumDisableSecurityWarningsBehavior disableSecurityWarningsBehavior)
    {
      return CreateCleanUpStrategy (disableSecurityWarningsBehavior, c_edgePoliciesPath);
    }

    private static IBrowserSessionCleanUpStrategy CreateCleanUpStrategy (ChromiumDisableSecurityWarningsBehavior disableSecurityWarningsBehavior, string policiesPath)
    {
      if (!SecurityWarningsEnabled (policiesPath))
        return new EmptyCleanUpStrategy();

      switch (disableSecurityWarningsBehavior)
      {
        case ChromiumDisableSecurityWarningsBehavior.Require:
        {
          throw new InvalidOperationException ($"The '{c_securityWarningsPolicyName}' policy in '{policiesPath}' needs to be set to 0.");
        }
        case ChromiumDisableSecurityWarningsBehavior.Automatic:
        {
          DisableSecurityWarningsViaRegistry (policiesPath);
          return new RestoreSecurityWarningsRegistryCleanUpStrategy (policiesPath);
        }
        default:
        {
          return new EmptyCleanUpStrategy();
        }
      }
    }

    private static void DisableSecurityWarningsViaRegistry (string policiesPath)
    {
      EnsureWriteAccessToPolicyKey (policiesPath);

      using (var key = GetOrCreatePoliciesKey (policiesPath))
      {
        key.SetValue (c_securityWarningsPolicyName, 0);
      }
    }

    private static bool SecurityWarningsEnabled (string policiesPath)
    {
      using (var currentUserKey = Registry.CurrentUser.OpenSubKey (policiesPath))
      using (var localMachineKey = Registry.LocalMachine.OpenSubKey (policiesPath))
      {
        var currentUserSecurityPolicyValue = currentUserKey?.GetValue (c_securityWarningsPolicyName) as int?;
        var localMachineSecurityPolicyValue = localMachineKey?.GetValue (c_securityWarningsPolicyName) as int?;

        return (currentUserSecurityPolicyValue ?? localMachineSecurityPolicyValue) != 0;
      }
    }

    private static void EnsureWriteAccessToPolicyKey (string policiesPath)
    {
      try
      {
        Registry.CurrentUser.CreateSubKey (policiesPath)?.Dispose();
      }
      catch (UnauthorizedAccessException)
      {
        throw new UnauthorizedAccessException ($"Cannot configure security policy, write access to '{policiesPath}' was denied.");
      }
    }

    private static RegistryKey GetOrCreatePoliciesKey (string policiesPath)
    {
      return Registry.CurrentUser.CreateSubKey (policiesPath);
    }
  }
}