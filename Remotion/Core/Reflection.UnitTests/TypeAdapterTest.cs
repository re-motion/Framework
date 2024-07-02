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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class TypeAdapterTest
  {
// ReSharper disable PossibleInterfaceMemberAmbiguity
    private interface IDoubleInheritingGenericInterface : IEnumerable<int>, IEnumerable<string>
// ReSharper restore PossibleInterfaceMemberAmbiguity
    {
    }

    private class GenericWithParameterConstraint<T>
        where T : Exception
    {
    }

    [Test]
    public void Create_ReturnsSameInstance ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type), Is.SameAs(TypeAdapter.Create(type)));
    }

    [Test]
    public void CreateOrNull_WithType_ReturnsSameInstanceAsCreate ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.CreateOrNull(type), Is.SameAs(TypeAdapter.Create(type)));
    }

    [Test]
    public void CreateOrNull_WithTypeNull_ReturnsNull ()
    {
      Assert.That(TypeAdapter.CreateOrNull(null), Is.Null);
    }

    [Test]
    public void Type ()
    {
      var _type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(_type).Type, Is.SameAs(_type));
    }

    [Test]
    public void Name ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).Name, Is.EqualTo(type.Name));
    }

    [Test]
    public void FullName ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).FullName, Is.EqualTo(type.FullName));
    }

    [Test]
    public void Namespace ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).Namespace, Is.EqualTo(type.Namespace));
    }

    [Test]
    public void AssemblyQualifiedName ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).AssemblyQualifiedName, Is.EqualTo(type.AssemblyQualifiedName));
    }

    [Test]
    public void Assembly ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).Assembly, Is.SameAs(type.Assembly));
    }

    [Test]
    public void DeclaringType_NestedType ()
    {
      var type = typeof(Environment.SpecialFolder);
      Assert.That(TypeAdapter.Create(type).DeclaringType, Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.DeclaringType));
    }

    [Test]
    public void DeclaringType_NotNestedType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).DeclaringType, Is.Null);
    }

    [Test]
    public void GetOriginalDeclaringType_NestedType ()
    {
      var type = typeof(Environment.SpecialFolder);
      Assert.That(TypeAdapter.Create(type).GetOriginalDeclaringType(), Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.DeclaringType));
    }

    [Test]
    public void GetOriginalDeclaringType_NotNestedType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).GetOriginalDeclaringType(), Is.Null);
    }

    [Test]
    public void IsClass_ReferenceType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsClass, Is.EqualTo(type.IsClass).And.True);
    }

    [Test]
    public void IsClass_ValueType ()
    {
      var type = typeof(Guid);
      Assert.That(TypeAdapter.Create(type).IsClass, Is.EqualTo(type.IsClass).And.False);
    }

    [Test]
    public void IsInterface_Interface ()
    {
      var type = typeof(IList<>);
      Assert.That(TypeAdapter.Create(type).IsInterface, Is.EqualTo(type.IsInterface).And.True);
    }

    [Test]
    public void IsInterface_ReferenceType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsInterface, Is.EqualTo(type.IsInterface).And.False);
    }

    [Test]
    public void IsValueType_ReferenceType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsValueType, Is.EqualTo(type.IsValueType).And.False);
    }

    [Test]
    public void IsValueType_ValueType ()
    {
      var type = typeof(int);
      Assert.That(TypeAdapter.Create(type).IsValueType, Is.EqualTo(type.IsValueType).And.True);
    }

    [Test]
    public void IsArray_Array ()
    {
      var type = typeof(object[]);
      Assert.That(TypeAdapter.Create(type).IsArray, Is.EqualTo(type.IsArray).And.True);
    }

    [Test]
    public void IsArray_NotArray ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsArray, Is.EqualTo(type.IsArray).And.False);
    }

    [Test]
    public void GetArrayRank_Array ()
    {
      var type = typeof(object[,]);
      Assert.That(TypeAdapter.Create(type).GetArrayRank(), Is.EqualTo(type.GetArrayRank()).And.EqualTo(2));
    }

    [Test]
    public void GetArrayRank_NotArray ()
    {
      var type = typeof(ArrayList);
      Assert.That(()=>TypeAdapter.Create(type).GetArrayRank(), Throws.ArgumentException);
    }

    [Test]
    public void MakeArrayType_WithRank ()
    {
      var type = typeof(object);
      Assert.That(
          TypeAdapter.Create(type).MakeArrayType(2),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.MakeArrayType(2)));
    }

    [Test]
    public void MakeArrayType_WithInvalidRank ()
    {
      Assert.That(() => TypeAdapter.Create(typeof(object)).MakeArrayType(0), Throws.TypeOf<IndexOutOfRangeException>());
    }

    [Test]
    public void MakeArrayType_WithoutRank ()
    {
      var type = typeof(object);
      Assert.That(
          TypeAdapter.Create(type).MakeArrayType(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.MakeArrayType()));
    }

    [Test]
    public void IsEnum_Enum ()
    {
      var type = typeof(System.Reflection.MemberTypes);
      Assert.That(TypeAdapter.Create(type).IsEnum, Is.EqualTo(type.IsEnum).And.True);
    }

    [Test]
    public void IsEnum_NotEnum ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsEnum, Is.EqualTo(type.IsEnum).And.False);
    }

    [Test]
    public void GetUnderlyingTypeOfEnum_EnumType ()
    {
      var type = typeof(System.Reflection.MemberTypes);
      Assert.That(
          TypeAdapter.Create(type).GetUnderlyingTypeOfEnum(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(Enum.GetUnderlyingType(type)));
    }

    [Test]
    public void GetUnderlyingTypeOfEnum_NotEnumType ()
    {
      var type = typeof(ArrayList);
      Assert.That(
          () => TypeAdapter.Create(type).GetUnderlyingTypeOfEnum(),
          Throws.InvalidOperationException.And.Message.EqualTo("The type 'System.Collections.ArrayList' is not an enum type."));
    }

    [Test]
    public void IsNullableValueType_NullableValueType ()
    {
      var type = typeof(int?);
      Assert.That(TypeAdapter.Create(type).IsNullableValueType, Is.True);
    }

    [Test]
    public void IsNullableValueType_NotNullableValueType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsNullableValueType, Is.False);
    }

    [Test]
    public void GetUnderlyingTypeOfNullableValueType_NullableValueType ()
    {
      var type = typeof(int?);
      Assert.That(
          TypeAdapter.Create(type).GetUnderlyingTypeOfNullableValueType(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(typeof(int)));
    }

    [Test]
    public void GetUnderlyingTypeOfNullableValueType_NotNullableValueType ()
    {
      var type = typeof(ArrayList);
      Assert.That(
          () => TypeAdapter.Create(type).GetUnderlyingTypeOfNullableValueType(),
          Throws.InvalidOperationException.And.Message.EqualTo("The type 'System.Collections.ArrayList' is not a nullable value type."));
    }

    [Test]
    public void IsPointer_Pointer ()
    {
      var type = typeof(int).MakePointerType();
      Assert.That(TypeAdapter.Create(type).IsPointer, Is.EqualTo(type.IsPointer).And.True);
    }

    [Test]
    public void IsPointer_NotPointer ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsPointer, Is.EqualTo(type.IsPointer).And.False);
    }

    [Test]
    public void MakePointerType ()
    {
      var type = typeof(int);
      Assert.That(
          TypeAdapter.Create(type).MakePointerType(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.MakePointerType()));
    }

    [Test]
    public void IsByRef_ByRef ()
    {
      var type = typeof(int).MakeByRefType();
      Assert.That(TypeAdapter.Create(type).IsByRef, Is.EqualTo(type.IsByRef).And.True);
    }

    [Test]
    public void IsByRef_NotByRef ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsByRef, Is.EqualTo(type.IsByRef).And.False);
    }

    [Test]
    public void MakeByRefType ()
    {
      var type = typeof(int);
      Assert.That(
          TypeAdapter.Create(type).MakeByRefType(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.MakeByRefType()));
    }

    [Test]
    public void IsSealed_SealedType ()
    {
      var type = typeof(Activator);
      Assert.That(TypeAdapter.Create(type).IsSealed, Is.EqualTo(type.IsSealed).And.True);
    }

    [Test]
    public void IsSealed_NotSealedType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsSealed, Is.EqualTo(type.IsSealed).And.False);
    }

    [Test]
    public void IsAbstract_AbstractType ()
    {
      var type = typeof(Array);
      Assert.That(TypeAdapter.Create(type).IsAbstract, Is.EqualTo(type.IsAbstract).And.True);
    }

    [Test]
    public void IsAbstract_NotAbstractType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsAbstract, Is.EqualTo(type.IsAbstract).And.False);
    }

    [Test]
    public void IsNested_NestedType ()
    {
      var type = typeof(Environment.SpecialFolder);
      Assert.That(TypeAdapter.Create(type).IsNested, Is.EqualTo(type.IsNested).And.True);
    }

    [Test]
    public void IsNested_NotNestedType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsNested, Is.EqualTo(type.IsNested).And.False);
    }

    [Test]
    public void HasElementType_TypeWithElementType ()
    {
      var type = typeof(int[]);
      Assert.That(TypeAdapter.Create(type).HasElementType, Is.EqualTo(type.HasElementType).And.True);
    }

    [Test]
    public void HasElementType_TypeWithoutElementType ()
    {
      var type = typeof(int);
      Assert.That(TypeAdapter.Create(type).HasElementType, Is.EqualTo(type.HasElementType).And.False);
    }

    [Test]
    public void GetElementType_TypeWithElementType ()
    {
      var type = typeof(int[]);
      Assert.That(
          TypeAdapter.Create(type).GetElementType(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.GetElementType()));
    }

    [Test]
    public void GetElementType_TypeWithoutElementType ()
    {
      var type = typeof(int);
      Assert.That(TypeAdapter.Create(type).GetElementType(), Is.EqualTo(type.GetElementType()).And.Null);
    }

    [Test]
    public void IsGenericType_ClosedGenericType ()
    {
      var type = typeof(List<int>);
      Assert.That(TypeAdapter.Create(type).IsGenericType, Is.EqualTo(type.IsGenericType).And.True);
    }

    [Test]
    public void IsGenericType_NotGenericType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsGenericType, Is.EqualTo(type.IsGenericType).And.False);
    }

    [Test]
    public void IsGenericTypeDefinition_GenericTypeDefinition ()
    {
      var type = typeof(List<>);
      Assert.That(TypeAdapter.Create(type).IsGenericTypeDefinition, Is.EqualTo(type.IsGenericTypeDefinition).And.True);
    }

    [Test]
    public void IsGenericTypeDefinition_NotGenericTypeDefinition ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).IsGenericTypeDefinition, Is.EqualTo(type.IsGenericTypeDefinition).And.False);
    }

    [Test]
    public void GetGenericTypeDefintion_ClosedGenericType ()
    {
      var type = typeof(List<int>);
      Assert.That(
          TypeAdapter.Create(type).GetGenericTypeDefinition(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.GetGenericTypeDefinition()));
    }

    [Test]
    public void GetGenericTypeDefintion_GenericTypeDefinition ()
    {
      var type = typeof(List<>);
      Assert.That(
          TypeAdapter.Create(type).GetGenericTypeDefinition(),
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.GetGenericTypeDefinition()));
    }

    [Test]
    public void GetGenericTypeDefintion_NotGenericType ()
    {
      Assert.That(()=>
          TypeAdapter.Create(typeof(ArrayList)).GetGenericTypeDefinition(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void ContainsGenericParameters_OpenGenericType ()
    {
      var type = typeof(List<>);
      Assert.That(TypeAdapter.Create(type).ContainsGenericParameters, Is.EqualTo(type.ContainsGenericParameters).And.True);
    }

    [Test]
    public void ContainsGenericParameters_ClosedGenericType ()
    {
      var type = typeof(List<int>);
      Assert.That(TypeAdapter.Create(type).ContainsGenericParameters, Is.EqualTo(type.ContainsGenericParameters).And.False);
    }

    [Test]
    public void ContainsGenericParameters_NotGenericType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).ContainsGenericParameters, Is.EqualTo(type.ContainsGenericParameters).And.False);
    }

    [Test]
    public void GetGenericArguments_ClosedGenericType ()
    {
      var type = typeof(List<int>);
      Assert.That(TypeAdapter.Create(type).GetGenericArguments(), Has.All.TypeOf<TypeAdapter>());
      Assert.That(TypeAdapter.Create(type).GetGenericArguments(), Is.EqualTo(new[] { TypeAdapter.Create(typeof(int)) }));
    }

    [Test]
    public void GetGenericArguments_NotGenericType ()
    {
      var type = typeof(ArrayList);
      Assert.That(TypeAdapter.Create(type).GetGenericArguments(), Is.Empty);
    }

    [Test]
    public void IsGenericParameter_GenericParameter ()
    {
      var type = typeof(Nullable<>).GetGenericArguments().Single();

      Assert.That(TypeAdapter.Create(type).IsGenericParameter, Is.EqualTo(type.IsGenericParameter).And.True);
    }

    [Test]
    public void IsGenericParameter_NotGenericParameter ()
    {
      var type = typeof(ValueType);

      Assert.That(TypeAdapter.Create(type).IsGenericParameter, Is.EqualTo(type.IsGenericParameter).And.False);
    }

    [Test]
    public void GetGenericParameterConstraints_GenericParameter ()
    {
      var type = typeof(GenericWithParameterConstraint<>).GetGenericArguments().Single();
      Assert.That(type.GetGenericParameterConstraints(), Is.EqualTo(new[] { typeof(Exception) }));

      Assert.That(TypeAdapter.Create(type).GetGenericParameterConstraints(), Has.All.TypeOf<TypeAdapter>());
      Assert.That(
          TypeAdapter.Create(type).GetGenericParameterConstraints(),
          Is.EqualTo(new[] { TypeAdapter.Create(typeof(Exception)) }));
    }

    [Test]
    public void GetGenericParameterConstraints_NotGenericParameter ()
    {
      Assert.That(() =>
          TypeAdapter.Create(typeof(ValueType)).GetGenericTypeDefinition(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GenericParameterPosition_GenericParameter ()
    {
      var type = typeof(Tuple<,>).GetGenericArguments()[1];

      Assert.That(TypeAdapter.Create(type).GenericParameterPosition, Is.EqualTo(type.GenericParameterPosition).And.EqualTo(1));
    }

    [Test]
    public void GenericParameterPosition_NotGenericParameter ()
    {
      Assert.That(() =>
          TypeAdapter.Create(typeof(ValueType)).GenericParameterPosition,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GenericParameterAttributes_GenericParameter ()
    {
      var type = typeof(Nullable<>).GetGenericArguments().Single();

      Assert.That(TypeAdapter.Create(type).GenericParameterAttributes, Is.EqualTo(type.GenericParameterAttributes));
    }

    [Test]
    public void GenericParameterAttributes_NotGenericParameter ()
    {
      Assert.That(() =>
          TypeAdapter.Create(typeof(ValueType)).GenericParameterAttributes,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var type = typeof(DateTime);
      var adapter = TypeAdapter.Create(type);
      Assert.That(
          adapter.GetCustomAttribute<SerializableAttribute>(true),
          Is.EqualTo(AttributeUtility.GetCustomAttribute<SerializableAttribute>(type, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var type = typeof(DateTime);
      var adapter = TypeAdapter.Create(type);
      Assert.That(
          adapter.GetCustomAttributes<SerializableAttribute>(true),
          Is.EqualTo(AttributeUtility.GetCustomAttributes<SerializableAttribute>(type, true)));
    }

    [Test]
    public void IsDefined ()
    {
      var type = typeof(DateTime);
      var adapter = TypeAdapter.Create(type);
      Assert.That(
          adapter.IsDefined<SerializableAttribute>(true),
          Is.EqualTo(AttributeUtility.IsDefined<SerializableAttribute>(type, true)));
    }

    [Test]
    public void BaseType ()
    {
      var type = typeof(SystemException);
      Assert.That(
          TypeAdapter.Create(type).BaseType,
          Is.TypeOf<TypeAdapter>().And.Property("Type").SameAs(type.BaseType));
    }

    [Test]
    public void BaseType_ForObject ()
    {
      var type = typeof(Object);
      Assert.That(TypeAdapter.Create(type).BaseType, Is.Null);
    }

    [Test]
    public void BaseType_ForInterface ()
    {
      var type = typeof(IList);
      Assert.That(TypeAdapter.Create(type).BaseType, Is.Null);
    }

    [Test]
    public void IsInstanceOf_SameType ()
    {
      var type = typeof(ArrayList);
      var value = new ArrayList();
      Assert.That(TypeAdapter.Create(type).IsInstanceOfType(value), Is.EqualTo(type.IsInstanceOfType(value)).And.True);
    }

    [Test]
    public void IsInstanceOf_DerivedType ()
    {
      var type = typeof(Exception);
      var value = new SystemException();
      Assert.That(TypeAdapter.Create(type).IsInstanceOfType(value), Is.EqualTo(type.IsInstanceOfType(value)).And.True);
    }

    [Test]
    public void IsInstanceOf_OtherType ()
    {
      var type = typeof(ArrayList);
      var value = new object();
      Assert.That(TypeAdapter.Create(type).IsInstanceOfType(value), Is.EqualTo(type.IsInstanceOfType(value)).And.False);
    }

    [Test]
    public void IsSubclassOf_BaseType ()
    {
      var currentType = typeof(SystemException);
      var otherType = typeof(Exception);
      Assert.That(
          TypeAdapter.Create(currentType).IsSubclassOf(TypeAdapter.Create(otherType)),
          Is.EqualTo(currentType.IsSubclassOf(otherType)).And.True);
    }

    [Test]
    public void IsSubclassOf_SameType ()
    {
      var currentType = typeof(SystemException);
      Assert.That(
          TypeAdapter.Create(currentType).IsSubclassOf(TypeAdapter.Create(currentType)),
          Is.EqualTo(currentType.IsSubclassOf(currentType)).And.False);
    }

    [Test]
    public void IsSubclassOf_NotBaseType ()
    {
      var currentType = typeof(SystemException);
      var otherType = typeof(string);
      Assert.That(
          TypeAdapter.Create(currentType).IsSubclassOf(TypeAdapter.Create(otherType)),
          Is.EqualTo(currentType.IsSubclassOf(otherType)).And.False);
    }

    [Test]
    public void IsSubclassOf_DifferentITypeInformationImplementation ()
    {
      var currentType = typeof(SystemException);
      var otherType = new Mock<ITypeInformation>();
      Assert.That(TypeAdapter.Create(currentType).IsSubclassOf(otherType.Object), Is.False);
    }

    [Test]
    public void IsSubclassOf_Null ()
    {
      Assert.That(() => TypeAdapter.Create(typeof(object)).IsSubclassOf(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void IsAssignableFrom_DerivedType ()
    {
      var currentType = typeof(Exception);
      var otherType = typeof(SystemException);
      Assert.That(
          TypeAdapter.Create(currentType).IsAssignableFrom(TypeAdapter.Create(otherType)),
          Is.EqualTo(currentType.IsAssignableFrom(otherType)).And.True);
    }

    [Test]
    public void IsAssignableFrom_SameType ()
    {
      var currentType = typeof(Exception);
      Assert.That(
          TypeAdapter.Create(currentType).IsAssignableFrom(TypeAdapter.Create(currentType)),
          Is.EqualTo(currentType.IsAssignableFrom(currentType)).And.True);
    }

    [Test]
    public void IsAssignableFrom_NotDerivedType ()
    {
      var currentType = typeof(Exception);
      var otherType = typeof(string);
      Assert.That(
          TypeAdapter.Create(currentType).IsAssignableFrom(TypeAdapter.Create(otherType)),
          Is.EqualTo(currentType.IsAssignableFrom(otherType)).And.False);
    }

    [Test]
    public void IsAssignableFrom_DifferentITypeInformationImplementation ()
    {
      var currentType = typeof(Exception);
      var otherType = new Mock<ITypeInformation>();
      Assert.That(TypeAdapter.Create(currentType).IsAssignableFrom(otherType.Object), Is.False);
    }

    [Test]
    public void IsAssignableFrom_Null ()
    {
      var currentType = typeof(Exception);
      Assert.That(
          TypeAdapter.Create(currentType).IsAssignableFrom(null),
          Is.EqualTo(currentType.IsAssignableFrom(null)).And.False);
    }

    [Test]
    public void CanAscribeTo_SameType ()
    {
      var currentType = typeof(SystemException);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(currentType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, currentType)).And.True);
    }

    [Test]
    public void CanAscribeTo_BaseType ()
    {
      var currentType = typeof(SystemException);
      var otherType = typeof(Exception);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(otherType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, otherType)).And.True);
    }

    [Test]
    public void CanAscribeTo_DerivedType ()
    {
      var currentType = typeof(Exception);
      var otherType = typeof(SystemException);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(otherType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, otherType)).And.False);
    }

    [Test]
    public void CanAscribeTo_OpenGeneric ()
    {
      var currentType = typeof(List<int>);
      var otherType = typeof(List<>);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(otherType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, otherType)).And.True);
    }

    [Test]
    public void CanAscribeTo_Interface ()
    {
      var currentType = typeof(ArrayList);
      var otherType = typeof(IList);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(otherType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, otherType)).And.True);
    }

    [Test]
    public void CanAscribeTo_OpenGenericInterface ()
    {
      var currentType = typeof(List<int>);
      var otherType = typeof(IList<>);
      Assert.That(
          TypeAdapter.Create(currentType).CanAscribeTo(TypeAdapter.Create(otherType)),
          Is.EqualTo(TypeExtensions.CanAscribeTo(currentType, otherType)).And.True);
    }

    [Test]
    public void CanAscribeTo_DifferentITypeInformationImplementation ()
    {
      var currentType = typeof(SystemException);
      var otherType = new Mock<ITypeInformation>();
      Assert.That(TypeAdapter.Create(currentType).CanAscribeTo(otherType.Object), Is.False);
    }

    [Test]
    public void CanAscribeTo_Null ()
    {
      Assert.That(() => TypeAdapter.Create(typeof(object)).CanAscribeTo(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_NotGenericType ()
    {
      var type = typeof(ArrayList);
      var arguments = TypeAdapter.Create(type).GetAscribedGenericArgumentsFor(TypeAdapter.Create(typeof(IList)));
      Assert.That(arguments, Has.All.TypeOf<TypeAdapter>());
      Assert.That(arguments, Is.Empty);
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_ClosedGenericType ()
    {
      var type = typeof(List<int>);
      var arguments = TypeAdapter.Create(type).GetAscribedGenericArgumentsFor(TypeAdapter.Create(typeof(IList<>)));
      Assert.That(arguments, Has.All.TypeOf<TypeAdapter>());
      Assert.That(arguments, Is.EqualTo(new[] { TypeAdapter.Create(typeof(int)) }));
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_ClosedGenericType_ForNotGenericBaseType ()
    {
      var type = typeof(List<int>);
      var arguments = TypeAdapter.Create(type).GetAscribedGenericArgumentsFor(TypeAdapter.Create(typeof(IList)));
      Assert.That(arguments, Has.All.TypeOf<TypeAdapter>());
      Assert.That(arguments, Is.Empty);
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_ClosedGenericType_ForOtherType ()
    {
      var type = typeof(List<int>);
      Assert.That(
          () => TypeAdapter.Create(type).GetAscribedGenericArgumentsFor(TypeAdapter.Create(typeof(string))),
          Throws.ArgumentException.And.ArgumentExceptionMessageEqualTo(
              "Parameter 'type' has type 'System.Collections.Generic.List`1[System.Int32]' "
              + "when type 'System.String' was expected.", "type"));
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_Interface_WithMultipleImplementations ()
    {
      var type = typeof(IDoubleInheritingGenericInterface);
      Assert.That(
          () => TypeAdapter.Create(type).GetAscribedGenericArgumentsFor(TypeAdapter.Create(typeof(IEnumerable<>))),
          Throws.TypeOf<AmbiguousMatchException>());
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_DifferentITypeInformationImplementation ()
    {
      var currentType = typeof(SystemException);
      var otherType = new Mock<ITypeInformation>();
      Assert.That(
          () => TypeAdapter.Create(currentType).GetAscribedGenericArgumentsFor(otherType.Object),
          Throws.ArgumentException.And.ArgumentExceptionMessageEqualTo(
              "Parameter 'c' has type '" + otherType.Object.GetType().FullName + "' when type 'Remotion.Reflection.TypeAdapter' was expected.",
              "c"));
    }

    [Test]
    public void GetAscribedGenericArgumentsFor_Null ()
    {
      Assert.That(() => TypeAdapter.Create(typeof(object)).GetAscribedGenericArgumentsFor(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Equals ()
    {
      var adapter = TypeAdapter.Create(typeof(ArrayList));
      Assert.That(adapter.Equals(null), Is.False);
      Assert.That(adapter.Equals("test"), Is.False);
      Assert.That(TypeAdapter.Create(typeof(List<int>)).Equals(TypeAdapter.Create(typeof(List<string>))), Is.False);
      Assert.That(TypeAdapter.Create(typeof(List<>)).Equals(TypeAdapter.Create(typeof(List<int>))), Is.False);

      Assert.That(adapter.Equals(TypeAdapter.Create(typeof(ArrayList))), Is.True);
      Assert.That(TypeAdapter.Create(typeof(List<int>)).GetGenericTypeDefinition().Equals(TypeAdapter.Create(typeof(List<>))), Is.True);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That(
          TypeAdapter.Create(typeof(ArrayList)).GetHashCode(),
          Is.EqualTo(TypeAdapter.Create(typeof(ArrayList)).GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      Assert.That(TypeAdapter.Create(typeof(ArrayList)).ToString(), Is.EqualTo("System.Collections.ArrayList"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((ITypeInformation)TypeAdapter.Create(typeof(ArrayList))).IsNull, Is.False);
    }

    [Test]
    public void IsSupportedByTypeConversionProvider ()
    {
      var typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();

      Assert.That(typeConversionProvider.CanConvert(typeof(TypeAdapter), typeof(Type)), Is.True);
    }
  }
}
