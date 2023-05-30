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
using System.Web.UI;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Groups all arguments required for rendering a <see cref="BocList"/>.
  /// </summary>
  public class BocListRenderingContext : BocRenderingContext<IBocList>
  {
    private readonly BusinessObjectWebServiceContext _businessObjectWebServiceContext;
    private readonly BocColumnRenderer[] _columnRenderers;
    private readonly BocListColumnIndexProvider _columnIndexProvider;

    public BocListRenderingContext (
        HttpContextBase httpContext,
        HtmlTextWriter writer,
        IBocList control,
        BusinessObjectWebServiceContext businessObjectWebServiceContext,
        BocColumnRenderer[] columnRenderers)
        : base(httpContext, writer, control)
    {
      ArgumentUtility.CheckNotNull("businessObjectWebServiceContext", businessObjectWebServiceContext);
      ArgumentUtility.CheckNotNull("columnRenderers", columnRenderers);

      _businessObjectWebServiceContext = businessObjectWebServiceContext;
      _columnRenderers = columnRenderers;
      _columnIndexProvider = new BocListColumnIndexProvider(columnRenderers);
    }

    public BusinessObjectWebServiceContext BusinessObjectWebServiceContext
    {
      get { return _businessObjectWebServiceContext; }
    }

    public BocColumnRenderer[] ColumnRenderers
    {
      get { return _columnRenderers; }
    }

    public IBocListColumnIndexProvider ColumnIndexProvider
    {
      get { return _columnIndexProvider; }
    }
  }
}
