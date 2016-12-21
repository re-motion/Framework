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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class InfrastructureStoragePropertyDefinitionProviderTest : StandardMappingTest
  {
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private StorageTypeInformation _idStorageTypeInformation;
    private StorageTypeInformation _classIDStorageTypeInformation;
    private StorageTypeInformation _timestampStorageTypeInformation;

    private IStorageNameProvider _storageNameProviderStub;
    
    private InfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider> ();

      _idStorageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      _storageTypeInformationProviderStub
          .Stub (stub => stub.GetStorageTypeForID (false))
          .Return (_idStorageTypeInformation);

      _classIDStorageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      _storageTypeInformationProviderStub
          .Stub (stub => stub.GetStorageTypeForClassID (false))
          .Return (_classIDStorageTypeInformation);

      _timestampStorageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      _storageTypeInformationProviderStub
          .Stub (stub => stub.GetStorageTypeForTimestamp (false))
          .Return (_timestampStorageTypeInformation);

      _storageNameProviderStub = MockRepository.GenerateStub<IStorageNameProvider>();
      _storageNameProviderStub.Stub (stub => stub.GetIDColumnName()).Return ("ID");
      _storageNameProviderStub.Stub (stub => stub.GetClassIDColumnName()).Return ("ClassID");
      _storageNameProviderStub.Stub (stub => stub.GetTimestampColumnName()).Return ("Timestamp");

      _infrastructureStoragePropertyDefinitionProvider = 
          new InfrastructureStoragePropertyDefinitionProvider (_storageTypeInformationProviderStub, _storageNameProviderStub);
    }

    [Test]
    public void GetObjectIDStoragePropertyDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition ();

      Assert.That (result.ValueProperty, Is.TypeOf<SimpleStoragePropertyDefinition> ());
      Assert.That (result.ValueProperty.PropertyType, Is.SameAs (typeof (object)));
      var idColumn = StoragePropertyDefinitionTestHelper.GetSingleColumn (result.ValueProperty);
      Assert.That (idColumn.Name, Is.EqualTo ("ID"));
      Assert.That (idColumn.IsPartOfPrimaryKey, Is.True);
      Assert.That (idColumn.StorageTypeInfo, Is.SameAs (_idStorageTypeInformation));

      Assert.That (result.ClassIDProperty, Is.TypeOf<SimpleStoragePropertyDefinition> ());
      Assert.That (result.ClassIDProperty.PropertyType, Is.SameAs (typeof (string)));
      var classIDColumn = StoragePropertyDefinitionTestHelper.GetSingleColumn (result.ClassIDProperty);
      Assert.That (classIDColumn.Name, Is.EqualTo ("ClassID"));
      Assert.That (classIDColumn.StorageTypeInfo, Is.SameAs (_classIDStorageTypeInformation));
      Assert.That (classIDColumn.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void GetObjectIDStoragePropertyDefinition_CachedInstance ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition ();
      var result2 = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition ();
      Assert.That (result2, Is.SameAs (result));
    }

    [Test]
    public void GetTimestampStoragePropertyDefinition ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition ();

      Assert.That (result, Is.TypeOf<SimpleStoragePropertyDefinition> ());
      Assert.That (result.PropertyType, Is.SameAs (typeof (object)));
      var column = ((SimpleStoragePropertyDefinition) result).ColumnDefinition;
      Assert.That (column.Name, Is.EqualTo ("Timestamp"));
      Assert.That (column.StorageTypeInfo, Is.SameAs (_timestampStorageTypeInformation));
      Assert.That (column.IsPartOfPrimaryKey, Is.False);
    }

    [Test]
    public void GetTimestampStoragePropertyDefinition_CachedInstance ()
    {
      var result = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition ();
      var result2 = _infrastructureStoragePropertyDefinitionProvider.GetTimestampStoragePropertyDefinition ();
      Assert.That (result2, Is.SameAs (result));
    }
  }
}