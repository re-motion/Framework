// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class TypeExtensionsTest
  {
    private class GenericTypeHelper
    {
      private static class Generic<T>
      {
        public static List<T> List = null!;
      }

      /// <summary>
      /// Provides a type like <c>List&lt;T&gt;</c>, that is an open (the T) constructed type.
      /// </summary>
      public static Type OpenConstructedGenericType => typeof(Generic<>).GetField(nameof(Generic<int>.List))!.FieldType;
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithSystemType_ReturnsAssemblyQualifiedName ()
    {
      var typeArgument = typeof(string);

      Assert.That(typeArgument.AssemblyQualifiedName, Is.Not.Null);
      Assert.That(typeArgument.GetAssemblyQualifiedNameSafe(), Does.StartWith("System.String,"));
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithGenericTypeParameter_ReturnsTypeName ()
    {
      var genericTypeParameters = typeof(CustomType<>).GetTypeInfo().GenericTypeParameters;

      Assert.That(genericTypeParameters[0].AssemblyQualifiedName, Is.Null);
      Assert.That(genericTypeParameters[0].FullName, Is.Null);
      Assert.That(genericTypeParameters[0].GetAssemblyQualifiedNameSafe(), Is.EqualTo("T"));
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithOpenConstructedGenericType_ReturnsTypeName ()
    {
      var openConstructedGenericType = GenericTypeHelper.OpenConstructedGenericType;

      Assert.That(openConstructedGenericType.AssemblyQualifiedName, Is.Null);
      Assert.That(openConstructedGenericType.FullName, Is.Null);
      Assert.That(openConstructedGenericType.GetAssemblyQualifiedNameSafe(), Is.EqualTo("List`1"));
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithClosedConstructedGenericType_ReturnsAssemblyQualifiedName ()
    {
      var closedConstructedGenericType = typeof(List<int>);

      Assert.That(closedConstructedGenericType.AssemblyQualifiedName, Is.Not.Null);
      Assert.That(closedConstructedGenericType.GetAssemblyQualifiedNameSafe(), Does.StartWith("System.Collections.Generic.List`1[[System.Int32,"));
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithSystemType_ReturnsAssemblyQualifiedName ()
    {
      var typeArgument = typeof(CustomType<string>).GetGenericArguments();

      Assert.That(typeArgument[0].AssemblyQualifiedName, Is.Not.Null);
      Assert.That(typeArgument[0].GetAssemblyQualifiedNameChecked(), Does.StartWith("System.String,"));
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithGenericTypeParameter_Throws ()
    {
      var genericTypeArgumentsParameters = typeof(CustomType<>).GetTypeInfo().GenericTypeParameters;

      Assert.That(genericTypeArgumentsParameters[0].AssemblyQualifiedName, Is.Null);
      Assert.That(
          () => genericTypeArgumentsParameters[0].GetAssemblyQualifiedNameChecked(),
          Throws.InvalidOperationException.With.Message.EqualTo("Type 'T' does not have an assembly qualified name."));
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithOpenConstructedGenericType_Throws ()
    {
      var openConstructedGenericType = GenericTypeHelper.OpenConstructedGenericType;

      Assert.That(openConstructedGenericType.AssemblyQualifiedName, Is.Null);
      Assert.That(
          () => openConstructedGenericType.GetAssemblyQualifiedNameChecked(),
          Throws.InvalidOperationException.With.Message.EqualTo("Type 'List`1' does not have an assembly qualified name."));
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithClosedConstructedGenericType_ReturnsAssemblyQualifiedName ()
    {
      var closedConstructedGenericType = typeof(List<int>);

      Assert.That(closedConstructedGenericType.AssemblyQualifiedName, Is.Not.Null);
      Assert.That(closedConstructedGenericType.GetAssemblyQualifiedNameChecked(), Does.StartWith("System.Collections.Generic.List`1[[System.Int32,"));
    }

    [Test]
    public void GetFullNameSafe_WithSystemType_ReturnsFullName ()
    {
      var typeArgument = typeof(string);

      Assert.That(typeArgument.FullName, Is.Not.Null);
      Assert.That(typeArgument.GetFullNameSafe(), Is.EqualTo("System.String"));
    }

    [Test]
    public void GetFullNameSafe_WithGenericTypeParameter_ReturnsTypeName ()
    {
      var genericTypeParameters = typeof(CustomType<>).GetTypeInfo().GenericTypeParameters;

      Assert.That(genericTypeParameters[0].FullName, Is.Null);
      Assert.That(genericTypeParameters[0].GetFullNameSafe(), Is.EqualTo("T"));
    }

    [Test]
    public void GetFullNameSafe_WithOpenConstructedGenericType_ReturnsTypeName ()
    {
      var openConstructedGenericType = GenericTypeHelper.OpenConstructedGenericType;

      Assert.That(openConstructedGenericType.FullName, Is.Null);
      Assert.That(openConstructedGenericType.GetFullNameSafe(), Is.EqualTo("List`1"));
    }

    [Test]
    public void GetFullNameSafe_WithClosedConstructedGenericType_ReturnsFullName ()
    {
      var closedConstructedGenericType = typeof(List<int>);

      Assert.That(closedConstructedGenericType.FullName, Is.Not.Null);
      Assert.That(closedConstructedGenericType.GetFullNameSafe(), Does.StartWith("System.Collections.Generic.List`1[[System.Int32,"));
    }

    [Test]
    public void GetFullNameChecked_WithSystemType_ReturnsFullName ()
    {
      var typeArgument = typeof(string);

      Assert.That(typeArgument.FullName, Is.Not.Null);
      Assert.That(typeArgument.GetFullNameChecked(), Is.EqualTo("System.String"));
    }

    [Test]
    public void GetFullNameChecked_WithGenericTypeParameter_ThrowsInvalidOperationException ()
    {
      var genericTypeParameters = typeof(CustomType<>).GetTypeInfo().GenericTypeParameters;

      Assert.That(genericTypeParameters[0].FullName, Is.Null);
      Assert.That(
          () => genericTypeParameters[0].GetFullNameChecked(),
          Throws.InvalidOperationException.With.Message.EqualTo("Type 'T' does not have a full name."));
    }

    [Test]
    public void GetFullNameChecked_WithOpenConstructedGenericType_ThrowsInvalidOperationException ()
    {
      var openConstructedType = GenericTypeHelper.OpenConstructedGenericType;

      Assert.That(openConstructedType.FullName, Is.Null);
      Assert.That(openConstructedType.GetFullNameChecked(), Is.EqualTo("System.Collections.Generic.List`1[T]"));
    }

    [Test]
    public void GetFullNameChecked_WithClosedConstructedGenericType_ReturnsFullName ()
    {
      var closedConstructedType = typeof(List<int>);

      Assert.That(closedConstructedType.FullName, Is.Not.Null);
      Assert.That(closedConstructedType.GetFullNameChecked(), Does.StartWith("System.Collections.Generic.List`1[[System.Int32,"));
    }

    [Test]
    public void GetNamespaceSafe_WithTypeWithNamespace_ReturnsNamespace ()
    {
      var typeWithNamespace = typeof(string);

      Assert.That(typeWithNamespace.Namespace, Is.Not.Null);
      Assert.That(typeWithNamespace.GetNamespaceSafe(), Is.EqualTo("System"));
    }

    [Test]
    public void GetNamespaceSafe_WithTypeWithoutNamespace_ReturnsUndefined ()
    {
      var typeWithoutNamespace = typeof(CustomType<>);

      Assert.That(typeWithoutNamespace.Namespace, Is.Null);
      Assert.That(typeWithoutNamespace.GetNamespaceSafe(), Is.EqualTo("<undefined>"));
    }

    [Test]
    public void GetNamespaceChecked_WithTypeWithNamespace_ReturnsNamespace ()
    {
      var typeWithNamespace = typeof(string);

      Assert.That(typeWithNamespace.Namespace, Is.Not.Null);
      Assert.That(typeWithNamespace.GetNamespaceChecked(), Is.EqualTo("System"));
    }

    [Test]
    public void GetNamespaceChecked_WithTypeWithoutNamespace_ThrowsInvalidOperationException ()
    {
      var typeWithoutNamespace = typeof(CustomType<>);

      Assert.That(typeWithoutNamespace.Namespace, Is.Null);
      Assert.That(
          () => typeWithoutNamespace.GetNamespaceChecked(),
          Throws.InvalidOperationException.With.Message.EqualTo("Type 'CustomType`1' does not have a namespace."));
    }
  }
}

// ReSharper disable once UnusedTypeParameter
internal class CustomType<T>
{
  // This class is purposefully declared outside of the namespace for
  // tests of the methods GetNamespaceSafe and GetNamespaceChecked.
}
