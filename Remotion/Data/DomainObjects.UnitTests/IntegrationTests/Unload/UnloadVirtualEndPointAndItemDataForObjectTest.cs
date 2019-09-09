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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Unload
{
  [TestFixture]
  public class UnloadVirtualEndPointAndItemDataForObjectTest : UnloadTestBase
  {
    [Test]
    public void UnloadVirtualEndPointAndItemData_Object ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderTicket = order.OrderTicket;

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (order, o => o.OrderTicket));

      CheckDataContainerExists (order, true);
      CheckDataContainerExists (orderTicket, false);

      CheckEndPointExists (order, "OrderTicket", false);

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Null ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee> ();
      Assert.That (employee.Computer, Is.Null);

      CheckDataContainerExists (employee, true);
      CheckEndPointExists (employee, "Computer", true);

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (employee, e => e.Computer));

      CheckDataContainerExists (employee, true);
      CheckEndPointExists (employee, "Computer", false);

      Assert.That (employee.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Reload ()
    {
      SetDatabaseModifyable ();

      var employee = DomainObjectIDs.Employee3.GetObject<Employee> ();
      var computer = employee.Computer;

      ObjectID newComputerID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var employeeInOtherTx = employee.ID.GetObject<Employee> ();
        var newComputer = Computer.NewObject ();
        newComputerID = newComputer.ID;
        employeeInOtherTx.Computer = newComputer;

        ClientTransaction.Current.Commit ();
      }

      Assert.That (employee.Computer, Is.SameAs (computer));

      UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (employee, e => e.Computer));

      Assert.That (employee.Computer, Is.Not.SameAs (computer));
      Assert.That (employee.Computer.ID, Is.EqualTo (newComputerID));
      Assert.That (computer.Employee, Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_Events ()
    {
      var order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderTicket = order1.OrderTicket;

      var listenerMock = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (TestableClientTransaction);
      using (listenerMock.GetMockRepository ().Ordered ())
      {
        listenerMock
            .Expect (mock => mock.ObjectsUnloading (
                Arg.Is (TestableClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderTicket.OnUnloadingCalled, Is.False, "items unloaded after this method is called");
              Assert.That (orderTicket.OnUnloadedCalled, Is.False, "items unloaded after this method is called");

              Assert.That (orderTicket.State, Is.EqualTo (StateType.Unchanged));
            });
        listenerMock
            .Expect (mock => mock.ObjectsUnloaded (
                Arg.Is (TestableClientTransaction),
                Arg<ReadOnlyCollection<DomainObject>>.List.Equal (new[] { orderTicket })))
            .WhenCalled (
            mi =>
            {
              Assert.That (orderTicket.OnUnloadingCalled, Is.True, "items unloaded before this method is called");
              Assert.That (orderTicket.OnUnloadedCalled, Is.True, "items unloaded before this method is called");

              Assert.That (orderTicket.State, Is.EqualTo (StateType.NotLoadedYet));
            });
      }

      listenerMock.Replay ();
      try
      {
        UnloadService.UnloadVirtualEndPointAndItemData (TestableClientTransaction, RelationEndPointID.Resolve (order1, o => o.OrderTicket));

        listenerMock.VerifyAllExpectations ();
      }
      finally
      {
        listenerMock.BackToRecord (); // For Discarding
      }

      Assert.That (orderTicket.UnloadingState, Is.EqualTo (StateType.Unchanged), "OnUnloading before state change");
      Assert.That (orderTicket.UnloadedState, Is.EqualTo (StateType.NotLoadedYet), "OnUnloaded after state change");
    }
  }
}