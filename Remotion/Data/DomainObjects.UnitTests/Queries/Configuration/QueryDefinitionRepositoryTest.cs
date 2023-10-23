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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryDefinitionRepositoryTest
  {
    [Test]
    public void Initialize_WithDuplicateQueryID_ThrowsArgumentExceptionWithID ()
    {
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("myId", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myId", storageProviderDefinition, "statement", default);

      Assert.That(
          () => new QueryDefinitionRepository(new[] { queryDefinition1, queryDefinition2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Duplicate query definition with ID 'myId' found.", "queryDefinitions"));
    }

    [Test]
    public void Initialize_WithMultipleDuplicateQueryID_ThrowsArgumentExceptionWithFirstID ()
    {
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("provider");
      var queryDefinition1 = new QueryDefinition("duplicateId1", storageProviderDefinition, "statement", default);
      var queryDefinition2 = new QueryDefinition("myId1", storageProviderDefinition, "statement", default);
      var queryDefinition3 = new QueryDefinition("duplicateId2", storageProviderDefinition, "statement", default);
      var queryDefinition4 = new QueryDefinition("duplicateId1", storageProviderDefinition, "statement", default);
      var queryDefinition5 = new QueryDefinition("duplicateId1", storageProviderDefinition, "statement", default);
      var queryDefinition6 = new QueryDefinition("duplicateId2", storageProviderDefinition, "statement", default);

      Assert.That(
          () => new QueryDefinitionRepository(new[] { queryDefinition1, queryDefinition2, queryDefinition3, queryDefinition4, queryDefinition5, queryDefinition6 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Duplicate query definition with ID 'duplicateId1' found.", "queryDefinitions"));
    }

    [Test]
    public void Initialize_WithNullItem_ThrowsException ()
    {
      var queryDefinition = new QueryDefinition("myId", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);

      Assert.That(
          () => new QueryDefinitionRepository(new[] { queryDefinition, null }),
          Throws.ArgumentNullException.With.ArgumentExceptionMessageEqualTo("Item 1 of parameter 'queryDefinitions' is null.", "queryDefinitions"));
    }

    [Test]
    public void Contains_WithNonExistentQuery_ReturnsFalse ()
    {
      var repository = new QueryDefinitionRepository(Array.Empty<QueryDefinition>());

      Assert.That(repository.Contains("myId"), Is.False);
    }

    [Test]
    public void Contains_WithExistentQuery_ReturnsTrue ()
    {
      var queryDefinition = new QueryDefinition("myId", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      var repository = new QueryDefinitionRepository(new[] { queryDefinition });

      Assert.That(repository.Contains("myId"), Is.True);
    }

    [Test]
    public void GetMandatory_WithNonExistentQuery_ThrowsException ()
    {
      var repository = new QueryDefinitionRepository(Array.Empty<QueryDefinition>());

      Assert.That(
          () => repository.GetMandatory("myId"),
          Throws.TypeOf<QueryConfigurationException>()
              .With.Message.EqualTo("QueryDefinition 'myId' does not exist."));
    }

    [Test]
    public void GetMandatory_WithExistentQuery_ReturnsQuery ()
    {
      var queryDefinition = new QueryDefinition("myId", new UnitTestStorageProviderStubDefinition("provider"), "statement", default);
      var repository = new QueryDefinitionRepository(new[] { queryDefinition });

      Assert.That(repository.GetMandatory("myId"), Is.SameAs(queryDefinition));
    }
  }
}
