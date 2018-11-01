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
using Coypu;

namespace Remotion.Web.Development.WebTesting.BrowserSession
{
  /// <summary>
  /// Represents a wrapper around a <see cref="Coypu.BrowserSession"/> which has additional cleanup routines via <see cref="IDisposable.Dispose"/>.
  /// </summary>
  public interface IBrowserSession : IDisposable
  {
    /// <summary>
    /// The <see cref="BrowserWindow"/> of this <see cref="IBrowserSession"/>.
    /// </summary>
    BrowserWindow Window { get; }

    /// <inheritdoc cref="Coypu.BrowserSession.Driver"/>
    Driver Driver { get; }

    /// <inheritdoc cref="Coypu.BrowserSession.FindWindow"/>
    /// <remarks>
    /// Only used internally.
    /// API access via <see cref="ControlObjectContext" />.<see cref="ControlObjectContext.CloneForNewPopupWindow" /> and <see cref="ControlObjectContext" />.<see cref="ControlObjectContext.CloneForNewWindow" />.
    /// </remarks>
    BrowserWindow FindWindow (string locator, Options options = null);

    /// <summary>
    /// Deletes all Cookies of this <see cref="IBrowserSession"/>.
    /// </summary>
    void DeleteAllCookies ();
  }
}