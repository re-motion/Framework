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
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   A <see cref="HyperLink"/> that provides integration into the <see cref="ISmartNavigablePage"/> framework by
///   automatically appending the navigation URL parameters to the rendered <see cref="HyperLink.NavigateUrl"/>.
/// </summary>
public class SmartHyperLink : HyperLink, IControl
{
	public SmartHyperLink ()
	{
	}

  /// <summary> 
  ///   Uses <see cref="ISmartNavigablePage.AppendNavigationUrlParameters"/> to include the navigation URL parameters
  ///   with the rendered <see cref="HyperLink.NavigateUrl"/>.
  /// </summary>
  protected override void AddAttributesToRender (HtmlTextWriter writer)
  {
    string navigateUrlBackup = NavigateUrl;
    bool hasNavigateUrl = ! string.IsNullOrEmpty(NavigateUrl);

    if (Page is ISmartNavigablePage && hasNavigateUrl)
      NavigateUrl = ((ISmartNavigablePage)Page).AppendNavigationUrlParameters(NavigateUrl);

    base.AddAttributesToRender(writer);

    if (hasNavigateUrl)
      NavigateUrl = navigateUrlBackup;
  }

  public new IPage? Page
  {
    get { return PageWrapper.CastOrCreate(base.Page); }
  }
}

}
