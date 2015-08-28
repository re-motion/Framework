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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class SingleTableInheritanceIntegrationTest : IntegrationTestBase
  {

    //TODO RM-6485: move, focus is basetype
    [Test]
    public void QueryWithBase ()
    {
      Company partner = DomainObjectIDs.Partner1.GetObject<Company>();
      IQueryable<Company> result = (from c in QueryFactory.CreateLinqQuery<Company>()
        where c.ID == partner.ID
        select c);
      CheckQueryResult (result, DomainObjectIDs.Partner1);
    }

    //TODO RM-6485: move, focus is basetype
    [Test]
    public void TableInheritance_AccessingPropertyFromBaseClass ()
    {
      var query = from c in QueryFactory.CreateLinqQuery<TIClassWithUnidirectionalRelation>()
        where c.DomainBase.CreatedAt == new DateTime (2006, 01, 04)
        select c;
      CheckQueryResult (query, new TableInheritanceDomainObjectIDs (Configuration).ClassWithUnidirectionalRelation);
    }


    //TODO RM-6485: move, focus is inheritance
    [Test]
    public void Query_Is ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().Where (c => c is Customer);

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }


    //TODO RM-6485: move, focus is inheritance
    [Test]
    public void Query_WithOfType_SelectingBaseType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Company>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    //TODO RM-6485: move, focus is inheritance
    [Test]
    public void Query_WithOfType_SameType ()
    {
      var query = QueryFactory.CreateLinqQuery<Customer>().OfType<Customer>();

      CheckQueryResult (
          query,
          DomainObjectIDs.Customer1,
          DomainObjectIDs.Customer2,
          DomainObjectIDs.Customer3,
          DomainObjectIDs.Customer4,
          DomainObjectIDs.Customer5);
    }

    //TODO RM-6485: move, focus is inheritance
    [Test]
    public void Query_WithOfType_DerivedType ()
    {
      var partnerIDs = new[]
                       {
                           (Guid) DomainObjectIDs.Partner1.Value,
                           (Guid) DomainObjectIDs.Distributor1.Value,
                           (Guid) DomainObjectIDs.Supplier1.Value,
                           (Guid) DomainObjectIDs.Company1.Value,
                           (Guid) DomainObjectIDs.Customer1.Value
                       };
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Partner>().Where (p => partnerIDs.Contains ((Guid) p.ID.Value));

      CheckQueryResult (
          query,
          DomainObjectIDs.Partner1,
          DomainObjectIDs.Distributor1,
          DomainObjectIDs.Supplier1);
    }

    //TODO RM-6485: move, focus is inheritance
    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Query_WithOfType_UnrelatedType ()
    {
      var query = QueryFactory.CreateLinqQuery<Company>().OfType<Order>();

      CheckQueryResult (query);
    }

  }
}