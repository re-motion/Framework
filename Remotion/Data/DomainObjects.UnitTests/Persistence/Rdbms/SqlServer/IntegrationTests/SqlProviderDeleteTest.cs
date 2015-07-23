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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderDeleteTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();
      SetDatabaseModifyable();
    }

    [Test]
    public void DeleteSingleDataContainer ()
    {
      IEnumerable<DataContainer> containers = new[] { GetDeletedOrderTicketContainer() };
      Provider.Connect();
      Provider.Save (containers);

      Assert.That (Provider.LoadDataContainer (DomainObjectIDs.OrderTicket1).LocatedObject, Is.Null);
    }

    [Test]
    public void DeleteRelatedDataContainers ()
    {
      Employee supervisor = DomainObjectIDs.Employee2.GetObject<Employee> ();
      Employee subordinate = DomainObjectIDs.Employee3.GetObject<Employee> ();
      Computer computer = DomainObjectIDs.Computer1.GetObject<Computer> ();

      supervisor.Delete();
      subordinate.Delete();
      computer.Delete();

      IEnumerable<DataContainer> containers = new[] 
                                              {
                                                  supervisor.InternalDataContainer,
                                                  subordinate.InternalDataContainer,
                                                  computer.InternalDataContainer
                                              };

      Provider.Connect();
      Provider.Save (containers);
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      OrderTicket changedOrderTicket;
      DataContainer changedDataContainer;
      using (clientTransaction1.EnterDiscardingScope())
      {
        changedOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
        changedOrderTicket.FileName = @"C:\NewFile.jpg";
        changedDataContainer = changedOrderTicket.InternalDataContainer;
      }

      OrderTicket deletedOrderTicket;
      DataContainer deletedDataContainer;
      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedOrderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
        deletedOrderTicket.Delete();
        deletedDataContainer = deletedOrderTicket.InternalDataContainer;
      }

      Provider.Connect();
      Provider.Save (new[] { changedDataContainer });
      Provider.Save (new[] { deletedDataContainer });
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException))]
    public void ConcurrentDeleteWithoutForeignKey ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();

      DataContainer changedDataContainer;
      ClassWithAllDataTypes changedObject;

      using (clientTransaction1.EnterDiscardingScope())
      {
        changedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
        changedDataContainer = changedObject.InternalDataContainer;
        changedObject.StringProperty = "New text";
      }

      DataContainer deletedDataContainer;
      ClassWithAllDataTypes deletedObject;

      using (clientTransaction2.EnterDiscardingScope())
      {
        deletedObject = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
        deletedDataContainer = deletedObject.InternalDataContainer;
        deletedObject.Delete();
      }

      Provider.Connect();
      Provider.Save (new[] { changedDataContainer });
      Provider.Save (new[] { deletedDataContainer });
    }

    private DataContainer GetDeletedOrderTicketContainer ()
    {
      OrderTicket orderTicket = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ();
      orderTicket.Delete();
      return orderTicket.InternalDataContainer;
    }
  }
}