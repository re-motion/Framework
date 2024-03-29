﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
  ///   The <see cref="DeleteObjectServiceTypeAttribute"/> declares the spefic implementation of <see cref="IDeleteObjectService"/> to be used by the 
  ///   <see cref="ReferenceProperty"/>'s <see cref="ReferenceProperty.Delete"/> method when deleting an <see cref="IBusinessObject"/> instance.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The <see cref="DeleteObjectServiceTypeAttribute"/> may be assigned to the property's <see cref="System.Type"/> or to the property declaration itself, 
  ///     in case a property specific behavior is required.
  ///   </para><para>
  ///     See <see cref="ReferenceProperty.Delete"/> on how to use the <see cref="DeleteObjectServiceTypeAttribute"/>.
  ///   </para>
  /// </remarks>
  /// <seealso cref="IDeleteObjectService"/>
  /// <seealso cref="ReferenceProperty"/>
  /// <seealso cref="ReferenceProperty.Delete"/>
  [Obsolete("The delete-object feature is not supported. (Version 1.13.142)")]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class DeleteObjectServiceTypeAttribute : Attribute, IBusinessObjectServiceTypeAttribute<IDeleteObjectService>
  {
    private readonly Type _type;

    public DeleteObjectServiceTypeAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(IDeleteObjectService));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }
  }
}
