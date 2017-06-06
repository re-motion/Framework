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
using Remotion.WindowFinder.Windows;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.Win32WindowSupport
{
  /// <summary>
  /// Declares the native methods used by the <see cref="InternetExplorerDownloadHelper"/>. See <see cref="Win32WindowsNativeMethodsExtended"/> for the implementation. 
  /// </summary>
  public interface IWin32WindowsNativeMethodsExtended : IWin32WindowsNativeMethods
  {
    /// <summary>
    /// Wraps GetForegroundWindow.
    /// </summary>
    /// <returns>
    /// The return value is a handle to the foreground window. 
    /// The foreground window can be NULL in certain circumstances, such as when a window is losing activation.  
    /// </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/ms633505.aspx"/>.
    IntPtr GetForegroundWindow ();

    /// <summary>
    /// Wraps IsWindowVisible.
    /// </summary>
    /// <returns>
    /// If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is nonzero. 
    /// Otherwise, the return value is zero.
    /// Because the return value specifies whether the window has the WS_VISIBLE style, it may be nonzero even if the window is totally obscured by other windows. 
    /// </returns>
    /// <seealso href="https://msdn.microsoft.com/en-us/library/ms633530.aspx"/>.
    bool IsWindowVisible (IntPtr windowHandle);
  }
}