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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.Data.UnitTesting.DomainObjects.Linq;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class FindMetadataObjectQueryBuilderTest : DomainTest
  {
    private readonly QueryableComparer _queryableComparer
        = new QueryableComparer((actual, exptected) => Assert.That(actual, Is.EqualTo(exptected)));

    private FindMetadataObjectQueryBuilder _queryBuilder;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _queryBuilder = new FindMetadataObjectQueryBuilder();
    }

    [Test]
    public void CreateQuery_ForMetadataObject ()
    {
      string metadataObjectID = "b8621bc9-9ab3-4524-b1e4-582657d6b420";

      var expected = from m in QueryFactory.CreateLinqQuery<MetadataObject>()
                     where m.MetadataItemID == new Guid(metadataObjectID)
                     select m;

      var actual = _queryBuilder.CreateQuery(metadataObjectID);

      _queryableComparer.Compare(expected, actual);
    }

    [Test]
    public void CreateQuery_ForStateDefinition ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|2";

      var expected = from state in QueryFactory.CreateLinqQuery<StateDefinition>()
                     where state.StateProperty.MetadataItemID == new Guid("9e689c4c-3758-436e-ac86-23171289fa5e") && state.Value == 2
                     select state;

      var actual = _queryBuilder.CreateQuery(metadataObjectID);

      _queryableComparer.Compare(expected, actual.Cast<StateDefinition>());
    }

    [Test]
    public void Find_InvalidMetadataItemID ()
    {
      string metadataObjectID = "Hello|42";
      Assert.That(
          () => _queryBuilder.CreateQuery(metadataObjectID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The metadata ID 'Hello|42' is invalid.", "metadataID"));
    }

    [Test]
    public void Find_InvalidStateValue ()
    {
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|Hello";
      Assert.That(
          () => _queryBuilder.CreateQuery(metadataObjectID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The metadata ID '9e689c4c-3758-436e-ac86-23171289fa5e|Hello' is invalid.", "metadataID"));
    }
  }
}
