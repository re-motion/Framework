// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Globalization;
using System.Threading;

// ReSharper disable once CheckNamespace
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
  partial struct CultureScope : IDisposable
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
    /// <param name="cultureName">Culture name string. <see langword="null" /> to not switch culture.</param>
    /// <param name="uiCultureName">User interface culture name string. <see langword="null" /> to not switch UI-culture.</param>
    public CultureScope (string cultureName, string uiCultureName)
      : this (
      cultureName == null ? null : CultureInfo.GetCultureInfo (cultureName),
      uiCultureName == null ? null : CultureInfo.GetCultureInfo (uiCultureName))
    {
    }


    /// <summary>
    /// Intialize both the culture and UI-culture with the same culture-name
    /// </summary>
    /// <param name="cultureAndUiCultureName">Culture and User interface culture name string.</param>
    public CultureScope (string cultureAndUiCultureName) : this (cultureAndUiCultureName, cultureAndUiCultureName) { }


    /// <summary>
    /// Intialize <see cref="CultureScope"/> from <see cref="CultureInfo"/> instances.
    /// </summary>
    /// <param name="cultureInfo">Culture to use. <see langword="null" /> to not switch culture.</param>
    /// <param name="uiCultureInfo">User interface culture to use. <see langword="null" /> to not switch UI-culture.</param>
    public CultureScope (CultureInfo cultureInfo, CultureInfo uiCultureInfo)
    {
      _backupCulture = null;
      _backupUICulture = null;

      Thread currentThread = Thread.CurrentThread;

      if (cultureInfo != null)
      {
        _backupCulture = currentThread.CurrentCulture;
        currentThread.CurrentCulture = cultureInfo;
      }

      if (uiCultureInfo != null)
      {
        _backupUICulture = currentThread.CurrentUICulture;
        currentThread.CurrentUICulture = uiCultureInfo;
      }
    }


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
