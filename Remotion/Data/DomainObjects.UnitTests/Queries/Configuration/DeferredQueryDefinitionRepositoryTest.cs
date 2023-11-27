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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class DeferredQueryDefinitionRepositoryTest : StandardMappingTest
  {
    [Test]
    public void Operation_WithDuplicateQueryID_ThrowsQueryConfigurationExceptionWithID ()
    {
      var queryDefinitionLoaderStub = new Mock<IQueryDefinitionLoader>();
      var repository = new DeferredQueryDefinitionRepository(queryDefinitionLoaderStub.Object);

      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);

      queryDefinitionLoaderStub.Setup(e => e.LoadAllQueryDefinitions()).Returns(new[] { queryDefinition1, queryDefinition2 });

      Assert.That(
          () => repository.GetMandatory("someID"),
          Throws.TypeOf<QueryConfigurationException>().With.Message.EqualTo("Duplicate query definitions with the following IDs: 'myID'."));
    }

    [Test]
    public void Operation_WithMultipleDuplicateQueryID_ThrowsQueryConfigurationExceptionAndListsAllDuplicateID ()
    {
      var queryDefinitionLoaderStub = new Mock<IQueryDefinitionLoader>();
      var repository = new DeferredQueryDefinitionRepository(queryDefinitionLoaderStub.Object);

      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myID", storageProviderDefinition, "statement", default);
      var queryDefinition3 = new QueryDefinition("duplicateID2", storageProviderDefinition, "statement", default);
      var queryDefinition4 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition5 = new QueryDefinition("duplicateID1", storageProviderDefinition, "statement", default);
      var queryDefinition6 = new QueryDefinition("duplicateID2", storageProviderDefinition, "statement", default);

      queryDefinitionLoaderStub
          .Setup(e => e.LoadAllQueryDefinitions())
          .Returns(new[] { queryDefinition1, queryDefinition2, queryDefinition3, queryDefinition4, queryDefinition5, queryDefinition6 });

      Assert.That(
          () => repository.Contains("someID"),
          Throws.TypeOf<QueryConfigurationException>().With.Message.EqualTo("Duplicate query definitions with the following IDs: 'duplicateID1', 'duplicateID2'."));
    }

    [Test]
    public void Contains_WithNonExistentQuery_ReturnsFalse ()
    {
      var queryDefinitionLoaderStub = new Mock<IQueryDefinitionLoader>();
      var repository = new DeferredQueryDefinitionRepository(queryDefinitionLoaderStub.Object);

      queryDefinitionLoaderStub
          .Setup(e => e.LoadAllQueryDefinitions())
          .Returns(Array.Empty<QueryDefinition>());

      Assert.That(repository.Contains("myID"), Is.False);
    }

    [Test]
    public void Contains_WithExistentQuery_ReturnsTrue ()
    {
      var queryDefinitionLoaderStub = new Mock<IQueryDefinitionLoader>();
      var repository = new DeferredQueryDefinitionRepository(queryDefinitionLoaderStub.Object);

      var queryDefinition = new QueryDefinition("myID", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      queryDefinitionLoaderStub
          .Setup(e => e.LoadAllQueryDefinitions())
          .Returns(new[] { queryDefinition });

      Assert.That(repository.Contains("myID"), Is.True);
    }

    [Test]
    public void GetMandatory_WithExistentQuery_ReturnsQuery ()
    {
      var queryDefinitionLoaderStub = new Mock<IQueryDefinitionLoader>();
      var repository = new DeferredQueryDefinitionRepository(queryDefinitionLoaderStub.Object);

      var queryDefinition = new QueryDefinition("myID", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      queryDefinitionLoaderStub
          .Setup(e => e.LoadAllQueryDefinitions())
          .Returns(new[] { queryDefinition });

      Assert.That(repository.GetMandatory("myID"), Is.SameAs(queryDefinition));
    }
  }
}
