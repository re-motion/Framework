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
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Hotkey;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="WebTab"/> controls in standard mode.
  /// </summary>
  [ImplementationFor(typeof(IWebTabRenderer), Lifetime = LifetimeKind.Singleton)]
  public class WebTabRenderer : IWebTabRenderer
  {
    private static readonly NoneHotkeyFormatter s_noneHotkeyFormatter = new NoneHotkeyFormatter();

    /// <summary> Returns the configured <see cref="IHotkeyFormatter"/> object. </summary>
    public IHotkeyFormatter HotkeyFormatter { get; }

    /// <summary> Returns the configured <see cref="IRenderingFeatures"/> object. </summary>
    public IRenderingFeatures RenderingFeatures { get; }

    public WebTabRenderer (IHotkeyFormatter hotkeyFormatter, IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("hotkeyFormatter", hotkeyFormatter);
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);

      HotkeyFormatter = hotkeyFormatter;
      RenderingFeatures = renderingFeatures;
    }

    public void Render (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isEnabled, bool isLast, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("style", style);

      RenderTabBegin(renderingContext);
      RenderSeperator(renderingContext);
      RenderWrapperBegin(renderingContext, tab);

      var command = RenderBeginTagForCommand(renderingContext, tab, isEnabled, style);
      RenderContents(renderingContext, tab);
      RenderEndTagForCommand(renderingContext, command);

      renderingContext.Writer.RenderEndTag(); // End tab span
      renderingContext.Writer.RenderEndTag(); // End tab wrapper span

      if (isLast)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabLast);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.RenderEndTag(); // End list item
    }

    private string GetTabClientID (WebTabStripRenderingContext renderingContext, IWebTab tab)
    {
      return renderingContext.Control.ClientID + "_" + tab.ItemID;
    }

    private void RenderWrapperBegin (WebTabStripRenderingContext renderingContext, IWebTab tab)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, GetTabClientID(renderingContext, tab));
      // role=none is required on span in this instance to remove the implied 'group' role.
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);

      string cssClass;
      if (tab.IsSelected)
        cssClass = CssClassTabSelected;
      else
        cssClass = CssClassTab;
      var isDisabled = !tab.EvaluateEnabled();

      if (isDisabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

      if (RenderingFeatures.EnableDiagnosticMetadata)
      {
        if (!string.IsNullOrEmpty(tab.ItemID))
          renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.ItemID, tab.ItemID);

        if (!tab.Text.IsEmpty)
        {
          var contentAttributeText = tab.Text.Type == WebStringType.Encoded ? tab.Text : s_noneHotkeyFormatter.GetFormattedText(tab.Text);
          HtmlUtility.ExtractPlainText(contentAttributeText).AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributes.Content);
        }

        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.IsDisabled, (isDisabled).ToString().ToLower());
      }

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span); // Begin tab span
    }

    private void RenderTabBegin (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Li); // Begin list item

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, "tabStripTabWrapper");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span); // Begin tab wrapper span
    }

    private void RenderSeperator (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassSeparator);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    protected virtual Command RenderBeginTagForCommand (WebTabStripRenderingContext renderingContext, IWebTab tab, bool isEnabled, WebTabStyle style)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("style", style);

      var attributes = new NameValueCollection();
      attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Tab);

      var command = new Command();
      command.OwnerControl = renderingContext.Control;
      command.ItemID = tab.ItemID + "_Command";

      if (isEnabled && tab.EvaluateEnabled())
      {
        command.Type = CommandType.Event;
        command.AccessKey = tab.AccessKey;
        if (string.IsNullOrEmpty(command.AccessKey))
        {
          var accessKey = HotkeyFormatter.GetAccessKey(tab.Text);
          if (accessKey.HasValue)
            command.AccessKey = accessKey.Value.ToString();
        }

        if (tab.IsSelected)
        {
          attributes.Add(HtmlTextWriterAttribute2.AriaSelected, HtmlAriaDisabledAttributeValue.True);
          // aria-controls will not be rendered at this time. Rendering the element isn't possible with the current composition graph.
          // Screenreaders do not depend/support aria-controls so this is not an issue at this time.
          //attributes.Add (HtmlTextWriterAttribute2.AriaControls, ??);
          attributes.Add("tabindex", "0");
        }
        else
        {
          attributes.Add(HtmlTextWriterAttribute2.AriaSelected, HtmlAriaDisabledAttributeValue.False);
          attributes.Add("tabindex", "-1");
        }
      }
      else
      {
        command.Type = CommandType.None;
        if (tab.IsSelected)
        {
          command.NoneCommand.EnableFocus = true;
          attributes.Add(HtmlTextWriterAttribute2.AriaSelected, HtmlAriaDisabledAttributeValue.True);
          // aria-controls will not be rendered at this time. Rendering the element isn't possible with the current composition graph.
          // Screenreaders do not depend/support aria-controls so this is not an issue at this time.
          //attributes.Add (HtmlTextWriterAttribute2.AriaControls, ??);
          attributes.Add("tabindex", "0");
        }
        else
        {
          attributes.Add(HtmlTextWriterAttribute2.AriaSelected, HtmlAriaDisabledAttributeValue.False);
          attributes.Add(HtmlTextWriterAttribute2.AriaDisabled, HtmlAriaDisabledAttributeValue.True);
          attributes.Add("tabindex", "-1");
        }
      }

      command.RenderBegin(
          renderingContext.Writer,
          RenderingFeatures,
          tab.GetPostBackClientEvent(),
          new string[0],
          string.Empty,
          null,
          new NameValueCollection(),
          false,
          style,
          attributes);

      return command;
    }

    protected virtual void RenderEndTagForCommand (WebTabStripRenderingContext renderingContext, Command command)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("command", command);

      command.RenderEnd(renderingContext.Writer);
    }

    protected virtual void RenderContents (WebTabStripRenderingContext renderingContext, IWebTab tab)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassTabAnchorBody);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span); // Begin anchor body span

      bool hasIcon = tab.Icon != null && tab.Icon.HasRenderingInformation;
      bool hasText = !tab.Text.IsEmpty;
      if (hasIcon)
      {
        Assertion.DebugIsNotNull(tab.Icon, "tab.Icon != null");
        tab.Icon.Render(renderingContext.Writer, renderingContext.Control);
      }

      if (hasText)
      {
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span); // Begin text span
        HotkeyFormatter.WriteTo(renderingContext.Writer, tab.Text);
        renderingContext.Writer.RenderEndTag(); // End text span
      }
      if (!hasIcon && !hasText)
        renderingContext.Writer.Write("&nbsp;");

      renderingContext.Writer.RenderEndTag(); // End anchor body span
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the inside of the anchor element. </summary>
    /// <remarks> 
    ///   <para> Class: <c>anchorBody</c>. </para>
    /// </remarks>
    public virtual string CssClassTabAnchorBody
    {
      get { return "anchorBody"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTab</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.TabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTab
    {
      get { return "tabStripTab"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSelected</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.SelectedTabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTabSelected
    {
      get { return "tabStripTabSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to a separator. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
    /// </remarks>
    public virtual string CssClassSeparator
    {
      get { return "tabStripTabSeparator"; }
    }


    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for clearing the space after the last tab. </summary>
    /// <remarks> 
    ///   <para> Class: <c>last</c>. </para>
    /// </remarks>
    public virtual string CssClassTabLast
    {
      get { return "last"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTab"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.tabStripTab.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }
  }
}
