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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="DatePickerButton"/> control in standard mode.
  /// <seealso cref="IDatePickerButton"/>
  /// </summary>
  [ImplementationFor(typeof(IDatePickerButtonRenderer), Lifetime = LifetimeKind.Singleton)]
  public class DatePickerButtonRenderer : RendererBase<IDatePickerButton>, IDatePickerButtonRenderer
  {
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    public DatePickerButtonRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("fallbackNavigationUrlProvider", fallbackNavigationUrlProvider);
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleFileKey = typeof(DatePickerButtonRenderer).GetFullNameChecked() + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(DatePickerButtonRenderer), ResourceType.Html, "DatePicker.css");
      htmlHeadAppender.RegisterStylesheetLink(styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }


    private const string c_datePickerPopupForm = "DatePickerForm.aspx";
    private const string c_datePickerIcon = "sprite.svg#DatePicker";

    /// <summary>
    /// Renders a click-enabled image that shows a <see cref="DatePickerPage"/> on click, which puts the selected value
    /// into the control specified by <see cref="P:Control.TargetControlID"/>.
    /// </summary>
    public void Render (DatePickerButtonRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);

      string cssClass = string.IsNullOrEmpty(renderingContext.Control.CssClass) ? CssClassBase : renderingContext.Control.CssClass;
      cssClass += " " + CssClassThemed;
      if (!renderingContext.Control.Enabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

      // TODO: hyperLink.ApplyStyle (Control.DatePickerButtonStyle);

      string script = GetClickScript(renderingContext, true);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, script);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");

      if (!renderingContext.Control.Enabled)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.A);

      var imageUrl = GetResolvedImageUrl();

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Src, imageUrl.GetUrl());
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Alt, renderingContext.Control.AlternateText ?? string.Empty);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Img);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.RenderEndTag();
    }

    public IResourceUrl GetDatePickerUrl ()
    {
      var datePickerUrl = string.Format(
          "{0}?{1}={2}&{3}={4}",
          c_datePickerPopupForm,
          DatePickerPage.CultureParameterName,
          CultureInfo.CurrentCulture.Name,
          DatePickerPage.UICultureParameterName,
          CultureInfo.CurrentUICulture.Name);
      return ResourceUrlFactory.CreateThemedResourceUrl(typeof(DatePickerPageRenderer), ResourceType.UI, datePickerUrl);
    }

    public IResourceUrl GetResolvedImageUrl ()
    {
      return ResourceUrlFactory.CreateThemedResourceUrl(typeof(DatePickerButtonRenderer), ResourceType.Image, c_datePickerIcon);
    }

    private string GetClickScript (DatePickerButtonRenderingContext renderingContext, bool hasClientScript)
    {
      string script;
      if (hasClientScript && renderingContext.Control.Enabled)
      {
        const string pickerActionButton = "this";

        string pickerActionContainer = "document.getElementById ('" + renderingContext.Control.ContainerControlID!.Replace('$', '_') + "')"; // TODO RM-8118: not null assertion
        string pickerActionTarget = "document.getElementById ('" + renderingContext.Control.TargetControlID!.Replace('$', '_') + "')"; // TODO RM-8118: not null assertion

        string pickerUrl = "'" + GetDatePickerUrl().GetUrl() + "'";

        Unit popUpWidth = PopUpWidth;
        string pickerWidth = "'" + popUpWidth + "'";

        Unit popUpHeight = PopUpHeight;
        string pickerHeight = "'" + popUpHeight + "'";

        script = "DatePicker.ShowDatePicker("
                 + pickerActionButton + ", "
                 + pickerActionContainer + ", "
                 + pickerActionTarget + ", "
                 + pickerUrl + ", "
                 + pickerWidth + ", "
                 + pickerHeight + ");"
                 + "return false;";
      }
      else
        script = "return false;";
      return script;
    }

    public string CssClassBase
    {
      get { return "DatePickerButton"; }
    }

    public string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }

    public string CssClassThemed
    {
      get { return CssClassDefinition.Themed; }
    }

    protected Unit PopUpWidth
    {
      get { return new Unit(14, UnitType.Em); }
    }

    protected Unit PopUpHeight
    {
      get { return new Unit(16, UnitType.Em); }
    }
  }
}
