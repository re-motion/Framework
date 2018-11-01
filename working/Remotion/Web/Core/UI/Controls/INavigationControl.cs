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
using System.Collections.Specialized;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface declares the methods required by controls used for navigating between individual pages of a
///   web application.
/// </summary>
/// <remarks>
///   A <see cref="Control"/> implementing <see cref="INavigationControl"/> should check whether it is located on an
///   <see cref="ISmartNavigablePage"/> and if so, register itself using the 
///   <see cref="ISmartNavigablePage.RegisterNavigationControl"/> method during the <b>OnInit</b> phase of the page 
///   life cycle.
/// </remarks>
/// <seealso cref="ISmartNavigablePage"/>
public interface INavigationControl: IControl
{
  /// <summary> 
  ///   Provides the URL parameters containing the navigation information for this control (e.g. the selected tab).
  /// </summary>
  /// <returns> 
  ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
  ///   <see cref="INavigationControl"/> to restore its navigation state when using hyperlinks.
  /// </returns>
  NameValueCollection GetNavigationUrlParameters();
}

}
