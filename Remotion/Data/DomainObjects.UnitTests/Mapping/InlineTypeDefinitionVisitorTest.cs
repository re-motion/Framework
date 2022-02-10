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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class InlineTypeDefinitionVisitorTest
  {
    public interface IVisitorCallReceiver<out T>
    {
      T HandleClassDefinition (ClassDefinition classDefinition);

      T HandleInterfaceDefinition (InterfaceDefinition interfaceDefinition);
    }

    public interface IVisitorCallReceiver
    {
      void HandleClassDefinition (ClassDefinition classDefinition);

      void HandleInterfaceDefinition (InterfaceDefinition interfaceDefinition);
    }

    private Mock<IVisitorCallReceiver<string>> _nonVoidReceiverMock;
    private Mock<IVisitorCallReceiver> _voidReceiverMock;

    [SetUp]
    public void SetUp ()
    {
      _nonVoidReceiverMock = new Mock<IVisitorCallReceiver<string>>(MockBehavior.Strict);
      _voidReceiverMock = new Mock<IVisitorCallReceiver>(MockBehavior.Strict);
    }

    [Test]
    public void Visit_WithResult_ClassDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _nonVoidReceiverMock
          .Setup(_ => _.HandleClassDefinition(classDefinition))
          .Returns("test")
          .Verifiable();

      Assert.That(ExecuteNonVoidVisit(classDefinition), Is.EqualTo("test"));

      _nonVoidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithResult_InterfaceDefinition ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      _nonVoidReceiverMock
          .Setup(_ => _.HandleInterfaceDefinition(interfaceDefinition))
          .Returns("test")
          .Verifiable();

      Assert.That(ExecuteNonVoidVisit(interfaceDefinition), Is.EqualTo("test"));

      _nonVoidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_ClassDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _voidReceiverMock
          .Setup(_ => _.HandleClassDefinition(classDefinition))
          .Verifiable();

      ExecuteVoidVisit(classDefinition);

      _voidReceiverMock.Verify();
    }

    [Test]
    public void Visit_WithoutResult_InterfaceDefinition ()
    {
      var classDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      _voidReceiverMock
          .Setup(_ => _.HandleInterfaceDefinition(classDefinition))
          .Verifiable();

      ExecuteVoidVisit(classDefinition);

      _voidReceiverMock.Verify();
    }

    private string ExecuteNonVoidVisit (TypeDefinition typeDefinition)
    {
      return InlineTypeDefinitionVisitor.Visit<string>(
          typeDefinition,
          _nonVoidReceiverMock.Object.HandleClassDefinition,
          _nonVoidReceiverMock.Object.HandleInterfaceDefinition);
    }

    private void ExecuteVoidVisit (TypeDefinition typeDefinition)
    {
      InlineTypeDefinitionVisitor.Visit(
          typeDefinition,
          _voidReceiverMock.Object.HandleClassDefinition,
          _voidReceiverMock.Object.HandleInterfaceDefinition);
    }
  }
}
