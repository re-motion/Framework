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
  public class QueryDefinitionTest : StandardMappingTest
  {
    [Test]
    public void InitializeCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.CollectionReadOnly);

      Assert.That(definition.CollectionType, Is.EqualTo(typeof(DomainObjectCollection)));
    }

    [Test]
    public void InitializeScalarReadOnlyQueryWithCollectionType_ThrowsArgumentException ()
    {
      Assert.That(
          () => new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.ScalarReadOnly, typeof(DomainObjectCollection)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The scalar query 'QueryID' must not specify a collectionType.", "collectionType"));
    }

    [Test]
    public void InitializeScalarReadWriteQueryWithCollectionType_ThrowsArgumentException ()
    {
      Assert.That(
          () => new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.ScalarReadWrite, typeof(DomainObjectCollection)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The scalar query 'QueryID' must not specify a collectionType.", "collectionType"));
    }

    [Test]
    public void InitializeCustomReadOnlyQueryWithCollectionType_ThrowsArgumentException ()
    {
      Assert.That(
          () => new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.CustomReadOnly, typeof(DomainObjectCollection)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The custom query 'QueryID' must not specify a collectionType.", "collectionType"));
    }

    [Test]
    public void InitializeCustomReadWriteQueryWithCollectionType_ThrowsArgumentException ()
    {
      Assert.That(
          () => new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.CustomReadWrite, typeof(DomainObjectCollection)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The custom query 'QueryID' must not specify a collectionType.", "collectionType"));
    }

    [Test]
    public void InitializeInvalidCollectionType_ThrowsArgumentException ()
    {
      Assert.That(
          () => new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.CollectionReadOnly, this.GetType()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The collectionType of query 'QueryID' must be 'Remotion.Data.DomainObjects.DomainObjectCollection' or derived from it.",
                  "collectionType"));
    }

    [Test]
    public void InitializeWithDomainObjectCollectionType_ThrowsArgumentException ()
    {
      QueryDefinition definition =
          new QueryDefinition("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.CollectionReadOnly, typeof(DomainObjectCollection));

      Assert.That(definition.CollectionType, Is.EqualTo(typeof(DomainObjectCollection)));
    }
  }
}
