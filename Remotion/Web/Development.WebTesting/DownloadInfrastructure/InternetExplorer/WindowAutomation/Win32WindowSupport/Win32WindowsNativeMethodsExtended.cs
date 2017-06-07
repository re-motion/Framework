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
using System.Runtime.InteropServices;
using System.Text;
using Remotion.WindowFinder.Windows;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.Win32WindowSupport
{
  /// <summary>
  /// Implementation of <see cref="IWin32WindowsNativeMethodsExtended"/>. Delegates calls to external methods.
  /// </summary>
  public class Win32WindowsNativeMethodsExtended : IWin32WindowsNativeMethodsExtended
  {
    [DllImport ("user32.dll")]
    private static extern IntPtr GetForegroundWindow ();

    [DllImport ("user32.dll")]
    private static extern bool IsWindowVisible (IntPtr windowHandle);
    
    private readonly IWin32WindowsNativeMethods _nativeMethods;

    public Win32WindowsNativeMethodsExtended ()
    {
      _nativeMethods = new Win32WindowsNativeMethods();
    }

    bool IWin32WindowsNativeMethodsExtended.IsWindowVisible (IntPtr windowHandle)
    {
      return IsWindowVisible (windowHandle);
    }

    IntPtr IWin32WindowsNativeMethodsExtended.GetForegroundWindow ()
    {
      return GetForegroundWindow();
    }

    bool IWin32WindowsNativeMethods.EnumWindows (EnumWindowsProc enumWindowsCallback, WindowFinderEnumWindowsProcContext context)
    {
      return _nativeMethods.EnumWindows (enumWindowsCallback, context);
    }

    void IWin32WindowsNativeMethods.EnumChildWindows (IntPtr parentWindowHandle, EnumChildWindowsProc enumWindowsCallback, WindowFinderEnumChildWindowsProcContext context)
    {
      _nativeMethods.EnumChildWindows (parentWindowHandle, enumWindowsCallback, context);
    }

    int IWin32WindowsNativeMethods.GetWindowThreadProcessID (IntPtr windowHandle)
    {
      return _nativeMethods.GetWindowThreadProcessID (windowHandle);
    }

    int IWin32WindowsNativeMethods.GetClassName (IntPtr windowHandle, StringBuilder className, int classNameMaxLength)
    {
      return _nativeMethods.GetClassName (windowHandle, className, classNameMaxLength);
    }

    int IWin32WindowsNativeMethods.GetWindowText (IntPtr windowHandle, StringBuilder windowText, int windowTextMaxLength)
    {
      return _nativeMethods.GetWindowText (windowHandle, windowText, windowTextMaxLength);
    }

    int IWin32WindowsNativeMethods.GetLastWin32Error ()
    {
      return _nativeMethods.GetLastWin32Error();
    }
  }
}