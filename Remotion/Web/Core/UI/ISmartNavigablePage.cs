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
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{

/// <summary>
///   This interface declares methods and properties used to control and enhance the client side user experience
///   for navigating the page and to simplify application development towards this goal.
/// </summary>
/// <remarks>
///   The features specified by the <see cref="ISmartNavigablePage"/> interface include smart scrolling 
///   (i.e. the restoration of the scroll position after a postback), smart focusing (i.e. restoring the focus after a
///   postback or explicitly setting it), and the use of controls implementing <see cref="INavigationControl"/>.
/// </remarks>
public interface ISmartNavigablePage: IPage
{
  /// <summary> Gets or sets the flag that determines whether smart scrolling is enabled on this page.  </summary>
  /// <value> <see langword="true"/> to enable smart scrolling. </value>
  /// <remarks>In order for the scroll position to be restored on a DOM element, the DOM element must have an ID set. </remarks>
  bool IsSmartScrollingEnabled { get; }

  /// <summary> Gets or sets the flag that determines whether smart naviagtion is enabled on this page.  </summary>
  /// <value> <see langword="true"/> to enable smart focusing. </value>
  bool IsSmartFocusingEnabled { get; }

  /// <summary> Clears scrolling and focus information on the page. </summary>
  void DiscardSmartNavigationData ();

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> 
  ///   The <see cref="IFocusableControl"/> to assign the focus to.
  /// </param>
  /// <remarks> 
  ///   In dotNet 2.0, the focus can be set even if smart focusing is disabled. 
  ///   <note type="inotes">
  ///     If <see langword="null"/> is passed for <paramref name="control"/>, an <see cref="ArgumentNullException"/>
  ///     should be thrown.
  ///   </note>
  /// </remarks>
  void SetFocus (IFocusableControl control);

  /// <summary> Registers a <see cref="INavigationControl"/> with the <see cref="ISmartNavigablePage"/>. </summary>
  /// <param name="control"> The <see cref="INavigationControl"/> to register. </param>
  /// <remarks> 
  ///   <note type="inotes">
  ///     If <see langword="null"/> is passed for <paramref name="control"/>, an <see cref="ArgumentNullException"/>
  ///     should be thrown.
  ///   </note>
  /// </remarks>
  void RegisterNavigationControl (INavigationControl control);

  /// <summary> 
  ///   Appends the URL parameters returned by <see cref="GetNavigationUrlParameters"/> to the <paramref name="url"/>.
  /// </summary>
  /// <param name="url"> A URL or a query string. </param>
  /// <returns> 
  ///   The <paramref name="url"/> appended with the URL parameters returned by 
  ///   <see cref="GetNavigationUrlParameters"/>. 
  /// </returns>
  /// <remarks> 
  ///   <note type="inotes">
  ///     If <see langword="null"/> is passed for <paramref name="url"/>, an <see cref="ArgumentNullException"/>
  ///     should be thrown.
  ///   </note>
  /// </remarks>
  string AppendNavigationUrlParameters (string url);
  
  /// <summary> 
  ///   Evaluates the <see cref="INavigationControl.GetNavigationUrlParameters"/> methods of all controls registered
  ///   using <see cref="RegisterNavigationControl"/>.
  /// </summary>
  /// <returns>
  ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
  ///   <see cref="ISmartNavigablePage"/> to restore its navigation state when using hyperlinks.
  /// </returns>
  NameValueCollection GetNavigationUrlParameters();
}

}
