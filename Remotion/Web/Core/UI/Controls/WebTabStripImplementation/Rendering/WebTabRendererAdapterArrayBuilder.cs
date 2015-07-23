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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// <see cref="WebTabRendererAdapterArrayBuilder"/> is responsible to create a collection of <see cref="WebTabRendererAdapter"/>s for the given
  /// <see cref="IWebTab"/>s.
  /// </summary>
  public class WebTabRendererAdapterArrayBuilder
  {
    private readonly IWebTab[] _webTabs;
    
    public WebTabRendererAdapterArrayBuilder (IWebTab[] webTabs)
    {
      ArgumentUtility.CheckNotNull ("webTabs", webTabs);
      
      _webTabs = webTabs;
    }

    public bool EnableSelectedTab { get; set; }
    public WebTabStyle SelectedTabStyle { get; set; }
    public WebTabStyle TabStyle { get; set; }

    public WebTabRendererAdapter[] GetWebTabRenderers ()
    {
      var rendererAdapters = new List<WebTabRendererAdapter>();
      for (int i = 0; i < _webTabs.Length; i++)
      {
        var webTab = _webTabs[i];
        var isLast = i == (_webTabs.Length - 1);
        var isEnabled = !_webTabs[i].IsSelected || EnableSelectedTab;
        var style = _webTabs[i].IsSelected ? SelectedTabStyle : TabStyle;

        rendererAdapters.Add (new WebTabRendererAdapter (webTab.GetRenderer(), webTab, isLast, isEnabled, style));
      }
      return rendererAdapters.ToArray();
    }
    
  }
}