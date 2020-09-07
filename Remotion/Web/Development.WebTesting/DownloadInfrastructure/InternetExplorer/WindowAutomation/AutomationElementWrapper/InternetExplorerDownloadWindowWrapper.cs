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
  /// Wraps an InternetExplorer download window.
  /// </summary>
  [Obsolete ("Support for Internet Explorer in web tests has been removed. (Version: 1.20.22)")]
  public class InternetExplorerDownloadWindowWrapper
  {
    /// <summary>
    /// Creates a new <see cref="InternetExplorerDownloadWindowWrapper"/> from the specified <paramref name="windowHandle"/>.
    /// </summary>
    [CanBeNull]
    public static InternetExplorerDownloadWindowWrapper CreateFromHandle (IntPtr windowHandle)
    {
      if (windowHandle == IntPtr.Zero)
        throw new ArgumentNullException ("windowHandle");

      var element = AutomationElement.FromHandle (windowHandle);

      if (element.FindAll (TreeScope.Children, Condition.TrueCondition).Count != 7)
        return null;

      if (element.FindAll (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button)).Count != 4)
        return null;

      return new InternetExplorerDownloadWindowWrapper (element);
    }

    private readonly AutomationElement _pane;

    public InternetExplorerDownloadWindowWrapper ([NotNull] AutomationElement root)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      _pane = root;
    }

    public void Save ()
    {
      var buttons = _pane.FindAll (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button));
      ((InvokePattern) buttons[1].GetCurrentPattern (InvokePattern.Pattern)).Invoke();
    }
  }
}