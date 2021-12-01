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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation.Cloning;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Cloning
{
  [TestFixture]
  public class CloneContextTest : ClientTransactionBaseTest
  {
    private Mock<DomainObjectCloner> _clonerMock;

    public override void SetUp ()
    {
      base.SetUp();
      _clonerMock = new Mock<DomainObjectCloner> (MockBehavior.Strict);
    }

    [Test]
    public void Initialization ()
    {
      CloneContext context = new CloneContext(_clonerMock.Object);
      Assert.That(context.CloneHulls.Count, Is.EqualTo(0));
    }

    [Test]
    public void GetCloneFor_CallsClonerForNewObject ()
    {
      CloneContext context = new CloneContext(_clonerMock.Object);
      Order source = DomainObjectIDs.Order1.GetObject<Order>();
      Order clone = Order.NewObject();

      _clonerMock.Setup (_ => _.CreateCloneHull<DomainObject> (source)).Returns (clone).Verifiable();
      Assert.That(context.GetCloneFor(source), Is.SameAs(clone));
      _clonerMock.Verify();
    }

    [Test]
    public void GetCloneFor_DoesntCallClonerTwiceForKnownObject ()
    {
      CloneContext context = new CloneContext(_clonerMock.Object);
      Order source = DomainObjectIDs.Order1.GetObject<Order>();
      Order clone = Order.NewObject();

      _clonerMock.Setup (_ => _.CreateCloneHull<DomainObject> (source)).Returns (clone).Verifiable();
      Assert.That(context.GetCloneFor(source), Is.SameAs(clone));
      Assert.That(context.GetCloneFor(source), Is.SameAs(clone));
      _clonerMock.Verify();
    }

    [Test]
    public void GetCloneFor_AddsToShallowClones ()
    {
      CloneContext context = new CloneContext(_clonerMock.Object);
      Order source = DomainObjectIDs.Order1.GetObject<Order>();
      Order clone = Order.NewObject();

      _clonerMock.Setup (_ => _.CreateCloneHull<DomainObject> (source)).Returns (clone);

      context.GetCloneFor(source);
      Assert.That(context.CloneHulls.Contains(new Tuple<DomainObject, DomainObject>(source, clone)));
    }

    [Test]
    public void GetCloneFor_DoesntAddToShallowClonesForKnownObject ()
    {
      CloneContext context = new CloneContext(_clonerMock.Object);
      Order source = DomainObjectIDs.Order1.GetObject<Order>();
      Order clone = Order.NewObject();

      _clonerMock.Setup (_ => _.CreateCloneHull<DomainObject> (source)).Returns (clone);

      context.GetCloneFor(source);
      context.GetCloneFor(source);
      Assert.That(context.CloneHulls.Count, Is.EqualTo(1));
    }
  }
}
