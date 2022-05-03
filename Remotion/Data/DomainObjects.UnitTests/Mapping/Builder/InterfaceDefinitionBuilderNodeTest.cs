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
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Builder
{
  [TestFixture]
  public class InterfaceDefinitionBuilderNodeTest
  {
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

    [Test]
    public void Initialize ()
    {
      var classDefinitionBuilderNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());

      Assert.That(classDefinitionBuilderNode.Type, Is.EqualTo(typeof(IBase)));
      Assert.That(classDefinitionBuilderNode.ExtendedInterfaceNodes, Is.Empty);
      Assert.That(classDefinitionBuilderNode.ImplementingClassNodes, Is.Empty);
      Assert.That(classDefinitionBuilderNode.ExtendingInterfaceNodes, Is.Empty);
      Assert.That(classDefinitionBuilderNode.InterfaceDefinition, Is.Null);
      Assert.That(classDefinitionBuilderNode.IsLeafNode, Is.True);
    }

    [Test]
    public void Initialize_WithExtendedInterface_AddsInstanceToExtendedInterface ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });

      Assert.That(subNode.ExtendedInterfaceNodes, Is.EqualTo(new[] { baseNode }));
      Assert.That(baseNode.IsLeafNode, Is.False);
      Assert.That(baseNode.ExtendingInterfaceNodes, Is.EqualTo(new[] { subNode }));
    }

    [Test]
    public void Initialize_WithNullType_Throws ()
    {
      Assert.That(
          () => new InterfaceDefinitionBuilderNode(null!, Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentNullException.With.ArgumentExceptionMessageWithParameterNameEqualTo("type"));
    }

    [Test]
    public void Initialize_WithNonInterface_Throws ()
    {
      Assert.That(
          () => new InterfaceDefinitionBuilderNode(typeof(string), Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified type must be an interface.", "type"));
    }

    [Test]
    public void Initialize_WithIDomainObject_Throws ()
    {
      Assert.That(
          () => new InterfaceDefinitionBuilderNode(typeof(IDomainObject), Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified type must not be the 'IDomainObject' interface.", "type"));
    }

    [Test]
    public void Initialize_WithNonDomainObject_Throws ()
    {
      Assert.That(
          () => new InterfaceDefinitionBuilderNode(typeof(IDisposable), Enumerable.Empty<InterfaceDefinitionBuilderNode>()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified type must implement 'IDomainObject'.", "type"));
    }

    [Test]
    public void EndBuildTypeDefinition ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var subInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(extendedInterfaces: new[] { baseInterfaceDefinition });
      var subClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(baseInterfaceDefinition);
      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(ISub), new[] { baseInterfaceDefinition })).Returns(subInterfaceDefinition);
      mappingObjectFactoryMock.Setup(_ => _.CreateClassDefinition(typeof(Sub), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(subClassDefinition);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });
      var subClassNode = new ClassDefinitionBuilderNode(typeof(Sub), null, Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      subNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      subClassNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      subNode.EndBuildTypeDefinition();
      subClassNode.EndBuildTypeDefinition();

      Assert.That(baseNode.GetBuiltTypeDefinition(), Is.SameAs(baseInterfaceDefinition));
      Assert.That(subNode.GetBuiltTypeDefinition(), Is.SameAs(subInterfaceDefinition));
      Assert.That(subClassNode.GetBuiltTypeDefinition(), Is.SameAs(subClassDefinition));

      Assert.That(baseInterfaceDefinition.ExtendingInterfaces, Is.EqualTo(new[] { subInterfaceDefinition }));
      Assert.That(baseInterfaceDefinition.ImplementingClasses, Is.Empty);
    }

    [Test]
    public void EndBuildTypeDefinition_OnParentNode_AlsoCallEndsBuildTypeDefinitionOnChildNode ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });

      baseNode.EndBuildTypeDefinition();

      Assert.That(baseNode.IsConstructed, Is.True);
      Assert.That(subNode.IsConstructed, Is.True);
    }

    [Test]
    public void EndBuildTypeDefinition_OnChildNode_AlsoCallsEndBuildTypeDefinitionOnParentNode ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });

      subNode.EndBuildTypeDefinition();

      Assert.That(baseNode.IsConstructed, Is.True);
      Assert.That(subNode.IsConstructed, Is.True);
    }

    [Test]
    public void BeginBuildTypeDefinition ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var subInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(baseInterfaceDefinition);
      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(ISub), new[] { baseInterfaceDefinition }))
          .Returns(subInterfaceDefinition);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });
      subNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.InterfaceDefinition, Is.SameAs(baseInterfaceDefinition));
      Assert.That(subNode.InterfaceDefinition, Is.SameAs(subInterfaceDefinition));

      mappingObjectFactoryMock.VerifyAll();
    }

    [Test]
    public void BeginBuildTypeDefinition_Twice_DoesNotRebuild ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(baseInterfaceDefinition);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.InterfaceDefinition, Is.SameAs(baseInterfaceDefinition));

      mappingObjectFactoryMock.Verify(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())), Times.Once);
    }

    [Test]
    public void BeginBuildTypeDefinition_AlreadyConstructedNode_Throws ()
    {
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.EndBuildTypeDefinition();

      Assert.That(
          () => baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot build type definition as the builder node is already constructed."));
    }

    [Test]
    public void BeginBuildTypeDefinition_WithNonLeafNode_DoesNothing ()
    {
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>();

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });

      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(baseNode.InterfaceDefinition, Is.Null);
      Assert.That(subNode.InterfaceDefinition, Is.Null);
    }

    [Test]
    public void GetBuiltTypeDefinition ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(baseInterfaceDefinition);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);
      baseNode.EndBuildTypeDefinition();

      Assert.That(baseNode.GetBuiltTypeDefinition(), Is.SameAs(baseInterfaceDefinition));
    }

    [Test]
    public void GetBuiltTypeDefinition_UnconstructedNode_Throws ()
    {
      var baseInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);

      mappingObjectFactoryMock.Setup(_ => _.CreateInterfaceDefinition(typeof(IBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty()))).Returns(baseInterfaceDefinition);

      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.BeginBuildTypeDefinition(mappingObjectFactoryMock.Object);

      Assert.That(
          () => baseNode.GetBuiltTypeDefinition(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot get the built type definition from an unconstructed builder node."));
    }

    [Test]
    public void AddImplementingClass ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new ClassDefinitionBuilderNode(typeof(Sub), null, new[] { baseNode });

      Assert.That(baseNode.ImplementingClassNodes, Is.EqualTo(new[] { subNode }));
    }

    [Test]
    public void AddImplementingClass_WithConstructedNode_Throws ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Array.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.EndBuildTypeDefinition();

      Assert.That(
          () => new ClassDefinitionBuilderNode(typeof(Sub), null, new[] { baseNode }),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified implemented interface nodes must not be constructed.", "implementedInterfaceNodes"));
    }

    [Test]
    public void AddExtendingInterface ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Enumerable.Empty<InterfaceDefinitionBuilderNode>());
      var subNode = new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode });

      Assert.That(baseNode.ExtendingInterfaceNodes, Is.EqualTo(new[] { subNode }));
    }

    [Test]
    public void AddExtendingInterface_WithConstructedNode_Throws ()
    {
      var baseNode = new InterfaceDefinitionBuilderNode(typeof(IBase), Array.Empty<InterfaceDefinitionBuilderNode>());
      baseNode.EndBuildTypeDefinition();

      Assert.That(
          () => new InterfaceDefinitionBuilderNode(typeof(ISub), new[] { baseNode }),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The specified extended interfaces must not be constructed.", "extendedInterfaceNodes"));
    }
  }
}
