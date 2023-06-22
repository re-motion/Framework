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
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  [ImplementationFor(typeof(IBocValidationErrorIndicatorColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocValidationErrorIndicatorColumnRenderer : BocColumnRendererBase<BocValidationErrorIndicatorColumnDefinition>, IBocValidationErrorIndicatorColumnRenderer
  {
    private const string c_nonBreakingSpace = "&nbsp;";

    private readonly IInfrastructureResourceUrlFactory _infrastructureResourceUrlFactory;
    private readonly IBocListValidationSummaryRenderer _validationSummaryRenderer;

    public BocValidationErrorIndicatorColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider,
        IInfrastructureResourceUrlFactory infrastructureResourceUrlFactory,
        IBocListValidationSummaryRenderer validationSummaryRenderer)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
      ArgumentUtility.CheckNotNull(nameof(infrastructureResourceUrlFactory), infrastructureResourceUrlFactory);
      ArgumentUtility.CheckNotNull(nameof(validationSummaryRenderer), validationSummaryRenderer);

      _infrastructureResourceUrlFactory = infrastructureResourceUrlFactory;
      _validationSummaryRenderer = validationSummaryRenderer;
    }

    protected override string GetAdditionalCssClassForDataColumnDeclaration (BocColumnRenderingContext<BocValidationErrorIndicatorColumnDefinition> renderingContext)
    {
      return CssClasses.DataColumnDeclarationValidationFailureIndicator;
    }

    protected override string GetAdditionalCssClassForTitleCell (BocCellAttributeRenderingContext<BocValidationErrorIndicatorColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      return CssClasses.TitleCellValidationFailureIndicator;
    }

    protected override string GetAdditionalCssClassForDataCell (BocCellAttributeRenderingContext<BocValidationErrorIndicatorColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      return CssClasses.DataCellValidationFailureIndicator;
    }

    protected override void RenderCellContents (BocColumnRenderingContext<BocValidationErrorIndicatorColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull(nameof(renderingContext), renderingContext);

      var bocListValidationFailureRepository = renderingContext.Control.ValidationFailureRepository;
      var validationFailures = bocListValidationFailureRepository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(arguments.BusinessObject, false);
      if (validationFailures.Count > 0)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.Content);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.ValidationErrorMarker);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

        var iconInfo = new IconInfo(_infrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, c_validationIndicatorIcon).GetUrl());
        iconInfo.Render(renderingContext.Writer, renderingContext.Control);

        renderingContext.Writer.RenderEndTag();

        var bocRenderingContext = new BocRenderingContext<IBocList>(
            renderingContext.HttpContext,
            renderingContext.Writer,
            renderingContext.Control);
        var renderArguments = new BocListValidationSummaryRenderArguments(
            renderingContext.ColumnIndexProvider,
            validationFailures,
            arguments.RowIndex);

        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.CssClassScreenReaderText);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
        _validationSummaryRenderer.Render(bocRenderingContext, in renderArguments);
        renderingContext.Writer.RenderEndTag();

        renderingContext.Writer.RenderEndTag();
      }
      else
      {
        renderingContext.Writer.Write(c_nonBreakingSpace);
      }
    }
  }
}
