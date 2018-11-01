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
using System.Web.UI;
using Remotion.Logging;
using Remotion.Utilities;
using Remotion.Web.Configuration;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI
{

public class WcagHelper
{
  private static readonly ILog s_log = LogManager.GetLogger (typeof (WcagHelper));
  private static readonly DoubleCheckedLockingContainer<WcagHelper> s_instance = new DoubleCheckedLockingContainer<WcagHelper>(()=>new WcagHelper());

  public static WcagHelper Instance
  {
    get { return s_instance.Value; }
  }

  public static void SetInstance (WcagHelper instance)
  {
    s_instance.Value = instance;
  }

  protected WcagHelper()
	{
	}

  public virtual bool IsWaiConformanceLevelARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.A) == WaiConformanceLevel.A;
  }

  public virtual bool IsWaiConformanceLevelARequired ()
  {
    return IsWaiConformanceLevelARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWaiConformanceLevelDoubleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.DoubleA) == WaiConformanceLevel.DoubleA;
  }

  public virtual bool IsWaiConformanceLevelDoubleARequired ()
  {
    return IsWaiConformanceLevelDoubleARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWaiConformanceLevelTripleARequired (WaiConformanceLevel conformanceLevel)
  {
    return (conformanceLevel & WaiConformanceLevel.TripleA) == WaiConformanceLevel.TripleA;
  }

  public virtual bool IsWaiConformanceLevelTripleARequired ()
  {
    return IsWaiConformanceLevelTripleARequired (GetWaiConformanceLevel());
  }

  public virtual bool IsWcagDebuggingEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging != WcagDebugMode.Disabled;
  }

  public virtual bool IsWcagDebugLogEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Logging
        || WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public virtual bool IsWcagDebugExceptionEnabled()
  {
    return WebConfiguration.Current.Wcag.Debugging == WcagDebugMode.Exception;
  }

  public virtual WaiConformanceLevel GetWaiConformanceLevel()
  {
    return WebConfiguration.Current.Wcag.ConformanceLevel;
  }

  public virtual void HandleWarning (int priority)
  {
    string message = string.Format (
        "An element on the page might not comply with a priority {0} checkpoint.", priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignModeForControl (control))
      return;

    string message = string.Format (
       "{0} '{1}' on page '{2}' might not comply with a priority {3} checkpoint.", 
        control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignModeForControl (control))
      return;

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' on page '{3}' might not comply with a priority {4} checkpoint.", 
        property, control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleWarning (message);
  }

  public virtual void HandleWarning (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Warn (message);
  }

  public virtual void HandleError (int priority)
  {
    string message = string.Format (
        "An element on the page does comply with a priority {0} checkpoint.", priority);
    HandleError (message);
  }

  public virtual void HandleError (int priority, Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignModeForControl (control))
      return;

    string message = string.Format (
       "{0} '{1}' on page '{2}' does not comply with a priority {3} checkpoint.", 
        control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleError (message);
  }

  public virtual void HandleError (int priority, Control control, string property)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (ControlHelper.IsDesignModeForControl (control))
      return;

    string message = string.Format (
        "The value of property '{0}' for {1} '{2}' on page '{3}' does not comply with a priority {4} checkpoint.", 
        property, control.GetType().Name, control.ID, control.Page.GetType().FullName, priority);
    HandleError (message);
  }

  public virtual void HandleError (string message)
  {
    if (IsWcagDebugLogEnabled())
      s_log.Error (message);
    if (IsWcagDebugExceptionEnabled())
      throw new WcagException (message, null);
  }
}

}
