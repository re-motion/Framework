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
  /// Responsible for rendering table cells of <see cref="BocCompoundColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor (typeof (IBocCompoundColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocCompoundColumnRenderer : BocValueColumnRendererBase<BocCompoundColumnDefinition>, IBocCompoundColumnRenderer
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCompoundColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocCompoundColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses)
        : base (resourceUrlFactory, renderingFeatures, cssClasses)
    {
    }

    /// <summary>
    /// Renders a string representation of the property of <paramref name="businessObject"/> that is shown in the column.
    /// </summary>
    /// <param name="renderingContext">The <see cref="BocColumnRenderingContext{BocColumnDefinition}"/>.</param>
    /// <param name="businessObject">The <see cref="IBusinessObject"/> whose property will be rendered.</param>
    /// <param name="editableRow">Ignored.</param>
    protected override void RenderCellDataForEditMode (
        BocColumnRenderingContext<BocCompoundColumnDefinition> renderingContext, IBusinessObject businessObject, IEditableRow editableRow)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      RenderValueColumnCellText (renderingContext, renderingContext.ColumnDefinition.GetStringValue (businessObject));
    }
  }
}