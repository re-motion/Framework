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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class MandatoryRelationValidatorTest : StandardMappingTest
  {
    private MandatoryRelationValidator _validator;

    private Mock<IRelationEndPoint> _endPointMock;

    private IRelationEndPointDefinition _mandatoryEndPointDefinition;
    private IRelationEndPointDefinition _nonMandatoryEndPointDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _validator = new MandatoryRelationValidator();

      _endPointMock = new Mock<IRelationEndPoint> (MockBehavior.Strict);

      _mandatoryEndPointDefinition = GetEndPointDefinition(typeof(Order), "OrderTicket");
      _nonMandatoryEndPointDefinition = GetEndPointDefinition(typeof(Computer), "Employee");
    }

    [Test]
    public void Validate_ChecksComplete_AndMandatoryRelationEndPoints ()
    {
      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, new[] { _endPointMock.Object });

      _endPointMock.Setup (stub => stub.Definition).Returns (_mandatoryEndPointDefinition);
      _endPointMock.Setup (stub => stub.IsDataComplete).Returns (true);
      _endPointMock.Setup (mock => mock.ValidateMandatory()).Verifiable();

      _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem);

      _endPointMock.Verify();
    }

    [Test]
    public void Validate_IgnoresIncompleteEndPoints ()
    {
      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, new[] { _endPointMock.Object });

      _endPointMock.Setup (stub => stub.Definition).Returns (_mandatoryEndPointDefinition);
      _endPointMock.Setup (stub => stub.IsDataComplete).Returns (false);

      _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem);

      _endPointMock.Verify (mock => mock.ValidateMandatory(), Times.Never());
    }

    [Test]
    public void Validate_IgnoresNonMandatoryEndPoints ()
    {
      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, new[] { _endPointMock.Object });

      _endPointMock.Setup (stub => stub.Definition).Returns (_nonMandatoryEndPointDefinition);
      _endPointMock.Setup (stub => stub.IsDataComplete).Returns (true);

      _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem);

      _endPointMock.Verify (mock => mock.ValidateMandatory(), Times.Never());
    }

    [Test]
    public void Validate_IgnoresDeletedObjects ()
    {
      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetDeleted().Value, new[] { _endPointMock.Object });

      _endPointMock.Setup (stub => stub.Definition).Returns (_mandatoryEndPointDefinition);
      _endPointMock.Setup (stub => stub.IsDataComplete).Returns (true);

      _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem);

      _endPointMock.Verify (mock => mock.ValidateMandatory(), Times.Never());
    }

    [Test]
    public void Validate_IntegrationTest_RelationsOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderTicket = OrderTicket.NewObject();
        orderTicket.Order = Order.NewObject();

        var persistableData = PersistableDataObjectMother.Create(ClientTransaction.Current, orderTicket);
        Assert.That(() => _validator.Validate(ClientTransaction.Current, persistableData), Throws.Nothing);
      }
    }

    [Test]
    public void Validate_IntegrationTest_RelationsNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderTicket = OrderTicket.NewObject();

        var persistableData = PersistableDataObjectMother.Create(ClientTransaction.Current, orderTicket);
        Assert.That(
            () => _validator.Validate(ClientTransaction.Current, persistableData),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.Matches(
                @"Mandatory relation property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.OrderTicket\.Order' of domain object "
                + @"'OrderTicket|.*|System\.Guid' cannot be null."));
      }
    }

    private PersistableData CreatePersistableData (DomainObjectState domainObjectState, IRelationEndPoint[] associatedEndPointSequence)
    {
      var domainObject = DomainObjectMother.CreateFakeObject<OrderItem>(DomainObjectIDs.OrderItem1);

      var dataContainer = DataContainer.CreateNew(domainObject.ID);
      return new PersistableData(domainObject, domainObjectState, dataContainer, associatedEndPointSequence);
    }

  }
}
