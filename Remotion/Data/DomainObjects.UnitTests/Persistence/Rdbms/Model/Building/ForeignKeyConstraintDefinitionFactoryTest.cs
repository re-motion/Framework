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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class ForeignKeyConstraintDefinitionFactoryTest : StandardMappingTest
  {
    private ObjectIDStoragePropertyDefinition _fakeObjectIDStoragePropertyDefinition;

    private Mock<IStorageNameProvider> _storageNameProviderMock;
    private IRdbmsPersistenceModelProvider _persistenceModelProvider;
    private Mock<IInfrastructureStoragePropertyDefinitionProvider> _infrastructureStoragePropertyDefintionProviderMock;

    private ForeignKeyConstraintDefinitionFactory _factory;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _fakeObjectIDStoragePropertyDefinition = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;

      _storageNameProviderMock = new Mock<IStorageNameProvider>(MockBehavior.Strict);
      _persistenceModelProvider = new RdbmsPersistenceModelProvider();
      _infrastructureStoragePropertyDefintionProviderMock = new Mock<IInfrastructureStoragePropertyDefinitionProvider>(MockBehavior.Strict);

      _factory = new ForeignKeyConstraintDefinitionFactory(
          _storageNameProviderMock.Object,
          _persistenceModelProvider,
          _infrastructureStoragePropertyDefintionProviderMock.Object);
    }

    [Test]
    public void CreateForeignKeyConstraints ()
    {
      var orderClassDefinition = Configuration.GetTypeDefinition(typeof(Order));
      var customerClassDefintion = Configuration.GetTypeDefinition(typeof(Customer));

      _infrastructureStoragePropertyDefintionProviderMock
          .Setup(mock => mock.GetObjectIDStoragePropertyDefinition())
          .Returns(_fakeObjectIDStoragePropertyDefinition)
          .Verifiable();

      var customerProperty = orderClassDefinition.MyPropertyDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"];
      var expectedComparedColumns = ((IRdbmsStoragePropertyDefinition)customerProperty.StoragePropertyDefinition).GetColumnsForComparison();

      _storageNameProviderMock
          .Setup(mock => mock.GetForeignKeyConstraintName(orderClassDefinition, expectedComparedColumns))
          .Returns("FakeConstraintName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(customerClassDefintion))
          .Returns(new EntityNameDefinition(null, "FakeTableName"))
          .Verifiable();

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints(orderClassDefinition).ToArray();

      _infrastructureStoragePropertyDefintionProviderMock.Verify();
      _storageNameProviderMock.Verify();

      //OrderItem and OrderTicket endpoints are virtual and Official endpoint has different storage provider
      Assert.That(foreignKeyConstraintDefinitions.Length, Is.EqualTo(1));
      var foreignKeyConstraint = foreignKeyConstraintDefinitions[0];
      Assert.That(foreignKeyConstraint.ReferencedTableName.EntityName, Is.EqualTo("FakeTableName"));
      Assert.That(foreignKeyConstraint.ReferencedTableName.SchemaName, Is.Null);
      Assert.That(foreignKeyConstraint.ConstraintName, Is.EqualTo("FakeConstraintName"));
      Assert.That(foreignKeyConstraint.ReferencingColumns, Is.EqualTo(expectedComparedColumns));
      Assert.That(foreignKeyConstraint.ReferencedColumns, Is.EqualTo(_fakeObjectIDStoragePropertyDefinition.GetColumnsForComparison()));
    }

    [Test]
    public void CreateForeignKeyConstraints_StorageClassTransactionPropertiesAreIgnored ()
    {
      var computerClassDefinition = Configuration.GetClassDefinition("Computer");
      var employeeClassDefinition = Configuration.GetClassDefinition("Employee");

      _infrastructureStoragePropertyDefintionProviderMock
          .Setup(mock => mock.GetObjectIDStoragePropertyDefinition())
          .Returns(_fakeObjectIDStoragePropertyDefinition)
          .Verifiable();

      var employeeProperty = computerClassDefinition.MyPropertyDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"];
      var expectedComparedColumns = ((IRdbmsStoragePropertyDefinition)employeeProperty.StoragePropertyDefinition).GetColumnsForComparison();

      _storageNameProviderMock
          .Setup(mock => mock.GetForeignKeyConstraintName(computerClassDefinition, expectedComparedColumns))
          .Returns("FakeConstraintName")
          .Verifiable();
      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(employeeClassDefinition))
          .Returns(new EntityNameDefinition(null, "FakeTableName"))
          .Verifiable();

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints(computerClassDefinition).ToArray();

      _infrastructureStoragePropertyDefintionProviderMock.Verify();
      _storageNameProviderMock.Verify();
      Assert.That(foreignKeyConstraintDefinitions.Length, Is.EqualTo(1)); //EmployeeTransactionProperty relation property is filtered
    }

    [Test]
    public void CreateForeignKeyConstraints_NoEntityName_NoForeignKeyConstrainedIsCreated ()
    {
      var orderClassDefinition = Configuration.GetClassDefinition("Order");

      _storageNameProviderMock
          .Setup(mock => mock.GetTableName(It.IsAny<ClassDefinition>()))
          .Returns((EntityNameDefinition)null)
          .Verifiable();

      var result = _factory.CreateForeignKeyConstraints(orderClassDefinition).ToArray();

      Assert.That(result.Length, Is.EqualTo(0));
    }
  }
}
