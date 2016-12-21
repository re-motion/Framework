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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Abstract base class for all column renderers. Provides a template for rendering a table cell from an <see cref="IBusinessObject"/>
  /// and a <see cref="BocColumnDefinition"/>. 
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocColumnQuirksModeRendererBase<TBocColumnDefinition> : IBocColumnRenderer
      where TBocColumnDefinition: BocColumnDefinition
  {
    /// <summary>Filename of the image used to indicate an ascending sort order of the column in its title cell.</summary>
    protected const string c_sortAscendingIcon = "SortAscending.gif";

    /// <summary>Filename of the image used to indicate an descending sort order of the column in its title cell.</summary>
    protected const string c_sortDescendingIcon = "SortDescending.gif";

    private const string c_designModeEmptyContents = "#";

    /// <summary>Entity definition for whitespace separating controls, e.g. icons from following text</summary>
    protected const string c_whiteSpace = "&nbsp;";

    /// <summary>Name of the JavaScript function to call when a command control has been clicked.</summary>
    protected const string c_onCommandClickScript = "BocList_OnCommandClick();";

    private readonly BocListQuirksModeCssClassDefinition _cssClasses;
    private readonly IResourceUrlFactory _resourceUrlFactory;

    /// <summary>
    /// Constructs the renderer.
    /// </summary>
    /// <param name="resourceUrlFactory"></param>
    /// <param name="cssClasses">The <see cref="BocListQuirksModeCssClassDefinition"/> containing the CSS classes to apply to the rendered elements.</param>
    protected BocColumnQuirksModeRendererBase (IResourceUrlFactory resourceUrlFactory, BocListQuirksModeCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
      _resourceUrlFactory = resourceUrlFactory;
    }

    public bool IsNull
    {
      get { return false; }
    }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }

    void IBocColumnRenderer.RenderTitleCell (BocColumnRenderingContext renderingContext, SortingDirection sortingDirection, int orderIndex)
    {
      RenderTitleCell (new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), sortingDirection, orderIndex);
    }

    public virtual void RenderTitleCell (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext, SortingDirection sortingDirection, int orderIndex)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      string cssClassTitleCell = CssClasses.TitleCell;
      if (!string.IsNullOrEmpty (renderingContext.ColumnDefinition.CssClass))
        cssClassTitleCell += " " + renderingContext.ColumnDefinition.CssClass;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTitleCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Th);

      RenderTitleCellMarkers (renderingContext);
      RenderBeginTagTitleCellSortCommand (renderingContext);
      RenderTitleCellText (renderingContext);
      if (renderingContext.Control.IsClientSideSortingEnabled || renderingContext.Control.HasSortingKeys)
        RenderTitleCellSortingButton (renderingContext, sortingDirection, orderIndex);
      RenderEndTagTitleCellSortCommand (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderTitleCellMarkers (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      renderingContext.Control.EditModeController.RenderTitleCellMarkers (
          renderingContext.Writer, renderingContext.ColumnDefinition, renderingContext.ColumnIndex);
    }

    private void RenderBeginTagTitleCellSortCommand (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      bool hasSortingCommand = renderingContext.Control.IsClientSideSortingEnabled
                               &&
                               (renderingContext.ColumnDefinition is IBocSortableColumnDefinition
                                && ((IBocSortableColumnDefinition) renderingContext.ColumnDefinition).IsSortable);

      if (hasSortingCommand)
      {
        if (!renderingContext.Control.EditModeController.IsRowEditModeActive && !renderingContext.Control.EditModeController.IsListEditModeActive
            && renderingContext.Control.HasClientScript)
        {
          var sortCommandID = renderingContext.Control.ClientID + "_" + renderingContext.ColumnIndex + "_SortCommand";
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, sortCommandID);

          string argument = BocList.SortCommandPrefix + renderingContext.ColumnIndex;
          string postBackEvent = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument);
          postBackEvent += "; return false;";
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);

          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");

          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);
        }
        else
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      else
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
    }

    private void RenderTitleCellText (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      if (renderingContext.Control.IsDesignMode && string.IsNullOrEmpty (renderingContext.ColumnDefinition.ColumnTitleDisplayValue))
        renderingContext.Writer.Write (c_designModeEmptyContents);
      else
      {
        string contents = StringUtility.EmptyToNull (renderingContext.ColumnDefinition.ColumnTitleDisplayValue) ?? c_whiteSpace;
        renderingContext.Writer.Write (contents);
      }
    }

    private void RenderTitleCellSortingButton (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext, SortingDirection sortingDirection, int orderIndex)
    {
      string imageUrl = GetImageUrl (renderingContext, sortingDirection);

      if (sortingDirection == SortingDirection.None)
        return;

      //  WhiteSpace before icon
      renderingContext.Writer.Write (c_whiteSpace);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.SortingOrder);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      BocList.ResourceIdentifier alternateTextID;
      if (sortingDirection == SortingDirection.Ascending)
        alternateTextID = BocList.ResourceIdentifier.SortAscendingAlternateText;
      else
        alternateTextID = BocList.ResourceIdentifier.SortDescendingAlternateText;

      var icon = new IconInfo (imageUrl);
      icon.AlternateText = renderingContext.Control.GetResourceManager().GetString (alternateTextID);
      icon.Render (renderingContext.Writer, renderingContext.Control);

      if (renderingContext.Control.IsShowSortingOrderEnabled && orderIndex >= 0)
        renderingContext.Writer.Write (c_whiteSpace + (orderIndex + 1));
      renderingContext.Writer.RenderEndTag();
    }

    private string GetImageUrl (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, SortingDirection sortingDirection)
    {
      string imageUrl = string.Empty;
      //  Button Asc -> Button Desc -> No Button
      switch (sortingDirection)
      {
        case SortingDirection.Ascending:
        {
          imageUrl = ResourceUrlFactory.CreateResourceUrl (
              typeof (BocColumnQuirksModeRendererBase<>),
              ResourceType.Image,
              c_sortAscendingIcon).GetUrl();
          break;
        }
        case SortingDirection.Descending:
        {
          imageUrl = ResourceUrlFactory.CreateResourceUrl (
              typeof (BocColumnQuirksModeRendererBase<>),
              ResourceType.Image,
              c_sortDescendingIcon).GetUrl();
          break;
        }
        case SortingDirection.None:
        {
          break;
        }
      }
      return imageUrl;
    }

    private void RenderEndTagTitleCellSortCommand (HtmlTextWriter writer)
    {
      writer.RenderEndTag();
    }

    void IBocColumnRenderer.RenderDataCell (
        BocColumnRenderingContext renderingContext,
        int rowIndex,
        bool showIcon,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      RenderDataCell (
          new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext),
          rowIndex,
          showIcon,
          dataRowRenderEventArgs);
    }

    /// <summary>
    /// Renders a table cell for <see cref="BocColumnRenderingContext.ColumnDefinition"/> containing the appropriate data from the 
    /// <see cref="IBusinessObject"/> contained in <paramref name="dataRowRenderEventArgs"/>
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{TBocColumnDefinition}"/>.</param>
    /// <param name="rowIndex">The zero-based index of the row on the page to be displayed.</param>
    /// <param name="showIcon">Specifies if an object-specific icon will be rendered in the table cell.</param>
    /// <param name="dataRowRenderEventArgs">Specifies row-specific arguments used in rendering the table cell.</param>
    /// <remarks>
    /// This is a template method. Deriving classes must implement <see cref="RenderCellContents"/> to provide the contents of
    /// the table cell (&lt;td&gt;) element.
    /// </remarks>
    public virtual void RenderDataCell (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        int rowIndex,
        bool showIcon,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      string cssClassTableCell = CssClasses.GetDataCell (dataRowRenderEventArgs.IsOddRow);

      if (!string.IsNullOrEmpty (renderingContext.ColumnDefinition.CssClass))
        cssClassTableCell += " " + renderingContext.ColumnDefinition.CssClass;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClassTableCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);

      RenderCellContents (renderingContext, dataRowRenderEventArgs, rowIndex, showIcon);

      renderingContext.Writer.RenderEndTag();
    }

    void IBocColumnRenderer.RenderDataColumnDeclaration (BocColumnRenderingContext renderingContext, bool isTextXml)
    {
      RenderDataColumnDeclaration (new BocColumnRenderingContext<TBocColumnDefinition>(renderingContext), isTextXml);
    }

    public virtual void RenderDataColumnDeclaration (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool isTextXml)
    {
      renderingContext.Writer.WriteBeginTag ("col");
      if (!renderingContext.ColumnDefinition.Width.IsEmpty)
      {
        renderingContext.Writer.Write (" style=\"");
        string width;
        var columnAsValueColumn = renderingContext.ColumnDefinition as BocValueColumnDefinition;
        if (columnAsValueColumn != null && columnAsValueColumn.EnforceWidth && renderingContext.ColumnDefinition.Width.Type != UnitType.Percentage)
          width = "2em";
        else
          width = renderingContext.ColumnDefinition.Width.ToString();
        renderingContext.Writer.WriteStyleAttribute ("width", width);
        renderingContext.Writer.Write ("\"");
      }
      if (isTextXml)
        renderingContext.Writer.Write (" />");
      else
        renderingContext.Writer.Write (">");
    }

    /// <summary>
    /// Renders the contents of the table cell. It is called by <see cref="RenderDataCell"/> and should not be called by other clients.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="dataRowRenderEventArgs">The row-specific rendering arguments.</param>
    /// <param name="rowIndex">The zero-based index of the row to render in <see cref="IBocList"/>.</param>
    /// <param name="showIcon">Specifies if the cell should contain an icon of the current <see cref="IBusinessObject"/>.</param>
    protected abstract void RenderCellContents (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs,
        int rowIndex,
        bool showIcon);
    
  }
}