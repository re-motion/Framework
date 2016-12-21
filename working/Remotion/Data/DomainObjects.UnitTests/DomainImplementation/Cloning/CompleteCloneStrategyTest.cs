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
  [TestFixture]
  public class CompleteCloneStrategyTest : CloneStrategyTestBase
  {
    private CompleteCloneStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp ();
      _strategy = new CompleteCloneStrategy ();
    }

    protected override void HandleReference_OneOne_RealSide_Checks (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneOne_RealSide_Checks_Null (Employee sourceRelated, PropertyAccessor sourceReference, Employee cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.Null);
    }

    protected override void HandleReference_OneOne_VirtualSide_Checks (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneOne_VirtualSide_Checks_Null (Computer sourceRelated, PropertyAccessor sourceReference, Computer cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.Null);
    }

    protected override void HandleReference_OneMany_RealSide_Checks (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneMany_RealSide_Checks_Null (Order sourceRelated, PropertyAccessor sourceReference, Order cloneRelated, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.Null);
    }

    protected override void HandleReference_OneMany_VirtualSide_Checks (OrderItem sourceRelated, PropertyAccessor sourceReference, OrderItem cloneRelated, PropertyAccessor cloneReference)
    {
      Expect.Call (ContextMock.GetCloneFor<DomainObject> (sourceRelated)).Return (cloneRelated);

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (((DomainObjectCollection)cloneReference.GetValueWithoutTypeCheck ())[0], Is.SameAs (cloneRelated));
    }

    protected override void HandleReference_OneMany_VirtualSide_Checks_Null (PropertyAccessor sourceReference, PropertyAccessor cloneReference)
    {
      // expect no call

      MockRepository.ReplayAll ();
      _strategy.HandleReference (sourceReference, cloneReference, ContextMock);
      MockRepository.VerifyAll ();

      Assert.That (cloneReference.GetValueWithoutTypeCheck (), Is.Empty);
    }
  }
}
