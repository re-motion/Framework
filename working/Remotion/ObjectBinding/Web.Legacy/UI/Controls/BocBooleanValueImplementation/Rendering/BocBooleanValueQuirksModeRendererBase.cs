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
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Base class for the renderers in the same namespace. Contains common constants and methods.
  /// <seealso cref="BocBooleanValueRenderer"/>
  /// <seealso cref="BocCheckBoxRenderer"/>
  /// </summary>
  /// <typeparam name="T">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocBooleanValueQuirksModeRendererBase<T> : BocQuirksModeRendererBase<T>
      where T: IBocBooleanValueBase
  {
    private const string c_defaultControlWidth = "100pt";

    protected BocBooleanValueQuirksModeRendererBase (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    protected override void AddAdditionalAttributes(RenderingContext<T> renderingContext)
    {
      base.AddAdditionalAttributes (renderingContext);
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
      renderingContext.Writer.AddStyleAttribute ("white-space", "nowrap");
      if (!renderingContext.Control.IsReadOnly)
      {
        bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
        bool isLabelWidthEmpty = renderingContext.Control.LabelStyle.Width.IsEmpty;
        if (isLabelWidthEmpty && isControlWidthEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
      }
    }
  }
}