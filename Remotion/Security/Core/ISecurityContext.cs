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

namespace Remotion.Security
{
  /// <summary>
  /// The <see cref="ISecurityContext"/> interface collects all data required for evaluating the permissions a user has for an object.
  /// </summary>
  /// <remarks>
  /// Use the the <see cref="ISecurityContextFactory"/> to create an instance of an object implementing <see cref="ISecurityContext"/>. This factory 
  /// is typically used by the <see cref="ISecurityStrategy"/>'s <see cref="ISecurityStrategy.HasAccess"/> method to create a
  /// security context for a security query.
  /// </remarks>
  public interface ISecurityContext
  {
    /// <summary>
    /// Gets the type name of the <see cref="ISecurableObject"/> for which the permissions are to be evaluated by the <see cref="ISecurityStrategy"/>.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that uniquely identifies the type of the <see cref="ISecurableObject"/> to the <see cref="IServiceProvider"/>.
    /// </value>
    /// <remarks>
    /// The default implementation of the security infrastructure in the re-motion framework uses partial assembly-qualified type names to identify 
    /// the <see cref="Class"/> of the <see cref="ISecurableObject"/>. This results from the sole reliance on reflection information for extracting 
    /// the  security metadata. If a different means of generating the security metadata is used, it is perfectly legal to use strings of any format
    /// for the <see cref="Class"/>. The only requirement is that the <see cref="IServiceProvider"/> can find the <see cref="Class"/> within its 
    /// datastore. Otherwise, the security query will fail and access will be denied.
    /// </remarks>
    string Class { get; }

    /// <summary>
    /// Gets the name of the user that owns the <see cref="ISecurableObject"/>.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that uniquely identifies the owning user of the <see cref="ISecurableObject"/> to the <see cref="ISecurityProvider"/>.
    /// This property may return <see langword="null" />.
    /// </value>
    string Owner { get; }

    /// <summary>
    /// Gets the name of the group that owns the <see cref="ISecurableObject"/>.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that uniquely identifies the owning group of the <see cref="ISecurableObject"/> to the <see cref="ISecurityProvider"/>.
    /// This property may return <see langword="null" />.
    /// </value>
    string OwnerGroup { get; }

    /// <summary>
    /// Gets the name of the tenant that owns the <see cref="ISecurableObject"/>.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that uniquely identifies the owning tenant of the <see cref="ISecurableObject"/> to the <see cref="ISecurityProvider"/>.
    /// This property may return <see langword="null" />.
    /// </value>
    string OwnerTenant { get; }

    /// <summary>
    /// Gets the list of abstract roles the current user has in regards to the <see cref="ISecurableObject"/>.
    /// </summary>
    /// <value>An <see cref="EnumWrapper"/> sequence containing zero or more abstract role identifiers.</value>
    IEnumerable<EnumWrapper> AbstractRoles { get; }

    //TODO: MK: Determine if null is allowed for the state of a property.
    /// <summary>
    /// Retrieves the value assigned to the specified property of the <see cref="ISecurableObject"/>.
    /// </summary>
    /// <param name="propertyName">
    /// A <see cref="string"/> identifying a property of the <see cref="Class"/> that contains state relevant to evaluating 
    /// the permissions of the object. Must not be <see langword="null" /> or empty.
    /// </param>
    /// <returns>
    /// An <see cref="EnumWrapper"/> whose value uniquely identifies the state to the <see cref="ISecurityProvider"/>.
    /// </returns>
    /// <remarks>
    /// Use <see cref="ContainsState"/> to safely test if the security context contains a property with the specified <paramref name="propertyName"/>.    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if the security context does not contain a property with the specified <paramref name="propertyName"/>.
    /// </exception>
    EnumWrapper GetState (string propertyName);

    /// <summary>
    /// Tests whether the <paramref name="propertyName"/> represents a state property defined for the <see cref="Class"/>.
    /// </summary>
    /// <param name="propertyName">
    /// A <see cref="string"/> identifying a property of the <see cref="Class"/> that contains state relevant to evaluating 
    /// the permissions of the object. Must not be <see langword="null" /> or empty.
    /// </param>
    /// <returns><see langword="true" /> if <see cref="GetState"/> can be safely invoked with the <paramref name="propertyName"/>.</returns>
    bool ContainsState (string propertyName);

    //TODO: MK: Determine whether this should actually map to the concept of a stateless security context
    /// <summary>
    /// Gets a flag describing whether the context contains any state properties.
    /// </summary>
    bool IsStateless { get; }

    /// <summary>
    /// Gets the total number of state properties defined for the <see cref="Class"/>.
    /// </summary>
    /// <returns>A positive <see cref="int"/>.</returns>
    int GetNumberOfStates ();
  }
}
