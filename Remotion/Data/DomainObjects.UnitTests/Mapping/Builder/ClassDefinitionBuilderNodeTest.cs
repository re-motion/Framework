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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Builder;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Builder
{
  [TestFixture]
  public class ClassDefinitionBuilderNodeTest
  {
    private interface ITop : IDomainObject
    {
    }

    private interface IBase : IDomainObject
    {
    }

    private interface ISub : IBase
    {
    }

    private class Base : DomainObject
    {
    }

    private class Sub : Base
    {
    }

    [TestDomain]
    private class SubWithStorageGroup : Base
    {
    }

    [Test]
    public void Initialize ()
    {
      var classDefinitionBuilderNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      Assert.That(classDefinitionBuilderNode.Type, Is.EqualTo(typeof(Base)));
      Assert.That(classDefinitionBuilderNode.BaseClass, Is.Null);
      Assert.That(classDefinitionBuilderNode.ImplementedInterfaces, Is.Empty);
      Assert.That(classDefinitionBuilderNode.DerivedClasses, Is.Empty);
      Assert.That(classDefinitionBuilderNode.ClassDefinition, Is.Null);
      Assert.That(classDefinitionBuilderNode.IsLeafNode, Is.True);
      Assert.That(classDefinitionBuilderNode.IsInheritanceRoot, Is.True);
    }

    [Test]
    public void Initialize_WithBaseClass_AddsInstanceToBaseClass ()
    {
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      Assert.That(subNode.BaseClass, Is.SameAs(baseNode));
      Assert.That(subNode.IsInheritanceRoot, Is.False);
      Assert.That(baseNode.IsLeafNode, Is.False);
      Assert.That(baseNode.DerivedClasses, Is.EqualTo(new[] { subNode }));
      Assert.That(baseNode.IsInheritanceRoot, Is.True);
    }

    [Test]
    public void Initialize_WithImplementedInterfaces_AddsInstanceToImplementedInterfaces ()
    {
      var baseInterfaceNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var baseNode = new ClassDefinitionBuilderNode(typeof(Sub), null, EnumerableUtility.Singleton(baseInterfaceNode));

      Assert.That(baseNode.ImplementedInterfaces, Is.EqualTo(new[] { baseInterfaceNode }));
      Assert.That(baseInterfaceNode.IsLeafNode, Is.False);
      Assert.That(baseInterfaceNode.ImplementingClasses, Is.EqualTo(new[] { baseNode }));
    }

    [Test]
    public void Initialize_WithAlreadyImplementedInterfaces_IsNotAddedToImplementedInterfaces ()
    {
      var baseInterfaceNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, EnumerableUtility.Singleton(baseInterfaceNode));
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, EnumerableUtility.Singleton(baseInterfaceNode));

      Assert.That(baseNode.ImplementedInterfaces, Is.EqualTo(new[] { baseInterfaceNode }));
      Assert.That(subNode.ImplementedInterfaces, Is.Empty);
    }

    [Test]
    public void Initialize_WithAlreadyImplementedInterfacesButInheritanceRoot_IsAddedToImplementedInterfaces ()
    {
      var baseInterfaceNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, EnumerableUtility.Singleton(baseInterfaceNode));
      var subNode = new ClassDefinitionBuilderNode(typeof(SubWithStorageGroup), baseNode, EnumerableUtility.Singleton(baseInterfaceNode));

      Assert.That(baseNode.ImplementedInterfaces, Is.EqualTo(new[] { baseInterfaceNode }));
      Assert.That(subNode.ImplementedInterfaces, Is.EqualTo(new[] { baseInterfaceNode }));
    }

    [Test]
    public void Initialize_WithNullType_Throws ()
    {
      Assert.That(
          () => new ClassDefinitionBuilderNode(null, null, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentNullException.With.ArgumentExceptionMessageWithParameterNameEqualTo("type"));
    }

    [Test]
    public void Initialize_WithNonClass_Throws ()
    {
      Assert.That(
          () => new ClassDefinitionBuilderNode(typeof(int), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified type must be a class.", "type"));
    }

    [Test]
    public void Initialize_WithDomainObject_Throws ()
    {
      Assert.That(
          () => new ClassDefinitionBuilderNode(typeof(DomainObject), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Cannot create a builder node for the DomainObject type.", "type"));
    }

    [Test]
    public void Initialize_WithNonDomainObject_Throws ()
    {
      Assert.That(
          () => new ClassDefinitionBuilderNode(typeof(string), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Cannot create a builder node for a type that is not a domain object.", "type"));
    }

    [Test]
    public void EndBuildTypeDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var subClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition(baseClass: baseClassDefinition);
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any()))).Returns(baseClassDefinition);
      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Sub), baseClassDefinition, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(subClassDefinition);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      // The constructor registers the parent -> child relationship
      _ = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      subNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      subNode.EndBuildTypeDefinition();

      Assert.That(baseNode.GetBuiltTypeDefinition(), Is.SameAs(baseClassDefinition));
      Assert.That(subNode.GetBuiltTypeDefinition(), Is.SameAs(subClassDefinition));

      Assert.That(baseClassDefinition.DerivedClasses, Is.EqualTo(new[] { subClassDefinition }));
      Assert.That(subClassDefinition.DerivedClasses, Is.Empty);
    }

    [Test]
    public void EndBuildTypeDefinition_OnParentNode_AlsoCallEndsBuildTypeDefinitionOnChildNode ()
    {
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      baseNode.EndBuildTypeDefinition();

      Assert.That(baseNode.IsConstructed, Is.True);
      Assert.That(subNode.IsConstructed, Is.True);
    }

    [Test]
    public void EndBuildTypeDefinition_OnChildNode_AlsoCallsEndBuildTypeDefinitionOnParentNode ()
    {
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      subNode.EndBuildTypeDefinition();

      Assert.That(baseNode.IsConstructed, Is.True);
      Assert.That(subNode.IsConstructed, Is.True);
    }

    [Test]
    public void BeginBuildTypeDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var subClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(baseClassDefinition);
      mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Sub), baseClassDefinition, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(subClassDefinition);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      subNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.ClassDefinition, Is.SameAs(baseClassDefinition));
      Assert.That(subNode.ClassDefinition, Is.SameAs(subClassDefinition));
    }

    [Test]
    public void BeginBuildTypeDefinition_Twice_DoesNotRebuild ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any()))).Returns(baseClassDefinition);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.ClassDefinition, Is.SameAs(baseClassDefinition));

      mappingObjectFactoryMock.Verify(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())), Times.Once);
    }

    [Test]
    public void BeginBuildTypeDefinition_AlreadyConstructedNode_Throws ()
    {
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.EndBuildTypeDefinition();

      Assert.That(
          () => baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot build type definition as the builder node is already constructed."));
    }

    [Test]
    public void BeginBuildTypeDefinition_WithInheritanceRoot_DoesNotBuildBaseClass ()
    {
      var subClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(SubWithStorageGroup), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(subClassDefinition)
          .Verifiable();

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(SubWithStorageGroup), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      subNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.ClassDefinition, Is.Null);
      Assert.That(subNode.ClassDefinition, Is.SameAs(subClassDefinition));

      mappingObjectFactoryMock.Verify();
    }

    [Test]
    public void BeginBuildTypeDefinition_WithNonLeafNode_DoesNothing ()
    {
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>();

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(SubWithStorageGroup), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.ClassDefinition, Is.Null);
      Assert.That(subNode.ClassDefinition, Is.Null);
    }

    [Test]
    public void GetBuiltTypeDefinition ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any()))).Returns(baseClassDefinition);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      baseNode.EndBuildTypeDefinition();

      Assert.That(baseNode.GetBuiltTypeDefinition(), Is.SameAs(baseClassDefinition));
    }

    [Test]
    public void GetBuiltTypeDefinition_UnconstructedNode_Throws ()
    {
      var baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Base), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any()))).Returns(baseClassDefinition);

      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(
          () => baseNode.GetBuiltTypeDefinition(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot get the built type definition from an unconstructed builder node."));
    }

    [Test]
    public void AddDerivedClass_ConstructedBaseClass_Throws ()
    {
      var baseNode = new ClassDefinitionBuilderNode(typeof(Base), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.EndBuildTypeDefinition();

      Assert.That(
          () => new ClassDefinitionBuilderNode(typeof(Sub), baseNode, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified base class must not be a constructed node.", "baseClass"));
    }
  }
}
