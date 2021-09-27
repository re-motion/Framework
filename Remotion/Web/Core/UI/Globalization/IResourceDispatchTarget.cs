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
using System.Collections;
using System.Web.UI;
using Remotion.Globalization;

namespace Remotion.Web.UI.Globalization
{

/// <summary>
///   Interface for controls who wish to use automatic resource dispatching
///   but implement the dispatching logic themselves.
/// </summary>
public interface IResourceDispatchTarget
{
  /// <summary>
  ///   <b>Dispatch</b> is called by the parent control
  ///   and receives the resources as an <b>IDictonary</b>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The implementation of <b>IResourceDispatchTarget</b> is responsible for interpreting
  ///     the resources provided through <b>ByElementName</b>.
  ///   </para><para>
  ///     The key of the <b>IDictonaryEntry</b> can be a simple property name
  ///     or a more complex string. It can be freely defined by the <c>IResourceDispatchTarget</c>
  ///     implementation. Inside the resource container, this key is prepended by the control
  ///     instance's ID and a prefix.For details, please refer to 
  ///     <see cref="ResourceDispatcher.Dispatch(Control, IResourceManager)" />
  ///   </para>
  /// </remarks>
  /// <param name="values">
  ///   An <b>IDictonary</b>: &lt;string key, string value&gt;.
  /// </param>
  void Dispatch (IDictionary values);
}

}
