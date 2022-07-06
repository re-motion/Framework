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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
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

    protected BocColumnRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);

      _resourceUrlFactory = resourceUrlFactory;
      _renderingFeatures = renderingFeatures;
      _cssClasses = cssClasses;
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

    void IBocColumnRenderer.RenderTitleCell (BocColumnRenderingContext renderingContext, SortingDirection sortingDirection, int orderIndex)
    {
      RenderTitleCell(
          new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext),
          sortingDirection,
          orderIndex);
    }

    protected virtual void RenderTitleCell (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        SortingDirection sortingDirection,
        int orderIndex)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      string cssClassTitleCell = CssClasses.TitleCell;
      if (!string.IsNullOrEmpty(renderingContext.ColumnDefinition.CssClass))
        cssClassTitleCell += " " + renderingContext.ColumnDefinition.CssClass;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClassTitleCell);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.ColumnHeader);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var columnItemID = renderingContext.ColumnDefinition.ItemID;
        if (!string.IsNullOrEmpty(columnItemID))
          renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.ItemID, columnItemID);

        var columnTitle = renderingContext.ColumnDefinition.ColumnTitleDisplayValue;
        HtmlUtility.ExtractPlainText(columnTitle).AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributes.Content);

        var oneBasedCellIndex = renderingContext.VisibleColumnIndex + 1;
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());

        renderingContext.Writer.AddAttribute(
            DiagnosticMetadataAttributesForObjectBinding.BocListColumnHasContentAttribute,
            HasContentAttribute.ToString().ToLower());
      }
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);

      RenderTitleCellMarkers(renderingContext);
      RenderBeginTagTitleCellSortCommand(renderingContext);
      RenderTitleCellText(renderingContext);
      if (renderingContext.Control.IsClientSideSortingEnabled || renderingContext.Control.HasSortingKeys)
        RenderTitleCellSortingButton(renderingContext, sortingDirection, orderIndex);
      RenderEndTagTitleCellSortCommand(renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    void IBocColumnRenderer.RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml)
    {
      RenderDataColumnDeclaration(new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), isTextXml);
    }

    protected virtual void RenderDataColumnDeclaration (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool isTextXml)
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

    protected string GetColumnTitleID (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      return string.Format("{0}_{1}_Title", renderingContext.Control.ClientID, renderingContext.ColumnIndex);
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

          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");

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
      var columnTitleID = GetColumnTitleID(renderingContext);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, columnTitleID);
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
      RenderDataCell(new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), arguments);
    }

    /// <summary>
    /// Renders a table cell for <see cref="BocColumnRenderingContext.ColumnDefinition"/> containing the appropriate data from the 
    /// <see cref="IBusinessObject"/> contained in <paramref name="arguments"/>
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="arguments">The cell-specific rendering arguments.</param>
    /// <remarks>
    /// This is a template method. Deriving classes must implement <see cref="RenderCellContents"/> to provide the contents of
    /// the table cell (&lt;td&gt;) element.
    /// </remarks>
    protected virtual void RenderDataCell (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      string cssClassTableCell = CssClasses.DataCell;

      if (!string.IsNullOrEmpty(renderingContext.ColumnDefinition.CssClass))
        cssClassTableCell += " " + renderingContext.ColumnDefinition.CssClass;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClassTableCell);
#pragma warning disable CS0618 // Type or member is obsolete
      var ariaRoleForTableDataElement = GetAriaRoleForTableDataElement();
#pragma warning restore CS0618 // Type or member is obsolete
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, ariaRoleForTableDataElement);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataAttributes(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);

      RenderCellContents(renderingContext, arguments);

      renderingContext.Writer.RenderEndTag();
    }

    [Obsolete("RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
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
    protected virtual void AddDiagnosticMetadataAttributes (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      var oneBasedCellIndex = renderingContext.VisibleColumnIndex + 1;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());
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
