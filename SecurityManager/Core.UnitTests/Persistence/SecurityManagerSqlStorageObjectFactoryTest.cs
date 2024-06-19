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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.SecurityManager.Persistence;

namespace Remotion.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class SecurityManagerSqlStorageObjectFactoryTest
  {
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private SecurityManagerSqlStorageObjectFactory _securityManagerSqlStorageObjectFactory;
    private Mock<IPersistenceExtension> _persistenceExtensionStub;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition("TestDomain", new SecurityManagerSqlStorageObjectFactory(), "ConnectionString", "ReadOnlyConnectionString");
      _securityManagerSqlStorageObjectFactory = new SecurityManagerSqlStorageObjectFactory();
      _persistenceExtensionStub = new Mock<IPersistenceExtension>();
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

      Assert.That(result, Is.TypeOf(typeof(SecurityManagerRdbmsProvider)));
      Assert.That(result.As<SecurityManagerRdbmsProvider>().PersistenceExtension, Is.SameAs(_persistenceExtensionStub.Object));
      Assert.That(result.As<SecurityManagerRdbmsProvider>().StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(result.As<SecurityManagerRdbmsProvider>().ConnectionString, Is.EqualTo(_rdbmsProviderDefinition.ConnectionString));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass(typeof(RdbmsProvider)).Clear().AddMixins(typeof(SecurityManagerRdbmsProviderTestMixin)).
              EnterScope())
      {
        var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

        Assert.That(Mixin.Get<SecurityManagerRdbmsProviderTestMixin>(result), Is.Not.Null);
      }
    }

    [Test]
    public void CreateReadOnlyStorageProvider ()
    {
      var result = _securityManagerSqlStorageObjectFactory.CreateReadOnlyStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

      Assert.That(result, Is.InstanceOf<ReadOnlyStorageProviderDecorator>());
      var innerStorageProvider = result.As<ReadOnlyStorageProviderDecorator>().InnerStorageProvider;
      Assert.That(innerStorageProvider.As<SecurityManagerRdbmsProvider>().PersistenceExtension, Is.SameAs(_persistenceExtensionStub.Object));
      Assert.That(innerStorageProvider.As<SecurityManagerRdbmsProvider>().StorageProviderDefinition, Is.SameAs(_rdbmsProviderDefinition));
      Assert.That(innerStorageProvider.As<SecurityManagerRdbmsProvider>().ConnectionString, Is.EqualTo(_rdbmsProviderDefinition.ReadOnlyConnectionString));
    }

    [Test]
    public void CreateReadOnlyStorageProviderWithMixin ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass(typeof(RdbmsProvider)).Clear().AddMixins(typeof(SecurityManagerRdbmsProviderTestMixin)).
              EnterScope())
      {
        var result = _securityManagerSqlStorageObjectFactory.CreateReadOnlyStorageProvider(_rdbmsProviderDefinition, _persistenceExtensionStub.Object);

        Assert.That(result, Is.InstanceOf<ReadOnlyStorageProviderDecorator>());

        var innerReadOnlyStorageProvider = result.As<ReadOnlyStorageProviderDecorator>().InnerStorageProvider;

        Assert.That(Mixin.Get<SecurityManagerRdbmsProviderTestMixin>(innerReadOnlyStorageProvider), Is.Not.Null);
      }
    }
  }
}
