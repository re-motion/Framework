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
using System.Web.UI;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering table cells of <see cref="BocSimpleColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor(typeof(IBocSimpleColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocSimpleColumnRenderer : BocValueColumnRendererBase<BocSimpleColumnDefinition>, IBocSimpleColumnRenderer
  {
    private readonly IRenderingFeatures _renderingFeatures;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocSimpleColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocSimpleColumnRenderer (
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
    /// Renders the edit mode control.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="arguments">The <see cref="BocDataCellRenderArguments"/> for the rendered cell.</param>
    /// <param name="editableRow">The <see cref="EditableRow"/> object used to actually render the edit row controls.</param>
    protected override void RenderCellDataForEditMode (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments, IEditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("editableRow", editableRow);

      RenderEditModeControl(renderingContext, arguments, editableRow);
    }

    /// <summary>
    /// Renders the icon of the <see cref="IBusinessObject"/> determined by the column's property path.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> that acts as a starting point for the property path.</param>
    protected override void RenderOtherIcons (BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      if (renderingContext.ColumnDefinition.EnableIcon)
      {
        var propertyPath = renderingContext.ColumnDefinition.GetPropertyPath();

        var result = propertyPath.GetResult(
            businessObject,
            BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
            BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

        if (result.ResultProperty is IBusinessObjectReferenceProperty && !result.ResultProperty.IsList)
        {
          var value = (IBusinessObject?)result.GetValue();
          if (value != null)
            RenderCellIcon(renderingContext, value);
        }
      }
    }

    /// <summary>
    /// Adds attributes to a title cell that include information about bound property paths of <see cref="BocSimpleColumnDefinition"/>.
    /// </summary>
    protected override void AddAttributesToRenderForTitleCell (BocCellAttributeRenderingContext<BocSimpleColumnDefinition> renderingContext, in BocTitleCellRenderArguments arguments)
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

    private void RenderEditModeControl (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments, IEditableRow editableRow)
    {
      var businessObject = arguments.BusinessObject;

      if (renderingContext.Control.HasClientScript)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, OnCommandClickScript);

      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var contentString = renderingContext.ColumnDefinition.GetStringValue(businessObject);
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BocListCellContents, contentString);
      }

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div); // Begin Div

      editableRow.RenderSimpleColumnCellEditModeControl(
          renderingContext.Writer,
          renderingContext.ColumnDefinition,
          businessObject,
          renderingContext.ColumnIndex,
          arguments.HeaderIDs);

      renderingContext.Writer.RenderEndTag(); // End Div
    }
  }
}
