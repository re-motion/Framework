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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the table (consisting of title and data rows) that shows the items contained in the <see cref="IBocList"/>.
  /// </summary>
  public class BocListTableBlockQuirksModeRenderer : IBocListTableBlockRenderer
  {
    private readonly BocListQuirksModeCssClassDefinition _cssClasses;
    private readonly IBocRowRenderer _rowRenderer;
    
    public BocListTableBlockQuirksModeRenderer (BocListQuirksModeCssClassDefinition cssClasses, IBocRowRenderer rowRenderer)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull ("rowRenderer", rowRenderer);

      _cssClasses = cssClasses;
      _rowRenderer = rowRenderer;
    }

    public BocListQuirksModeCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public IBocRowRenderer RowRenderer
    {
      get { return _rowRenderer; }
    }

    /// <summary>
    /// Renders the data contained in <see cref="IBocList"/> as a table.
    /// </summary>
    /// <remarks>
    /// The table consists of a title row showing the column titles, and a data row for each <see cref="IBusinessObject"/>
    /// in <see cref="IBocList"/>. If there is no data, the table will be completely hidden (only one cell containing only whitespace)
    /// if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListEditMode"/> is <see langword="false"/> and 
    /// <see cref="IBocList"/> is editable
    /// or if <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ShowEmptyListReadOnlyMode"/> is <see langword="false"/> and 
    /// <see cref="IBocList"/> is read-only.
    /// Exception: at design time, the title row will always be visible.
    /// </remarks>
    /// <seealso cref="RenderTableBlockColumnGroup"/>
    /// <seealso cref="RenderTableBody"/>
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      bool isDesignMode = ControlHelper.IsDesignMode (renderingContext.Control);
      bool isReadOnly = renderingContext.Control.IsReadOnly;
      bool showForEmptyList = isReadOnly && renderingContext.Control.ShowEmptyListReadOnlyMode
                              || !isReadOnly && renderingContext.Control.ShowEmptyListEditMode;

      if (!renderingContext.Control.HasValue && !showForEmptyList)
        RenderTable (renderingContext, isDesignMode, false);
      else
        RenderTable (renderingContext, true, true);
    }

    private void RenderTable (BocListRenderingContext renderingContext, bool tableHead, bool tableBody)
    {
      if (!tableHead && !tableBody)
      {
        RenderEmptyTable (renderingContext);
        return;
      }

      RenderTableOpeningTag (renderingContext);
      RenderTableBlockColumnGroup (renderingContext);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableHead);

      if (tableHead)
        RenderTableHead (renderingContext);

      if (tableBody)
        RenderTableBody (renderingContext);

      RenderTableClosingTag (renderingContext);
    }

    private void RenderEmptyTable (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      renderingContext.Writer.Write ("&nbsp;");
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data row of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table head, actual rendering of each row is delegated to
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer.RenderTitlesRow"/>.
    /// The rows are nested within a &lt;thead&gt; element.
    /// </remarks>
    /// <seealso cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer"/>
    protected virtual void RenderTableHead (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Thead);
      RowRenderer.RenderTitlesRow (renderingContext);
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data rows of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table body, actual rendering of each row is delegated to
    /// <see cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer.RenderDataRow"/>.
    /// The rows are nested within a &lt;tbody&gt; element.
    /// </remarks>
    /// <seealso cref="Web.UI.Controls.BocListImplementation.Rendering.BocRowRenderer"/>
    protected virtual void RenderTableBody (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.TableBody);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tbody);

      if (!renderingContext.Control.HasValue && renderingContext.Control.ShowEmptyListMessage)
      {
        RowRenderer.RenderEmptyListDataRow (renderingContext);
      }
      else
      {
        var rowRenderingContexts = renderingContext.Control.GetRowsToRender();
        for (int index = 0; index < rowRenderingContexts.Length; index++)
        {
          RowRenderer.RenderDataRow (renderingContext, rowRenderingContexts[index], index);
        }
      }

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary> Renderes the opening tag of the table. </summary>
    private void RenderTableOpeningTag (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_Table");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Table);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);
    }

    /// <summary> Renderes the closing tag of the table. </summary>
    private void RenderTableClosingTag (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderEndTag(); // table
      renderingContext.Writer.RenderEndTag(); // div
    }

    /// <summary> Renders the column group, which provides the table's column layout. </summary>
    private void RenderTableBlockColumnGroup (BocListRenderingContext renderingContext)
    {
      BocColumnRenderer[] columnRenderers = renderingContext.ColumnRenderers;

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Colgroup);

      bool isTextXml = false;

      if (!renderingContext.Control.IsDesignMode)
        isTextXml = ControlHelper.IsXmlConformResponseTextRequired (renderingContext.HttpContext);

      RenderIndexColumnDeclaration (renderingContext, isTextXml);
      RenderSelectorColumnDeclaration (renderingContext, isTextXml);

      foreach (var renderer in columnRenderers)
        renderer.RenderDataColumnDeclaration (renderingContext, isTextXml);
      
      //  Design-mode and empty table
      if (ControlHelper.IsDesignMode (renderingContext.Control) && columnRenderers.Length == 0)
      {
        for (int i = 0; i < BocRowQuirksModeRenderer.DesignModeDummyColumnCount; i++)
        {
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Col);
          renderingContext.Writer.RenderEndTag();
        }
      }

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>Renders the col element for the selector column</summary>
    private void RenderSelectorColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsSelectionEnabled)
      {
        renderingContext.Writer.WriteBeginTag ("col");
        renderingContext.Writer.Write (" style=\"");
        renderingContext.Writer.WriteStyleAttribute ("width", "1.6em");
        renderingContext.Writer.Write ("\"");
        if (isTextXml)
          renderingContext.Writer.Write (" />");
        else
          renderingContext.Writer.Write (">");
      }
    }

    /// <summary>Renders the col element for the index column</summary>
    private void RenderIndexColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsIndexEnabled)
      {
        renderingContext.Writer.WriteBeginTag ("col");
        renderingContext.Writer.Write (" style=\"");
        renderingContext.Writer.WriteStyleAttribute ("width", "1.6em");
        renderingContext.Writer.Write ("\"");
        if (isTextXml)
          renderingContext.Writer.Write (" />");
        else
          renderingContext.Writer.Write (">");
      }
    }
  }
}