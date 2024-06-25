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
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class UserControlExecutor : IUserControlExecutor
  {
    private readonly WxeFunction _function;
    private readonly string _backedUpUserControlState;
    private readonly string _backedUpUserControl;
    private readonly string _userControlID;
    private NameValueCollection? _postBackCollection;
    private NameValueCollection? _backedUpPostBackData;
    private bool _isReturningPostBack;
    private readonly WxePageStep _pageStep;

    public UserControlExecutor (WxePageStep parentStep, WxeUserControl userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
      : this((WxeStep)parentStep, userControl, subFunction, sender, usesEventTarget)
    {
    }

    public UserControlExecutor (WxeUserControlStep parentStep, WxeUserControl userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
      : this((WxeStep)parentStep, userControl, subFunction, sender, usesEventTarget)
    {
    }

    protected UserControlExecutor (WxeStep parentStep, WxeUserControl userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull("parentStep", parentStep);
      ArgumentUtility.CheckNotNull("userControl", userControl);
      ArgumentUtility.CheckNotNull("subFunction", subFunction);
      ArgumentUtility.CheckNotNull("sender", sender);
      if (userControl.WxePage == null)
        throw new ArgumentException("Execution of user controls that are no longer part of the control hierarchy is not supported.", "userControl");

      _backedUpUserControlState = userControl.SaveAllState();
      _backedUpUserControl = userControl.AppRelativeVirtualPath;
      _userControlID = userControl.UniqueID;
      _function = subFunction;

      _function.SetParentStep(parentStep);
      if (parentStep is WxeUserControlStep)
        _pageStep = ((WxeUserControlStep)parentStep).PageStep;
      else
        _pageStep = ((WxePageStep)parentStep);

      if (userControl.WxePage.IsPostBack)
      {
        _postBackCollection = userControl.WxePage.GetPostBackCollection()!.Clone(); // TODO RM-8118: Debug assert not null
        _backedUpPostBackData = new NameValueCollection();

        if (usesEventTarget)
        {
          //TODO: Update PreProcessingSubFunctionState with this check as well.
          if (sender.UniqueID.Contains(":"))
            throw new InvalidOperationException("Executing WxeUserControls are only supported on pages not rendered in XhtmlConformanceMode.Legacy.");

          //TODO: Is this check really necessary?
          if (_postBackCollection[ControlHelper.PostEventSourceID] != sender.UniqueID)
          {
            throw new ArgumentException(
                string.Format(
                    "The 'sender' does not match the value in {0}. Please pass the control that orignated the postback.",
                    ControlHelper.PostEventSourceID),
                "sender");
          }

          _backedUpPostBackData.Add(ControlHelper.PostEventSourceID, _postBackCollection[ControlHelper.PostEventSourceID]);
          _backedUpPostBackData.Add(ControlHelper.PostEventArgumentID, _postBackCollection[ControlHelper.PostEventArgumentID]);
          _postBackCollection.Remove(ControlHelper.PostEventSourceID);
          _postBackCollection.Remove(ControlHelper.PostEventArgumentID);
        }
        else
        {
          throw new InvalidOperationException(
              "The WxeUserControl does not support controls that do not use __EventTarget for signaling a postback event.");
          //TODO: Check if controls that do not use __EventTarget can be supported
          // _backedUpPostBackData.Add (sender.UniqueID, _postBackCollection[sender.UniqueID]);
          // _postBackCollection.Remove (sender.UniqueID);
        }

        string uniqueIDPrefix = _userControlID + userControl.WxePage.IdSeparator;
        foreach (var key in _postBackCollection.AllKeys.Where(s => s!.StartsWith(uniqueIDPrefix))) // TODO RM-8118: not null assertion
        {
          _backedUpPostBackData.Add(key, _postBackCollection[key]);
          _postBackCollection.Remove(key);
        }
      }
    }

    public void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      _pageStep.SetPostBackCollection(_postBackCollection);
      _postBackCollection = null;
      _function.Execute(context);

      Return(context);
    }

    private void Return (WxeContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      NameValueCollection postBackCollection;
      if (StringUtility.AreEqual(context.HttpContext.Request.HttpMethod, "POST", false))
        postBackCollection = context.HttpContext.Request.Form;
      else
        postBackCollection = context.HttpContext.Request.QueryString;

      postBackCollection = postBackCollection.Clone();
      if (_backedUpPostBackData != null)
      {
        foreach (var key in _backedUpPostBackData.AllKeys)
          postBackCollection[key] = _backedUpPostBackData[key];
      }
      else
      {
        postBackCollection.Remove(ControlHelper.PostEventSourceID);
        postBackCollection.Remove(ControlHelper.PostEventArgumentID);
      }
      _pageStep.SetReturnState(_function, true, postBackCollection);

      _backedUpPostBackData = null;

      _isReturningPostBack = true;
    }

    public WxeFunction Function
    {
      get { return _function; }
    }

    public string BackedUpUserControlState
    {
      get { return _backedUpUserControlState; }
    }

    public string BackedUpUserControl
    {
      get { return _backedUpUserControl; }
    }

    public string UserControlID
    {
      get { return _userControlID; }
    }

    public bool IsReturningPostBack
    {
      get { return _isReturningPostBack; }
    }

    public bool IsNull
    {
      get { return false; }
    }

    public WxeStep? ExecutingStep
    {
      get
      {
        ArgumentUtility.CheckNotNull("userControlExecutor", this);

        if (!_isReturningPostBack)
          return _function.ExecutingStep;
        else
          return null;
      }
    }
  }
}
