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
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Abstract base class for column renderers that handle classes derived from <see cref="BocValueColumnDefinition"/>.
  /// Defines <see cref="RenderCellContents"/> as template method and common utility methods.
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class that the derived class can handle.</typeparam>
  public abstract class BocValueColumnRendererBase<TBocColumnDefinition> : BocCommandEnabledColumnRendererBase<TBocColumnDefinition>
      where TBocColumnDefinition: BocValueColumnDefinition
  {
    protected BocValueColumnRendererBase (IResourceUrlFactory resourceUrlFactory, IRenderingFeatures renderingFeatures, BocListCssClassDefinition cssClasses, IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
    }

    /// <summary>
    /// Renders a table cell for a <see cref="BocValueColumnDefinition"/>.
    /// </summary>
    protected override void RenderCellContents (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      int originalRowIndex = arguments.ListIndex;
      IBusinessObject businessObject = arguments.BusinessObject;

      IEditableRow? editableRow = renderingContext.Control.EditModeController.GetEditableRow(originalRowIndex);

      bool hasEditModeControl = editableRow != null && editableRow.HasEditControl(renderingContext.ColumnIndex);
      bool showEditModeControl = hasEditModeControl && !editableRow!.GetEditControl(renderingContext.ColumnIndex)!.IsReadOnly;

      PlainTextString valueColumnText = PlainTextString.Empty;
      if (!showEditModeControl)
        valueColumnText = PlainTextString.CreateFromText(renderingContext.ColumnDefinition.GetStringValue(businessObject));

      bool enforceWidth = RenderCropSpanBeginTag(renderingContext, showEditModeControl, valueColumnText);
      bool isCommandEnabled = RenderBeginTag(renderingContext, originalRowIndex, businessObject, valueColumnText);

      if (!hasEditModeControl)
      {
        if (arguments.ShowIcon)
          RenderCellIcon(renderingContext, businessObject);

        RenderOtherIcons(renderingContext, businessObject);
      }
      if (showEditModeControl)
        RenderCellDataForEditMode(renderingContext, arguments, editableRow!);
      else
        RenderValueColumnCellText(renderingContext, valueColumnText!);

      RenderEndTag(renderingContext, isCommandEnabled);
      RenderCropSpanEndTag(renderingContext, enforceWidth);
    }

    protected abstract void RenderCellDataForEditMode (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments, IEditableRow editableRow);

    /// <summary>
    /// Used by <see cref="RenderCellContents"/> to render icons in addition to the <paramref name="businessObject"/>'s icon.
    /// Deriving classes should override this empty implementation if they wish to add other icons.
    /// Should not be used by other clients.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">Can be used to derive the icon to render.</param>
    protected virtual void RenderOtherIcons (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, IBusinessObject businessObject)
    {
    }

    protected void RenderValueColumnCellText (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, PlainTextString contents)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute("class", CssClasses.CommandText);
      if (RenderingFeatures.EnableDiagnosticMetadata)
        contents.AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributesForObjectBinding.BocListCellContents);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (contents.IsEmpty)
        renderingContext.Writer.Write(c_whiteSpace);
      else
        contents.WriteTo(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// If the column width must be enforced, this method renders a &lt;span&gt; block container that crops any overflowing content.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="showEditModeControl">Specifies if the cell contains edit mode controls.</param>
    /// <param name="spanTitle">Specifies the text to be written to the 'title' attribute.</param>
    /// <returns><see langword="true"/> if the crop span begin tag has been rendered, <see langword="false"/> otherwise.</returns>
    private bool RenderCropSpanBeginTag (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool showEditModeControl, PlainTextString spanTitle)
    {
      bool enforceWidth =
          renderingContext.ColumnDefinition.EnforceWidth
          && !renderingContext.ColumnDefinition.Width.IsEmpty
          && renderingContext.ColumnDefinition.Width.Type != UnitType.Percentage
          && !showEditModeControl;

      if (enforceWidth)
      {
        renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, renderingContext.ColumnDefinition.Width.ToString());
        renderingContext.Writer.AddStyleAttribute("overflow", "hidden");
        renderingContext.Writer.AddStyleAttribute("white-space", "nowrap");
        renderingContext.Writer.AddStyleAttribute("display", "block");
        spanTitle.AddAttributeTo(renderingContext.Writer, HtmlTextWriterAttribute.Title);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      }
      return enforceWidth;
    }

    /// <summary>
    /// Renders the end tag to the crop span element if a begin tag has been rendered.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="enforceWidth">Specifies if a corresponding begin tag has been rendered.</param>
    private void RenderCropSpanEndTag (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool enforceWidth)
    {
      if (enforceWidth)
        renderingContext.Writer.RenderEndTag();
    }


    private bool RenderBeginTag (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext, int originalRowIndex, IBusinessObject businessObject, PlainTextString valueColumnText)
    {
      bool isCommandEnabled = false;
      if (!valueColumnText.IsEmpty)
        isCommandEnabled = RenderBeginTagDataCellCommand(renderingContext, businessObject, originalRowIndex);
      if (!isCommandEnabled)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.Content);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      }
      return isCommandEnabled;
    }

    private void RenderEndTag (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, bool isCommandEnabled)
    {
      if (isCommandEnabled)
        RenderEndTagDataCellCommand(renderingContext);
      else
        renderingContext.Writer.RenderEndTag();
    }

    /// <summary>
    /// <see cref="BocValueColumnRendererBase{TBocColumnDefinition}"/> renders diagnostic metadata.
    /// </summary>
    protected override bool HasContentAttribute
    {
      get { return true; }
    }
  }
}
