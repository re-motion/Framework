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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  //  TODO: Command: Move long comment blocks to xml-file
  /// <summary> A <see cref="Command"/> defines an action the user can invoke. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class Command : IControlItem
  {
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class EventCommandInfo
    {
      //  EventPermissionProvider; //None, EventHandler, Properties
      //  public class Permission
      //  {
      //    WxeFunctionType;
      //    Method;
      //    AccessTypes;
      //  }

      private bool _requiresSynchronousPostBack;

      public EventCommandInfo ()
      {
      }

      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("True to require a synchronous postback within Ajax Update Panels.")]
      [DefaultValue (false)]
      [NotifyParentProperty (true)]
      public bool RequiresSynchronousPostBack
      {
        get { return _requiresSynchronousPostBack; }
        set { _requiresSynchronousPostBack = value; }
      }

      /// <summary> Returns a string representation of this <see cref="EventCommandInfo"/>. </summary>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        if (_requiresSynchronousPostBack)
          return "Synchronous Postback required";
        else
          return string.Empty;
      }
    }

    /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class HrefCommandInfo
    {
      private string _href = string.Empty;
      private string _target = string.Empty;

      /// <summary> Simple constructor. </summary>
      public HrefCommandInfo ()
      {
      }

      /// <summary> Returns a string representation of this <see cref="HrefCommandInfo"/>. </summary>
      /// <remarks> Format: Href, Target </remarks>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        if (_href == string.Empty || _target == string.Empty)
          return _href;
        else
          return _href + ", " + _target;
      }

      public virtual string FormatHref (params string[] parameters)
      {
        string[] encodedParameters = new string[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
          if (HttpContext.Current != null)
            encodedParameters[i] = HttpUtility.UrlEncode (parameters[i], HttpContext.Current.Response.ContentEncoding);
          else
            encodedParameters[i] = "";
        }
        return string.Format (Href, encodedParameters);
      }

      /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
      /// <value> 
      ///   The URL to link to when the rendered command is clicked. The default value is 
      ///   an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The hyperlink reference of the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Href
      {
        get { return _href; }
        set { _href = (value ?? string.Empty).Trim(); }
      }

      /// <summary> 
      ///   Gets or sets the target window or frame to display the web page specified by <see cref="Href"/> 
      ///   when  the rendered command is clicked.
      /// </summary>
      /// <value> 
      ///   The target window or frame to load the web page specified by <see cref="Href"/> when the rendered command
      ///   is clicked.  The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The target window or frame of the command. Leave it blank for no target.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Target
      {
        get { return _target; }
        set { _target = (value ?? string.Empty).Trim(); }
      }
    }

    /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
    [TypeConverter (typeof (ExpandableObjectConverter))]
    public class WxeFunctionCommandInfo
    {
      private string _mappingID = string.Empty;
      private string _typeName = string.Empty;
      private string _parameters = string.Empty;
      private string _target = string.Empty;

      /// <summary> Simple constructor. </summary>
      public WxeFunctionCommandInfo ()
      {
      }

      /// <summary>
      ///   Returns a string representation of this <see cref="WxeFunctionCommandInfo"/>.
      /// </summary>
      /// <remarks> Format: Href, Target </remarks>
      /// <returns> A <see cref="string"/>. </returns>
      public override string ToString ()
      {
        if (_typeName == string.Empty)
          return string.Empty;
        else
          return _typeName + " (" + _parameters + ")";
      }

      /// <summary> 
      ///   Gets or sets the complete type name of the <see cref="WxeFunction"/> to call when the rendered 
      ///   command is clicked. Either the <see cref="TypeName"/> or the <see cref="MappingID"/> is required.
      /// </summary>
      /// <value> 
      ///   The complete type name of the <see cref="WxeFunction"/> to call when the rendered command is clicked. 
      ///   The default value is an empty <see cref="String"/>. 
      /// </value>
      /// <remarks>
      ///   Valid type names include the classic .net syntax and typenames using the abbreviated form as defined by the
      ///   <see cref="TypeUtility.ParseAbbreviatedTypeName">TypeUtility.ParseAbbreviatedTypeName</see> method.
      ///   In ASP.NET 2.0, it is possible to use functions located in the <b>App_Code</b> assembly by not specifying an
      ///   assembly name.
      /// </remarks>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The complete type name (type[, assembly]) of the WxeFunction used for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string TypeName
      {
        get { return _typeName; }
        set { _typeName = (value ?? string.Empty).Trim(); }
      }

      /// <summary> 
      ///   Gets or sets the ID of the function as defined in the <see cref="UrlMappingEntry"/>.
      ///   Either the <see cref="TypeName"/> or the <see cref="MappingID"/> is required.
      /// </summary>
      /// <value> 
      ///   The <see cref="UrlMappingEntry.ID"/> associated with the <see cref="WxeFunction"/> to call when the 
      ///   rendered command is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("The Url-Mapping ID associated with the WxeFunction used for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public string MappingID
      {
        get { return _mappingID; }
        set { _mappingID = (value ?? string.Empty).Trim(); }
      }

      /// <summary> 
      ///   Gets or sets the comma separated list of parameters passed to the <see cref="WxeFunction"/> when the 
      ///   rendered command is clicked.
      /// </summary>
      /// <value> 
      ///   The comma separated list of parameters passed to the <see cref="WxeFunction"/> when the rendered command 
      ///   is clicked. The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behavior")]
      [Description ("A comma separated list of parameters for the command.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public virtual string Parameters
      {
        get { return _parameters; }
        set { _parameters = (value ?? string.Empty).Trim(); }
      }

      /// <summary> 
      ///   Gets or sets the target window or frame to open the <see cref="WxeFunction"/> when the rendered command is 
      ///   clicked.
      /// </summary>
      /// <value> 
      ///   The target window or frame to open the Wxe <see cref="WxeFunction"/> when the rendered command is clicked. 
      ///   The default value is an empty <see cref="String"/>. 
      /// </value>
      [PersistenceMode (PersistenceMode.Attribute)]
      [Category ("Behaviour")]
      [Description ("The target window or frame of the command. Leave it blank for no target.")]
      [DefaultValue ("")]
      [NotifyParentProperty (true)]
      public string Target
      {
        get { return _target; }
        set { _target = (value ?? string.Empty).Trim(); }
      }


      public virtual WxeFunction InitializeFunction (NameObjectCollection additionalWxeParameters)
      {
        Type functionType = ResolveFunctionType();
        WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType);

        function.VariablesContainer.InitializeParameters (_parameters, additionalWxeParameters);

        return function;
      }

      public virtual Type ResolveFunctionType ()
      {
        UrlMappingEntry mapping = UrlMappingConfiguration.Current.Mappings.FindByID (_mappingID);

        bool hasMapping = mapping != null;
        bool hasTypeName = !string.IsNullOrEmpty (_typeName);

        Type functionType = null;
        if (hasTypeName)
          functionType = WebTypeUtility.GetType (_typeName, true);

        if (hasMapping)
        {
          if (functionType == null)
            functionType = mapping.FunctionType;
          else if (mapping.FunctionType != functionType)
          {
            throw new InvalidOperationException (
                string.Format (
                    "The WxeFunctionCommand in has both a MappingID ('{0}') and a TypeName ('{1}') defined, but they resolve to different WxeFunctions.",
                    _mappingID,
                    _typeName));
          }
        }
        else if (!hasTypeName)
          throw new InvalidOperationException ("The WxeFunctionCommand has no valid MappingID or FunctionTypeName specified.");

        return functionType;
      }
    }

    private string _toolTip = string.Empty;
    private string _accessKey = string.Empty;
    private CommandType _type;
    private readonly CommandType _defaultType = CommandType.None;
    private CommandShow _show = CommandShow.Always;
    private EventCommandInfo _eventCommand = new EventCommandInfo();
    private HrefCommandInfo _hrefCommand = new HrefCommandInfo();
    private WxeFunctionCommandInfo _wxeFunctionCommand = new WxeFunctionCommandInfo();
    //private ScriptCommandInfo _scriptCommand = null;
    private bool _hasClickFired;
    private string _itemID = string.Empty;

    private IControl _ownerControl;

    [Browsable (false)]
    public CommandClickEventHandler Click;

    private readonly IWxeSecurityAdapter _wxeSecurityAdapter;
    private readonly IWebSecurityAdapter _webSecurityAdapter;

    public Command ()
        : this (CommandType.None, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public Command (CommandType defaultType)
      : this (defaultType, GetWebSecurityAdapter(), GetWxeSecurityAdapter())
    {
    }

    public Command (CommandType defaultType, [CanBeNull] IWebSecurityAdapter webSecurityAdapter, [CanBeNull] IWxeSecurityAdapter wxeSecurityAdapter)
    {
      _defaultType = defaultType;
      _type = _defaultType;
      _webSecurityAdapter = webSecurityAdapter;
      _wxeSecurityAdapter = wxeSecurityAdapter;
    }

    /// <summary> Fires the <see cref="Click"/> event. </summary>
    public virtual void OnClick ()
    {
      if (_hasClickFired)
        return;
      _hasClickFired = true;
      if (Click != null)
        Click (OwnerControl, new CommandClickEventArgs (this));
    }

    /// <summary> Renders the opening tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="renderingFeatures"> The rendering features to use. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    /// </param>
    /// <param name="style"> The style applied to the opening tag. </param>
    public virtual void RenderBegin (
        HtmlTextWriter writer,
        IRenderingFeatures renderingFeatures,
        string postBackEvent,
        string[] parameters,
        string onClick,
        ISecurableObject securableObject,
        NameValueCollection additionalUrlParameters,
        bool includeNavigationUrlParameters,
        Style style)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("style", style);

      var commandInfo = GetCommandInfo (postBackEvent, parameters, onClick, securableObject, additionalUrlParameters, includeNavigationUrlParameters);
      if (commandInfo != null)
        commandInfo.AddAttributesToRender (writer, renderingFeatures);
      
      if (OwnerControl != null && !string.IsNullOrEmpty (OwnerControl.ClientID) && !string.IsNullOrEmpty ( ItemID))
      {
        var clientID = OwnerControl.ClientID + "_" + ItemID;
        writer.AddAttribute (HtmlTextWriterAttribute.Id, clientID);
      }
      
      style.AddAttributesToRender (writer);
      
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }

    /// <summary> Renders the opening tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
    /// <param name="renderingFeatures"> The rendering features to use. </param>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    public void RenderBegin (HtmlTextWriter writer, IRenderingFeatures renderingFeatures, string postBackEvent, string[] parameters, string onClick, ISecurableObject securableObject)
    {
      RenderBegin (writer, renderingFeatures, postBackEvent, parameters, onClick, securableObject, new NameValueCollection (0), true, new Style());
    }

    /// <summary> Gets the <see cref="CommandInfo"/> for the command. </summary>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back.
    /// </param>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="securableObject">
    ///   The <see cref="ISecurableObject"/> for which security is evaluated. Use <see landword="null"/> if security is stateless or not evaluated.
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    /// </param>
    public CommandInfo GetCommandInfo (
        string postBackEvent,
        string[] parameters,
        string onClick,
        ISecurableObject securableObject,
        NameValueCollection additionalUrlParameters,
        bool includeNavigationUrlParameters)
    {
      if (!HasAccess (securableObject))
        return null;

      switch (_type)
      {
        case CommandType.Href:
          return GetCommandInfoForHrefCommand (parameters, onClick, additionalUrlParameters, includeNavigationUrlParameters);
        case CommandType.Event:
          return GetCommandInfoForEventCommand (postBackEvent, onClick);
        case CommandType.WxeFunction:
          return GetCommandInfoForWxeFunctionCommand (postBackEvent, onClick, additionalUrlParameters, includeNavigationUrlParameters);
        case CommandType.None:
          return null;
        default:
          throw new InvalidOperationException (
              string.Format ("The CommandType '{0}' is not supported by the '{1}'.", _type, typeof (Command).FullName));
      }
    }

    /// <summary> Creates a <see cref="CommandInfo"/> for the <see cref="HrefCommand"/>. </summary>
    /// <param name="parameters">
    ///   The strings inserted into the href attribute using <c>string.Format</c>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    ///   Defaults to <see langword="true"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Href"/>.
    /// </exception> 
    protected virtual CommandInfo GetCommandInfoForHrefCommand (
        string[] parameters,
        string onClick,
        NameValueCollection additionalUrlParameters,
        bool includeNavigationUrlParameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters);
      if (Type != CommandType.Href)
        throw new InvalidOperationException ("Call to GetCommandInfoForHrefCommand not allowed unless Type is set to CommandType.Href.");

      string href = HrefCommand.FormatHref (parameters);
      if (includeNavigationUrlParameters)
      {
        ISmartNavigablePage page = null;
        if (OwnerControl != null)
          page = OwnerControl.Page as ISmartNavigablePage;

        if (page != null)
        {
          additionalUrlParameters = additionalUrlParameters.Clone();
          NameValueCollectionUtility.Append (additionalUrlParameters, page.GetNavigationUrlParameters());
        }
      }
      href = UrlUtility.AddParameters (href, additionalUrlParameters);
      if (OwnerControl != null)
        href = OwnerControl.ResolveClientUrl (href);

      return CommandInfo.CreateForLink (
          StringUtility.EmptyToNull (_toolTip),
          StringUtility.EmptyToNull (_accessKey),
          href,
          StringUtility.EmptyToNull (HrefCommand.Target),
          StringUtility.EmptyToNull (onClick));
    }

    /// <summary> Creates a <see cref="CommandInfo"/> for the <see cref="EventCommand"/>. </summary>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Event"/>.
    /// </exception> 
    protected virtual CommandInfo GetCommandInfoForEventCommand (string postBackEvent, string onClick)
    {
      ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent);
      if (Type != CommandType.Event)
        throw new InvalidOperationException ("Call to GetCommandInfoForEventCommand not allowed unless Type is set to CommandType.Event.");

      return CommandInfo.CreateForPostBack (
          StringUtility.EmptyToNull (_toolTip),
          StringUtility.EmptyToNull (_accessKey),
          postBackEvent + (onClick ?? string.Empty));
    }

    /// <summary> Creates a <see cref="CommandInfo"/> for the <see cref="WxeFunctionCommand"/>. </summary>
    /// <param name="postBackEvent">
    ///   The string executed upon the click on a command of types
    ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
    ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
    ///   to force a post back. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="onClick"> 
    ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
    /// </param>
    /// <param name="additionalUrlParameters">
    ///   The <see cref="NameValueCollection"/> containing additional url parameters.
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="includeNavigationUrlParameters"> 
    ///   <see langword="true"/> to include URL parameters provided by <see cref="ISmartNavigablePage"/>.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
    /// </exception> 
    protected virtual CommandInfo GetCommandInfoForWxeFunctionCommand (
        string postBackEvent,
        string onClick,
        NameValueCollection additionalUrlParameters,
        bool includeNavigationUrlParameters)
    {
      ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent);
      if (Type != CommandType.WxeFunction)
      {
        throw new InvalidOperationException (
            "Call to GetCommandInfoForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");
      }

      return CommandInfo.CreateForPostBack (
          StringUtility.EmptyToNull (_toolTip),
          StringUtility.EmptyToNull (_accessKey),
          postBackEvent + (onClick ?? string.Empty));
    }

    /// <summary> Renders the closing tag for the command. </summary>
    /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
    public virtual void RenderEnd (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);
      writer.RenderEndTag();
    }

    /// <summary>
    ///   Returns a string representation of this <see cref="Command"/>.
    /// </summary>
    /// <remarks>
    ///   <list type="table">
    ///     <listheader>
    ///     <term>Type</term> 
    ///     <description>Format</description>
    ///     </listheader>
    ///     <item>
    ///       <term>Href</term>
    ///       <description> Href: &lt;HrefCommand.ToString()&gt; </description>
    ///     </item>
    ///     <item>
    ///       <term>WxeFunction</term>
    ///       <description> WxeFunction: &lt;WxeFunctionCommand.ToString()&gt; </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString ()
    {
      StringBuilder stringBuilder = new StringBuilder (50);

      stringBuilder.Append (Type.ToString());

      switch (Type)
      {
        case CommandType.Href:
          if (HrefCommand != null)
            stringBuilder.AppendFormat (": {0}", HrefCommand);
          break;
        case CommandType.WxeFunction:
          if (WxeFunctionCommand != null)
            stringBuilder.AppendFormat (": {0}", WxeFunctionCommand);
          break;
        default:
          break;
      }

      return stringBuilder.ToString();
    }

    /// <summary> Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. </summary>
    /// <param name="wxePage"> 
    ///   The <see cref="IWxePage"/> where this command is rendered on. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="additionalWxeParameters"> 
    ///   The parameters passed to the <see cref="WxeFunction"/> in addition to the executing function's variables.
    ///   Use <see langword="null"/> or an empty collection if all parameters are supplied by the 
    ///   <see cref="WxeFunctionCommandInfo.Parameters"/> string and the function stack.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///   <para>
    ///     Thrown if called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
    ///   </para><para>
    ///     Thrown if neither the <see cref="WxeFunctionCommandInfo.MappingID"/> nor the 
    ///     <see cref="WxeFunctionCommandInfo.TypeName"/> are set.
    ///   </para><para>
    ///     Thrown if the <see cref="WxeFunctionCommandInfo.MappingID"/> and <see cref="WxeFunctionCommandInfo.TypeName"/>
    ///     specify different functions.
    ///   </para>
    /// </exception> 
    public virtual void ExecuteWxeFunction (IWxePage wxePage, NameObjectCollection additionalWxeParameters)
    {
      ArgumentUtility.CheckNotNull ("wxePage", wxePage);

      if (Type != CommandType.WxeFunction)
        throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

      if (!wxePage.IsReturningPostBack)
      {
        string target = WxeFunctionCommand.Target;
        bool hasTarget = !string.IsNullOrEmpty (target);
        WxeFunction function = WxeFunctionCommand.InitializeFunction (additionalWxeParameters);

        IWxeCallArguments callArguments;
        if (hasTarget)
          callArguments = new WxeCallArguments ((Control) OwnerControl, new WxeCallOptionsExternal (target, null, false));
        else
          callArguments = WxeCallArguments.Default;

        try
        {
          wxePage.ExecuteFunction (function, callArguments);
        }
        catch (WxeCallExternalException)
        {
        }
      }
    }

    /// <summary> The <see cref="CommandType"/> represented by this instance of <see cref="Command"/>. </summary>
    /// <value> One of the <see cref="CommandType"/> enumeration values. The default is <see cref="CommandType.None"/>. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The type of command generated.")]
    [NotifyParentProperty (true)]
    public CommandType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The ToolTip/Title rendered in the anchor tag.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string ToolTip
    {
      get { return _toolTip; }
      set { _toolTip = (value ?? string.Empty).Trim(); }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The AccessKey rendered in the anchor tag.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string AccessKey
    {
      get { return _accessKey; }
      set { _accessKey = (value ?? string.Empty).Trim(); }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsDefaultType
    {
      get { return _type == _defaultType; }
    }

    /// <summary> Controls the persisting of the <see cref="Type"/>. </summary>
    protected bool ShouldSerializeType ()
    {
      return !IsDefaultType;
    }

    /// <summary> Sets the <see cref="Type"/> to its default value. </summary>
    protected void ResetType ()
    {
      _type = _defaultType;
    }

    /// <summary>
    ///   Determines when the item command is shown to the user in regard of the parent control's read-only setting.
    /// </summary>
    /// <value> 
    ///   One of the <see cref="CommandShow"/> enumeration values. The default is <see cref="CommandShow.Always"/>.
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("Determines when to show the item command to the user in regard to the parent control's read-only setting.")]
    [DefaultValue (CommandShow.Always)]
    [NotifyParentProperty (true)]
    public CommandShow Show
    {
      get { return _show; }
      set { _show = value; }
    }

    /// <summary>
    ///   The <see cref="EventCommandInfo"/> used when rendering the command as an event.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Event"/>.
    /// </remarks>
    /// <value> A <see cref="EventCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the event. Interpreted if Type is set to Event.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual EventCommandInfo EventCommand
    {
      get { return _eventCommand; }
      set { _eventCommand = value; }
    }

    /// <summary>
    ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>.
    /// </remarks>
    /// <value> A <see cref="HrefCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the hyperlink. Interpreted if Type is set to Href.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual HrefCommandInfo HrefCommand
    {
      get { return _hrefCommand; }
      set { _hrefCommand = value; }
    }

    /// <summary>
    ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
    /// </summary>
    /// <remarks> 
    ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>.
    /// </remarks>
    /// <value> A <see cref="WxeFunctionCommandInfo"/> object. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The properties of the WxeFunction. Interpreted if Type is set to WxeFunction.")]
    [DefaultValue ((string) null)]
    [NotifyParentProperty (true)]
    public virtual WxeFunctionCommandInfo WxeFunctionCommand
    {
      get { return _wxeFunctionCommand; }
      set { _wxeFunctionCommand = value; }
    }

    /// <summary> Gets or sets the control to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IControl OwnerControl
    {
      get { return OwnerControlImplementation; }
      set { OwnerControlImplementation = value; }
    }

    protected virtual IControl OwnerControlImplementation
    {
      get { return _ownerControl; }
      set
      {
        if (_ownerControl != value)
          _ownerControl = value;
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Advanced)]
    public string ItemID
    {
      get { return _itemID; }
      set
      {
        _itemID = value ?? string.Empty;
      }
    }

    public virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      var key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!string.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);
    }

    public void RegisterForSynchronousPostBackOnDemand ([NotNull]Control control, [NotNull]string argument, [NotNull]string commandID)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("argument", argument);
      ArgumentUtility.CheckNotNullOrEmpty ("commandID", commandID);

      bool isSynchronousEventCommand = Type == CommandType.Event && EventCommand.RequiresSynchronousPostBack;
      bool isSynchronousWxeFunctionCommand = Type == CommandType.WxeFunction && string.IsNullOrEmpty (WxeFunctionCommand.Target);

      if (!isSynchronousEventCommand && !isSynchronousWxeFunctionCommand)
        return;

      if (!ControlHelper.IsNestedInUpdatePanel (control))
        return;

      if (isSynchronousEventCommand)
      {
        ISmartPage smartPage = control.Page as ISmartPage;
        if (smartPage == null)
        {
          throw new InvalidOperationException (
              string.Format (
                  "{0}: EventCommands with RequiresSynchronousPostBack set to true are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                  commandID));
        }
        smartPage.RegisterCommandForSynchronousPostBack (control, argument);
      }
      else if (isSynchronousWxeFunctionCommand)
      {
        ISmartPage smartPage = control.Page as ISmartPage;
        if (smartPage == null)
        {
          throw new InvalidOperationException (
              string.Format (
                  "{0}: WxeCommands are only supported on pages implementing ISmartPage when used within an UpdatePanel.",
                  commandID));
        }
        smartPage.RegisterCommandForSynchronousPostBack (control, argument);
      }
    }

    public virtual bool HasAccess (ISecurableObject securableObject)
    {
      switch (_type)
      {
        case CommandType.Href:
          return true;
        case CommandType.Event:
          return HasAccessForEventCommand (securableObject);
        case CommandType.WxeFunction:
          return HasAccessForWxeFunctionCommand();
        case CommandType.None:
          return true;
        default:
          throw new InvalidOperationException (
              string.Format ("The CommandType '{0}' is not supported by the '{1}'.", _type, typeof (Command).FullName));
      }
    }

    private bool HasAccessForEventCommand (ISecurableObject securableObject)
    {
      if (_webSecurityAdapter == null)
        return true;
      return _webSecurityAdapter.HasAccess (securableObject, Click);
    }

    private bool HasAccessForWxeFunctionCommand ()
    {
      if (_wxeSecurityAdapter == null)
        return true;
      return _wxeSecurityAdapter.HasStatelessAccess (WxeFunctionCommand.ResolveFunctionType());
    }

    [CanBeNull]
    protected internal static IWebSecurityAdapter GetWebSecurityAdapter ()
    {
      return SafeServiceLocator.Current.GetAllInstances<IWebSecurityAdapter>()
          .SingleOrDefault (() => new InvalidOperationException ("Only a single IWebSecurityAdapter can be registered."));
    }

    [CanBeNull]
    protected static IWxeSecurityAdapter GetWxeSecurityAdapter ()
    {
      return WxeFunction.GetWxeSecurityAdapter();
    }
  }

  /// <summary> The possible command types of a <see cref="Command"/>. </summary>
  public enum CommandType
  {
    /// <summary> No command will be generated. </summary>
    None,
    /// <summary> A server side event will be raised upon a command click. </summary>
    Event,
    /// <summary> A hyperlink will be rendered on the page. </summary>
    Href,
    /// <summary> A <see cref="WxeFunction"/> will be called upon a command click. </summary>
    WxeFunction
  }

  /// <summary> Defines when the command will be active on the page. </summary>
  public enum CommandShow
  {
    /// <summary> The command is always active. </summary>
    Always,
    /// <summary> The command is only active if the containing element is read-only. </summary>
    ReadOnly,
    /// <summary> The command is only active if the containing element is in edit-mode. </summary>
    EditMode
  }

  /// <summary>
  ///   Represents the method that handles the <see cref="Command.Click"/> event
  ///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
  /// </summary>
  public delegate void CommandClickEventHandler (object sender, CommandClickEventArgs e);

  /// <summary> Provides data for the <see cref="Remotion.Web.UI.Controls.Command.Click"/> event. </summary>
  public class CommandClickEventArgs : EventArgs
  {
    private readonly Command _command;

    /// <summary> Initializes a new instance. </summary>
    public CommandClickEventArgs (Command command)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      _command = command;
    }

    /// <summary> The <see cref="Command"/> that caused the event. </summary>
    public Command Command
    {
      get { return _command; }
    }
  }
}