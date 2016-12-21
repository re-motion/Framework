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
using Remotion.Data.DomainObjects.DomainImplementation.Cloning;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Cloning
{
  public abstract class CloneStrategyTestBase : StandardMappingTest
  {
    private MockRepository _mockRepository;
    private DomainObjectCloner _cloner;
    private CloneContext _contextMock;
    private ClientTransaction _sourceTransaction;
    private ClientTransaction _cloneTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _cloner = new DomainObjectCloner ();
      _contextMock = MockRepository.StrictMock<CloneContext>(Cloner);
      _sourceTransaction = ClientTransaction.CreateRootTransaction ();
      _cloneTransaction = ClientTransaction.CreateRootTransaction ();
    }

    protected MockRepository MockRepository
    {
      get { return _mockRepository; }
    }

    protected DomainObjectCloner Cloner
    {
      get { return _cloner; }
    }

    protected CloneContext ContextMock
    {
      get { return _contextMock; }
    }

    protected ClientTransaction SourceTransaction
    {
      get { return _sourceTransaction; }
    }

    protected ClientTransaction CloneTransaction
    {
      get { return _cloneTransaction; }
      set { _cloneTransaction = value; }
    }

    [Test]
    public void HandleReference_OneOne_RealSide ()
    {
      Computer source = DomainObjectMother.CreateObjectInTransaction<Computer> (SourceTransaction);
      Computer clone = DomainObjectMother.CreateObjectInTransaction<Computer> (CloneTransaction);
      Employee sourceRelated = DomainObjectMother.CreateObjectInTransaction<Employee> (SourceTransaction);
      Employee cloneRelated = DomainObjectMother.CreateObjectInTransaction<Employee> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Computer), "Employee"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Computer), "Employee"];

      source.Employee = sourceRelated;

      HandleReference_OneOne_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_RealSide_Checks (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneOne_RealSide_Null ()
    {
      Computer source = DomainObjectMother.CreateObjectInTransaction<Computer> (SourceTransaction);
      Computer clone = DomainObjectMother.CreateObjectInTransaction<Computer> (CloneTransaction);
      Employee sourceRelated = null;
      Employee cloneRelated = null;

      PropertyAccessor sourceReference = source.Properties[typeof (Computer), "Employee"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Computer), "Employee"];

      source.Employee = sourceRelated;

      HandleReference_OneOne_RealSide_Checks_Null (sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_RealSide_Checks_Null (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneOne_VirtualSide ()
    {
      Employee source = DomainObjectMother.CreateObjectInTransaction<Employee> (SourceTransaction);
      Employee clone = DomainObjectMother.CreateObjectInTransaction<Employee> (CloneTransaction);
      Computer sourceRelated = DomainObjectMother.CreateObjectInTransaction<Computer> (SourceTransaction);
      Computer cloneRelated = DomainObjectMother.CreateObjectInTransaction<Computer> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Employee), "Computer"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Employee), "Computer"];

      source.Computer = sourceRelated;

      HandleReference_OneOne_VirtualSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_VirtualSide_Checks (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneOne_VirtualSide_Null ()
    {
      Employee source = DomainObjectMother.CreateObjectInTransaction<Employee> (SourceTransaction);
      Employee clone = DomainObjectMother.CreateObjectInTransaction<Employee> (CloneTransaction);
      Computer sourceRelated = null;
      Computer cloneRelated = null;

      PropertyAccessor sourceReference = source.Properties[typeof (Employee), "Computer"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Employee), "Computer"];

      source.Computer = sourceRelated;

      HandleReference_OneOne_VirtualSide_Checks_Null (sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneOne_VirtualSide_Checks_Null (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public virtual void HandleReference_OneMany_RealSide ()
    {
      OrderItem source = DomainObjectMother.CreateObjectInTransaction<OrderItem> (SourceTransaction);
      OrderItem clone = DomainObjectMother.CreateObjectInTransaction<OrderItem> (CloneTransaction);
      Order sourceRelated = DomainObjectMother.CreateObjectInTransaction<Order> (SourceTransaction);
      Order cloneRelated = DomainObjectMother.CreateObjectInTransaction<Order> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (OrderItem), "Order"];
      PropertyAccessor cloneReference = clone.Properties[typeof (OrderItem), "Order"];

      source.Order = sourceRelated;

      HandleReference_OneMany_RealSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_RealSide_Checks (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference);
    
    [Test]
    public void HandleReference_OneMany_RealSide_Null ()
    {
      OrderItem source = DomainObjectMother.CreateObjectInTransaction<OrderItem> (SourceTransaction);
      OrderItem clone = DomainObjectMother.CreateObjectInTransaction<OrderItem> (CloneTransaction);
      Order sourceRelated = null;
      Order cloneRelated = null;

      PropertyAccessor sourceReference = source.Properties[typeof (OrderItem), "Order"];
      PropertyAccessor cloneReference = clone.Properties[typeof (OrderItem), "Order"];

      source.Order = sourceRelated;

      HandleReference_OneMany_RealSide_Checks_Null (sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_RealSide_Checks_Null (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_VirtualSide ()
    {
      Cloner.CloneTransaction = CloneTransaction;
      
      Order source = DomainObjectMother.CreateObjectInTransaction<Order> (SourceTransaction);
      Order clone = DomainObjectMother.CreateObjectInTransaction<Order> (CloneTransaction);
      OrderItem sourceRelated = DomainObjectMother.CreateObjectInTransaction<OrderItem> (SourceTransaction);
      OrderItem cloneRelated = DomainObjectMother.CreateObjectInTransaction<OrderItem> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Order), "OrderItems"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Order), "OrderItems"];

      source.OrderItems.Add (sourceRelated);

      HandleReference_OneMany_VirtualSide_Checks(sourceRelated, sourceReference, cloneRelated, cloneReference);
    }

    protected abstract void HandleReference_OneMany_VirtualSide_Checks (OrderItem sourceRelated, PropertyAccessor sourceReference, OrderItem cloneRelated, PropertyAccessor cloneReference);

    [Test]
    public void HandleReference_OneMany_VirtualSide_Empty ()
    {
      Cloner.CloneTransaction = CloneTransaction;

      Order source = DomainObjectMother.CreateObjectInTransaction<Order> (SourceTransaction);
      Order clone = DomainObjectMother.CreateObjectInTransaction<Order> (CloneTransaction);

      PropertyAccessor sourceReference = source.Properties[typeof (Order), "OrderItems"];
      PropertyAccessor cloneReference = clone.Properties[typeof (Order), "OrderItems"];

      HandleReference_OneMany_VirtualSide_Checks_Null (sourceReference, cloneReference);
    }

    protected abstract void HandleReference_OneMany_VirtualSide_Checks_Null (PropertyAccessor sourceReference, PropertyAccessor cloneReference);
  }
}
