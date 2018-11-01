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
  /// Provides information about a method and offers a way to invoke the method.
  /// </summary>
  public interface IMethodInformation : IMemberInformation
  {
    /// <summary>
    /// Gets the return type of the method.
    /// </summary>
    /// <value>The return type of the method.</value>
    [NotNull]Type ReturnType { get; }

    /// <summary>
    /// Invokes the method on the given instance using the given parameters.
    /// </summary>
    /// <param name="instance">The instance on which to invoke the method. If the method is static this argument is ignored.</param>
    /// <param name="parameters">An argument list for the invoked method. If there are no parameters, this should be <see langword="null" />.</param>
    /// <returns>An object containing the return value of the invoked method.</returns>
    [CanBeNull]object Invoke ([CanBeNull]object instance, [CanBeNull]object[] parameters);

    /// <summary>
    /// Finds the implementation <see cref="IMethodInformation"/> corresponding to this <see cref="IMethodInformation"/> on the given 
    /// <see cref="Type"/>. This <see cref="IMethodInformation"/> object must denote an interface method.
    /// </summary>
    /// <param name="implementationType">The type to search for an implementation of this <see cref="IMethodInformation"/> on.</param>
    /// <returns>An instance of <see cref="IMethodInformation"/> describing the method implementing this interface 
    /// <see cref="IMethodInformation"/> on <paramref name="implementationType"/>, or <see langword="null" /> if the 
    /// <paramref name="implementationType"/> does not implement the interface.</returns>
    /// <exception cref="ArgumentException">The <paramref name="implementationType"/> is itself an interface.</exception>
    /// <exception cref="InvalidOperationException">This <see cref="IMethodInformation"/> does not describe an interface method.</exception>
    [CanBeNull]IMethodInformation FindInterfaceImplementation (Type implementationType);

    /// <summary>
    /// Finds the property declaration corresponding to this <see cref="IMethodInformation"/> on the given <see cref="Type"/> and it's base types.
    /// </summary>
    /// <returns>Returns the <see cref="IPropertyInformation"/> of the declared property, or <see langword="null" /> if no corresponding property was 
    /// found.</returns>
    [CanBeNull]IPropertyInformation FindDeclaringProperty ();

    /// <summary>
    /// Finds the interface declaration for this <see cref="IMethodInformation"/>, returning <see langword="null" /> if this 
    /// <see cref="IMethodInformation"/> is not an implementation of an interface member.
    /// </summary>
    /// <returns>An <see cref="IMethodInformation"/> for the interface member this <see cref="IMethodInformation"/> implements, or 
    /// <see langword="null" /> if this <see cref="IMethodInformation"/> is not an implementation of an interface member.</returns>
    /// <exception cref="InvalidOperationException">This <see cref="IMethodInformation"/> is itself an interface member, so it cannot have an 
    /// interface declaration.</exception>
    [NotNull]IEnumerable<IMethodInformation> FindInterfaceDeclarations ();

    /// <summary>
    /// Returns a delegate invoking the method described by this <see cref="IMethodInformation"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The delegate type. The number of parameters and the fact whether or not a return type is present must match the signature of the method. The
    /// types need not match exactly; the values will be converted at run-time. 
    /// </typeparam>
    /// <returns>A delegate of type <typeparamref name="T"/> invoking the method described by this <see cref="IMethodInformation"/>.</returns>
    [NotNull]T GetFastInvoker<T> () where T: class;

    /// <summary>
    /// Returns a delegate invoking the method described by this <see cref="IMethodInformation"/>.
    /// </summary>
    /// <param name="delegateType">
    /// The delegate type. The number of parameters and the fact whether or not a return type is present must match the signature of the method. The
    /// types need not match exactly; the values will be converted at run-time. 
    /// </param>
    /// <returns>A delegate of type <paremref name="delegateType"/> invoking the method described by this <see cref="IMethodInformation"/>.</returns>
    [NotNull]Delegate GetFastInvoker ([NotNull]Type delegateType);

    [NotNull]ParameterInfo[] GetParameters ();

    /// <summary>
    /// Gets the <see cref="IMethodInformation"/> corresponding to the method declared by type returned via <see cref="IMemberInformation.GetOriginalDeclaringType()"/>.
    /// </summary>
    /// <returns>An instance of <see cref="IMethodInformation"/>.</returns>
    [NotNull]IMethodInformation GetOriginalDeclaration ();
  }
}