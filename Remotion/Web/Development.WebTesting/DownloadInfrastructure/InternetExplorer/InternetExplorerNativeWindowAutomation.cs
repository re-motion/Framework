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
using Remotion.WindowFinder;

namespace Remotion.Web.Development.WebTesting.DownloadInfrastructure.InternetExplorer
{
  /// <summary>
  /// Automates native windows for the Internet Explorer download.
  /// Uses only indices to find the right button to click and is therefore localization safe.
  /// </summary>
  public class InternetExplorerNativeWindowAutomation
  {
    /// <summary>
    /// Clicks the download information bar save button.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <para>Thrown if either the download information bar contains an unexpected number of child elements</para>
    /// <para>- or -</para>
    /// <para>the save button cannot be clicked.</para>
    /// </exception>
    public void ClickDownloadInformationBarSaveButton (WindowInformation downloadInformationBarWindowInformation)
    {
      var saveButton = FindSaveButton (downloadInformationBarWindowInformation);

      ClickButton (saveButton);
    }

    /// <summary>
    /// Closes the download information bar by clicking the close button.
    /// </summary>
    /// <param name="downloadInformationBarWindowInformation"></param>
    public void CloseDownloadInformationBar (WindowInformation downloadInformationBarWindowInformation)
    {
      var closeButton = FindCloseButton (downloadInformationBarWindowInformation);

      ClickButton (closeButton);
    }

    private AutomationElement FindSaveButton (WindowInformation downloadInformationBarWindowInformation)
    {
      var childrenInsideOfDownloadBar = FindLayerWithButtons (downloadInformationBarWindowInformation);

      AutomationElement saveButton;

      //Only 4 children are present (Download information bar text, save button, cancel button, close button).
      //This happens if a file without a file extension is downloaded.
      if (childrenInsideOfDownloadBar.Count == 4)
      {
        saveButton = childrenInsideOfDownloadBar[1];
      }
      //5 children are present (Download information bar text, open button, save button, cancel button, close button).
      //Standard use case.
      else if (childrenInsideOfDownloadBar.Count == 5)
      {
        saveButton = childrenInsideOfDownloadBar[2];
      }
      else
      {
        throw new InvalidOperationException (
          "The 'Save'-button cannot be identified because the download information bar contains an unexpected number of elements.");
      }

      return saveButton;
    }

    private AutomationElement FindCloseButton (WindowInformation downloadInformationBarWindowInformation)
    {
      var childrenInsideOfDownloadBar = FindLayerWithButtons (downloadInformationBarWindowInformation);

      //Close button is always the last button
      var closeButton = childrenInsideOfDownloadBar[childrenInsideOfDownloadBar.Count - 1];
      return closeButton;
    }

    private AutomationElementCollection FindLayerWithButtons (WindowInformation downloadInformationBarWindowInformation)
    {
      var downloadInformationBarAutomationElement = AutomationElement.FromHandle (downloadInformationBarWindowInformation.WindowHandle);

      //Buttons are two nesting levels after the downloadInformationBarAutomationElement
      var firstChild = downloadInformationBarAutomationElement.FindFirst (TreeScope.Children, Condition.TrueCondition);
      var childrenInsideOfDownloadBar = firstChild.FindAll (TreeScope.Children, Condition.TrueCondition);

      return childrenInsideOfDownloadBar;
    }

    private void ClickButton (AutomationElement button)
    {
      var invokePattern = button.GetCurrentPattern (InvokePattern.Pattern) as InvokePattern;

      if (invokePattern == null)
        throw new InvalidOperationException (string.Format ("The '{0}'-button cannot be clicked.", button.Current.Name));

      invokePattern.Invoke();
    }
  }
}