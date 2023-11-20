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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocRowEditModeColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor(typeof(IBocRowEditModeColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocRowEditModeColumnRenderer : BocColumnRendererBase<BocRowEditModeColumnDefinition>, IBocRowEditModeColumnRenderer
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocRowEditModeColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocRowEditModeColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
    }

    protected override string GetAdditionalCssClassForTitleCell (BocCellAttributeRenderingContext<BocRowEditModeColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      return CssClasses.TitleCellEditModeButtons;
    }

    protected override string GetAdditionalCssClassForDataCell (BocCellAttributeRenderingContext<BocRowEditModeColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      return CssClasses.DataCellEditModeButtons;
    }

    /// <summary>
    /// Renders the cell contents depending on the <paramref name="arguments"/>'s
    /// <see cref="BocListDataRowRenderEventArgs.IsEditableRow"/> property.
    /// <seealso cref="BocColumnRendererBase{TBocColumnDefinition}.RenderCellContents"/>
    /// </summary>
    /// <remarks>
    /// If the current row is being edited, "Save" and "Cancel" controls are rendered; if the row can be edited, an "Edit" control is rendered;
    /// if the row cannot be edited, an empty cell is rendered.
    /// Since the "Save", "Cancel" and "Edit" controls are structurally identical, their actual rendering is done by <see cref="RenderCommandControl"/>
    /// </remarks>
    protected override void RenderCellContents (BocColumnRenderingContext<BocRowEditModeColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      int originalRowIndex = arguments.ListIndex;
      var businessObject = arguments.BusinessObject;
      bool isEditableRow = arguments.IsEditableRow;
      bool isEditedRow = renderingContext.Control.EditModeController.GetEditableRow(originalRowIndex) != null;

      if (isEditedRow)
        RenderEditedRowCellContents(renderingContext, originalRowIndex, businessObject);
      else if (isEditableRow)
        RenderEditableRowCellContents(renderingContext, originalRowIndex, businessObject);
      else
        renderingContext.Writer.Write(c_whiteSpace);
    }

    protected override void AddDiagnosticMetadataAttributes (BocCellAttributeRenderingContext<BocRowEditModeColumnDefinition> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownEditCell, "true");
    }

    private void RenderEditableRowCellContents (
        BocColumnRenderingContext<BocRowEditModeColumnDefinition> renderingContext,
        int originalRowIndex,
        IBusinessObject businessObject)
    {
      RenderCommandControl(
          renderingContext,
          originalRowIndex,
          businessObject,
          BocList.RowEditModeCommand.Edit,
          BocList.ResourceIdentifier.RowEditModeEditAlternateText,
          renderingContext.ColumnDefinition.EditIcon,
          renderingContext.ColumnDefinition.EditText);
    }

    private void RenderEditedRowCellContents (
        BocColumnRenderingContext<BocRowEditModeColumnDefinition> renderingContext,
        int originalRowIndex,
        IBusinessObject businessObject)
    {
      RenderCommandControl(
          renderingContext,
          originalRowIndex,
          businessObject,
          BocList.RowEditModeCommand.Save,
          BocList.ResourceIdentifier.RowEditModeSaveAlternateText,
          renderingContext.ColumnDefinition.SaveIcon,
          renderingContext.ColumnDefinition.SaveText);

      renderingContext.Writer.Write(" ");

      RenderCommandControl(
          renderingContext,
          originalRowIndex,
          businessObject,
          BocList.RowEditModeCommand.Cancel,
          BocList.ResourceIdentifier.RowEditModeCancelAlternateText,
          renderingContext.ColumnDefinition.CancelIcon,
          renderingContext.ColumnDefinition.CancelText);
    }

    /// <summary>
    /// Renders a command control as link with an icon, a text or both.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="originalRowIndex">The zero-based index of the current row in <see cref="IBocList"/></param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> associated with the current row.</param>
    /// <param name="command">The <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.RowEditModeCommand"/> that is issued
    ///   when the control is clicked. Must not be <see langword="null" />.</param>
    /// <param name="alternateText">The <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier"/>
    ///   specifying which resource to load as alternate text to the icon.</param>
    /// <param name="icon">The icon to render; must not be <see langword="null"/>.
    ///   To skip the icon, set <see cref="IconInfo.Url"/> to <see langword="null" />.</param>
    /// <param name="text">The text to render after the icon. May be <see langword="null"/>, in which case no text is rendered.</param>
    protected virtual void RenderCommandControl (
        BocColumnRenderingContext<BocRowEditModeColumnDefinition> renderingContext,
        int originalRowIndex,
        IBusinessObject businessObject,
        BocList.RowEditModeCommand command,
        BocList.ResourceIdentifier alternateText,
        IconInfo icon,
        WebString text)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNull("icon", icon);

      string argument = renderingContext.Control.GetRowEditCommandArgument(new BocListRow(originalRowIndex, businessObject), command);
      string postBackEvent = renderingContext.Control.Page!.ClientScript.GetPostBackEventReference(renderingContext.Control, argument) + ";";
      var commandItemID = "Column_" + renderingContext.ColumnIndex + "_RowEditCommand_" + command + "_Row_" + originalRowIndex;

      Command c;
      if (!renderingContext.Control.IsReadOnly && renderingContext.Control.HasClientScript)
        c = new Command(CommandType.Event) { EventCommand = new Command.EventCommandInfo() };
      else
        c = new Command(CommandType.None);

      c.ItemID = commandItemID;
      c.OwnerControl = renderingContext.Control;

      c.RenderBegin(renderingContext.Writer, RenderingFeatures, postBackEvent, new string[0], OnCommandClickScript, null);

      bool hasIcon = icon.HasRenderingInformation;
      bool hasText = !text.IsEmpty;

      if (hasIcon && hasText)
      {
        icon.Render(renderingContext.Writer, renderingContext.Control);
        renderingContext.Writer.Write(c_whiteSpace);
      }
      else if (hasIcon)
      {
        bool hasAlternateText = !string.IsNullOrEmpty(icon.AlternateText);
        if (!hasAlternateText)
          icon.AlternateText = renderingContext.Control.GetResourceManager().GetString(alternateText);

        icon.Render(renderingContext.Writer, renderingContext.Control);
      }
      if (hasText)
        text.WriteTo(renderingContext.Writer);

      c.RenderEnd(renderingContext.Writer);
    }
  }
}
