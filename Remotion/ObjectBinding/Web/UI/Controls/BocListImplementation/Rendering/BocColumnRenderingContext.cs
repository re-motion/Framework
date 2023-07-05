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
  public class BocColumnRenderingContext
  {
    private readonly HttpContextBase _httpContext;
    private readonly HtmlTextWriter _writer;
    private readonly IBocList _control;
    private readonly BusinessObjectWebServiceContext _businessObjectWebServiceContext;
    private readonly BocColumnDefinition _columnDefinition;
    private readonly IBocListColumnIndexProvider _columnIndexProvider;
    private readonly int _columnIndex;
    private readonly int _visibleColumnIndex;

    public BocColumnRenderingContext (
        HttpContextBase httpContext,
        HtmlTextWriter writer,
        IBocList control,
        BusinessObjectWebServiceContext businessObjectWebServiceContext,
        BocColumnDefinition columnDefinition,
        IBocListColumnIndexProvider columnIndexProvider,
        int columnIndex,
        int visibleColumnIndex)
    {
      ArgumentUtility.CheckNotNull("httpContext", httpContext);
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("control", control);
      ArgumentUtility.CheckNotNull("businessObjectWebServiceContext", businessObjectWebServiceContext);
      ArgumentUtility.CheckNotNull("columnDefinition", columnDefinition);
      ArgumentUtility.CheckNotNull("columnIndexProvider", columnIndexProvider);

      _httpContext = httpContext;
      _writer = writer;
      _control = control;
      _businessObjectWebServiceContext = businessObjectWebServiceContext;
      _columnDefinition = columnDefinition;
      _columnIndexProvider = columnIndexProvider;
      _columnIndex = columnIndex;
      _visibleColumnIndex = visibleColumnIndex;
    }

    public HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }

    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    public IBocList Control
    {
      get { return _control; }
    }

    public BusinessObjectWebServiceContext BusinessObjectWebServiceContext
    {
      get { return _businessObjectWebServiceContext; }
    }

    public BocColumnDefinition ColumnDefinition
    {
      get { return _columnDefinition; }
    }

    public IBocListColumnIndexProvider ColumnIndexProvider
    {
      get { return _columnIndexProvider; }
    }

    public int ColumnIndex
    {
      get { return _columnIndex; }
    }

    public int VisibleColumnIndex
    {
      get { return _visibleColumnIndex; }
    }
  }
}
