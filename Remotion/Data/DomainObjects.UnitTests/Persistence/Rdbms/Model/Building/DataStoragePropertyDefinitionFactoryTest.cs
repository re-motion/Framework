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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class DataStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private Mock<IValueStoragePropertyDefinitionFactory> _valuePropertyFactoryMock;
    private Mock<IRelationStoragePropertyDefinitionFactory> _relationPropertyFactoryMock;

    private DataStoragePropertyDefinitionFactory _factory;

    private Mock<IRdbmsStoragePropertyDefinition> _fakeStoragePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _valuePropertyFactoryMock = new Mock<IValueStoragePropertyDefinitionFactory>(MockBehavior.Strict);
      _relationPropertyFactoryMock = new Mock<IRelationStoragePropertyDefinitionFactory>(MockBehavior.Strict);

      _factory = new DataStoragePropertyDefinitionFactory(_valuePropertyFactoryMock.Object, _relationPropertyFactoryMock.Object);

      _fakeStoragePropertyDefinition = new Mock<IRdbmsStoragePropertyDefinition>();
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_RelationEndPoint ()
    {
      var endPointDefinition = GetNonVirtualEndPointDefinition(typeof(OrderItem), "Order");
      var propertyDefinition = endPointDefinition.PropertyDefinition;
      _relationPropertyFactoryMock.Setup(mock => mock.CreateStoragePropertyDefinition(endPointDefinition)).Returns(_fakeStoragePropertyDefinition.Object).Verifiable();

      var result = _factory.CreateStoragePropertyDefinition(propertyDefinition);

      _relationPropertyFactoryMock.Verify();
      Assert.That(result, Is.SameAs(_fakeStoragePropertyDefinition.Object));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_NoRelationEndPoint ()
    {
      var propertyDefinition = GetPropertyDefinition(typeof(Order), "OrderNumber");
      _valuePropertyFactoryMock.Setup(mock => mock.CreateStoragePropertyDefinition(propertyDefinition)).Returns(_fakeStoragePropertyDefinition.Object).Verifiable();

      var result = _factory.CreateStoragePropertyDefinition(propertyDefinition);

      _valuePropertyFactoryMock.Verify();
      Assert.That(result, Is.SameAs(_fakeStoragePropertyDefinition.Object));
    }
  }
}
