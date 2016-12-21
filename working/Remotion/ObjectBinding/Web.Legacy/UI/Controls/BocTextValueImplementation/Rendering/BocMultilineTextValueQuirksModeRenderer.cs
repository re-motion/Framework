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
using System.Web;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTextValueImplementation.Rendering
{
  /// <summary>
  /// Provides a label for rendering a <see cref="BocMultilineTextValue"/> control in read-only mode. 
  /// Rendering is done by the parent class.
  /// </summary>
  /// <include file='..\..\..\..\doc\include\UI\Controls\BocMultilineTextValueRenderer.xml' path='BocMultilineTextValueQuirksModeRenderer/Class/*'/>
  public class BocMultilineTextValueQuirksModeRenderer : BocTextValueQuirksModeRendererBase<IBocMultilineTextValue>, IBocMultilineTextValueRenderer
  {
    public BocMultilineTextValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, TextBoxStyle textBoxStyle)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      textBoxStyle.RegisterJavaScriptInclude (SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>(), htmlHeadAppender);

      string styleKey = typeof (BocMultilineTextValueQuirksModeRenderer).FullName + "_Style";

      var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocMultilineTextValueQuirksModeRenderer), ResourceType.Html, "BocMultilineTextValue.css");
      
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (BocMultilineTextValueRenderingContext renderingContext)
    {
      base.Render (renderingContext);
    }

    protected override Label GetLabel (BocRenderingContext<IBocMultilineTextValue> renderingContext)
    {
      Label label = new Label();
      label.ID = renderingContext.Control.GetValueName ();
      label.EnableViewState = false;

      string[] lines = renderingContext.Control.Value;
      string text = null;
      if (lines != null)
      {
        for (int i = 0; i < lines.Length; i++)
          lines[i] = HttpUtility.HtmlEncode (lines[i]);
        text = string.Join ("<br />", lines);
      }
      if (string.IsNullOrEmpty (text))
      {
        if (renderingContext.Control.IsDesignMode)
        {
          text = c_designModeEmptyLabelContents;
          //  Too long, can't resize in designer to less than the content's width
          //  label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
        }
        else
          text = "&nbsp;";
      }
      label.Text = text;

      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      return label;
    }

    public override string CssClassBase
    {
      get { return "bocMultilineTextValue"; }
    }
  }
}