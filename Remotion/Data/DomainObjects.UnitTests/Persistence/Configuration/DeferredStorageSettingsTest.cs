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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class DeferredStorageSettingsTest
  {
    [Test]
    public void GetStorageProviderDefinition_WithString_CallsContainedStorageSettings ()
    {
      var expectedStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("fake");

      var storageSettingsMock = new Mock<IStorageSettings>();
      storageSettingsMock
          .Setup(_ => _.GetStorageProviderDefinition("storageProviderName"))
          .Returns(expectedStorageProviderDefinition);

      var deferredStorageSettings = CreateDeferredStorageSettings(storageSettingsMock);

      Assert.That(
          deferredStorageSettings.GetStorageProviderDefinition("storageProviderName"),
          Is.SameAs(expectedStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_WithType_CallsContainedStorageSettings ()
    {
      var expectedStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("fake");

      var storageSettingsMock = new Mock<IStorageSettings>();
      storageSettingsMock
          .Setup(_ => _.GetStorageProviderDefinition(typeof(string)))
          .Returns(expectedStorageProviderDefinition);

      var deferredStorageSettings = CreateDeferredStorageSettings(storageSettingsMock);

      Assert.That(
          deferredStorageSettings.GetStorageProviderDefinition(typeof(string)),
          Is.SameAs(expectedStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassDefinition_CallsContainedStorageSettings ()
    {
      var expectedStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("fake");
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(string), baseClass: null);

      var storageSettingsMock = new Mock<IStorageSettings>();
      storageSettingsMock
          .Setup(_ => _.GetStorageProviderDefinition(classDefinition))
          .Returns(expectedStorageProviderDefinition);

      var deferredStorageSettings = CreateDeferredStorageSettings(storageSettingsMock);

      Assert.That(
          deferredStorageSettings.GetStorageProviderDefinition(classDefinition),
          Is.SameAs(expectedStorageProviderDefinition));
    }

    [Test]
    public void GetDefaultStorageProviderDefinition_CallsContainedStorageSettings ()
    {
      var expectedStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("fake");

      var storageSettingsMock = new Mock<IStorageSettings>();
      storageSettingsMock
          .Setup(_ => _.GetDefaultStorageProviderDefinition())
          .Returns(expectedStorageProviderDefinition);

      var deferredStorageSettings = CreateDeferredStorageSettings(storageSettingsMock);

      Assert.That(
          deferredStorageSettings.GetDefaultStorageProviderDefinition(),
          Is.SameAs(expectedStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinitions_CallsContainedStorageSettings ()
    {
      var expectedStorageProviderDefinitions = new[] { new UnitTestStorageProviderStubDefinition("fake1"), new UnitTestStorageProviderStubDefinition("fake2") };

      var storageSettingsMock = new Mock<IStorageSettings>();
      storageSettingsMock
          .Setup(_ => _.GetStorageProviderDefinitions())
          .Returns(expectedStorageProviderDefinitions);

      var deferredStorageSettings = CreateDeferredStorageSettings(storageSettingsMock);

      deferredStorageSettings.GetStorageProviderDefinitions();

      Assert.That(
          deferredStorageSettings.GetStorageProviderDefinitions(),
          Is.EquivalentTo(expectedStorageProviderDefinitions));
    }

    private static DeferredStorageSettings CreateDeferredStorageSettings (Mock<IStorageSettings> storageSettingsMock)
    {
      var storageSettingsFactoryMock = new Mock<IStorageSettingsFactory>();
      storageSettingsFactoryMock
          .Setup(_ => _.Create(It.IsAny<IStorageObjectFactoryFactory>()))
          .Returns(storageSettingsMock.Object);

      var storageSettingsFactoryResolverStub = new Mock<IStorageSettingsFactoryResolver>();
      storageSettingsFactoryResolverStub
          .Setup(_ => _.Resolve())
          .Returns(storageSettingsFactoryMock.Object);

      return new DeferredStorageSettings(Mock.Of<IStorageObjectFactoryFactory>(), storageSettingsFactoryResolverStub.Object);
    }

  }
}
