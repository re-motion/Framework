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
using System.Windows.Automation;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.AutomationElementWrapper
{
  /// <summary>
  /// Wraps an InternetExplorer download notification bar.
  /// </summary>
  public class InternetExplorerDownloadNotificationBarWrapper
  {
    /// <summary>
    /// Creates a new <see cref="InternetExplorerDownloadNotificationBarWrapper"/> from the specified <paramref name="windowHandle"/>.
    /// </summary>
    [CanBeNull]
    public static InternetExplorerDownloadNotificationBarWrapper CreateFromHandle (IntPtr windowHandle)
    {
      if (windowHandle == IntPtr.Zero)
        throw new ArgumentNullException ("windowHandle");

      var element = AutomationElement.FromHandle (windowHandle);

      var root = element.FindFirst (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.ToolBar));
      if (root == null)
        return null;

      return new InternetExplorerDownloadNotificationBarWrapper (root);
    }

    private readonly AutomationElement _root;

    private InternetExplorerDownloadNotificationBarWrapper (AutomationElement root)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      _root = root;
    }

    public void Save ()
    {
      // Find and press the save button (is always the split button)
      var button = _root.FindFirst (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.SplitButton));
      ((InvokePattern) button.GetCurrentPattern (InvokePattern.Pattern)).Invoke();
    }

    public void Close ()
    {
      // Find and press the close button (is always the last button)
      var button = _root.FindAll (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button));
      ((InvokePattern) button[button.Count - 1].GetCurrentPattern (InvokePattern.Pattern)).Invoke();
    }
  }
}