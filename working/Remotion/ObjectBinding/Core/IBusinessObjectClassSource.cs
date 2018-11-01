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
using System.ComponentModel;

namespace Remotion.ObjectBinding
{

/// <summary>
///   This interface is used for the <see cref="ITypeDescriptorContext.Instance"/> argument of the 
///   Visual Studio .NET designer editor. 
/// </summary>
/// <remarks>
///   <para>
///     The <see cref="Remotion.ObjectBinding.Design.PropertyPathPickerControl"/> uses this interface 
///     to query the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
///     or an <see cref="IBusinessObjectDataSource"/>, respectively.
///   </para><para>
///     Implemented by <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocSimpleColumnDefinition"/> 
///     and <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.PropertyPathBinding"/>.
///   </para>
/// </remarks>
public interface IBusinessObjectClassSource
{
  /// <summary>
  ///   Gets the <see cref="IBusinessObjectClass"/> of an <see cref="IBusinessObjectReferenceProperty"/>
  ///   or an <see cref="IBusinessObjectDataSource"/>, respectively.
  /// </summary>
  /// <value> 
  ///   The <see cref="IBusinessObjectClass"/> to be queried for the properties offered by the 
  ///   <see cref="Remotion.ObjectBinding.Design.PropertyPathPickerControl"/>.
  /// </value>
  IBusinessObjectClass BusinessObjectClass { get; }
}

}
