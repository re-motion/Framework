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
using System.Linq;
using System.Windows.Automation;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer.WindowAutomation.AutomationElementWrapper
{
  /// <summary>
  /// Wraps an Internet Explorer download manager window.
  /// </summary>
  /// <remarks>
  /// The <see cref="Items"/> contents are cached and can be refreshed by using <see cref="Refresh"/>.
  /// </remarks>
  public class InternetExplorerDownloadManagerWrapper
  {
    /// <summary>
    /// Creates a new <see cref="InternetExplorerDownloadManagerWrapper"/> from the specified <paramref name="windowHandle"/>.
    /// </summary>
    [CanBeNull]
    public static InternetExplorerDownloadManagerWrapper CreateFromHandle (IntPtr windowHandle)
    {
      if (windowHandle == IntPtr.Zero)
        throw new ArgumentNullException ("windowHandle");

      var element = AutomationElement.FromHandle (windowHandle);

      if (element.FindAll (TreeScope.Children, Condition.TrueCondition).Count != 2)
        return null;

      return new InternetExplorerDownloadManagerWrapper (element);
    }

    private readonly AutomationElement _pane;
    private AutomationElementCollection _items;

    private InternetExplorerDownloadManagerWrapper ([NotNull] AutomationElement root)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      _pane = root.FindFirst (TreeScope.Children, Condition.TrueCondition);
      _items = root.FindAll (TreeScope.Children, Condition.FalseCondition);
    }

    public int ItemsCount
    {
      get { return _items.Count; }
    }

    public IEnumerable<InternetExplorerDownloadListItemWrapper> Items
    {
      get { return _items.Cast<AutomationElement>().Select (item => new InternetExplorerDownloadListItemWrapper (item)); }
    }

    public void Close ()
    {
      // Find and press the close button (is always the last button)
      var buttons = _pane.FindAll (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.Button));
      ((InvokePattern) buttons[buttons.Count - 1].GetCurrentPattern (InvokePattern.Pattern)).Invoke();
    }

    public void Refresh ()
    {
      _items = _pane.FindAll (TreeScope.Children, new PropertyCondition (AutomationElement.ControlTypeProperty, ControlType.ListItem));
    }
  }
}