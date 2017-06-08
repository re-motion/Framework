// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Mixins;
using Remotion.SecurityManager.Persistence;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class SecurityManagerSqlStorageObjectFactoryTest
  {
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private SecurityManagerSqlStorageObjectFactory _securityManagerSqlStorageObjectFactory;
    private IPersistenceExtension _persistenceExtensionStub;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", new SecurityManagerSqlStorageObjectFactory(), "ConnectionString");
      _securityManagerSqlStorageObjectFactory = new SecurityManagerSqlStorageObjectFactory ();
      _persistenceExtensionStub = MockRepository.GenerateStub<IPersistenceExtension>();
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider (_rdbmsProviderDefinition, _persistenceExtensionStub);

      Assert.That (result, Is.TypeOf (typeof (SecurityManagerRdbmsProvider)));
      Assert.That (result.PersistenceExtension, Is.SameAs (_persistenceExtensionStub));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (RdbmsProvider)).Clear().AddMixins (typeof (SecurityManagerRdbmsProviderTestMixin)).
              EnterScope())
      {
        var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider (_rdbmsProviderDefinition, _persistenceExtensionStub);

        Assert.That (Mixin.Get<SecurityManagerRdbmsProviderTestMixin> (result), Is.Not.Null);
      }
    }
  }
}