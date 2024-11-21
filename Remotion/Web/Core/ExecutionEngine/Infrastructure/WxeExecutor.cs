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
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// Encapsulates execute logic for WXE functions.
  /// </summary>
  /// <remarks>
  /// Dispose the <see cref="WxeExecutor"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
  /// </remarks>
  public class WxeExecutor : IDisposable, IWxeExecutor
  {
    private readonly HttpContext _httpContext;
    private readonly IWxePage _page;
    private readonly WxePageInfo _wxePageInfo;

    public WxeExecutor (HttpContext context, IWxePage page, WxePageInfo wxePageInfo)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("wxePageInfo", wxePageInfo);

      _wxePageInfo = wxePageInfo;
      _page = page;
      _httpContext = context;
      _httpContext.Handler = page;
    }

    /// <summary>
    /// Invoke <see cref="IDisposable.Dispose"/> at the end of the page life cycle, i.e. in the <see cref="Control.Dispose"/> method.
    /// </summary>
    void IDisposable.Dispose ()
    {
      _httpContext.Handler = _wxePageInfo.WxeHandler;
    }

    public HttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public void ExecuteFunction (WxeFunction function, Control? sender, WxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull("function", function);
      // sender can be null
      ArgumentUtility.CheckNotNull("options", options);

      WxePermaUrlOptions permaUrlOptions = options.PermaUrlOptions;
      WxeRepostOptions repostOptions = WxeRepostOptions.DoRepost(sender);
      _wxePageInfo.CurrentPageStep.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_page, function, permaUrlOptions), repostOptions);
    }

    public void ExecuteFunctionNoRepost (WxeFunction function, Control sender, WxeCallOptionsNoRepost options)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);
      ArgumentUtility.CheckNotNull("options", options);

      bool usesEventTarget = options.UsesEventTarget ?? UsesEventTarget;
      WxePermaUrlOptions permaUrlOptions = options.PermaUrlOptions;
      WxeRepostOptions repostOptions = WxeRepostOptions.SuppressRepost(sender, usesEventTarget);
      _wxePageInfo.CurrentPageStep.ExecuteFunction(new PreProcessingSubFunctionStateParameters(_page, function, permaUrlOptions), repostOptions);
    }

    public void ExecuteFunctionExternalByRedirect (WxeFunction function, Control sender, WxeCallOptionsExternalByRedirect options)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);
      ArgumentUtility.CheckNotNull("options", options);

      WxeReturnOptions returnOptions;
      if (options.ReturnToCaller)
        returnOptions = new WxeReturnOptions(options.CallerUrlParameters ?? _page.GetPermanentUrlParameters());
      else
        returnOptions = WxeReturnOptions.Null;

      WxePermaUrlOptions permaUrlOptions = options.PermaUrlOptions;
      _wxePageInfo.CurrentPageStep.ExecuteFunctionExternalByRedirect(new PreProcessingSubFunctionStateParameters(_page, function, permaUrlOptions), returnOptions);
    }

    public void ExecuteFunctionExternal (WxeFunction function, Control sender, WxeCallOptionsExternal options)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);
      ArgumentUtility.CheckNotNull("options", options);

      string functionToken = WxeContext.Current!.GetFunctionTokenForExternalFunction(function, options.ReturningPostback); // TODO RM-8118: not null assertion

      string href = WxeContext.Current.GetDestinationUrlForExternalFunction(function, functionToken, options.PermaUrlOptions);

      var functionKey  = Guid.NewGuid().ToString();
      // Execute after Smart-Page was restored
      string functionName = "ExecuteFunctionExternal_" + functionKey.Replace('-', '_');
      string openScript = string.Format(
          "function {0} () {{ setTimeout( function () {{ window.open('{1}', '{2}', '{3}'); }}, 0); }}",
          functionName,
          href,
          options.Target,
          options.Features ?? string.Empty);

      _page.ClientScript.RegisterClientScriptBlock(_page, typeof(WxeExecutor), functionKey, openScript);
      _page.RegisterClientSidePageEventHandler(SmartPageEvents.OnLoaded, functionKey, functionName);

      function.SetExecutionCompletedScript(GetClosingScriptForExternalFunction(functionToken, sender, options.ReturningPostback));
    }

    /// <summary> 
    ///   Gets a flag describing whether the post back was most likely caused by the ASP.NET post back mechanism.
    /// </summary>
    /// <value> <see langword="true"/> if the post back collection contains the <b>__EVENTTARGET</b> field. </value>
    //TODO: Remove CodeDuplication with WxeUserControl.UsesEventTarget
    private bool UsesEventTarget
    {
      get
      {
        NameValueCollection? postBackCollection = _page.GetPostBackCollection();
        if (postBackCollection == null)
        {
          if (_page.IsPostBack)
            throw new InvalidOperationException("The IWxePage has no PostBackCollection even though this is a post back.");
          return false;
        }
        return !string.IsNullOrEmpty(postBackCollection[ControlHelper.PostEventSourceID]);
      }
    }

    /// <summary> Gets the client script to be used as the return URL for the window of the external function. </summary>
    private string GetClosingScriptForExternalFunction (string functionToken, Control sender, bool returningPostback)
    {
      if (!returningPostback)
        return "window.close();";

      ArgumentUtility.CheckNotNull("sender", sender);

      if (UsesEventTarget)
      {
        NameValueCollection? postBackCollection = _page.GetPostBackCollection();
        if (postBackCollection == null)
          throw new InvalidOperationException("The IWxePage has no PostBackCollection even though this is a post back.");

        string? eventTarget = postBackCollection[ControlHelper.PostEventSourceID];
        string? eventArgument = postBackCollection[ControlHelper.PostEventArgumentID];
        return FormatDoPostBackClientScript(functionToken, _page.CurrentPageStep.PageToken, sender.ClientID, eventTarget, eventArgument);
      }
      else
      {
        if (!(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
        {
          throw new ArgumentException(
              "The sender must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
        }
        return FormatDoSubmitClientScript(functionToken, _page.CurrentPageStep.PageToken, sender.ClientID);
      }
    }

    /// <summary> 
    ///   Gets the client script used to execute <c>__dopostback</c> in the parent form before closing the window of the 
    ///   external function.
    /// </summary>
    private string FormatDoPostBackClientScript (string functionToken, string pageToken, string senderID, string? eventTarget, string? eventArgument)
    {
      return string.Format(
@"
if (   window.opener != null
    && ! window.opener.closed
    && window.opener.wxeDoPostBack != null
    && window.opener.document.getElementById('{0}') != null
    && window.opener.document.getElementById('{0}').value == '{1}')
{{
  window.opener.postMessage({{
    type: 'wxeDoPostBack',
    args: ['{2}', '{3}', '{4}']
  }});
}}
window.close();
",
          WxePageInfo.PageTokenID,
          pageToken,
          eventTarget,
          eventArgument,
          functionToken);
    }

    /// <summary> 
    ///   Gets the client script used to submit the parent form before closing the window of the external function. 
    /// </summary>
    private string FormatDoSubmitClientScript (string functionToken, string pageToken, string senderID)
    {
      return string.Format(
@"
if (   window.opener != null
    && ! window.opener.closed
    && window.opener.wxeDoSubmit != null
    && window.opener.document.getElementById('{0}') != null
    && window.opener.document.getElementById('{0}').value == '{1}')
{{
  window.opener.postMessage({{
    type: 'wxeDoSubmit',
    args: ['{2}', '{3}']
  }});
}}
window.close();
",
          WxePageInfo.PageTokenID,
          pageToken,
          senderID,
          functionToken);
    }
  }
}
