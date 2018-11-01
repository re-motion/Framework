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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class IndirectDomainObjectCollectionEventRaiserTest : ClientTransactionBaseTest
  {
    private Order _order;

    public override void SetUp ()
    {
      base.SetUp();
      _order = Order.NewObject();
    }

    [Test]
    public void Initialization_WithoutEventRaiser ()
    {
      var indirectRaiser = new IndirectDomainObjectCollectionEventRaiser ();

      Assert.That (indirectRaiser.EventRaiser, Is.Null);
    }

    [Test]
    public void Initialization_WithEventRaiser ()
    {
      var innerRaiserStub = MockRepository.GenerateStub<IDomainObjectCollectionEventRaiser> ();
      var indirectRaiser = new IndirectDomainObjectCollectionEventRaiser (innerRaiserStub);

      Assert.That (indirectRaiser.EventRaiser, Is.SameAs (innerRaiserStub));
    }

    [Test]
    public void Members_AreDelegated ()
    {
      CheckDelegation (raiser => raiser.BeginAdd (1, _order));
      CheckDelegation (raiser => raiser.EndAdd (1, _order));
      CheckDelegation (raiser => raiser.BeginRemove (1, _order));
      CheckDelegation (raiser => raiser.EndRemove (1, _order));
      CheckDelegation (raiser => raiser.BeginDelete());
      CheckDelegation (raiser => raiser.EndDelete ());
      CheckDelegation (raiser => raiser.WithinReplaceData());
    }

    [Test]
    public void Members_ThrowWhenEventRaiserNotSet ()
    {
      CheckThrowOnNoEventRaiser (raiser => raiser.BeginAdd (1, _order));
      CheckThrowOnNoEventRaiser (raiser => raiser.EndAdd (1, _order));
      CheckThrowOnNoEventRaiser (raiser => raiser.BeginRemove (1, _order));
      CheckThrowOnNoEventRaiser (raiser => raiser.EndRemove (1, _order));
      CheckThrowOnNoEventRaiser (raiser => raiser.BeginDelete ());
      CheckThrowOnNoEventRaiser (raiser => raiser.EndDelete ());
      CheckDelegation (raiser => raiser.WithinReplaceData());
    }

    [Test]
    public void Serializable ()
    {
      var indirectRaiser = new IndirectDomainObjectCollectionEventRaiser ();
      Serializer.SerializeAndDeserialize (indirectRaiser);
    }

    private void CheckDelegation (Action<IDomainObjectCollectionEventRaiser> action)
    {
      var indirectRaiser = new IndirectDomainObjectCollectionEventRaiser();
      var actualRaiserMock = MockRepository.GenerateMock<IDomainObjectCollectionEventRaiser>();
      indirectRaiser.EventRaiser = actualRaiserMock;

      action (indirectRaiser);

      actualRaiserMock.AssertWasCalled (action);
    }

    private void CheckThrowOnNoEventRaiser (Action<IDomainObjectCollectionEventRaiser> action)
    {
      var indirectRaiser = new IndirectDomainObjectCollectionEventRaiser();

      try
      {
        action (indirectRaiser);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // ok
      }
    }
  }
}