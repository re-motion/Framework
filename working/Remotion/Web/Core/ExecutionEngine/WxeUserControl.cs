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
using System.ComponentModel;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.Compilation;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [FileLevelControlBuilder(typeof(CodeProcessingUserControlBuilder))]
  public class WxeUserControl : UserControl, IWxeTemplateControl, IReplaceableControl
  {
    private readonly WxeTemplateControlInfo _wxeInfo;
    private readonly LazyInitializationContainer _lazyContainer;
    private ControlReplacer _replacer;
    private bool _executeNextStep;
    private bool _isWxeInfoInitialized;
    private string _permanentUniqueID;

    public WxeUserControl ()
    {
      _wxeInfo = new WxeTemplateControlInfo (this);
      _lazyContainer = new LazyInitializationContainer();
    }

#pragma warning disable 0809
    /// <summary>
    /// In order for the control-replacing to work, the initialization logic of the <see cref="UserControl"/> 
    /// has to be placed into the <see cref="OnInitComplete"/> method.
    /// </summary>
    [Obsolete ("Override OnInitComplete instead.", true)]
    protected override sealed void OnInit (EventArgs e)
#pragma warning restore 0809
    {
      if (!_isWxeInfoInitialized)
      {
        _wxeInfo.Initialize (Context);  
        _isWxeInfoInitialized = true;
      }

      if (_replacer == null)
      {
        var replacer = new ControlReplacer (new InternalControlMemberCaller());
        replacer.ID = ID + "_Parent";

        _permanentUniqueID = UniqueID.Insert (UniqueID.Length - ID.Length, replacer.ID + IdSeparator);

        IUserControlExecutor userControlExecutor = GetUserControlExecutor();

        WxeUserControl control;
        IStateModificationStrategy stateModificationStrategy;
        if (!userControlExecutor.IsNull && !userControlExecutor.IsReturningPostBack)
        {
          Assertion.IsTrue (userControlExecutor.UserControlID == _permanentUniqueID);
          var currentUserControlStep = (WxeUserControlStep) userControlExecutor.Function.ExecutingStep;
          control = (WxeUserControl) Page.LoadControl (currentUserControlStep.UserControl);

          if (!currentUserControlStep.IsPostBack)
            stateModificationStrategy = new StateClearingStrategy();
          else
            stateModificationStrategy = new StateLoadingStrategy();
        }
        else
        {
          if (userControlExecutor.IsReturningPostBack)
          {
            control = (WxeUserControl) Page.LoadControl (userControlExecutor.BackedUpUserControl);
            stateModificationStrategy = new StateReplacingStrategy (userControlExecutor.BackedUpUserControlState);
          }
          else
          {
            control = this;
            stateModificationStrategy = new StateLoadingStrategy();
          }
        }

        control.ID = ID;
        control._permanentUniqueID = _permanentUniqueID;
        replacer.ReplaceAndWrap (this, control, stateModificationStrategy);
      }
      else
      {
        CompleteInitialization();
      }
    }

    private void CompleteInitialization ()
    {
      Assertion.IsNotNull (Parent, "The control has not been wrapped by the ControlReplacer during initialization or control replacement.");

      _lazyContainer.Ensure (base.Controls);

      OnInitComplete (EventArgs.Empty);
    }

    protected virtual void OnInitComplete (EventArgs e)
    {
      base.OnInit (e);
    }

    public override sealed ControlCollection Controls
    {
      get
      {
        EnsureChildControls();

        return _lazyContainer.GetControls (base.Controls);
      }
    }

    protected override sealed void CreateChildControls ()
    {
      if (ControlHelper.IsDesignMode (this))
        _lazyContainer.Ensure (base.Controls);
    }

    public void ExecuteFunction (WxeFunction function, Control sender, bool? usesEventTarget)
    {
      if (CurrentPageStep.UserControlExecutor.IsNull)
        CurrentPageStep.ExecuteFunction (this, function, sender, usesEventTarget ?? UsesEventTarget);
      else
        CurrentUserControlStep.ExecuteFunction (this, function, sender, usesEventTarget ?? UsesEventTarget);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public string SaveAllState ()
    {
      return _replacer.SaveAllState();
    }

    public WxePageStep CurrentPageStep
    {
      get { return _wxeInfo.CurrentPageStep; }
    }

    public WxeUserControlStep CurrentUserControlStep
    {
      get { return _wxeInfo.CurrentUserControlStep; }
    }

    private IUserControlExecutor GetUserControlExecutor ()
    {
      IUserControlExecutor userControlExecutor = CurrentPageStep.UserControlExecutor;
      if (!userControlExecutor.IsNull && !userControlExecutor.IsReturningPostBack && !CurrentUserControlStep.UserControlExecutor.IsNull)
        userControlExecutor = CurrentUserControlStep.UserControlExecutor;
      return userControlExecutor;
    }

    public WxeFunction CurrentFunction
    {
      get { return _wxeInfo.CurrentFunction; }
    }

    public NameObjectCollection Variables
    {
      get { return _wxeInfo.Variables; }
    }

    public IWxePage WxePage
    {
      get { return (IWxePage) base.Page; }
    }

    public bool IsUserControlPostBack
    {
      get { return CurrentUserControlStep != null ? CurrentUserControlStep.IsPostBack : CurrentPageStep.IsPostBack; }
    }
    public string PermanentUniqueID
    {
      get { return _permanentUniqueID; }
    }

    /// <summary> Implements <see cref="IWxePage.ExecuteNextStep">IWxePage.ExecuteNextStep</see>. </summary>
    public void ExecuteNextStep ()
    {
      _executeNextStep = true;
      Page.Visible = false; // suppress prerender and render events
    }

    public override void Dispose ()
    {
      base.Dispose();

      if (_executeNextStep)
      {
        if (Context != null)
          Context.Response.Clear(); // throw away page trace output
        throw new WxeExecuteUserControlNextStepException();
      }
    }

    bool IReplaceableControl.IsInitialized
    {
      get { return _lazyContainer.IsInitialized; }
    }

    ControlReplacer IReplaceableControl.Replacer
    {
      get { return _replacer; }
      set { _replacer = value; }
    }

    //TODO: Code duplication with WxeExecutor.UsesEventTarget
    private bool UsesEventTarget
    {
      get
      {
        NameValueCollection postBackCollection = WxePage.GetPostBackCollection ();
        if (postBackCollection == null)
        {
          if (WxePage.IsPostBack)
            throw new InvalidOperationException ("The IWxePage has no PostBackCollection even though this is a post back.");
          return false;
        }
        return !string.IsNullOrEmpty (postBackCollection[ControlHelper.PostEventSourceID]);
      }
    }

    IPage IControl.Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }
  }
}
