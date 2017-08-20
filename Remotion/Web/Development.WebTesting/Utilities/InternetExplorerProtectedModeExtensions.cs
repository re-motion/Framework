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
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Win32;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides utilities for controlling the protected mode settings of the IE.
  /// </summary>
  /// <remarks>
  /// For information about how to programmatically change the protected
  /// mode see: https://support.microsoft.com/en-in/help/182569/internet-explorer-security-zones-registry-entries-for-advanced-users
  /// </remarks>
  public static class InternetExplorerProtectedModeExtensions
  {
    /// <summary>
    /// Represents the different settings of 
    /// </summary>
    /// <remarks>
    /// Do not change the values of enum entries.
    /// </remarks>
    [Flags]
    private enum ZoneProtectedModeSetting
    {
      Enabled = 0,
      Prompt = 1,
      Disabled = 3
    }

    /// <summary>
    /// Provides functionality for automatically resetting IE protected mode settings when disposing the object.
    /// </summary>
    /// <remarks>
    /// The setting will only be reset when calling the <see cref="Dispose"/> method.
    /// The object finalizer will not reset any values.
    /// </remarks>
    private class AutoRevertProtectedModeSettings : IDisposable
    {
      private const string c_zoneKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones";
      private const string c_protectedModeValueName = "2500";

      /// <summary>
      /// Set all zones to the specified <paramref name="protectedMode"/> and remembers which
      /// ones to reset. In order to reset the values call <see cref="Dispose"/>.
      /// </summary>
      public static AutoRevertProtectedModeSettings SetProtectMode (bool protectedMode)
      {
        ZoneProtectedModeSetting protectedModeSetting;
        if (protectedMode)
          protectedModeSetting = ZoneProtectedModeSetting.Enabled;
        else
          protectedModeSetting = ZoneProtectedModeSetting.Disabled;

        using (var root = Registry.CurrentUser.OpenSubKey (c_zoneKey))
        {
          Assertion.IsNotNull (root, "Can not find the registry node for changing IE user settings.");

          var resetTargets = new List<KeyValuePair<string, ZoneProtectedModeSetting>>();
          foreach (var name in root.GetSubKeyNames())
          {
            using (var currentZone = root.OpenSubKey (name, true))
            {
              Assertion.IsNotNull (currentZone, "Can not open the zone '{0}'.", name);

              var setting = GetZoneProtectedModeSetting ((int?) currentZone.GetValue (c_protectedModeValueName));
              if (!setting.HasValue)
                continue;
              if (protectedMode && setting == ZoneProtectedModeSetting.Enabled)
                continue;
              if (!protectedMode && setting == ZoneProtectedModeSetting.Disabled)
                continue;

              currentZone.SetValue (c_protectedModeValueName, (int) protectedModeSetting, RegistryValueKind.DWord);

              resetTargets.Add (new KeyValuePair<string, ZoneProtectedModeSetting> (name, setting.Value));
            }
          }

          return new AutoRevertProtectedModeSettings (resetTargets.ToArray());
        }
      }

      private static ZoneProtectedModeSetting? GetZoneProtectedModeSetting (int? value)
      {
        if (!value.HasValue)
          return null;
        switch (value.Value)
        {
          case 0:
            return ZoneProtectedModeSetting.Enabled;
          case 1:
            return ZoneProtectedModeSetting.Prompt;
          case 3:
            return ZoneProtectedModeSetting.Disabled;
          default:
            throw new InvalidOperationException (string.Format ("Invalid protected mode setting '{0}'.", value.Value));
        }
      }

      private readonly KeyValuePair<string, ZoneProtectedModeSetting>[] _zoneSettingsToRevert;

      private bool _disposed;

      private AutoRevertProtectedModeSettings (KeyValuePair<string, ZoneProtectedModeSetting>[] zoneSettingsToRevert)
      {
        _zoneSettingsToRevert = zoneSettingsToRevert;
      }

      /// <inheritdoc />
      public void Dispose ()
      {
        if (_disposed)
          return;

        _disposed = true;
        RevertSettings();
      }

      /// <summary>
      /// Reverts all settings specified in <see cref="_zoneSettingsToRevert"/> to their previous setting.
      /// </summary>
      private void RevertSettings ()
      {
        if (_zoneSettingsToRevert.Length == 0)
          return;

        using (var root = Registry.CurrentUser.OpenSubKey (c_zoneKey))
        {
          Assertion.IsNotNull (root, "Can not find the registry node for changing IE user settings.");

          foreach (var revertSettings in _zoneSettingsToRevert)
          {
            using (var currentZone = root.OpenSubKey (revertSettings.Key, true))
            {
              Assertion.IsNotNull (currentZone, string.Format ("Can not open the zone '{0}'.", revertSettings.Key));

              currentZone.SetValue (c_protectedModeValueName, (int) revertSettings.Value, RegistryValueKind.DWord);
            }
          }
        }
      }
    }

    /// <summary>
    /// Represents a <see cref="IDisposable"/> that does nothing.
    /// </summary>
    private class NullDisposable : IDisposable
    {
      public static readonly IDisposable Instance = new NullDisposable();

      private NullDisposable ()
      {
      }

      /// <inheritdoc />
      public void Dispose ()
      {
      }
    }

    /// <summary>
    /// Sets all IE Zones to the specified <paramref name="protectedMode"/> and returns an 
    /// <see cref="IDisposable"/> which resets the protected mode settings when disposed.
    /// If the current browser is not IE, nothing happens and a NOP disposable will be returned.
    /// </summary>
    public static IDisposable SetProtectedModeForAllZones ([NotNull] this WebTestHelper helper, bool protectedMode)
    {
      ArgumentUtility.CheckNotNull ("helper", helper);

      if (helper.BrowserConfiguration.IsInternetExplorer())
        return AutoRevertProtectedModeSettings.SetProtectMode (protectedMode);

      return NullDisposable.Instance;
    }
  }
}