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
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebButtonImplementation;
using Remotion.Web.UI.Controls.WebButtonImplementation.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A <c>Button</c> using <c>&amp;</c> as access key prefix in <see cref="Button.Text"/>. </summary>
  /// <include file='..\..\doc\include\UI\Controls\WebButton.xml' path='WebButton/Class/*' />
  [ToolboxData("<{0}:WebButton runat=server></{0}:WebButton>")]
  public class WebButton
      :
          Button,
          IWebButton,
          // Required because Page.ProcessPostData always registers the last IPostBackEventHandler in the controls 
          // collection for controls (buttons) having PostData but no IPostBackDataHandler. 
          IPostBackDataHandler
  {
    private static readonly object s_clickEvent = new object();
    private static readonly NoneHotkeyFormatter s_noneHotkeyFormatter = new NoneHotkeyFormatter();

    private const string c_textViewStateKey = nameof(Text);
    private const string c_textWebStringViewStateKey = c_textViewStateKey + "_" + nameof(WebStringType);

    private IconInfo _icon;
    private readonly IRenderingFeatures _renderingFeatures;
    private PostBackOptions? _options;

    private bool _useLegacyButton;
    private bool _isDefaultButton;

    private ISecurableObject? _securableObject;
    private MissingPermissionBehavior _missingPermissionBehavior = MissingPermissionBehavior.Invisible;
    private bool _requiresSynchronousPostBack;
    private bool _hasPagePreRenderCompleted;

    private ButtonType _buttonType;
    private readonly IHotkeyFormatter _hotkeyFormatter;

    public WebButton ()
    {
      _icon = new IconInfo();
      _renderingFeatures = SafeServiceLocator.Current.GetInstance<IRenderingFeatures>();
      _hotkeyFormatter = SafeServiceLocator.Current.GetInstance<IHotkeyFormatter>();
    }

    [Category("Appearance")]
    [Description("The text to be shown on the button.")]
    [DefaultValue(typeof(WebString), "")]
    public new WebString Text
    {
      get
      {
        var value = (string?)ViewState[c_textViewStateKey];
        var type = (WebStringType?)ViewState[c_textWebStringViewStateKey] ?? WebStringType.PlainText;

        return type switch
        {
            WebStringType.PlainText => WebString.CreateFromText(value),
            WebStringType.Encoded => WebString.CreateFromHtml(value),
            _ => throw new InvalidOperationException(
                $"The value for key '{c_textWebStringViewStateKey}' in the ViewState contains invalid data '{type}'."),
        };
      }
      set
      {
        ViewState[c_textViewStateKey] = value.GetValue();
        ViewState[c_textWebStringViewStateKey] = value.Type;
      }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(HtmlHeadAppender.Current);

      ScriptUtility.Instance.RegisterJavaScriptInclude(this, HtmlHeadAppender.Current);
    }

    protected virtual IWebButtonRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IWebButtonRenderer>();
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
    }

    /// <remarks>
    ///   This method is never called if the button is rendered as a legacy button.
    /// </remarks>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      ArgumentUtility.CheckNotNull("postCollection", postCollection);

      string? eventTarget = postCollection[ControlHelper.PostEventSourceID];
      bool isScriptedPostBack = !string.IsNullOrEmpty(eventTarget);
      if (!isScriptedPostBack && IsLegacyButtonEnabled)
      {
        // The button can only fire a click event if client script is active or the button is used in legacy mode
        // A more general fallback is not possible becasue of compatibility issues with ExecuteFunctionNoRepost
        bool isSuccessfulControl = !string.IsNullOrEmpty(postCollection[postDataKey]);
        if (isSuccessfulControl)
          Page!.RegisterRequiresRaiseEvent(this);
      }
      return false;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(this, true) ?? NullResourceManager.Instance;
      LoadResources(resourceManager);

      if (_isDefaultButton && Page != null && string.IsNullOrEmpty(Page.Form.DefaultButton))
        Page.Form.DefaultButton = UniqueID;

      if (Page != null)
        Page.PreRenderComplete += Page_PreRenderComplete;
    }

    private void Page_PreRenderComplete (object? sender, EventArgs e)
    {
      if (_requiresSynchronousPostBack)
      {
        var scriptManager = ScriptManager.GetCurrent(base.Page!);
        if (scriptManager != null)
          scriptManager.RegisterPostBackControl(this);
      }
      _hasPagePreRenderCompleted = true;
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      if (string.IsNullOrEmpty(AccessKey))
      {
        var accessKey = _hotkeyFormatter.GetAccessKey(Text);
        if (accessKey.HasValue)
          writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, accessKey.Value.ToString());
      }

      if (Page != null)
        Page.VerifyRenderingInServerForm(this);

      if (IsEnabled)
      {
        string onClick = EnsureEndWithSemiColon(OnClientClick);
        if (HasAttributes)
        {
          string? onClickAttribute = Attributes["onclick"];
          if (onClickAttribute != null)
          {
            onClick += EnsureEndWithSemiColon(onClickAttribute);
            Attributes.Remove("onclick");
          }
        }

        if (Page != null)
        {
          var options = GetPostBackOptions();
          options.ClientSubmit = true;

          var postBackEventReference = Page.ClientScript.GetPostBackEventReference(options, false);
          if (string.IsNullOrEmpty(postBackEventReference))
            postBackEventReference = Page.ClientScript.GetPostBackEventReference(this, null);
          var postBackScript = EnsureEndWithSemiColon(postBackEventReference);

          onClick += postBackScript;
          onClick += "return false;";
        }

        if (!string.IsNullOrEmpty(onClick))
          writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);

        writer.AddAttribute("onmousedown", "WebButton.MouseDown (this, '" + CssClassMouseDown + "');");
        writer.AddAttribute("onmouseup", "WebButton.MouseUp (this, '" + CssClassMouseDown + "');");
        writer.AddAttribute("onmouseout", "WebButton.MouseOut (this, '" + CssClassMouseDown + "');");
      }


      _options = base.GetPostBackOptions();
      _options.ClientSubmit = false;
      _options.PerformValidation = false;
      _options.AutoPostBack = false;

      string backUpOnClientClick = OnClientClick;
      OnClientClick = null;

      var cssClassBackup = ControlStyle.CssClass;
      var originalCssClass = (CssClass ?? "").Replace(DisabledCssClass, "").Trim();
      var isCssStyleOverridden = !string.IsNullOrEmpty(originalCssClass);
      var computedCssClass = isCssStyleOverridden ? originalCssClass : CssClassBase;
      if (ButtonType == ButtonType.Primary)
        computedCssClass += " " + CssClassPrimary;
      else if (ButtonType == ButtonType.Supplemental)
        computedCssClass += " " + CssClassSupplemental;

      computedCssClass += " " + CssClassThemed;

      ControlStyle.CssClass = computedCssClass;

      var currentText = Text;
      if (Text.IsEmpty && IsLegacyButtonEnabled && _icon != null && _icon.HasRenderingInformation)
        base.Text = _icon.AlternateText;
      else
        base.Text = Text.GetValue();

      base.AddAttributesToRender(writer);

      Text = currentText;

      ControlStyle.CssClass = cssClassBackup;

      OnClientClick = backUpOnClientClick;

      // Must be after OnClientClick has been reset
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataAttributes(writer);

      _options = null;
    }

    private void AddDiagnosticMetadataAttributes (HtmlTextWriter writer)
    {
      IControlWithDiagnosticMetadata controlWithDiagnosticMetadata = this;
      writer.AddAttribute(DiagnosticMetadataAttributes.ControlType, controlWithDiagnosticMetadata.ControlType);

      if (!Text.IsEmpty)
      {
        var contentAttributeText = Text.Type == WebStringType.Encoded ? Text : s_noneHotkeyFormatter.GetFormattedText(Text);
        HtmlUtility.ExtractPlainText(contentAttributeText).AddAttributeTo(writer, DiagnosticMetadataAttributes.Content);
      }

      if (!string.IsNullOrEmpty(CommandName))
        writer.AddAttribute(DiagnosticMetadataAttributes.CommandName, CommandName);

      if (!string.IsNullOrEmpty(ID))
        writer.AddAttribute(DiagnosticMetadataAttributes.ItemID, ID);

      var triggersPostBack = !OnClientClick.Contains(";return ");
      writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, triggersPostBack.ToString().ToLower());

      writer.AddAttribute(DiagnosticMetadataAttributes.ButtonType, ButtonType.ToString());
    }

    protected override PostBackOptions GetPostBackOptions ()
    {
      if (_options == null)
        return base.GetPostBackOptions();
      else
        return _options;
    }

    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        if (IsLegacyButtonEnabled)
          return HtmlTextWriterTag.Input;
        //For new styles
        //if (ControlHelper.IsDesignMode (this))
        //  return HtmlTextWriterTag.Button;
        return HtmlTextWriterTag.Button;
      }
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled() && WcagHelper.Instance.IsWaiConformanceLevelARequired())
      {
        if (!_useLegacyButton)
          WcagHelper.Instance.HandleError(1, this, "UseLegacyButton");
      }
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      EvaluateWaiConformity();

      if (IsLegacyButtonEnabled)
        return;

      //For new styles
      //if (ControlHelper.IsDesignMode (this))
      //  return;

      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassButtonBody);
      writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (HasControls())
        base.RenderContents(writer);
      else
      {
        bool hasIcon = _icon.HasRenderingInformation;
        bool hasText = !Text.IsEmpty;
        if (hasIcon)
        {
          writer.AddAttribute(HtmlTextWriterAttribute.Src, _icon.Url);
          if (!_icon.Height.IsEmpty)
            writer.AddAttribute(HtmlTextWriterAttribute.Height, _icon.Height.ToString());
          if (!_icon.Width.IsEmpty)
            writer.AddAttribute(HtmlTextWriterAttribute.Width, _icon.Width.ToString());
          writer.AddAttribute(HtmlTextWriterAttribute.Alt, _icon.AlternateText);
          writer.RenderBeginTag(HtmlTextWriterTag.Img);
          writer.RenderEndTag();
        }
        if (hasText)
        {
          writer.RenderBeginTag(HtmlTextWriterTag.Span); // Begin text span
          _hotkeyFormatter.WriteTo(writer, Text);
          writer.RenderEndTag(); // End text span
        }
      }

      writer.RenderEndTag(); // End acnhorBody span
    }

    /// <summary> Gets or sets the icon displayed in this menu item. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Category("Appearance")]
    [Description("The icon displayed.")]
    [NotifyParentProperty(true)]
    public IconInfo Icon
    {
      get { return _icon; }
      set { _icon = value; }
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

      //  Dispatch simple properties
      string? key = ResourceManagerUtility.GetGlobalResourceKey(Text.GetValue());
      if (!string.IsNullOrEmpty(key))
        Text = resourceManager.GetWebString(key, Text.Type);

      key = ResourceManagerUtility.GetGlobalResourceKey(AccessKey);
      if (!string.IsNullOrEmpty(key))
        AccessKey = resourceManager.GetString(key);

      key = ResourceManagerUtility.GetGlobalResourceKey(ToolTip);
      if (!string.IsNullOrEmpty(key))
        ToolTip = resourceManager.GetString(key);

      if (Icon != null)
        Icon.LoadResources(resourceManager);
    }

    /// <summary>
    /// Gets or sets the button type that determines how the button is displayed on the page.
    /// </summary>
    /// <remarks>
    /// Depending on the button types the css classes <see cref="CssClassPrimary"/> or <see cref="CssClassSupplemental"/> will be applied to the button.
    /// </remarks>
    [Description("Determines how the button is displayed on the page.")]
    [Category("Appearance")]
    [DefaultValue(UI.Controls.ButtonType.Standard)]
    public ButtonType ButtonType
    {
      get { return _buttonType; }
      set { _buttonType = value; }
    }

    /// <summary> 
    ///   Gets or sets the flag that determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button). 
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to enable the legacy version. Defaults to <see langword="false"/>.
    /// </value>
    [Description("Determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button).")]
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool UseLegacyButton
    {
      get { return _useLegacyButton; }
      set { _useLegacyButton = value; }
    }

    protected bool IsLegacyButtonEnabled
    {
      get { return WcagHelper.Instance.IsWaiConformanceLevelARequired() || _useLegacyButton; }
    }

    [Category("Behavior")]
    [DefaultValue(false)]
    public bool IsDefaultButton
    {
      get { return _isDefaultButton; }
      set { _isDefaultButton = value; }
    }

    [JetBrains.Annotations.NotNull]
    private string EnsureEndWithSemiColon (string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        value = value.Trim();

        if (!value.EndsWith(";"))
          value += ";";
      }

      return value ?? string.Empty;
    }

    protected bool HasAccess ()
    {
      var securityAdapter = WebSecurityAdapter;
      if (securityAdapter == null)
        return true;

      EventHandler? clickHandler = (EventHandler?)Events[s_clickEvent];
      if (clickHandler == null)
        return true;

      return securityAdapter.HasAccess(_securableObject, Events[s_clickEvent]);
    }

    public override bool Visible
    {
      get
      {
        if (!base.Visible)
          return false;

        if (_missingPermissionBehavior == MissingPermissionBehavior.Invisible)
          return HasAccess();

        return true;
      }
      set { base.Visible = value; }
    }

    public override bool Enabled
    {
      get
      {
        if (!base.Enabled)
          return false;

        if (_missingPermissionBehavior == MissingPermissionBehavior.Disabled)
          return HasAccess();

        return true;
      }
      set { base.Enabled = value; }
    }

    [Category("Action")]
    public new event EventHandler Click
    {
      add { Events.AddHandler(s_clickEvent, value); }
      remove { Events.RemoveHandler(s_clickEvent, value); }
    }

    protected override void OnClick (EventArgs e)
    {
      base.OnClick(e);

      EventHandler? clickHandler = (EventHandler?)Events[s_clickEvent];
      if (clickHandler != null)
        clickHandler(this, e);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ISecurableObject? SecurableObject
    {
      get { return _securableObject; }
      set { _securableObject = value; }
    }

    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [NotifyParentProperty(true)]
    [DefaultValue(MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }

    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("True to require a synchronous postback within Ajax Update Panels.")]
    [DefaultValue(false)]
    public bool RequiresSynchronousPostBack
    {
      get { return _requiresSynchronousPostBack; }
      set
      {
        if (_hasPagePreRenderCompleted)
        {
          throw new InvalidOperationException(
              string.Format(
                  "Attempting to set the RequiresSynchronousPostBack flag on button '{0}' is not supported after the PreRenderComplete event has fired.",
                  ID));
        }
        _requiresSynchronousPostBack = value;
      }
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }


    protected virtual IServiceLocator ServiceLocator
    {
      get { return SafeServiceLocator.Current; }
    }

    private IWebSecurityAdapter? WebSecurityAdapter
    {
      get { return UI.Controls.Command.GetWebSecurityAdapter(); }
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "WebButton"; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebButton"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>webButton</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "webButton"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    protected virtual string CssClassButtonBody
    {
      get { return "buttonBody"; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to the <c>div</c> when itself and child elements
    /// that are standard browser controls (e.g. input elements) should be styled in the current theme.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>remotion-themed</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public virtual string CssClassThemed
    {
      get { return CssClassDefinition.Themed; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to the <c>div</c> when the <see cref="ButtonType"/> is set to <see cref="Controls.ButtonType"/>.<see cref="Controls.ButtonType.Primary"/>.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>primary</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public virtual string CssClassPrimary
    {
      get { return CssClassDefinition.ButtonTypePrimary; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to the <c>div</c> when the <see cref="ButtonType"/> is set to <see cref="Controls.ButtonType"/>.<see cref="Controls.ButtonType.Supplemental"/>.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>supplemental</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public virtual string CssClassSupplemental
    {
      get { return CssClassDefinition.ButtonTypeSupplemental; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>mouseDown</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>a.webButton.mouseDown</c>as a selector.
    ///   </para>
    /// </remarks>
    protected virtual string CssClassMouseDown
    {
      get { return "mouseDown"; }
    }

    #endregion
  }
}
