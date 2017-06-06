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
  /// Wraps a single download list item in the Internet Explorer download manager window.
  /// </summary>
  public class InternetExplorerDownloadListItemWrapper
  {
    /// <summary>
    /// All states a <see cref="InternetExplorerDownloadListItemWrapper"/> can be in.
    /// </summary>
    private enum State
    {
      Done,
      SaveOrOpen,
      Save
    }

    private readonly AutomationElementCollection _children;
    private readonly State _state;

    public InternetExplorerDownloadListItemWrapper ([NotNull] AutomationElement target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      _children = target.FindAll (TreeScope.Children, Condition.TrueCondition);

      if (_children.Count < 5 || _children.Count > 7)
        throw new ArgumentException ("The specified automation element does not contain the right amount of children.", "target");

      _state = GetState();
    }

    public string FileName
    {
      get { return _children[0].Current.Name; }
    }

    public string FileSource
    {
      get { return _children[2].Current.Name; }
    }

    public bool HasSaveButton
    {
      get { return _state == State.Save || _state == State.SaveOrOpen; }
    }

    public void Save ()
    {
      switch (_state)
      {
        case State.Done:
          throw new InvalidOperationException (string.Format ("The download list item '{0}' is already downloaded.", FileName));
        case State.SaveOrOpen:
          ((InvokePattern) _children[5].GetCurrentPattern (InvokePattern.Pattern)).Invoke();
          break;
        case State.Save:
          ((InvokePattern) _children[4].GetCurrentPattern (InvokePattern.Pattern)).Invoke();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private State GetState ()
    {
      // The close button is only enabled if the item is selected or hovered
      var hasCloseButton = _children[_children.Count - 1].Current.ControlType.Equals (ControlType.Button);
      var offset = hasCloseButton ? 0 : 1;

      // There are three different states:
      // - Done       : Is anything that does not classify as SaveOrOpen 
      // - Save       : Has 6 children and the 4th element is a text and the 5th element is the save button
      // - SaveOrOpen : Has 7 children
      switch (_children.Count + offset)
      {
        case 6:
          if (_children[3].Current.ControlType.Equals (ControlType.Text)
              && _children[4].Current.ControlType.Equals (ControlType.SplitButton))
          {
            return State.Save;
          }
          else
          {
            return State.Done;
          }
        case 7:
          return State.SaveOrOpen;
        default:
          throw new ArgumentException ("The specified automation element does not contain the right amount of children.", "target");
      }
    }
  }
}