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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation
{
  public class WebTabStripMock : WebTabStrip
  {
    private bool _isDesignMode;

    public string CssClassBasePublic
    {
      get { return CssClassBase; }
    }

    public string CssClassTabsPanePublic
    {
      get { return CssClassTabsPane; }
    }

    public string CssClassTabsPaneEmptyPublic
    {
      get { return CssClassTabsPaneEmpty; }
    }

    public string CssClassTapStripTabWrapperPublic
    {
      get { return "tabStripTabWrapper"; }
    }

    public string CssClassSeparatorPublic
    {
      get { return CssClassSeparator; }
    }

    public string CssClassTabPublic
    {
      get { return CssClassTab; }
    }

    public string CssClassTabAnchorBodyPublic
    {
      get { return CssClassTabAnchorBody; }
    }

    public override bool IsDesignMode
    {
      get
      {
        return _isDesignMode;
      }
    }

    public string CssClassTabSelectedPublic
    {
      get { return CssClassTabSelected; }
    }

    public string CssClassDisabledPublic
    {
      get { return CssClassDisabled; }
    }

    public void SetDesignMode (bool value)
    {
      _isDesignMode = value;
    }
  }
}