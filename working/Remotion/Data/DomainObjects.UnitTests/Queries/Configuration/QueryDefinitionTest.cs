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

namespace Remotion.Data.DomainObjects.UnitTests.Queries.Configuration
{
  [TestFixture]
  public class QueryDefinitionTest : StandardMappingTest
  {
    [Test]
    public void InitializeCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.Collection);

      Assert.That (definition.CollectionType, Is.EqualTo (typeof (DomainObjectCollection)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The scalar query 'QueryID' must not specify a collectionType.\r\nParameter name: collectionType")]
    public void InitializeScalarQueryWithCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.Scalar, typeof (DomainObjectCollection));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collectionType of query 'QueryID' must be 'Remotion.Data.DomainObjects.DomainObjectCollection' or derived from it.\r\n"
        + "Parameter name: collectionType")]
    public void InitializeInvalidCollectionType ()
    {
      QueryDefinition definition = new QueryDefinition ("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.Collection, this.GetType ());
    }

    [Test]
    public void InitializeWithDomainObjectCollectionType ()
    {
      QueryDefinition definition = 
          new QueryDefinition ("QueryID", TestDomainStorageProviderDefinition, "Statement", QueryType.Collection, typeof (DomainObjectCollection));

      Assert.That (definition.CollectionType, Is.EqualTo (typeof (DomainObjectCollection)));
    }
  }
}
