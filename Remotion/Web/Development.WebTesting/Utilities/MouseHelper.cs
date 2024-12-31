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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using Coypu;
using JetBrains.Annotations;
using OpenQA.Selenium;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  public class MouseHelper
  {
    [StructLayout(LayoutKind.Sequential)]
    private struct SendInputDto
    {
      public uint Type;
      public MouseInputDto MouseInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseInputDto
    {
      public int X;
      public int Y;
      public uint MouseData;
      public uint Flags;
      public uint Timestamp;
      public IntPtr ExtraInfo;
    }

    private const uint c_moveRelative = 0x1;
    private const uint c_leftDown = 0x2;
    private const uint c_leftUp = 0x4;
    private const uint c_rightDown = 0x8;
    private const uint c_rightUp = 0x10;

    private static readonly int s_dtoStructureSize = Marshal.SizeOf(typeof(SendInputDto));
    private static readonly uint[] s_doubleLeftClickData = { c_leftDown, c_leftUp, c_leftDown, c_leftUp };
    private static readonly uint[] s_fakeMoveData = { c_moveRelative };
    private static readonly uint[] s_leftClickData = { c_leftDown, c_leftUp };
    private static readonly uint[] s_rightClickData = { c_rightDown, c_rightUp };

    [DllImport("user32.dll", SetLastError = true)]
    private static extern void SendInput (uint inputCount, SendInputDto[] data, int structureSize);

    public readonly IBrowserConfiguration BrowserConfiguration;

    public MouseHelper ([NotNull] IBrowserConfiguration browserConfiguration)
    {
      ArgumentUtility.CheckNotNull("browserConfiguration", browserConfiguration);

      BrowserConfiguration = browserConfiguration;
    }

    /// <summary>
    /// Sends a double left mouse button click.
    /// </summary>
    public void DoubleLeftClick ()
    {
      Send(s_doubleLeftClickData);
    }

    /// <summary>
    /// Hovers with the mouse over the specified <paramref name="control"/>.
    /// </summary>
    public void Hover ([NotNull] ControlObject control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      Hover(BrowserConfiguration.LocatorHelper.GetBounds(control, CoordinateSystem.Desktop));
    }

    /// <summary>
    /// Hovers with the mouse over the specified <paramref name="element"/>.
    /// </summary>
    public void Hover ([NotNull] ElementScope element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      Hover(BrowserConfiguration.LocatorHelper.GetBounds(element, CoordinateSystem.Desktop));
    }

    /// <summary>
    /// Hovers with the mouse over the specified <paramref name="webElement"/>.
    /// </summary>
    public void Hover ([NotNull] IWebElement webElement)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      Hover(BrowserConfiguration.LocatorHelper.GetBounds(webElement, CoordinateSystem.Desktop));
    }

    /// <summary>
    /// Hovers with the mouse over the specified <paramref name="rectangle"/>.
    /// </summary>
    public void Hover (Rectangle rectangle)
    {
      if (rectangle.Width == 0 && rectangle.Height == 0)
        throw new ArgumentException("Cannot hover over an empty rectangle.");

      Hover(new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2));
    }

    /// <summary>
    /// Hovers with the mouse over the specified <paramref name="point"/>.
    /// </summary>
    public void Hover (Point point)
    {
#if PLATFORM_WINDOWS
      Cursor.Position = point;
      SendFakeMove();
#else
      throw new PlatformNotSupportedException("Mouse is only supported on Windows");
#endif
    }

    /// <summary>
    /// Sends a left mouse button click.
    /// </summary>
    public void LeftClick ()
    {
      Send(s_leftClickData);
    }

    /// <summary>
    /// Sends a right mouse button click.
    /// </summary>
    public void RightClick ()
    {
      Send(s_rightClickData);
    }

    /// <summary>
    /// Displays the tooltip of the specified <paramref name="control"/> by hovering over it.
    /// </summary>
    [Obsolete("Tooltip display is not working correctly in certain configurations and should be used with that in mind.")]
    public void ShowTooltip ([NotNull] ControlObject control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      Hover(control);
      Thread.Sleep(3000);
    }

    /// <summary>
    /// Displays the tooltip of the specified <paramref name="element"/> by hovering over it.
    /// </summary>
    [Obsolete("Tooltip display is not working correctly in certain configurations and should be used with that in mind.")]
    public void ShowTooltip ([NotNull] ElementScope element)
    {
      ArgumentUtility.CheckNotNull("element", element);

      Hover(element);
      Thread.Sleep(3000);
    }

    /// <summary>
    /// Displays the tooltip of the specified <paramref name="webElement"/> by hovering over it.
    /// </summary>
    [Obsolete("Tooltip display is not working correctly in certain configurations and should be used with that in mind.")]
    public void ShowTooltip ([NotNull] IWebElement webElement)
    {
      ArgumentUtility.CheckNotNull("webElement", webElement);

      Hover(webElement);
      Thread.Sleep(3000);
    }

    /// <summary>
    /// Triggers a mouse move, but the mouse does not actually move.
    /// This is done so that a browser recognizes a mouse move when
    /// the mouse is set via <c>Cursor</c>.<c>Cursor.Position</c>
    /// </summary>
    private void SendFakeMove ()
    {
      Send(s_fakeMoveData);
    }

    private void Send (uint[] flags)
    {
      var data = new SendInputDto[flags.Length];
      for (var i = 0; i < data.Length; i++)
        data[i] = new SendInputDto { Type = 0, MouseInfo = { Flags = flags[i] } };

      SendInput((uint)data.Length, data, s_dtoStructureSize);
    }
  }
}
