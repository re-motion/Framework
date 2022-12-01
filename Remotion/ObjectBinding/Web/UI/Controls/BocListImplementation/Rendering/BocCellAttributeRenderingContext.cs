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

using System.Web;
using System.Web.UI;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocCellAttributeRenderingContext{TBocColumnDefinition}"/> wraps <see cref="BocColumnRenderingContext{TBocColumnDefinition}"/> for
  /// use in <see cref="BocColumnRendererBase{TBocColumnDefinition}"/>.<see cref="BocColumnRendererBase{TBocColumnDefinition}.AddAttributesToRenderForTitleCell"/>
  /// and in <see cref="BocColumnRendererBase{TBocColumnDefinition}"/>.<see cref="BocColumnRendererBase{TBocColumnDefinition}.AddAttributesToRenderForDataCell"/>.
  /// </summary>
  public readonly struct BocCellAttributeRenderingContext<TBocColumnDefinition>
      where TBocColumnDefinition : BocColumnDefinition
  {
    private readonly BocColumnRenderingContext<TBocColumnDefinition> _renderingContext;

    public BocCellAttributeRenderingContext (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      _renderingContext = renderingContext;
    }

    public HttpContextBase HttpContext => _renderingContext.HttpContext;

    public IBocList Control => _renderingContext.Control;

    public BusinessObjectWebServiceContext BusinessObjectWebServiceContext => _renderingContext.BusinessObjectWebServiceContext;

    public int ColumnIndex => _renderingContext.ColumnIndex;

    public int VisibleColumnIndex => _renderingContext.VisibleColumnIndex;

    public TBocColumnDefinition ColumnDefinition => _renderingContext.ColumnDefinition;

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(string,string)" />
    public void AddAttributeToRender (string name, string? value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      _renderingContext.Writer.AddAttribute(name, value);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(string,string,bool)" />
    public void AddAttributeToRender (string name, string? value, bool fEncode)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      _renderingContext.Writer.AddAttribute(name, value, fEncode);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(string,string)" />
    public void AddAttributeToRender (string name, PlainTextString value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      value.AddAttributeTo(_renderingContext.Writer, name);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(HtmlTextWriterAttribute,string)" />
    public void AddAttributeToRender (HtmlTextWriterAttribute key, string? value)
    {
      _renderingContext.Writer.AddAttribute(key, value);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(HtmlTextWriterAttribute,string,bool)" />
    public void AddAttributeToRender (HtmlTextWriterAttribute key, string? value, bool fEncode)
    {
      _renderingContext.Writer.AddAttribute(key, value, fEncode);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddAttribute(HtmlTextWriterAttribute,string)" />
    public void AddAttributeToRender (HtmlTextWriterAttribute key, PlainTextString value)
    {
      value.AddAttributeTo(_renderingContext.Writer, key);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddStyleAttribute(string,string)" />
    public void AddStyleAttributeToRender (string name, string? value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      _renderingContext.Writer.AddStyleAttribute(name, value);
    }

    /// <inheritdoc cref="HtmlTextWriter.AddStyleAttribute(HtmlTextWriterStyle,string)" />
    public void AddStyleAttributeToRender (HtmlTextWriterStyle key, string? value)
    {
      _renderingContext.Writer.AddStyleAttribute(key, value);
    }
  }
}
