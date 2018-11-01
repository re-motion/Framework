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
  public class FilterViewDefinitionTest
  {
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    private SimpleStoragePropertyDefinition _timestampProperty;
    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SimpleStoragePropertyDefinition _property3;
    
    private TableDefinition _baseEntityDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");

      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");

      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _baseEntityDefinition = TableDefinitionObjectMother.Create (_storageProviderDefinition);

      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition ("Schema", "Test"),
          _baseEntityDefinition,
          new[] { "ClassId1", "ClassId2" },
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_filterViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_filterViewDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));
      Assert.That (_filterViewDefinition.ClassIDs, Is.EqualTo (new[] { "ClassId1", "ClassId2" }));
      Assert.That (_filterViewDefinition.BaseEntity, Is.SameAs (_baseEntityDefinition));

      Assert.That (_filterViewDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_filterViewDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_filterViewDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_filterViewDefinition.Indexes, Is.EqualTo (_indexes));
      Assert.That (_filterViewDefinition.Synonyms, Is.EqualTo (_synonyms));
    }

    [Test]
    public void Initialization_WithBaseFilterViewEntity ()
    {
      new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _filterViewDefinition,
          new[] { "x" },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The base entity must either be a TableDefinition or a FilterViewDefinition.\r\nParameter name: baseEntity")]
    public void Initialization_WithInvalidBaseEntity ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (_storageProviderDefinition);

      new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          unionViewDefinition,
          new[] { "x" },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          null,
          _baseEntityDefinition,
          new[] { "ClassId" },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (filterViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void GetBaseTable ()
    {
      var table = _filterViewDefinition.GetBaseTable();

      Assert.That (table, Is.SameAs (_baseEntityDefinition));
    }

    [Test]
    public void GetBaseTable_IndirectTable ()
    {
      var derivedFilterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          _filterViewDefinition,
          new[] { "x" },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var table = derivedFilterViewDefinition.GetBaseTable();

      Assert.That (table, Is.SameAs (_baseEntityDefinition));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitFilterViewDefinition (_filterViewDefinition));
      visitorMock.Replay();

      _filterViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}