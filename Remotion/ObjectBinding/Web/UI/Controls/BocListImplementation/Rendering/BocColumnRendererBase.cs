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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Abstract base class for all column renderers. Provides a template for rendering a table cell from an <see cref="IBusinessObject"/>
  /// and a <see cref="BocColumnDefinition"/>. 
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocColumnRendererBase<TBocColumnDefinition> : IBocColumnRenderer
      where TBocColumnDefinition : BocColumnDefinition
  {
    /// <summary>Filename of the image used to indicate an ascending sort order of the column in its title cell.</summary>
    protected const string c_sortAscendingIcon = "sprite.svg#SortAscending";

    /// <summary>Filename of the image used to indicate an descending sort order of the column in its title cell.</summary>
    protected const string c_sortDescendingIcon = "sprite.svg#SortDescending";

    /// <summary>Entity definition for whitespace separating controls, e.g. icons from following text</summary>
    protected const string c_whiteSpace = "&nbsp;";

    /// <summary>Name of the JavaScript function to call when a command control has been clicked.</summary>
    protected const string c_onCommandClickScript = "BocList.OnCommandClick();";

    private readonly IResourceUrlFactory _resourceUrlFactory;
    private readonly IRenderingFeatures _renderingFeatures;
    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    protected BocColumnRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
    {
      ArgumentUtility.CheckNotNull("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull("fallbackNavigationUrlProvider", fallbackNavigationUrlProvider);

      _resourceUrlFactory = resourceUrlFactory;
      _renderingFeatures = renderingFeatures;
      _cssClasses = cssClasses;
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    public bool IsNull
    {
      get { return false; }
    }

    public IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }

    /// <summary>
    /// Returns the configured <see cref="IRenderingFeatures"/> object.
    /// </summary>
    protected IRenderingFeatures RenderingFeatures
    {
      get { return _renderingFeatures; }
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary>
    /// Returns whether the renderer is able to render diagnostic metadata.
    /// </summary>
    protected virtual bool HasContentAttribute
    {
      get { return false; }
    }

    void IBocColumnRenderer.RenderTitleCell (BocColumnRenderingContext renderingContext, in BocTitleCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      if (arguments.CellID == null)
        throw new ArgumentException("arguments.CellID is null", "arguments");

      RenderTitleCell(new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), arguments);
    }

    private void RenderTitleCell (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      var bocCellAttributeRenderingContext = new BocCellAttributeRenderingContext<TBocColumnDefinition>(renderingContext);
      AddAttributesToRenderForTitleCell(bocCellAttributeRenderingContext, in arguments);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);

      RenderTitleCellMarkers(renderingContext);
      RenderBeginTagTitleCellSortCommand(renderingContext);
      RenderTitleCellText(renderingContext);
      if (renderingContext.Control.IsClientSideSortingEnabled || renderingContext.Control.HasSortingKeys)
        RenderTitleCellSortingButton(renderingContext, arguments.SortingDirection, arguments.OrderIndex);
      RenderEndTagTitleCellSortCommand(renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    /// <remarks>
    /// This is a template method. Deriving classes can implement <see cref="AddAttributesToRenderForTitleCell"/> to add attributes to
    /// table title cell (&lt;th&gt;) element.
    /// </remarks>
    protected virtual void AddAttributesToRenderForTitleCell (BocCellAttributeRenderingContext<TBocColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      if (arguments.CellID == null)
        throw new ArgumentException("arguments.CellID is null", "arguments");

      renderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Id, arguments.CellID);

      string cssClassTitleCell = CssClasses.TitleCell;
      if (!string.IsNullOrEmpty(renderingContext.ColumnDefinition.CssClass))
        cssClassTitleCell += " " + renderingContext.ColumnDefinition.CssClass;
      renderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Class, cssClassTitleCell);
      renderingContext.AddAttributeToRender(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.ColumnHeader);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var columnItemID = renderingContext.ColumnDefinition.ItemID;
        if (!string.IsNullOrEmpty(columnItemID))
          renderingContext.AddAttributeToRender(DiagnosticMetadataAttributes.ItemID, columnItemID);

        var columnTitle = HtmlUtility.ExtractPlainText(renderingContext.ColumnDefinition.ColumnTitleDisplayValue);
        renderingContext.AddAttributeToRender(DiagnosticMetadataAttributes.Content, columnTitle);

        var oneBasedCellIndex = renderingContext.VisibleColumnIndex + 1;
        renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());

        renderingContext.AddAttributeToRender(
            DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasContentAttribute,
            HasContentAttribute.ToString().ToLower());

        renderingContext.AddAttributeToRender(
            DiagnosticMetadataAttributesForObjectBinding.BocListColumnIsRowHeader,
            arguments.IsRowHeader.ToString().ToLower());
      }
    }

    void IBocColumnRenderer.RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RenderDataColumnDeclaration(new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), isTextXml);
    }

    private void RenderDataColumnDeclaration (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool isTextXml)
    {
      renderingContext.Writer.WriteBeginTag("col");
      if (!renderingContext.ColumnDefinition.Width.IsEmpty)
      {
        renderingContext.Writer.Write(" style=\"");
        string width;
        var columnAsValueColumn = renderingContext.ColumnDefinition as BocValueColumnDefinition;
        if (columnAsValueColumn != null && columnAsValueColumn.EnforceWidth && renderingContext.ColumnDefinition.Width.Type != UnitType.Percentage)
          width = "2em";
        else
          width = renderingContext.ColumnDefinition.Width.ToString();
        renderingContext.Writer.WriteStyleAttribute("width", width);
        renderingContext.Writer.Write("\"");
      }
      if (isTextXml)
        renderingContext.Writer.Write(" />");
      else
        renderingContext.Writer.Write(">");
    }

    private void RenderTitleCellMarkers (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      renderingContext.Control.EditModeController.RenderTitleCellMarkers(
          renderingContext.Writer,
          renderingContext.ColumnDefinition,
          renderingContext.ColumnIndex);
    }

    private void RenderBeginTagTitleCellSortCommand (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      bool hasSortingCommand = renderingContext.Control.IsClientSideSortingEnabled
                               &&
                               (renderingContext.ColumnDefinition is IBocSortableColumnDefinition
                                && ((IBocSortableColumnDefinition)renderingContext.ColumnDefinition).IsSortable);

      if (hasSortingCommand)
      {
        if (!renderingContext.Control.EditModeController.IsRowEditModeActive && !renderingContext.Control.EditModeController.IsListEditModeActive
            && renderingContext.Control.HasClientScript)
        {
          var sortCommandID = renderingContext.Control.ClientID + "_" + renderingContext.ColumnIndex + "_SortCommand";
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, sortCommandID);

          string argument = BocList.SortCommandPrefix + renderingContext.ColumnIndex;
          string postBackEvent = renderingContext.Control.Page!.ClientScript.GetPostBackEventReference(renderingContext.Control, argument);
          postBackEvent += "; return false;";
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, postBackEvent);

          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());

          renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.A);
        }
        else
          renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      }
      else
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
    }

    private void RenderTitleCellText (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      var columnTitle = renderingContext.ColumnDefinition.ColumnTitleDisplayValue;
      if (!renderingContext.ColumnDefinition.ShowColumnTitle)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, _cssClasses.CssClassScreenReaderText);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      columnTitle.WriteTo(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
      if (!renderingContext.ColumnDefinition.ShowColumnTitle)
      {
        // Render a non-breaking space to allow screen readers to highlight a visual cue for the current reading position.
        // For sortable columns, this will push the sorting icon one space to the right, but given that a sortable should 
        // always have a visible column title, this is can be regarded as a non-issue. The alternative of using CSS to
        // generate a non-zero-width element does not help with screenreaders, at least JAWS 2019.
        renderingContext.Writer.Write(c_whiteSpace);
      }
    }

    private void RenderTitleCellSortingButton (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        SortingDirection sortingDirection,
        int orderIndex)
    {
      if (sortingDirection == SortingDirection.None)
        return;

      //  WhiteSpace before icon
      renderingContext.Writer.Write(c_whiteSpace);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.SortingOrder);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      BocList.ResourceIdentifier alternateTextID;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = BocList.ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = BocList.ResourceIdentifier.SortDescendingAlternateText;

      var imageUrl = GetImageUrl(sortingDirection);
      Assertion.IsNotNull(imageUrl);

      var icon = new IconInfo(imageUrl.GetUrl());
      icon.AlternateText = renderingContext.Control.GetResourceManager().GetString(alternateTextID);
      icon.Render(renderingContext.Writer, renderingContext.Control);

      if (renderingContext.Control.IsShowSortingOrderEnabled && orderIndex >= 0)
        renderingContext.Writer.Write(c_whiteSpace + (orderIndex + 1));
      renderingContext.Writer.RenderEndTag();
    }

    private IResourceUrl? GetImageUrl (SortingDirection sortingDirection)
    {
      //  Button Asc -> Button Desc -> No Button
      switch (sortingDirection)
      {
        case SortingDirection.Ascending:
          return ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocColumnRendererBase<>), ResourceType.Image, c_sortAscendingIcon);
        case SortingDirection.Descending:
          return ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocColumnRendererBase<>), ResourceType.Image, c_sortDescendingIcon);
        case SortingDirection.None:
          return null;
        default:
          throw new ArgumentOutOfRangeException("sortingDirection");
      }
    }

    private void RenderEndTagTitleCellSortCommand (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      renderingContext.Writer.RenderEndTag();
    }

    void IBocColumnRenderer.RenderDataCell (BocColumnRenderingContext renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RenderDataCell(new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), arguments);
    }

    /// <summary>
    /// Renders a table cell for <see cref="BocColumnRenderingContext.ColumnDefinition"/> containing the appropriate data from the 
    /// <see cref="IBusinessObject"/> contained in <paramref name="arguments"/>
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="arguments">The cell-specific rendering arguments.</param>
    private void RenderDataCell (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      var bocCellAttributeRenderingContext = new BocCellAttributeRenderingContext<TBocColumnDefinition>(renderingContext);
      AddAttributesToRenderForDataCell(bocCellAttributeRenderingContext, in arguments);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);

      RenderCellContents(renderingContext, arguments);

      renderingContext.Writer.RenderEndTag();
    }

    /// <remarks>
    /// This is a template method. Deriving classes can implement <see cref="AddAttributesToRenderForDataCell"/> to add attributes to
    /// a table cell (&lt;td&gt;) element.
    /// </remarks>
    protected virtual void AddAttributesToRenderForDataCell (BocCellAttributeRenderingContext<TBocColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      string cssClassTableCell = CssClasses.DataCell;

      if (!string.IsNullOrEmpty(renderingContext.ColumnDefinition.CssClass))
        cssClassTableCell += " " + renderingContext.ColumnDefinition.CssClass;

      if (renderingContext is { Control: IBocListWithValidationSupport validatedBocList, ColumnDefinition: IBocColumnDefinitionWithValidationSupport })
      {
        var validationFailures = validatedBocList.ValidationFailureRepository.GetCellValidationFailures(arguments.BusinessObject, renderingContext.ColumnDefinition, false);

        if (validationFailures.Any())
          cssClassTableCell += " " + CssClasses.DataCellHasValidationErrors;
      }

      renderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Class, cssClassTableCell);

      if (arguments.IsRowHeader)
        renderingContext.AddAttributeToRender(HtmlTextWriterAttribute.Id, arguments.CellID);

      // Rendering the header IDs is problematic for split tables and doesn't help with columns to the left of the header column.
      // Therefor, the header IDs are simply not rendered in the first place.
      // renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Headers, string.Join(" ", arguments.HeaderIDs));

      string ariaRoleForTableDataElement;
      if (arguments.IsRowHeader)
      {
        ariaRoleForTableDataElement = HtmlRoleAttributeValue.RowHeader;
      }
      else
      {
#pragma warning disable CS0618 // Type or member is obsolete
        ariaRoleForTableDataElement = GetAriaRoleForTableDataElement();
#pragma warning restore CS0618 // Type or member is obsolete
      }

      renderingContext.AddAttributeToRender(HtmlTextWriterAttribute2.Role, ariaRoleForTableDataElement);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataAttributes(renderingContext);
    }

    [Obsolete(
        "RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
    protected virtual string GetAriaRoleForTableDataElement ()
    {
      return HtmlRoleAttributeValue.Cell;
    }

    /// <summary>
    /// Allows to render additional diagnostic metadata attributes. The base implementation renders the
    /// <see cref="BocColumnRenderingContext.VisibleColumnIndex"/> as <see cref="DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex"/>
    /// attribute.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocCOlumnDefinition}"/>.</param>
    protected virtual void AddDiagnosticMetadataAttributes (BocCellAttributeRenderingContext<TBocColumnDefinition> renderingContext)
    {
      var oneBasedCellIndex = renderingContext.VisibleColumnIndex + 1;
      renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());
    }

    /// <summary>
    /// Renders the contents of the table cell. It is called by <see cref="RenderDataCell"/> and should not be called by other clients.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocCOlumnDefinition}"/>.</param>
    /// <param name="arguments">The cell-specific rendering arguments.</param>
    protected abstract void RenderCellContents (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        in BocDataCellRenderArguments arguments);
  }
}
