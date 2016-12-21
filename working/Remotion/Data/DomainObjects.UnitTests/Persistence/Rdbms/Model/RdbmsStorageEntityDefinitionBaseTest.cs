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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class RdbmsStorageEntityDefinitionBaseTest
  {
    private TestableRdbmsStorageEntityDefinitionBase _rdbmsStorageEntityDefinition;

    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _timestampProperty;
    private SimpleStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SimpleStoragePropertyDefinition _property3;

    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");

      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");

      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _rdbmsStorageEntityDefinition = new TestableRdbmsStorageEntityDefinitionBase (
          _storageProviderDefinition,
          new EntityNameDefinition ("Schema", "Test"),
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_rdbmsStorageEntityDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_rdbmsStorageEntityDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));

      Assert.That (_rdbmsStorageEntityDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_rdbmsStorageEntityDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_rdbmsStorageEntityDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_rdbmsStorageEntityDefinition.Indexes, Is.EqualTo (_indexes));
      Assert.That (_rdbmsStorageEntityDefinition.Synonyms, Is.EqualTo (_synonyms));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var entityDefinition = new TestableRdbmsStorageEntityDefinitionBase (
          _storageProviderDefinition,
          null,
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]
          );
      Assert.That (entityDefinition.ViewName, Is.Null);
    }

    [Test]
    public void StorageProviderID ()
    {
      Assert.That (_rdbmsStorageEntityDefinition.StorageProviderID, Is.EqualTo ("SPID"));
    }

    [Test]
    public void GetAllProperties ()
    {
      var result = _rdbmsStorageEntityDefinition.GetAllProperties();

      Assert.That (
          result,
          Is.EqualTo (
              new IRdbmsStoragePropertyDefinition[]
              {
                  _objectIDProperty,
                  _timestampProperty,
                  _property1,
                  _property2,
                  _property3
              }));
    }

    [Test]
    public void GetAllColumns ()
    {
      var result = _rdbmsStorageEntityDefinition.GetAllColumns();

      Assert.That (
          result,
          Is.EqualTo (
              new[]
              {
                  StoragePropertyDefinitionTestHelper.GetIDColumnDefinition (_objectIDProperty),
                  StoragePropertyDefinitionTestHelper.GetClassIDColumnDefinition (_objectIDProperty),
                  _timestampProperty.ColumnDefinition,
                  _property1.ColumnDefinition,
                  _property2.ColumnDefinition,
                  _property3.ColumnDefinition
              }));
    }
  }
}