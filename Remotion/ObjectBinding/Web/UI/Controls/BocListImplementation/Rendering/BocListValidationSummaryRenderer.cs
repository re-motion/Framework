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

using System.Web.UI;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Renders a validation summary in a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  [ImplementationFor(typeof(IBocListValidationSummaryRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocListValidationSummaryRenderer : IBocListValidationSummaryRenderer
  {
    /// <summary>Name of the JavaScript function to call when a command control has been clicked.</summary>
    private const string c_onInlineValidationEntryClickScript = "BocList.OnInlineValidationEntryClick(event);";

    private readonly IRenderingFeatures _renderingFeatures;
    private readonly BocListCssClassDefinition _bocListCssClassDefinition;

    public BocListValidationSummaryRenderer (IRenderingFeatures renderingFeatures, BocListCssClassDefinition bocListCssClassDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(renderingFeatures), renderingFeatures);
      ArgumentUtility.CheckNotNull(nameof(bocListCssClassDefinition), bocListCssClassDefinition);

      _renderingFeatures = renderingFeatures;
      _bocListCssClassDefinition = bocListCssClassDefinition;
    }

    public void Render (BocRenderingContext<IBocList> renderingContext, in BocListValidationSummaryRenderArguments arguments)
    {
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Ul);

      foreach (var validationFailureWithLocation in arguments.ValidationFailures)
        RenderValidationFailure(renderingContext, validationFailureWithLocation, in arguments);

      renderingContext.Writer.RenderEndTag();
    }

    protected virtual WebString GetDisplayName (IBusinessObjectProperty property)
    {
      return WebString.CreateFromText(property.DisplayName);
    }

    private void RenderValidationFailure (
        BocRenderingContext<IBocList> renderingContext,
        BocListValidationFailureWithLocationInformation validationFailureWithLocation,
        in BocListValidationSummaryRenderArguments arguments)
    {
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var validatedObjectWithIdentity = validationFailureWithLocation.Failure.ValidatedObject as IBusinessObjectWithIdentity;
        var rowObjectWithIdentity = validationFailureWithLocation.RowObject as IBusinessObjectWithIdentity;

        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceRow, rowObjectWithIdentity?.UniqueIdentifier ?? string.Empty);
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListValidationFailureSourceColumn, validationFailureWithLocation.ColumnDefinition?.ItemID ?? string.Empty);
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceBusinessObject, validatedObjectWithIdentity?.UniqueIdentifier ?? string.Empty);
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.ValidationFailureSourceProperty, validationFailureWithLocation.Failure.ValidatedProperty?.Identifier ?? string.Empty);
      }

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Li);

      var columnDefinition = validationFailureWithLocation.ColumnDefinition;
      if (columnDefinition == null || !arguments.RenderCellValidationFailuresAsLinks)
      {
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

        var validatedProperty = validationFailureWithLocation.Failure.ValidatedProperty;
        if (validatedProperty != null)
        {
          GetDisplayName(validatedProperty).WriteTo(renderingContext.Writer);
          renderingContext.Writer.Write(": ");
        }
      }
      else
      {
        var visibleColumnIndex = arguments.ColumnIndexProvider.GetVisibleColumnIndex(columnDefinition);
        Assertion.IsTrue(visibleColumnIndex != -1, "visibleColumnIndex != -1");

        var validationMarkerCellID = BocRowRenderer.GetCellIDForValidationMarker(renderingContext.Control, arguments.RowIndex, visibleColumnIndex);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, $"#{validationMarkerCellID}");
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, c_onInlineValidationEntryClickScript);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.A);

        var columnTitle = columnDefinition.ColumnTitleDisplayValue.IsEmpty
            ? WebString.CreateFromText($"Column {visibleColumnIndex}")
            : columnDefinition.ColumnTitleDisplayValue;
        columnTitle.WriteTo(renderingContext.Writer);
        renderingContext.Writer.Write(": ");
      }

      PlainTextString.CreateFromText(validationFailureWithLocation.Failure.ErrorMessage).WriteTo(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag(); // a/span
      renderingContext.Writer.RenderEndTag(); // li
    }
  }
}
