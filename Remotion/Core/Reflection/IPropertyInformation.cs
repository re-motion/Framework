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
using System.Reflection;
using JetBrains.Annotations;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides information about a property and offers a way to get or set the property's value.
  /// </summary>
  public interface IPropertyInformation : IMemberInformation
  {
    /// <summary>
    /// Gets the type of the property, i.e. the type of values the property can store.
    /// </summary>
    /// <value>The type of the property.</value>
    [NotNull]Type PropertyType { get; }
    
    /// <summary>
    /// Determines whether the property can be set from the outside.
    /// </summary>
    /// <value>True if this instance has can be set from the outside; otherwise, false.</value>
    bool CanBeSetFromOutside { get; }

    /// <summary>
    /// Gets the value of the property for the given instance.
    /// </summary>
    /// <param name="instance">The instance to retrieve the value for, or <see langword="null"/> for a static property.</param>
    /// <param name="indexParameters">
    /// The index parameters to be used for property value retrieval. 
    /// For non-indexed properties, this should be <see langword="null" />.
    /// </param>
    /// <returns>The property's value for the given instance.</returns>
    /// <exception cref="ArgumentException">
    /// <para>The type of the elements of the <paramref name="indexParameters"/> array does not match the index argument types expected by the
    /// property.</para>
    /// <para>-or-</para>
    /// <para>The get accessor cannot be found.</para>
    /// </exception>
    /// <exception cref="TargetException">The <paramref name="instance"/> parameter is <see langword="null"/> although the property is not a static
    /// property or it does not match the property's declaring type.</exception>
    /// <exception cref="TargetParameterCountException">The number of items in the <paramref name="indexParameters"/> array does not match the number
    /// of index parameters expected by the property.</exception>
    /// <exception cref="TargetInvocationException">The property's get method throw an exception, see the <see cref="Exception.InnerException"/>
    /// property.</exception>
    /// <exception cref="MethodAccessException">The accessor was private or protected and could not be executed.</exception>
    object GetValue ([CanBeNull]object instance, [CanBeNull]object[] indexParameters);

    /// <summary>
    /// Sets the value of the property for the given instance.
    /// </summary>
    /// <param name="instance">The instance to set the value for, or <see langword="null"/> for a static property.</param>
    /// <param name="value">The property's value for the given instance.</param>
    /// <param name="indexParameters">
    /// The index parameters to be used for setting the property value.
    /// For non-indexed properties, this should be <see langword="null" />.
    /// </param>
    /// <exception cref="ArgumentException">
    /// <para>The type of the elements of the <paramref name="indexParameters"/> array does not match the index argument types expected by the
    /// property.</para>
    /// <para>-or-</para>
    /// <para>The set accessor cannot be found.</para>
    /// </exception>
    /// <exception cref="TargetException">The <paramref name="instance"/> parameter is <see langword="null"/> although the property is not a static
    /// property or it does not match the property's declaring type.</exception>
    /// <exception cref="TargetParameterCountException">The number of items in the <paramref name="indexParameters"/> array does not match the number
    /// of index parameters expected by the property.</exception>
    /// <exception cref="TargetInvocationException">The property's get method throw an exception, see the <see cref="Exception.InnerException"/>
    /// property.</exception>
    /// <exception cref="MethodAccessException">The accessor was private or protected and could not be executed.</exception>
    void SetValue ([CanBeNull]object instance, [CanBeNull]object value, [CanBeNull]object[] indexParameters);

    /// <summary>
    /// Gets the <see cref="IMethodInformation"/> of the get method for the current <see cref="PropertyInfo"/>.
    /// </summary>
    /// <param name="nonPublic">Indicates whether a non-public accessor method may also be returned.</param>
    /// <returns>
    /// An instance of <see cref="IMethodInformation"/> for the get method.
    /// </returns>
    [CanBeNull]IMethodInformation GetGetMethod (bool nonPublic);

    /// <summary>
    /// Gets the <see cref="IMethodInformation"/> of the set method for the current <see cref="PropertyInfo"/>.
    /// </summary>
    /// <param name="nonPublic">Indicates whether a non-public accessor method may also be returned.</param>
    /// <returns>
    /// An instance of <see cref="IMethodInformation"/> for the set method.
    /// </returns>
    [CanBeNull]IMethodInformation GetSetMethod (bool nonPublic);

    /// <summary>
    /// Finds the implementation <see cref="IPropertyInformation"/> corresponding to this <see cref="IPropertyInformation"/> on the given 
    /// <see cref="Type"/>. This <see cref="IPropertyInformation"/> object must denote an interface property.
    /// </summary>
    /// <param name="implementationType">The type to search for an implementation of this <see cref="IPropertyInformation"/> on.</param>
    /// <returns>An instance of <see cref="IPropertyInformation"/> describing the property implementing this interface 
    /// <see cref="IPropertyInformation"/> on <paramref name="implementationType"/>, or <see langword="null" /> if the 
    /// <paramref name="implementationType"/> does not implement the interface.</returns>
    /// <exception cref="ArgumentException"><paramref name="implementationType"/> is itself an interface.</exception>
    /// <exception cref="InvalidOperationException">This <see cref="IPropertyInformation"/> does not describe an interface property.</exception>
    [CanBeNull]IPropertyInformation FindInterfaceImplementation ([NotNull]Type implementationType);

    /// <summary>
    /// Finds the interface declaration for this <see cref="IPropertyInformation"/>, returning <see langword="null" /> if this 
    /// <see cref="IPropertyInformation"/> is not an implementation of an interface member.
    /// </summary>
    /// <returns>An <see cref="IPropertyInformation"/> for the interface member this <see cref="IPropertyInformation"/> implements, or 
    /// <see langword="null" /> if this <see cref="IPropertyInformation"/> is not an implementation of an interface member.</returns>
    /// <exception cref="InvalidOperationException">This <see cref="IPropertyInformation"/> is itself an interface member, so it cannot have an 
    /// interface declaration.</exception>
    [NotNull]IEnumerable<IPropertyInformation> FindInterfaceDeclarations ();

    [NotNull]ParameterInfo[] GetIndexParameters ();

    [NotNull]IMethodInformation[] GetAccessors (bool nonPublic);

    /// <summary>
    /// Gets the <see cref="IPropertyInformation"/> corresponding to the property declared by the type returned via <see cref="IMemberInformation.GetOriginalDeclaringType()"/>.
    /// </summary>
    /// <returns>An instance of <see cref="IPropertyInformation"/>.</returns>
    /// <exception cref="MissingMethodException">
    /// This <see cref="IPropertyInformation"/> does not represent the original declaration but the original declaration could not be discovered.
    /// </exception>
    [NotNull]IPropertyInformation GetOriginalDeclaration ();
  }
}
