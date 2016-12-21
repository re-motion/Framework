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
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocTextValue"/> and <see cref="BocMultilineTextValue"/> controls, which is done
  /// by a template method for which deriving classes have to supply the <see cref="GetLabel"/> method.
  /// <seealso cref="BocTextValueRenderer"/>
  /// <seealso cref="BocMultilineTextValueRenderer"/>
  /// </summary>
  /// <typeparam name="T">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocTextValueRendererBase<T> : BocRendererBase<T>
      where T: IBocTextValueBase
  {
    /// <summary> Text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    protected const string c_designModeEmptyLabelContents = "##";

    protected BocTextValueRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    /// <summary>
    /// Renders a label when <see cref="IBusinessObjectBoundEditableControl.IsReadOnly"/> is <see langword="true"/>,
    /// a textbox in edit mode.
    /// </summary>    
    protected void Render (BocRenderingContext<T> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);

      WebControl innerControl = renderingContext.Control.IsReadOnly ? (WebControl) GetLabel (renderingContext) : GetTextBox (renderingContext);
      innerControl.Page = renderingContext.Control.Page.WrappedInstance;

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && string.IsNullOrEmpty (innerControl.Style["height"]);

      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      innerControl.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<T> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);
      
      var hasAutoPostBack = renderingContext.Control.TextBoxStyle.AutoPostBack.HasValue && renderingContext.Control.TextBoxStyle.AutoPostBack.Value;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());
    }

    /// <summary>
    /// Creates a <see cref="TextBox"/> control to use for rendering the <see cref="BocTextValueBase"/> control in edit mode.
    /// </summary>
    /// <returns>A <see cref="TextBox"/> control with the all relevant properties set and all appropriate styles applied to it.</returns>
    protected virtual TextBox GetTextBox (BocRenderingContext<T> renderingContext)
    {
      TextBox textBox = new RenderOnlyTextBox { ClientIDMode = ClientIDMode.Static };
      textBox.Text = renderingContext.Control.Text;
      textBox.ID = renderingContext.Control.GetValueName();
      textBox.EnableViewState = false;
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.ReadOnly = !renderingContext.Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.TextBoxStyle.ApplyStyle (textBox);

      return textBox;
    }

    /// <summary>
    /// Creates a <see cref="Label"/> control to use for rendering the <see cref="BocTextValueBase"/> control in read-only mode.
    /// </summary>
    /// <returns>A <see cref="Label"/> control with all relevant properties set and all appropriate styles applied to it.</returns>
    protected abstract Label GetLabel (BocRenderingContext<T> renderingContext);

    private string CssClassContent
    {
      get { return "content"; }
    }
  }
}