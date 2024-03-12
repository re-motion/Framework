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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Development.Data.UnitTesting.DomainObjects.Configuration;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.Configuration
{
  [TestFixture]
  public class FakeStorageSettingsTest
  {
    [Test]
    public void CreateForSqlServer ()
    {
      var result = FakeStorageSettings.CreateForSqlServer("DummyConnectionString", "DummyReadOnlyConnectionString");

      Assert.That(result, Is.InstanceOf<DeferredStorageSettings>());
      Assert.That(result.GetDefaultStorageProviderDefinition(), Is.InstanceOf<RdbmsProviderDefinition>());

      var defaultStorageProviderDefinition = (RdbmsProviderDefinition)result.GetDefaultStorageProviderDefinition()!;
      Assert.That(defaultStorageProviderDefinition.Name, Is.EqualTo("Default"));
      Assert.That(defaultStorageProviderDefinition.ConnectionString, Is.EqualTo("DummyConnectionString"));
      Assert.That(defaultStorageProviderDefinition.Factory, Is.InstanceOf<SqlStorageObjectFactory>());

      var storageSettings = PrivateInvoke.GetNonPublicField(defaultStorageProviderDefinition.Factory, typeof(SqlStorageObjectFactory), "StorageSettings");

      Assert.That(storageSettings, Is.EqualTo(result));

      Assert.That(result.GetStorageProviderDefinitions(), Is.EquivalentTo(new[] { defaultStorageProviderDefinition }));
    }
  }
}
