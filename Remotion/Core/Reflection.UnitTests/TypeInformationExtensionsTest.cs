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
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class TypeInformationExtensionsTest
  {
    [Test]
    public void GetAssemblyQualifiedNameSafe_WithAssemblyQualifiedNameAvailable_ReturnsAssemblyQualifiedName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (id);

      var result = typeInformation.GetAssemblyQualifiedNameSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithNullAssemblyQualifiedName_ReturnsFullName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (null);
      typeInformation.Expect (_ => _.FullName).Return (id);

      var result = typeInformation.GetAssemblyQualifiedNameSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetAssemblyQualifiedNameSafe_WithNullAssemblyQualifiedNameAndFullName_ReturnsName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (null);
      typeInformation.Expect (_ => _.FullName).Return (null);
      typeInformation.Expect (_ => _.Name).Return (id);

      var result = typeInformation.GetAssemblyQualifiedNameSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithAssemblyQualifiedNameAvailable_ReturnsAssemblyQualifiedName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (id);

      var result = typeInformation.GetAssemblyQualifiedNameChecked();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithNullAssemblyQualifiedName_ThrowsInvalidOperationExceptionWithFullNameInMessage ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (null);
      typeInformation.Expect (_ => _.FullName).Return (id);

      Assert.That (
          () => typeInformation.GetAssemblyQualifiedNameChecked(),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("Type '7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5' does not have an assembly qualified name."));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetAssemblyQualifiedNameChecked_WithNullAssemblyQualifiedNameAndFullName_ThrowsInvalidOperationExceptionWithNameInMessage ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.AssemblyQualifiedName).Return (null);
      typeInformation.Expect (_ => _.FullName).Return (null);
      typeInformation.Expect (_ => _.Name).Return (id);

      Assert.That (
          () => typeInformation.GetAssemblyQualifiedNameChecked(),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("Type '7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5' does not have an assembly qualified name."));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetFullNameSafe_WithFullNameAvailable_ReturnsFullName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.FullName).Return (id);

      var result = typeInformation.GetFullNameSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetFullNameSafe_WithNullFullName_ReturnsName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.FullName).Return (null);
      typeInformation.Expect (_ => _.Name).Return (id);

      var result = typeInformation.GetFullNameSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetFullNameChecked_WithFullNameAvailable_ReturnsFullName ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.FullName).Return (id);

      var result = typeInformation.GetFullNameChecked();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetFullNameChecked_WithNullFullName_ThrowsInvalidOperationExceptionWithNameInMessage ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.FullName).Return (null);
      typeInformation.Expect (_ => _.Name).Return (id);

      Assert.That (
          () => typeInformation.GetFullNameChecked(),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("Type '7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5' does not have a full name."));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetNamespaceSafe_WithNamespaceAvailable_ReturnsNamespace ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.Namespace).Return (id);

      var result = typeInformation.GetNamespaceSafe();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetNamespaceSafe_WithNullNamespace_ReturnsFallback ()
    {
      var fallback = "<undefined>";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.Namespace).Return (null);

      var result = typeInformation.GetNamespaceSafe();

      Assert.That (result, Is.EqualTo (fallback));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetNamespaceChecked_WithNamespaceAvailable_ReturnsNamespace ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.Namespace).Return (id);

      var result = typeInformation.GetNamespaceChecked();

      Assert.That (result, Is.EqualTo (id));
      typeInformation.VerifyAllExpectations();
    }

    [Test]
    public void GetNamespaceChecked_WithNullNamespace_ThrowsInvalidOperationExceptionWithNameInMessage ()
    {
      var id = "7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5";
      var typeInformation = MockRepository.GenerateStrictMock<ITypeInformation>();
      typeInformation.Expect (_ => _.Namespace).Return (null);
      typeInformation.Expect (_ => _.Name).Return (id);

      Assert.That (
          () => typeInformation.GetNamespaceChecked(),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("Type '7EFC8044-EEAB-4F2B-9A77-2B331C13D7F5' does not have a namespace."));
      typeInformation.VerifyAllExpectations();
    }
  }
}
