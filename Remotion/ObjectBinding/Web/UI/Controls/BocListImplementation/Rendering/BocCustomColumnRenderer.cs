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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocCustomColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor(typeof(IBocCustomColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocCustomColumnRenderer : BocColumnRendererBase<BocCustomColumnDefinition>, IBocCustomColumnRenderer
  {
    private readonly IRenderingFeatures _renderingFeatures;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList"/> to render, 
    /// an <see cref="HtmlTextWriter"/> to render to, and a <see cref="BocCustomColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocCustomColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);

      _renderingFeatures = renderingFeatures;
    }

    /// <summary>
    /// Renders a custom column cell either directly or by wrapping the contained controls, depending on <see cref="BocCustomColumnDefinition.Mode"/>
    /// and the current row state.
    /// </summary>
    /// <remarks>
    /// If the <see cref="BocCustomColumnDefinition.Mode"/> property of <see cref="BocColumnRenderingContext.ColumnDefinition"/> indicates that
    /// the custom cell does not contain any controls (<see cref="BocCustomColumnDefinitionMode.NoControls"/> or 
    /// <see cref="BocCustomColumnDefinitionMode.ControlInEditedRow"/> when the current row is not being edited),
    /// a <see cref="BocCustomCellRenderArguments"/> object is created and passed to the custom cell's 
    /// <see cref="BocCustomColumnDefinitionCell.RenderInternal"/> method.
    /// Otherwise, a click wrapper is rendered around the child control obtained from
    /// <see cref="IBocList"/>'s <see cref="IBocList.CustomColumns"/> property.
    /// </remarks>
    protected override void RenderCellContents (BocColumnRenderingContext<BocCustomColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      int originalRowIndex = arguments.ListIndex;
      IBusinessObject businessObject = arguments.BusinessObject;
      bool isEditedRow = renderingContext.Control.EditModeController.IsRowEditModeActive &&
                         renderingContext.Control.EditModeController.GetEditableRow(originalRowIndex) != null;
      var headerIDs = arguments.HeaderIDs;

      if (renderingContext.ColumnDefinition.Mode == BocCustomColumnDefinitionMode.ControlsInAllRows
          || (renderingContext.ColumnDefinition.Mode == BocCustomColumnDefinitionMode.ControlInEditedRow && isEditedRow))
        RenderCustomCellInnerControls(renderingContext, originalRowIndex, arguments.RowIndex, headerIDs);
      else
        RenderCustomCellDirectly(renderingContext, businessObject, originalRowIndex, headerIDs);
    }

    /// <summary>
    /// Adds attributes to a title cell that include information about bound property paths of <see cref="BocCustomColumnDefinition"/>.
    /// </summary>
    protected override void AddAttributesToRenderForTitleCell (BocCellAttributeRenderingContext<BocCustomColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
    {
      base.AddAttributesToRenderForTitleCell(renderingContext, in arguments);

      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var boundPropertyPath = renderingContext.ColumnDefinition.PropertyPathIdentifier;

        if (!string.IsNullOrEmpty(boundPropertyPath))
        {
          renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.HasPropertyPaths, "true");
          renderingContext.AddAttributeToRender(
              DiagnosticMetadataAttributesForObjectBinding.BoundPropertyPaths,
              boundPropertyPath);
        }
        else
        {
          renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.HasPropertyPaths, "false");
        }
      }
    }

    private void RenderCustomCellInnerControls (BocColumnRenderingContext<BocCustomColumnDefinition> renderingContext, int originalRowIndex,
        int rowIndex,
        IReadOnlyCollection<string> headerIDs)
    {
      BocListCustomColumnTuple[] customColumnTuples = renderingContext.Control.CustomColumns[renderingContext.ColumnDefinition];
      BocListCustomColumnTuple? customColumnTuple;
      if (customColumnTuples.Length > rowIndex && customColumnTuples[rowIndex].Item2 == originalRowIndex)
        customColumnTuple = customColumnTuples[rowIndex];
      else
        customColumnTuple = customColumnTuples.FirstOrDefault(t => t.Item2 == originalRowIndex);

      if (customColumnTuple == null)
      {
        renderingContext.Writer.Write(c_whiteSpace);
        return;
      }

      RenderClickWrapperBeginTag(renderingContext);

      Control control = customColumnTuple.Item3;
      if (control != null)
      {
        if (control is ISmartControl smartControl)
          smartControl.AssignLabels(headerIDs);

        ApplyStyleDefaults(control);
        control.RenderControl(renderingContext.Writer);
      }

      RenderClickWrapperEndTag(renderingContext);
    }

    private void RenderClickWrapperBeginTag (BocColumnRenderingContext<BocCustomColumnDefinition> renderingContext)
    {
      string onClick = renderingContext.Control.HasClientScript ? OnCommandClickScript : string.Empty;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
    }

    private void RenderClickWrapperEndTag (BocColumnRenderingContext<BocCustomColumnDefinition> renderingContext)
    {
      renderingContext.Writer.RenderEndTag();
    }

    private void ApplyStyleDefaults (Control control)
    {
      bool isControlWidthEmpty;
      CssStyleCollection? controlStyle = GetControlStyle(control, out isControlWidthEmpty);
      if (controlStyle == null)
        return;

      if (string.IsNullOrEmpty(controlStyle["width"]) && isControlWidthEmpty)
        controlStyle["width"] = "100%";
      if (string.IsNullOrEmpty(controlStyle["vertical-align"]))
        controlStyle["vertical-align"] = "middle";
    }

    private CssStyleCollection? GetControlStyle (Control control, out bool isControlWidthEmpty)
    {
      CssStyleCollection? controlStyle = null;
      isControlWidthEmpty = true;
      if (control is WebControl)
      {
        controlStyle = ((WebControl)control).Style;
        isControlWidthEmpty = ((WebControl)control).Width.IsEmpty;
      }
      else if (control is HtmlControl)
        controlStyle = ((HtmlControl)control).Style;
      return controlStyle;
    }

    private void RenderCustomCellDirectly (
        BocColumnRenderingContext<BocCustomColumnDefinition> renderingContext,
        IBusinessObject businessObject,
        int originalRowIndex,
        IReadOnlyCollection<string> headerIDs)
    {
      string onClick = renderingContext.Control.HasClientScript ? OnCommandClickScript : string.Empty;
      BocCustomCellRenderArguments arguments = new BocCustomCellRenderArguments(
          renderingContext.Control,
          businessObject,
          renderingContext.ColumnDefinition,
          renderingContext.ColumnIndex,
          originalRowIndex,
          headerIDs,
          onClick);
      renderingContext.ColumnDefinition.CustomCell.RenderInternal(renderingContext.Writer, arguments);
    }
  }
}
