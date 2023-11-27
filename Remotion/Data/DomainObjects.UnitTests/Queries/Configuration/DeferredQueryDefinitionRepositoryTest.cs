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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  [Ignore("RM-8992")]
  public class DeferredQueryDefinitionRepositoryTest : StandardMappingTest
  {
    [Test]
    public void Operation_WithDuplicateQueryID_ThrowsQueryConfigurationExceptionWithID ()
    {
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);

      var queryConfiguration = new QueryConfiguration();
      queryConfiguration.QueryDefinitions.Add(queryDefinition1);
      queryConfiguration.QueryDefinitions.Add(queryDefinition2);
      DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(new StorageConfiguration(), queryConfiguration));

      var repository = new DeferredQueryDefinitionRepository();

      Assert.That(
          () => repository.GetMandatory("someID"),
          Throws.TypeOf<QueryConfigurationException>().With.Message.EqualTo("Duplicate query definitions with the following IDs: 'myID'."));
    }

    [Test]
    public void Operation_WithMultipleDuplicateQueryID_ThrowsQueryConfigurationExceptionAndListsAllDuplicateID ()
    {
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);
      var queryDefinition3 = new QueryDefinition("duplicateID2", storageProviderDefinition, "statement", default);
      var queryDefinition4 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition5 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition6 = new QueryDefinition("duplicateID2", storageProviderDefinition, "statement", default);

      var queryConfiguration = new QueryConfiguration();
      queryConfiguration.QueryDefinitions.Add(queryDefinition1);
      queryConfiguration.QueryDefinitions.Add(queryDefinition2);
      queryConfiguration.QueryDefinitions.Add(queryDefinition3);
      queryConfiguration.QueryDefinitions.Add(queryDefinition4);
      queryConfiguration.QueryDefinitions.Add(queryDefinition5);
      queryConfiguration.QueryDefinitions.Add(queryDefinition6);
      DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(new StorageConfiguration(), queryConfiguration));

      var repository = new DeferredQueryDefinitionRepository();

      Assert.That(
          () => repository.Contains("someID"),
          Throws.TypeOf<QueryConfigurationException>().With.Message.EqualTo("Duplicate query definitions with the following IDs: 'duplicateID1', 'duplicateID2'."));
    }

    [Test]
    public void Contains_WithNonExistentQuery_ReturnsFalse ()
    {
      var queryConfiguration = new QueryConfiguration();
      DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(new StorageConfiguration(), queryConfiguration));

      var repository = new DeferredQueryDefinitionRepository();

      Assert.That(repository.Contains("myID"), Is.False);
    }

    [Test]
    public void Contains_WithExistentQuery_ReturnsTrue ()
    {
      var queryDefinition = new QueryDefinition("myID", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      var queryConfiguration = new QueryConfiguration();
      queryConfiguration.QueryDefinitions.Add(queryDefinition);
      DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(new StorageConfiguration(), queryConfiguration));
      var repository = new DeferredQueryDefinitionRepository();

      Assert.That(repository.Contains("myID"), Is.True);
    }

    [Test]
    public void GetMandatory_WithExistentQuery_ReturnsQuery ()
    {
      var queryDefinition = new QueryDefinition("myID", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      var queryConfiguration = new QueryConfiguration();
      queryConfiguration.QueryDefinitions.Add(queryDefinition);
      DomainObjectsConfiguration.SetCurrent(new FakeDomainObjectsConfiguration(new StorageConfiguration(), queryConfiguration));
      var repository = new DeferredQueryDefinitionRepository();

      Assert.That(repository.GetMandatory("myID"), Is.SameAs(queryDefinition));
    }
  }
}
