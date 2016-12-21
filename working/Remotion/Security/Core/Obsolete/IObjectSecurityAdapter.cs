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
using Microsoft.Practices.ServiceLocation;
using Remotion.Reflection;

// ReSharper disable once CheckNamespace
namespace Remotion.Security
{
  /// <summary>Defines an adapter between the security layer and the business object implementation.</summary>
  /// <remarks>
  /// It is registered in the <see cref="ServiceLocator"/> and is used for security checks 
  /// in implementations of <see cref="I:Remotion.ObjectBinding.IBusinessObjectProperty"/>.
  /// <note type="inotes">
  /// A typical implementation uses a <see cref="T:Remotion.Security.SecurityClient"/> that further dispatches to an 
  /// <see cref="IObjectSecurityStrategy"/> retrieved from the <see cref="ISecurableObject"/>.
  /// </note>
  /// </remarks>
  [Obsolete (
      "The ObjectSecurityAdapter extension point is no longer supported. "
      + "Use implementations of the Remotion.ObjectBinding.BindableObject.IBindablePropertyReadAccessStrategy and Remotion.ObjectBinding.BindableObject.IBindablePropertyWriteAccessStrategy instead. (Version 1.15.20.0)",
      true)]
  public interface IObjectSecurityAdapter
  {
    /// <summary>Determines whether read access to a property of <paramref name="securableObject"/> is granted.</summary>
    /// <param name="securableObject">The <see cref="ISecurableObject"/> whose permissions are checked.</param>
    /// <param name="propertyInformation">The property for which the permissions are checked.</param>
    /// <returns><see langword="true"/> when the property value can be retrieved.</returns>
    /// <remarks>If access is denied, the property is hidden in the UI.</remarks>
    bool HasAccessOnGetAccessor (ISecurableObject securableObject, IPropertyInformation propertyInformation);

    /// <summary>Determines whether write access to a property of <paramref name="securableObject"/> is granted.</summary>
    /// <param name="securableObject">The <see cref="ISecurableObject"/> whose permissions are checked.</param>
    /// <param name="propertyInformation">The property for which the permissions are checked.</param>
    /// <returns><see langword="true"/> when the property can be changed.</returns>
    /// <remarks>If access is denied, the property is displayed as read-only in the UI.</remarks>
    bool HasAccessOnSetAccessor (ISecurableObject securableObject, IPropertyInformation propertyInformation);
  }
}
