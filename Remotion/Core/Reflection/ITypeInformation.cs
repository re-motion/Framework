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
using System.Reflection;
using JetBrains.Annotations;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides information about a type.
  /// </summary>
  public interface ITypeInformation : IMemberInformation
  {
    /// <summary>
    /// Gets the fully qualified name of the type, including the namespace of the type but not the assembly.
    /// </summary>
    /// <returns>
    /// The fully qualified name of the type, including the namespace of the type but not the assembly; 
    /// or <see langword="null"/> if the current instance represents a generic type parameter, an array type, pointer type, 
    /// or byref type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.
    /// </returns>
    [CanBeNull]string? FullName { get; }

    /// <summary>
    /// Gets the namespace of the type.
    /// </summary>
    /// <returns>
    /// The namespace of the type, or <see langword="null"/> if the current instance represents a generic parameter.
    /// </returns>
    [CanBeNull]string? Namespace { get; }

    /// <summary>
    /// Gets the assembly-qualified name of the type, which includes the name of the assembly from which the type was loaded.
    /// </summary>
    /// <returns>
    /// The assembly-qualified name of the type, which includes the name of the assembly from which the type was loaded,
    /// or <see langword="null"/> if the current instance represents a generic type parameter.
    /// </returns>
    [CanBeNull]string? AssemblyQualifiedName { get; }

    /// <summary>
    /// Gets the <see cref="System.Reflection.Assembly"/> in which the type is declared. For generic types,
    /// gets the <see cref="System.Reflection.Assembly"/> in which the generic type is defined.
    /// </summary>
    /// <returns>
    /// An <see cref="System.Reflection.Assembly"/> instance that describes the assembly containing the current type.
    /// For generic types, the instance describes the assembly that contains the generic type definition, not the assembly that creates and uses a particular constructed type.
    /// </returns>
    [CanBeNull]Assembly? Assembly { get; }

        /// <summary>
    /// Gets the type that declares the current nested type or generic type parameter.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object representing the enclosing type, if the current type is a nested type; or the generic type definition, 
    /// if the current type is a type parameter of a generic type; or the type that declares the generic method, 
    /// if the current type is a type parameter of a generic method; otherwise, <see langword="null"/>.
    /// </returns>
    [CanBeNull]new ITypeInformation? DeclaringType { get; }

    /// <summary>
    /// Gets a value indicating whether the type is a class; that is, not a value type or interface.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is a class; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsClass { get; }

    /// <summary>
    /// Gets a value indicating whether the type is a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is a value type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsValueType { get; }

    /// <summary>
    /// Gets a value indicating whether the type is an interface; that is, not a class or a value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is an interface; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsInterface { get; }

    /// <summary>
    /// Gets a value indicating whether the type is an array.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is an array; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsArray { get; }

    /// <summary>
    /// Gets the number of dimensions in an <see cref="Array"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="Int32"/> containing the number of dimensions in the current Type.
    /// </returns>
    /// <exception cref="NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead.</exception>
    /// <exception cref="ArgumentException">The current Type is not an array.</exception>
    int GetArrayRank ();

    /// <summary>
    /// Returns a type object representing an array of the current type, with the specified number of dimensions.
    /// </summary>
    /// <returns>
    /// A type object representing an array of the current type, with the specified number of dimensions.
    /// </returns>
    /// <param name="rank">The number of dimensions for the array.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="rank"/> is invalid. For example, 0 or negative.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    [NotNull]ITypeInformation MakeArrayType (int rank);

    /// <summary>
    /// Returns a type object representing a one-dimensional array of the current type, with a lower bound of zero.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object representing a one-dimensional array of the current type, with a lower bound of zero.
    /// </returns>
    [NotNull]ITypeInformation MakeArrayType ();

    /// <summary>
    /// Gets a value indicating whether the current type represents an enumeration.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current type represents an enumeration; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsEnum { get; }

    /// <summary>
    /// Returns a type object that represents the underlying type for the enumeration.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object representing the underlying type for the enumeration.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type is not an enumeration. That is, <see cref="IsEnum"/> returns <see langword="false"/>.</exception>
    [NotNull]ITypeInformation GetUnderlyingTypeOfEnum ();

    /// <summary>
    /// Gets a value indicating whether the current type represents a nullable value type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current type represents a nullable value type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsNullableValueType { get; }

    /// <summary>
    /// Returns a <see cref="ITypeInformation"/> object that represents the underlying type argument for the <see cref="Nullable{T}"/>.
    /// <note type="caution">This method throws if <see cref="IsNullableValueType"/> returns <see langword="false" />, 
    /// which differs from the behavior <see cref="Nullable.GetUnderlyingType"/>, which would return <see langword="null" />.</note>
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object representing the underlying type argument for the <see cref="Nullable{T}"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The current type is not an instantiation of <see cref="Nullable{T}"/>. That is, <see cref="IsNullableValueType"/> returns <see langword="false"/>.
    /// </exception>
    [NotNull]ITypeInformation GetUnderlyingTypeOfNullableValueType ();

    /// <summary>
    /// Gets a value indicating whether the type is a pointer.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is a pointer; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsPointer { get; }

    /// <summary>
    /// Returns a type object that represents a pointer to the current type.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object that represents a pointer to the current type.
    /// </returns>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    [NotNull]ITypeInformation MakePointerType ();

    /// <summary>
    /// Gets a value indicating whether the type is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsByRef { get; }

    /// <summary>
    /// Returns a <see cref="ITypeInformation"/> object that represents the current type when passed as a ref parameter.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object that represents the current type when passed as a ref parameter.
    /// </returns>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    [NotNull]ITypeInformation MakeByRefType ();

    /// <summary>
    /// Gets a value indicating whether the type is declared sealed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is declared sealed; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsSealed { get; }

    /// <summary>
    /// Gets a value indicating whether the type is abstract and must be overridden.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is abstract; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsAbstract { get; }

    /// <summary>
    /// Gets a value indicating whether the current type object represents a type whose definition is nested inside the definition of another type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is nested inside another type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsNested { get; }

    /// <summary>
    /// Gets a value indicating whether the current type encompasses or refers to another type; 
    /// that is, whether the current type is an array, a pointer, or is passed by reference.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type is an array, a pointer, or is passed by reference; otherwise, <see langword="false"/>.
    /// </returns>
    bool HasElementType { get; }

    /// <summary>
    /// When overridden in a derived class, returns the <see cref="ITypeInformation"/> of the object encompassed or referred to by the current array, pointer or reference type.
    /// </summary>
    /// <returns>
    /// The <see cref="ITypeInformation"/> of the object encompassed or referred to by the current array, pointer, or reference type, 
    /// or <see langword="null"/> if the current type is not an array or a pointer, or is not passed by reference, 
    /// or represents a generic type or a type parameter in the definition of a generic type or generic method.
    /// </returns>
    [CanBeNull]ITypeInformation? GetElementType ();

    /// <summary>
    /// Gets a value indicating whether the current type is a generic type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current type is a generic type; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericType { get; }

    /// <summary>
    /// Gets a value indicating whether the current type represents a generic type definition, from which other generic types can be constructed.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type object represents a generic type definition; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericTypeDefinition { get; }

    /// <summary>
    /// Returns a <see cref="ITypeInformation"/> object that represents a generic type definition from which the current generic type can be constructed.
    /// </summary>
    /// <returns>
    /// A <see cref="ITypeInformation"/> object representing a generic type from which the current type can be constructed.
    /// </returns>
    /// <exception cref="InvalidOperationException">The current type is not a generic type. That is, <see cref="IsGenericType"/> returns <see langword="false"/>.</exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
    [NotNull]ITypeInformation GetGenericTypeDefinition ();

    /// <summary>
    /// Gets a value indicating whether the current type object has type parameters that have not been replaced by specific types.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type object is itself a generic type parameter or has type parameters for which specific types have not been supplied;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsGenericParameters { get; }

    /// <summary>
    /// Returns an array of <see cref="ITypeInformation"/> objects that represent the type arguments of a generic type or the type parameters of a generic type definition.
    /// </summary>
    /// <returns>
    /// An array of <see cref="ITypeInformation"/> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.
    /// </returns>
    [NotNull]ITypeInformation[] GetGenericArguments ();

    /// <summary>
    /// Gets a value indicating whether the current type represents a type parameter in the definition of a generic type or method.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGenericParameter { get; }

    /// <summary>
    /// Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, 
    /// when the type object represents a type parameter of a generic type or a generic method.
    /// </summary>
    /// <returns>
    /// The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The current type does not represent a type parameter. That is, <see cref="IsGenericParameter"/> returns <see langword="false"/>.
    /// </exception>
    int GenericParameterPosition { get; }

    /// <summary>
    /// Returns an array of <see cref="ITypeInformation"/> objects that represent the constraints on the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// An array of <see cref="ITypeInformation"/> objects that represent the constraints on the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The current type object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.
    /// </exception>
    [NotNull]ITypeInformation[] GetGenericParameterConstraints ();

    /// <summary>
    /// Gets a combination of <see cref="GenericParameterAttributes"/> flags that describe the covariance and special constraints of the current generic type parameter. 
    /// </summary>
    /// <returns>
    /// A bitwise combination of <see cref="GenericParameterAttributes"/> values that describes the covariance and special constraints of the current generic type parameter.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The current type object is not a generic type parameter. That is, the <see cref="IsGenericParameter"/> property returns <see langword="false"/>.
    /// </exception>
    /// <exception cref="NotSupportedException">The invoked method is not supported in the base class.</exception>
    GenericParameterAttributes GenericParameterAttributes { get; }

    /// <summary>
    /// Gets the <see cref="ITypeInformation"/> object from which the current type directly inherits.
    /// </summary>
    /// <returns>
    /// The <see cref="ITypeInformation"/> from which the current type directly inherits, 
    /// or <see langword="null" /> if the current Type represents the <see cref="Object"/> class or an interface.
    /// </returns>
    [CanBeNull]ITypeInformation? BaseType { get; }

    /// <summary>
    /// Determines whether the specified object <paramref name="o"/> is an instance of the current type.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current Type is in the inheritance hierarchy of the object represented by <paramref name="o"/>, 
    /// or if the current Type is an interface that <paramref name="o"/> supports. <see langword="false"/> if neither of these conditions is the case, 
    /// or if <paramref name="o"/> is <see langword="null"/>, or if the current Type is an open generic type 
    /// (that is, <see cref="ContainsGenericParameters"/> returns <see langword="true"/>).
    /// </returns>
    /// <param name="o">The object to compare with the current <see cref="ITypeInformation"/>. </param>
    bool IsInstanceOfType ([CanBeNull]object? o);

    /// <summary>
    /// Determines whether the class represented by the current type derives from the class represented by <paramref name="c"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the Type represented by the <paramref name="c"/> parameter and the current type represent classes, 
    /// and the class represented by the current Type derives from the class represented by <paramref name="c"/>; otherwise, <see langword="false"/>. 
    /// This method also returns <see langword="false"/> if <paramref name="c"/> and the current type represent the same class.
    /// In addition, the implementation of <paramref name="c"/> must match the implementation of this <see cref="ITypeInformation"/>,
    /// otherwise this method will also return <see langword="false" />.
    /// </returns>
    /// <param name="c">The <see cref="ITypeInformation"/> to compare with the current <see cref="ITypeInformation"/>. </param>
    /// <exception cref="ArgumentNullException">The <paramref name="c"/> parameter is <see langword="null"/>. </exception>
    bool IsSubclassOf ([NotNull]ITypeInformation c);

    /// <summary>
    /// Determines whether an instance of the current type can be assigned from an instance of the type represented by <paramref name="c"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="c"/> and the current Type represent the same type, 
    /// or if the current Type is in the inheritance hierarchy of <paramref name="c"/>, 
    /// or if the current Type is an interface that <paramref name="c"/> implements, 
    /// or if <paramref name="c"/> is a generic type parameter and the current Type represents one of the constraints of <paramref name="c"/>. 
    /// <see langword="false"/> if none of these conditions are <see langword="true"/>, or if <paramref name="c"/> is <see langword="null"/>,
    /// or the implementation of <paramref name="c"/> does not match the implementation of this <see cref="ITypeInformation"/>.
    /// </returns>
    /// <param name="c">The <see cref="ITypeInformation"/> to compare with the current <see cref="ITypeInformation"/>. </param>
    bool IsAssignableFrom ([NotNull]ITypeInformation c);

    /// <summary>
    /// Determines whether the instance of the current type can be ascribed to the instance of the type represented by <paramref name="c"/>.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the type represented by the <paramref name="c"/> parameter and the current type represent the same type,
    /// or if <paramref name="c"/> is in the inheritance hierarchy of the current type, 
    /// or the current type is a closed version of the generic type represented by <paramref name="c"/>; otherwise, <see langword="false"/>. 
    /// This method also returns <see langword="false"/> if the implementation of <paramref name="c"/> 
    /// does not match the implementation of this <see cref="ITypeInformation"/>.
    /// </returns>
    /// <param name="c">The <see cref="ITypeInformation"/> to compare with the current <see cref="ITypeInformation"/>. </param>
    /// <exception cref="ArgumentNullException">The <paramref name="c"/> parameter is <see langword="null"/>. </exception>
    bool CanAscribeTo ([NotNull]ITypeInformation c);

    /// <summary>
    /// Returns the ascribed type arguments for the type represented by <paramref name="c"/> as inherited or implemented by the current type.
    /// </summary>
    /// <returns>An array of <see cref="ITypeInformation"/> objects containing the generic arguments of the type 
    /// represented by <paramref name="c"/> as it is inherited or implemented by the current type.
    /// </returns>
    /// <param name="c">The <see cref="ITypeInformation"/> to retrieve the ascribed generic arguments for Must not be <see langword="null" />.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the type represented by <paramref name="c"/> is not equal to the current type, 
    /// or if <paramref name="c"/> is in the inheritance hierarchy of the current type,
    /// or if the implementation of <paramref name="c"/> does not match the implementation of the current <see cref="ITypeInformation"/> object.
    /// </exception>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown if the current type is an interface and implements the interface represented by <paramref name="c"/> or its instantiations
    /// more than once.
    /// </exception>
    [NotNull]ITypeInformation[] GetAscribedGenericArgumentsFor ([NotNull]ITypeInformation c);
  }
}
