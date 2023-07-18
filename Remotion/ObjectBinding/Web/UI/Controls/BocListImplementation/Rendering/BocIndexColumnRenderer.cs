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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the index column of a <see cref="IBocList"/>.
  /// </summary>
  [ImplementationFor(typeof(IBocIndexColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocIndexColumnRenderer : IBocIndexColumnRenderer
  {
    private readonly IRenderingFeatures _renderingFeatures;
    private readonly BocListCssClassDefinition _cssClasses;

    public BocIndexColumnRenderer (IRenderingFeatures renderingFeatures, BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);

      _renderingFeatures = renderingFeatures;
      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    public void RenderDataCell (BocListRenderingContext renderingContext, int originalRowIndex, int absoluteRowIndex, IReadOnlyCollection<string> headerIDs)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("headerIDs", headerIDs);

      if (!renderingContext.Control.IsIndexEnabled)
        return;

      string selectorControlID = renderingContext.Control.GetSelectorControlName().Replace('$', '_') + "_" + originalRowIndex;
      string cssClass = CssClasses.DataCell + " " + CssClasses.DataCellIndex;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
#pragma warning disable CS0618 // Type or member is obsolete
      var ariaRoleForTableDataElement = GetAriaRoleForTableDataElement();
#pragma warning restore CS0618 // Type or member is obsolete
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, ariaRoleForTableDataElement);
      if (_renderingFeatures.EnableDiagnosticMetadata)
        AddDiagnosticMetadataListCellIndex(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
      if (renderingContext.Control.Index == RowIndex.InitialOrder)
        RenderRowIndex(renderingContext, originalRowIndex, selectorControlID);
      else if (renderingContext.Control.Index == RowIndex.SortedOrder)
        RenderRowIndex(renderingContext, absoluteRowIndex, selectorControlID);
      renderingContext.Writer.RenderEndTag();
    }

    [Obsolete(
        "RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
    protected virtual string GetAriaRoleForTableDataElement ()
    {
      return HtmlRoleAttributeValue.Cell;
    }

    private static void AddDiagnosticMetadataListCellIndex (BocListRenderingContext renderingContext)
    {
      const int oneBasedCellIndex = 1;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListCellIndex, oneBasedCellIndex.ToString());
    }

    public void RenderTitleCell (BocListRenderingContext renderingContext, string cellID)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNullOrEmpty("cellID", cellID);

      if (!renderingContext.Control.IsIndexEnabled)
        return;

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, cellID);
      string cssClass = CssClasses.TitleCell + " " + CssClasses.TitleCellIndex;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.ColumnHeader);
      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var columnTitle = renderingContext.Control.IndexColumnTitle;
        if (!columnTitle.IsEmpty)
          HtmlUtility.ExtractPlainText(columnTitle).AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributes.Content);

        AddDiagnosticMetadataListCellIndex(renderingContext);
      }
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Th);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      WebString indexColumnTitle = renderingContext.Control.IndexColumnTitle;
      if (indexColumnTitle.IsEmpty)
        indexColumnTitle = renderingContext.Control.GetResourceManager().GetText(BocList.ResourceIdentifier.IndexColumnTitle);

      indexColumnTitle.WriteTo(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary> Renders the zero-based row index normalized to a one-based format. </summary>
    private void RenderRowIndex (BocListRenderingContext renderingContext, int index, string selectorControlID)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.CellStructureElement);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.Content);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
      int renderedIndex = index + 1;
      if (renderingContext.Control.IndexOffset != null)
        renderedIndex += renderingContext.Control.IndexOffset.Value;
      renderingContext.Writer.Write(renderedIndex);
      renderingContext.Writer.RenderEndTag(); // div.Content
      renderingContext.Writer.RenderEndTag(); // div.CellStructureElement
    }
  }
}
