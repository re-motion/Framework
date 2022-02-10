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
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class MappingConfigurationExtensionsTest
  {
    private interface IDummyInterface
    {
    }

    private class DummyClass
    {
    }

    [Test]
    public void GetTypeDefinition ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition();
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      mappingConfigurationMock.Setup(_ => _.GetTypeDefinition(typeof(IDummyInterface), It.IsAny<Func<Type, Exception>>())).Returns(typeDefinition);

      var result = mappingConfigurationMock.Object.GetTypeDefinition(typeof(IDummyInterface));
      Assert.That(result, Is.SameAs(typeDefinition));
    }

    [Test]
    public void GetTypeDefinition_WithNonExistentType_Throws ()
    {
      Assert.That(
          () => MappingConfiguration.Current.GetTypeDefinition(typeof(IDummyInterface)),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo("Mapping does not contain type 'Remotion.Data.DomainObjects.UnitTests.Mapping.MappingConfigurationExtensionsTest+IDummyInterface'."));
    }

    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void ContainsClassDefinition (bool value)
    {
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      mappingConfigurationMock.Setup(_ => _.ContainsTypeDefinition(typeof(DummyClass)))
          .Returns(value)
          .Verifiable();

      Assert.That(mappingConfigurationMock.Object.ContainsClassDefinition(typeof(DummyClass)), Is.EqualTo(value));
      mappingConfigurationMock.Verify();
    }

    [Test]
    public void ContainsClassDefinition_NonClassType_ReturnsFalse ()
    {
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      Assert.That(mappingConfigurationMock.Object.ContainsClassDefinition(typeof(IDummyInterface)), Is.False);
    }

    [Test]
    public void GetClassDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      mappingConfigurationMock
          .Setup(_ => _.GetTypeDefinition(typeof(DummyClass), It.IsAny<Func<Type, Exception>>()))
          .Returns(classDefinition);

      var result = mappingConfigurationMock.Object.GetClassDefinition(typeof(DummyClass));
      Assert.That(result, Is.SameAs(classDefinition));
    }

    [Test]
    public void GetClassDefinition_WithNonClass_Throws ()
    {
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);

      Assert.That(
          () => mappingConfigurationMock.Object.GetClassDefinition(typeof(IDummyInterface)),
          Throws.TypeOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "The specified type 'Remotion.Data.DomainObjects.UnitTests.Mapping.MappingConfigurationExtensionsTest+IDummyInterface' must be a class.",
                  "type"));
    }

    [Test]
    public void GetClassDefinition_WithCustomExceptionFactory ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition();
      var exceptionFactory = new Func<Type, Exception>(_ => new Exception());
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      mappingConfigurationMock
          .Setup(_ => _.GetTypeDefinition(typeof(DummyClass), exceptionFactory))
          .Returns(typeDefinition);

      var result = mappingConfigurationMock.Object.GetClassDefinition(typeof(DummyClass), exceptionFactory);
      Assert.That(result, Is.SameAs(typeDefinition));
    }

    [Test]
    public void GetClassDefinition_WithCustomerExceptionFactoryAndNonClass_Throws ()
    {
      var exceptionFactoryMock = new Mock<Func<Type, Exception>>(MockBehavior.Strict);
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);

      Assert.That(
          () => mappingConfigurationMock.Object.GetClassDefinition(typeof(IDummyInterface), exceptionFactoryMock.Object),
          Throws.TypeOf<ArgumentException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "The specified type 'Remotion.Data.DomainObjects.UnitTests.Mapping.MappingConfigurationExtensionsTest+IDummyInterface' must be a class.",
                  "type"));
    }

    [Test]
    public void GetClassDefinition_WithCustomerExceptionFactoryAndNonExistingClass_ThrowsExceptionFromFactory ()
    {
      var exception = new Exception("Test");
      var exceptionFactory = new Func<Type, Exception>(_ => exception);

      Assert.That(
          Assert.Throws<Exception>(() => MappingConfiguration.Current.GetClassDefinition(typeof(DummyClass), exceptionFactory)),
          Is.SameAs(exception));
    }

    [Test]
    public void GetClassDefinition_WithClassID ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      var mappingConfigurationMock = new Mock<IMappingConfiguration>(MockBehavior.Strict);
      mappingConfigurationMock.Setup(_ => _.GetClassDefinition("asdNonExistent", It.IsAny<Func<string, Exception>>())).Returns(classDefinition);

      var result = mappingConfigurationMock.Object.GetClassDefinition("asdNonExistent");
      Assert.That(result, Is.SameAs(classDefinition));
    }

    [Test]
    public void GetClassDefinition_WithNonClassID_ThrowsExceptionFromFactory ()
    {
      Assert.That(
          () => MappingConfiguration.Current.GetClassDefinition("asdNonExistent"),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo("Mapping does not contain class 'asdNonExistent'."));
    }
  }
}
