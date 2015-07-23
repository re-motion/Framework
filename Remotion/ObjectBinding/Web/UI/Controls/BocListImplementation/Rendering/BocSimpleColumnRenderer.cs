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
  [ImplementationFor (typeof (IBocSimpleColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocSimpleColumnRenderer : BocValueColumnRendererBase<BocSimpleColumnDefinition>, IBocSimpleColumnRenderer
  {
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
        BocListCssClassDefinition cssClasses)
        : base (resourceUrlFactory, renderingFeatures, cssClasses)
    {
    }

    /// <summary>
    /// Renders the edit mode control.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> whose property will be rendered.</param>
    /// <param name="editableRow">The <see cref="EditableRow"/> object used to actually render the edit row controls.</param>
    protected override void RenderCellDataForEditMode (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject, IEditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("editableRow", editableRow);

      RenderEditModeControl (renderingContext, businessObject, editableRow);
    }

    /// <summary>
    /// Renders the icon of the <see cref="IBusinessObject"/> determined by the column's property path.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> that acts as a starting point for the property path.</param>
    protected override void RenderOtherIcons (BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      if (renderingContext.ColumnDefinition.EnableIcon)
      {
        var propertyPath = renderingContext.ColumnDefinition.GetPropertyPath();

        var result = propertyPath.GetResult (
            businessObject,
            BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
            BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

        if (result.ResultProperty is IBusinessObjectReferenceProperty && !result.ResultProperty.IsList)
        {
          var value = (IBusinessObject) result.GetValue();
          if (value != null)
            RenderCellIcon (renderingContext, value);
        }
      }
    }

    private void RenderEditModeControl (
        BocColumnRenderingContext<BocSimpleColumnDefinition> renderingContext, IBusinessObject businessObject, IEditableRow editableRow)
    {
      if (renderingContext.Control.HasClientScript)
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, c_onCommandClickScript);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin span

      editableRow.RenderSimpleColumnCellEditModeControl (
          renderingContext.Writer,
          renderingContext.ColumnDefinition,
          businessObject,
          renderingContext.ColumnIndex);

      renderingContext.Writer.RenderEndTag(); // End span
    }
  }
}