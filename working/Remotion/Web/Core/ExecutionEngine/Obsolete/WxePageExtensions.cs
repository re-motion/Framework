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
using System.Web.UI;

namespace Remotion.Web.ExecutionEngine.Obsolete
{
  /// <summary>
  /// This class contains extension methods for the <see cref="IWxePage"/> interface.
  /// </summary>
  public static class WxePageExtensions
  {
    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunction/param[@name="page" or @name="function"]' />
    public static void ExecuteFunction (this IWxePage page, WxeFunction function)
    {
      ExecuteFunction (page, function, false, false, null);
    }

    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunction/param[@name="page" or @name="function" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunction (this IWxePage page, WxeFunction function, bool createPermaUrl, bool useParentPermaUrl)
    {
      ExecuteFunction (page, function, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> Executes the <paramref name="function"/> in the current window. </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunction/param[@name="page" or @name="function" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    public static void ExecuteFunction (
        this IWxePage page, WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
    {
      var permaUrlOptions = CreatePermaUrlOptions (createPermaUrl, useParentPermaUrl, permaUrlParameters);
      var arguments = new WxeCallArgumentsWithoutSender (permaUrlOptions);
      Execute (page, function, arguments);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.</remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender)
    {
      ExecuteFunctionNoRepost (page, function, sender, false, false, null);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>
    ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
    ///   When in doubt, use <see cref="M:Remotion.Web.ExecutionEngine.Obsolete.WxePageExtensions.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.IWxePage,Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control)">WxePageExtensions.ExecuteFunctionNoRepost(IWxePage,WxeFunction,Control)</see>.
    /// </remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender" or @name="usesEventTarget"]' />
    public static void ExecuteFunctionNoRepost (this IWxePage page, WxeFunction function, Control sender, bool usesEventTarget)
    {
      ExecuteFunctionNoRepost (page, function, sender, usesEventTarget, false, false, null);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.</remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionNoRepost (
        this IWxePage page, WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl)
    {
      ExecuteFunctionNoRepost (page, function, sender, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>This overload tries to determine automatically whether the current event was caused by the <c>__EVENTTARGET</c> field.</remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    public static void ExecuteFunctionNoRepost (
        this IWxePage page, WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection permaUrlParameters)
    {
      var permaUrlOptions = CreatePermaUrlOptions (createPermaUrl, useParentPermaUrl, permaUrlParameters);
      var options = new WxeCallOptionsNoRepost (permaUrlOptions);
      var arguments = new WxeCallArguments (sender, options);
      Execute (page, function, arguments);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>
    ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
    ///   When in doubt, use <see cref="M:Remotion.Web.ExecutionEngine.Obsolete.WxePageExtensions.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.IWxePage,Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean)">WxePageExtensions.ExecuteFunctionNoRepost(IWxePage,WxeFunction,Control,Boolean,Boolean)</see>.
    /// </remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionNoRepost (
        this IWxePage page, WxeFunction function, Control sender, bool usesEventTarget, bool createPermaUrl, bool useParentPermaUrl)
    {
      ExecuteFunctionNoRepost (page, function, sender, usesEventTarget, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary>Executes the <paramref name="function"/> in the current window without triggering the current post-back event on returning.</summary>
    /// <remarks>
    ///   This overload allows you to specify whether the current event was caused by the <c>__EVENTTARGET</c> field.
    ///   When in doubt, use <see cref="M:Remotion.Web.ExecutionEngine.Obsolete.WxePageExtensions.ExecuteFunctionNoRepost(Remotion.Web.ExecutionEngine.IWxePage,Remotion.Web.ExecutionEngine.WxeFunction,System.Web.UI.Control,System.Boolean,System.Boolean,System.Collections.Specialized.NameValueCollection)">WxePageExtensions.ExecuteFunctionNoRepost(IWxePage,WxeFunction,Control,Boolean,Boolean,NameValueCollection)</see>.
    /// </remarks>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionNoRepost/param[@name="page" or @name="function" or @name="sender" or @name="usesEventTarget" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="permaUrlParameters"]' />
    public static void ExecuteFunctionNoRepost (
        this IWxePage page,
        WxeFunction function,
        Control sender,
        bool usesEventTarget,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection permaUrlParameters)
    {
      var permaUrlOptions = CreatePermaUrlOptions (createPermaUrl, useParentPermaUrl, permaUrlParameters);
      var options = new WxeCallOptionsNoRepost (usesEventTarget, permaUrlOptions);
      var arguments = new WxeCallArguments (sender, options);
      Execute (page, function, arguments);
    }

    [Obsolete("ExecuteFunctionExternal for redirects requires a sender to be specified. (1.13.180)", true)]
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      throw new NotSupportedException("ExecuteFunctionExternal for redirects requires a sender to be specified. (1.13.180)");
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the new function.
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, Control sender, bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      ExecuteFunctionExternal (page, function, sender, createPermaUrl, useParentPermaUrl, urlParameters, true, null);
    }

    [Obsolete ("ExecuteFunctionExternal for redirects requires a sender to be specified. (1.13.180)", true)]
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters,
        bool returnToCaller,
        NameValueCollection callerUrlParameters)
    {
      throw new NotSupportedException ("ExecuteFunctionExternal for redirects requires a sender to be specified. (1.13.180)");
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   current window or frame. The execution engine uses a redirect request to transfer the execution to the new function.
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="sender" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters" or @name="returnToCaller" or @name="callerUrlParameters"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        Control sender,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters,
        bool returnToCaller,
        NameValueCollection callerUrlParameters)
    {
      var permaUrlOptions = CreatePermaUrlOptions ((createPermaUrl || urlParameters != null), useParentPermaUrl, urlParameters);
      var options = new WxeCallOptionsExternalByRedirect (permaUrlOptions, returnToCaller, callerUrlParameters);
      var arguments = new WxeCallArguments (sender, options);
      Execute (page, function, arguments);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame by through a javascript call.
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="sender" or @name="returningPostback"]' />
    public static void ExecuteFunctionExternal (this IWxePage page, WxeFunction function, string target, Control sender, bool returningPostback)
    {
      ExecuteFunctionExternal (page, function, target, null, sender, returningPostback, false, false, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, string target, string features, Control sender, bool returningPostback)
    {
      ExecuteFunctionExternal (page, function, target, features, sender, returningPostback, false, false, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page, WxeFunction function, string target, Control sender, bool returningPostback, bool createPermaUrl, bool useParentPermaUrl)
    {
      ExecuteFunctionExternal (page, function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        string target,
        Control sender,
        bool returningPostback,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters)
    {
      ExecuteFunctionExternal (page, function, target, null, sender, returningPostback, createPermaUrl, useParentPermaUrl, urlParameters);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        string target,
        string features,
        Control sender,
        bool returningPostback,
        bool createPermaUrl,
        bool useParentPermaUrl)
    {
      ExecuteFunctionExternal (page, function, target, features, sender, returningPostback, createPermaUrl, useParentPermaUrl, null);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> outside the current function's context (i.e. asynchron) using the 
    ///   specified window or frame through javascript window.open(...).
    /// </summary>
    /// <include file='..\..\doc\include\ExecutionEngine\WxePageExtensions.xml' path='WxePageExtensions/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="sender" or @name="returningPostback" or @name="createPermaUrl" or @name="useParentPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        this IWxePage page,
        WxeFunction function,
        string target,
        string features,
        Control sender,
        bool returningPostback,
        bool createPermaUrl,
        bool useParentPermaUrl,
        NameValueCollection urlParameters)
    {
      var permaUrlOptions = CreatePermaUrlOptions (createPermaUrl, useParentPermaUrl, urlParameters);
      var options = new WxeCallOptionsExternal (target, features, returningPostback, permaUrlOptions);
      var arguments = new WxeCallArguments (sender, options);
      Execute (page, function, arguments);
    }

    private static void Execute (IWxePage page, WxeFunction function, IWxeCallArguments arguments)
    {
      try
      {
        page.ExecuteFunction (function, arguments);
      }
      catch (WxeCallExternalException)
      {
      }
    }

    private static WxePermaUrlOptions CreatePermaUrlOptions (bool createPermaUrl, bool useParentPermaUrl, NameValueCollection urlParameters)
    {
      return createPermaUrl ? new WxePermaUrlOptions (useParentPermaUrl, urlParameters) : WxePermaUrlOptions.Null;
    }
  }
}
