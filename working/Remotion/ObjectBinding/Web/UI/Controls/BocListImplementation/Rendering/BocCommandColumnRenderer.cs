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
  /// Responsible for rendering cells of <see cref="BocCommandColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor (typeof (IBocCommandColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocCommandColumnRenderer : BocCommandEnabledColumnRendererBase<BocCommandColumnDefinition>, IBocCommandColumnRenderer
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocCommandColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocCommandColumnRenderer (IResourceUrlFactory resourceUrlFactory, IRenderingFeatures renderingFeatures, BocListCssClassDefinition cssClasses)
        : base (resourceUrlFactory, renderingFeatures, cssClasses)
    {
    }

    /// <summary>
    /// Renders a command control with an icon, text, or both.
    /// </summary>
    /// <remarks>
    /// A <see cref="BocCommandColumnDefinition"/> can contain both an object icon and a command icon. The former is rendered according to
    /// <paramref name="showIcon"/>, the latter if the column defintion's <see cref="BocCommandColumnDefinition.Icon"/> property contains
    /// an URL. Furthermore, the command text in <see cref="BocCommandColumnDefinition.Text"/> is rendered after any icons.
    /// </remarks>
    protected override void RenderCellContents (
        BocColumnRenderingContext<BocCommandColumnDefinition> renderingContext,
        BocListDataRowRenderEventArgs dataRowRenderEventArgs,
        int rowIndex,
        bool showIcon)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull ("dataRowRenderEventArgs", dataRowRenderEventArgs);

      int originalRowIndex = dataRowRenderEventArgs.ListIndex;
      IBusinessObject businessObject = dataRowRenderEventArgs.BusinessObject;

      IEditableRow editableRow = renderingContext.Control.EditModeController.GetEditableRow (originalRowIndex);

      bool hasEditModeControl = editableRow != null && editableRow.HasEditControl (renderingContext.ColumnIndex);

      bool isCommandEnabled = RenderBeginTag (renderingContext, originalRowIndex, businessObject);

      RenderCellIcon (renderingContext, businessObject, hasEditModeControl, showIcon);
      RenderCellCommand (renderingContext);

      RenderEndTag (renderingContext, isCommandEnabled);
    }

    private void RenderCellCommand (BocColumnRenderingContext<BocCommandColumnDefinition> renderingContext)
    {
      if (renderingContext.ColumnDefinition.Icon.HasRenderingInformation)
        renderingContext.ColumnDefinition.Icon.Render (renderingContext.Writer, renderingContext.Control);

      if (!string.IsNullOrEmpty (renderingContext.ColumnDefinition.Text))
        renderingContext.Writer.Write (renderingContext.ColumnDefinition.Text); // Do not HTML encode
    }

    private void RenderCellIcon (
        BocColumnRenderingContext<BocCommandColumnDefinition> renderingContext, IBusinessObject businessObject, bool hasEditModeControl, bool showIcon)
    {
      if (!hasEditModeControl && showIcon)
        RenderCellIcon (renderingContext, businessObject);
    }

    private bool RenderBeginTag (
        BocColumnRenderingContext<BocCommandColumnDefinition> renderingContext, int originalRowIndex, IBusinessObject businessObject)
    {
      bool isCommandEnabled = RenderBeginTagDataCellCommand (renderingContext, businessObject, originalRowIndex);
      if (!isCommandEnabled)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Content);
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      return isCommandEnabled;
    }

    protected void RenderEndTag (BocColumnRenderingContext<BocCommandColumnDefinition> renderingContext, bool isCommandEnabled)
    {
      if (isCommandEnabled)
        RenderEndTagDataCellCommand (renderingContext);
      else
        renderingContext.Writer.RenderEndTag();
    }
  }
}