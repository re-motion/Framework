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
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the table (consisting of title and data rows) that shows the items contained in the <see cref="IBocList"/>.
  /// </summary>
  [ImplementationFor(typeof(IBocListTableBlockRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocListTableBlockRenderer : IBocListTableBlockRenderer
  {
    private readonly BocListCssClassDefinition _cssClasses;
    private readonly IBocRowRenderer _rowRenderer;
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IValidationErrorRenderer _validationErrorRenderer;

    public BocListTableBlockRenderer (
        BocListCssClassDefinition cssClasses,
        IBocRowRenderer rowRenderer,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
    {
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);
      ArgumentUtility.CheckNotNull("rowRenderer", rowRenderer);
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull("validationErrorRenderer", validationErrorRenderer);

      _cssClasses = cssClasses;
      _rowRenderer = rowRenderer;
      _labelReferenceRenderer = labelReferenceRenderer;
      _validationErrorRenderer = validationErrorRenderer;
    }

    public BocListCssClassDefinition CssClasses
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
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      bool isReadOnly = renderingContext.Control.IsReadOnly;
      bool showForEmptyList = isReadOnly && renderingContext.Control.ShowEmptyListReadOnlyMode
                              || !isReadOnly && renderingContext.Control.ShowEmptyListEditMode;

      if (!renderingContext.Control.HasValue && !showForEmptyList)
        RenderTable(renderingContext, false, false);
      else
        RenderTable(renderingContext, true, true);
    }

    private void RenderTable (BocListRenderingContext renderingContext, bool tableHead, bool tableBody)
    {
      var validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID(renderingContext);

      if (!tableHead && !tableBody)
      {
        RenderEmptyTable(renderingContext, validationErrorsID, validationErrors);
      }
      else
      {
        RenderTableOpeningTag(renderingContext, validationErrorsID, validationErrors);
        RenderTableBlockColumnGroup(renderingContext);

        if (tableHead)
          RenderTableHead(renderingContext);

        if (tableBody)
          RenderTableBody(renderingContext);

        RenderTableClosingTag(renderingContext);
      }

      // Build the actual error messages since we have rendered all the inline error messages
      renderingContext.Control.BuildErrorMessages();
      validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();

      _validationErrorRenderer.RenderValidationErrors(renderingContext.Writer, validationErrorsID, validationErrors);
    }

    private void RenderEmptyTable (
        BocListRenderingContext renderingContext,
        string validationErrorsID,
        IReadOnlyCollection<PlainTextString> validationErrors)
    {
      renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute("tabindex", "0");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Table);

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, labelIDs);

      AddValidationErrorsReference(renderingContext, validationErrorsID, validationErrors);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Table);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Row);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Cell);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);
      renderingContext.Writer.Write("&nbsp;");
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data row of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table head, actual rendering of each row is delegated to
    /// <see cref="BocRowRenderer.RenderTitlesRow"/>.
    /// The rows are nested within a &lt;thead&gt; element.
    /// </remarks>
    /// <seealso cref="BocRowRenderer"/>
    protected virtual void RenderTableHead (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.TableHead);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.RowGroup);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Thead);
      RowRenderer.RenderTitlesRow(renderingContext);
      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// Renders the data rows of the <see cref="BocList"/>.
    /// </summary>
    /// <remarks>
    /// This method provides the outline of the table body, actual rendering of each row is delegated to
    /// <see cref="BocRowRenderer.RenderDataRow"/>.
    /// The rows are nested within a &lt;tbody&gt; element.
    /// </remarks>
    /// <seealso cref="BocRowRenderer"/>
    protected virtual void RenderTableBody (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.TableBody);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.RowGroup);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

      if (!renderingContext.Control.HasValue && renderingContext.Control.ShowEmptyListMessage)
      {
        RowRenderer.RenderEmptyListDataRow(renderingContext);
      }
      else
      {
        var rowRenderingContexts = renderingContext.Control.GetRowsToRender();

        var validationFailureRepository = renderingContext.Control.ValidationFailureRepository;
        var columnDefinitionsWithValidationFailures = rowRenderingContexts
            .SelectMany(e => validationFailureRepository.GetUnhandledValidationFailuresForDataRowAndContainingDataCells(e.Row.BusinessObject, false))
            .Where(e => e.ColumnDefinition != null)
            .Select(e => e.ColumnDefinition)
            .ToHashSet();

        var columnsWithValidationFailures = new bool[renderingContext.ColumnRenderers.Length];
        for (var i = 0; i < columnsWithValidationFailures.Length; i++)
          columnsWithValidationFailures[i] = columnDefinitionsWithValidationFailures.Contains(renderingContext.ColumnRenderers[i].ColumnDefinition);

        for (int index = 0; index < rowRenderingContexts.Length; index++)
        {
          var arguments = new BocRowRenderArguments(index, columnsWithValidationFailures);
          RowRenderer.RenderDataRow(renderingContext, rowRenderingContexts[index], in arguments);
        }
      }

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary> Renderes the opening tag of the table. </summary>
    private void RenderTableOpeningTag (
        BocListRenderingContext renderingContext,
        string validationErrorsID,
        IReadOnlyCollection<PlainTextString> validationErrors)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.TableContainer);
#pragma warning disable CS0618 // Type or member is obsolete
      var ariaRoleForTableElement = GetAriaRoleForTableElement();
#pragma warning restore CS0618 // Type or member is obsolete
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, ariaRoleForTableElement);
      renderingContext.Writer.AddAttribute("tabindex", "0");

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, labelIDs);

      AddValidationErrorsReference(renderingContext, validationErrorsID, validationErrors);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.TableScrollContainer);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_TableScrollContainer");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.Table);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Table);
    }

    [Obsolete(
        "RM-7053: Only intended for ARIA-role workaround. May be removed in future releases without warning once there is infrastructure option for specifying the table type.")]
    protected virtual string GetAriaRoleForTableElement ()
    {
      return HtmlRoleAttributeValue.Table;
    }

    /// <summary> Renderes the closing tag of the table. </summary>
    private void RenderTableClosingTag (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderEndTag(); // table

      renderingContext.Writer.RenderEndTag(); // div TableScrollContainer

      renderingContext.Writer.RenderEndTag(); // div TableContainer
    }

    /// <summary> Renders the column group, which provides the table's column layout. </summary>
    private void RenderTableBlockColumnGroup (BocListRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Colgroup);

      var isTextXml = ControlHelper.IsXmlConformResponseTextRequired(renderingContext.HttpContext);

      RenderIndexColumnDeclaration(renderingContext, isTextXml);
      RenderSelectorColumnDeclaration(renderingContext, isTextXml);

      foreach (var columnRenderer in renderingContext.ColumnRenderers)
        columnRenderer.RenderDataColumnDeclaration(renderingContext, isTextXml);

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>Renders the col element for the selector column</summary>
    private void RenderSelectorColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsSelectionEnabled)
      {
        renderingContext.Writer.WriteBeginTag("col");
        renderingContext.Writer.Write(" style=\"");
        renderingContext.Writer.WriteStyleAttribute("width", "1.6em");
        renderingContext.Writer.Write("\"");
        if (isTextXml)
          renderingContext.Writer.Write(" />");
        else
          renderingContext.Writer.Write(">");
      }
    }

    /// <summary>Renders the col element for the index column</summary>
    private void RenderIndexColumnDeclaration (BocListRenderingContext renderingContext, bool isTextXml)
    {
      if (renderingContext.Control.IsIndexEnabled)
      {
        renderingContext.Writer.WriteBeginTag("col");
        renderingContext.Writer.Write(" style=\"");
        renderingContext.Writer.WriteStyleAttribute("width", "1.6em");
        renderingContext.Writer.Write("\"");
        if (isTextXml)
          renderingContext.Writer.Write(" />");
        else
          renderingContext.Writer.Write(">");
      }
    }

    private IEnumerable<PlainTextString> GetValidationErrorsToRender (BocRenderingContext<IBocList> renderingContext)
    {
      return renderingContext.Control.GetValidationErrors();
    }

    private string GetValidationErrorsID (BocRenderingContext<IBocList> renderingContext)
    {
      return renderingContext.Control.ClientID + "_ValidationErrors";
    }

    private void AddValidationErrorsReference (BocListRenderingContext renderingContext, string validationErrorsID, IReadOnlyCollection<PlainTextString> validationErrors)
    {
      var attributeCollection = new AttributeCollection(new StateBag());
      _validationErrorRenderer.AddValidationErrorsReference(attributeCollection, validationErrorsID, validationErrors);
      attributeCollection.AddAttributes(renderingContext.Writer);
    }
  }
}
