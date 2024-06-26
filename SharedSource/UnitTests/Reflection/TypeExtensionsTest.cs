// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
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
