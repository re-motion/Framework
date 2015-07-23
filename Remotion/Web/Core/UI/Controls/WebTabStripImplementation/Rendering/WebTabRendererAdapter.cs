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
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// <see cref="WebTabRendererAdapter"/> holds a <see cref="IWebTab"/> and it's corresponding <see cref="IWebTabRenderer"/>. It exposes a render 
  /// method which delegates to the <see cref="IWebTabRenderer"/> to render a <see cref="IWebTabStrip"/> tab. 
  /// </summary>
  public class WebTabRendererAdapter
  {
    private readonly IWebTabRenderer _webTabRenderer;
    private readonly IWebTab _webTab;
    private readonly bool _isLast;
    private readonly bool _isEnabled;
    private readonly WebTabStyle _webTabStyle;


    public WebTabRendererAdapter (IWebTabRenderer webTabRenderer, IWebTab webTab, bool isLast, bool isEnabled, WebTabStyle webTabStyle)
    {
      ArgumentUtility.CheckNotNull ("webTabRenderer", webTabRenderer);
      ArgumentUtility.CheckNotNull ("webTab", webTab);

      _webTabRenderer = webTabRenderer;
      _webTab = webTab;
      _isLast = isLast;
      _isEnabled = isEnabled;
      _webTabStyle = webTabStyle;
    }

    public IWebTab WebTab
    {
      get { return _webTab; }
    }

    public bool IsLast
    {
      get { return _isLast; }
    }

    public bool IsEnabled
    {
      get { return _isEnabled; }
    }

    public WebTabStyle WebTabStyle
    {
      get { return _webTabStyle; }
    }

    public void Render (WebTabStripRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);
      
      _webTabRenderer.Render (renderingContext, _webTab, _isEnabled, _isLast, _webTabStyle);
    }

  }
}