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
  public class InlineTypeDefinitionWalkerTest
  {
    private Mock<Action<ClassDefinition>> _classDefinitionActionMock;
    private Mock<Action<InterfaceDefinition>> _interfaceDefinitionActionMock;
    private Mock<Func<ClassDefinition, string>> _classDefinitionFuncMock;
    private Mock<Func<InterfaceDefinition, string>> _interfaceDefinitionFuncMock;

    [SetUp]
    public void SetUp ()
    {
      _classDefinitionActionMock = new Mock<Action<ClassDefinition>>();
      _interfaceDefinitionActionMock = new Mock<Action<InterfaceDefinition>>();
      _classDefinitionFuncMock = new Mock<Func<ClassDefinition, string>>();
      _interfaceDefinitionFuncMock = new Mock<Func<InterfaceDefinition, string>>();
    }

    [Test]
    public void WalkAncestors_WithoutResult ()
    {
      var (iPerson, person) = CreateTestDomain();

      _classDefinitionActionMock.Setup(_ => _(person));
      _interfaceDefinitionActionMock.Setup(_ => _(iPerson));

      InlineTypeDefinitionWalker.WalkAncestors(
          person,
          _classDefinitionActionMock.Object,
          _interfaceDefinitionActionMock.Object);

      _classDefinitionActionMock.Verify(_ => _(person), Times.Once);
      _interfaceDefinitionActionMock.Verify(_ => _(iPerson), Times.Once);
    }

    [Test]
    public void WalkWalkAncestors_WithResult ()
    {
      var (iPerson, person) = CreateTestDomain();

      _classDefinitionFuncMock.Setup(_ => _(person)).Returns((string)null);
      _interfaceDefinitionFuncMock.Setup(_ => _(iPerson)).Returns("result");

      var result = InlineTypeDefinitionWalker.WalkAncestors(
          person,
          _classDefinitionFuncMock.Object,
          _interfaceDefinitionFuncMock.Object,
          match: value => value != null);
      Assert.That(result, Is.EqualTo("result"));

      _classDefinitionFuncMock.Verify(_ => _(person), Times.Once);
      _interfaceDefinitionFuncMock.Verify(_ => _(iPerson), Times.Once);
    }

    [Test]
    public void WalkDescendents_WithoutResult ()
    {
      var (iPerson, person) = CreateTestDomain();

      _classDefinitionActionMock.Setup(_ => _(person));
      _interfaceDefinitionActionMock.Setup(_ => _(iPerson));

      InlineTypeDefinitionWalker.WalkDescendants(
          iPerson,
          _classDefinitionActionMock.Object,
          _interfaceDefinitionActionMock.Object);

      _classDefinitionActionMock.Verify(_ => _(person), Times.Once);
      _interfaceDefinitionActionMock.Verify(_ => _(iPerson), Times.Once);
    }

    [Test]
    public void WalkDescendents_WithResult ()
    {
      var (iPerson, person) = CreateTestDomain();

      _classDefinitionFuncMock.Setup(_ => _(person)).Returns("result");
      _interfaceDefinitionFuncMock.Setup(_ => _(iPerson)).Returns((string)null);

      var result = InlineTypeDefinitionWalker.WalkDescendants(
          iPerson,
          _classDefinitionFuncMock.Object,
          _interfaceDefinitionFuncMock.Object,
          value => value != null);
      Assert.That(result, Is.EqualTo("result"));

      _classDefinitionFuncMock.Verify(_ => _(person), Times.Once);
      _interfaceDefinitionFuncMock.Verify(_ => _(iPerson), Times.Once);
    }

    private static (InterfaceDefinition iPerson, ClassDefinition person) CreateTestDomain ()
    {
      var iPerson = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      var person = ClassDefinitionObjectMother.CreateClassDefinition(implementedInterfaces: new[] { iPerson });
      iPerson.SetImplementingClasses(new[] { person });
      iPerson.SetExtendingInterfaces(Array.Empty<InterfaceDefinition>());
      person.SetDerivedClasses(Array.Empty<ClassDefinition>());

      return (iPerson, person);
    }
  }
}
