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
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  ///   The <see cref="SearchAvailableObjectsServiceTypeAttribute"/> declares the spefic implementation of <see cref="ISearchAvailableObjectsService"/> 
  ///   to be used by the <see cref="ReferenceProperty"/>'s <see cref="ReferenceProperty.SearchAvailableObjects"/> method when searching 
  ///   for the list of <see cref="IBusinessObject"/> instances that may be assigned to the property.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="SearchAvailableObjectsServiceTypeAttribute"/> may be assigned to the property's <see cref="System.Type"/> or to the property declaration itself, 
  ///     in case a property specific behavior is required.
  ///   </para><para>
  ///     See <see cref="ReferenceProperty.SearchAvailableObjects"/> on how to use the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  ///   </para>
  /// </remarks>
  /// <seealso cref="ISearchAvailableObjectsService"/>
  /// <seealso cref="ReferenceProperty"/>
  /// <seealso cref="ReferenceProperty.SearchAvailableObjects"/>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class SearchAvailableObjectsServiceTypeAttribute : Attribute, IBusinessObjectServiceTypeAttribute<ISearchAvailableObjectsService>
  {
    private readonly Type _type;

    public SearchAvailableObjectsServiceTypeAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISearchAvailableObjectsService));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}
