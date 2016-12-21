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
using System.Globalization;
using System.Threading;

namespace Remotion.Utilities
{
  /// <summary>
  /// Represents a scope with a specific culture and UI-culture (see <see cref="CultureInfo"/>). 
  /// </summary>
  /// <example>
  /// Set German culture and Russian user-interface-culture for
  /// for current thread within using-block, automatically restore previously set cultures
  /// after using-block:
  /// <code><![CDATA[
  /// using (new CultureScope ("de","ru"))
  /// {
  ///   // Do something with German Culture and Russian UI-Culture here
  /// }
  /// ]]></code></example>
  public struct CultureScope : IDisposable
  {
    private readonly CultureInfo _backupCulture;
    private readonly CultureInfo _backupUICulture;


    /// <summary>
    /// Returns an invariant culture scope, i.e. initialized with <see cref="CultureInfo"/> = <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    public static CultureScope CreateInvariantCultureScope()
    {
      return new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// Intialize <see cref="CultureScope"/> with culture-names-strings, e.g. "de-AT", "en-GB".
    /// </summary>
    /// <param name="cultureName">Culture name string. null to not switch culture.</param>
    /// <param name="uiCultureName">User interface culture name string. null to not switch UI-culture.</param>
    public CultureScope (string cultureName, string uiCultureName)
    {
      _backupCulture = null;
      _backupUICulture = null;

      Thread currentThread = Thread.CurrentThread;

      if (cultureName != null)
      {
        _backupCulture = currentThread.CurrentCulture;
        currentThread.CurrentCulture = CultureInfo.GetCultureInfo (cultureName);
      }

      if (uiCultureName != null)
      {
        _backupUICulture = currentThread.CurrentUICulture;
        currentThread.CurrentUICulture = CultureInfo.GetCultureInfo (uiCultureName);
      }
    }


    /// <summary>
    /// Intialize both the culture and UI-culture with the same culture-name
    /// </summary>
    /// <param name="cultureAndUiCultureName">Culture and User interface culture name string.</param>
    public CultureScope (string cultureAndUiCultureName) : this (cultureAndUiCultureName, cultureAndUiCultureName) { }


    /// <summary>
    /// Intialize <see cref="CultureScope"/> from <see cref="CultureInfo"/> instances.
    /// </summary>
    /// <param name="cultureInfo">Culture to use.</param>
    /// <param name="uiCultureInfo">User interface culture to use.</param>
    public CultureScope (CultureInfo cultureInfo, CultureInfo uiCultureInfo)
        : this (cultureInfo.Name, uiCultureInfo.Name)
    {}




    public void Dispose ()
    {
      Thread currentThread = Thread.CurrentThread;
      if (_backupCulture != null)
      {
        currentThread.CurrentCulture = _backupCulture;
      }
      if (_backupUICulture != null)
      {
        currentThread.CurrentUICulture = _backupUICulture;
      }
    }
  }
}
