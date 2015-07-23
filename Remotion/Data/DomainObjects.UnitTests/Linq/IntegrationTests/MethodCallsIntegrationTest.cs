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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class MethodCallsIntegrationTest : IntegrationTestBase
  {
    
    [Test]
    public void Trim ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Product.Trim () == "CPU Fan" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void Insert ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Product.Insert (1, "Test") == "CTestPU Fan" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void Insert_AtEndOfString ()
    {
      var query = from oi in QueryFactory.CreateLinqQuery<OrderItem> () where oi.Product.Insert (6, "Test") == "CPU FanTest" select oi;

      CheckQueryResult (query, DomainObjectIDs.OrderItem2);
    }
  }
}