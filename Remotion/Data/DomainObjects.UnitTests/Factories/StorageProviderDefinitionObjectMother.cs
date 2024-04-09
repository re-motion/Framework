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
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class StorageProviderDefinitionObjectMother
  {
    public static ProviderCollection<StorageProviderDefinition> CreateTestDomainStorageProviders ()
    {
      ProviderCollection<StorageProviderDefinition> storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>();
      var sqlStorageObjectFactory = new SqlStorageObjectFactory();
      var nonPersistentStorageObjectFactory = new NonPersistentStorageObjectFactory();

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              DatabaseTest.c_testDomainProviderID,
              sqlStorageObjectFactory,
              DatabaseTest.TestDomainConnectionString,
              DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add(
          new NonPersistentProviderDefinition(
              DatabaseTest.c_nonPersistentTestDomainProviderID,
              nonPersistentStorageObjectFactory));

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              DatabaseTest.DefaultStorageProviderID,
              sqlStorageObjectFactory,
              DatabaseTest.TestDomainConnectionString,
              DatabaseTest.TestDomainConnectionString));

      storageProviderDefinitionCollection.Add(
          new UnitTestStorageProviderStubDefinition(
              DatabaseTest.c_unitTestStorageProviderStubID));

      storageProviderDefinitionCollection.Add(
          new RdbmsProviderDefinition(
              TableInheritanceMappingTest.TableInheritanceTestDomainProviderID,
              sqlStorageObjectFactory,
              DatabaseTest.TestDomainConnectionString,
              DatabaseTest.TestDomainConnectionString));

      return storageProviderDefinitionCollection;
    }
  }
}
