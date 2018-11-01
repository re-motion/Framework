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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class DataStoragePropertyDefinitionFactoryTest : StandardMappingTest
  {
    private IValueStoragePropertyDefinitionFactory _valuePropertyFactoryMock;
    private IRelationStoragePropertyDefinitionFactory _relationPropertyFactoryMock;

    private DataStoragePropertyDefinitionFactory _factory;

    private IRdbmsStoragePropertyDefinition _fakeStoragePropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _valuePropertyFactoryMock = MockRepository.GenerateStrictMock<IValueStoragePropertyDefinitionFactory> ();
      _relationPropertyFactoryMock = MockRepository.GenerateStrictMock<IRelationStoragePropertyDefinitionFactory> ();

      _factory = new DataStoragePropertyDefinitionFactory (_valuePropertyFactoryMock, _relationPropertyFactoryMock);

      _fakeStoragePropertyDefinition = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_RelationEndPoint ()
    {
      var endPointDefinition = GetNonVirtualEndPointDefinition (typeof (OrderItem), "Order");
      var propertyDefinition = endPointDefinition.PropertyDefinition;
      _relationPropertyFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (endPointDefinition)).Return (_fakeStoragePropertyDefinition);

      var result = _factory.CreateStoragePropertyDefinition (propertyDefinition);

      _relationPropertyFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeStoragePropertyDefinition));
    }

    [Test]
    public void CreateStoragePropertyDefinition_PropertyDefinition_NoRelationEndPoint ()
    {
      var propertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");
      _valuePropertyFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (propertyDefinition)).Return (_fakeStoragePropertyDefinition);

      var result = _factory.CreateStoragePropertyDefinition (propertyDefinition);

      _valuePropertyFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeStoragePropertyDefinition));
    }

    [Test]
    public void CreateStoragePropertyDefinition_Value_Null ()
    {
      _valuePropertyFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition (null, "Value")).Return (_fakeStoragePropertyDefinition);

      var result = _factory.CreateStoragePropertyDefinition ((object) null);

      _valuePropertyFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fakeStoragePropertyDefinition));
    }

    [Test]
    public void CreateStoragePropertyDefinition_Value_NonObjectID ()
    {
      _valuePropertyFactoryMock.Expect (mock => mock.CreateStoragePropertyDefinition ("test", "Value")).Return (_fakeStoragePropertyDefinition);

      var result = _factory.CreateStoragePropertyDefinition ("test");

      _valuePropertyFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeStoragePropertyDefinition));
    }

    [Test]
    public void CreateStoragePropertyDefinition_Value_ObjectID ()
    {
      var expectedClassDefinition = GetTypeDefinition (typeof (Order));
      _relationPropertyFactoryMock
          .Expect (mock => mock.CreateStoragePropertyDefinition (expectedClassDefinition, "Value", "ValueClassID"))
          .Return (_fakeStoragePropertyDefinition);

      var result = _factory.CreateStoragePropertyDefinition (DomainObjectIDs.Order1);

      _relationPropertyFactoryMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_fakeStoragePropertyDefinition));
    }
  }
}