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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UnionViewDefinitionTest
  {
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition;

    private SimpleStoragePropertyDefinition _timestampProperty;
    private ObjectIDStoragePropertyDefinition _objectIDProperty;
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
      
      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");

      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition ("Schema", "Test") };

      _tableDefinition1 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table1"),
          null,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          _property1);
      _tableDefinition2 = TableDefinitionObjectMother.Create (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Table2"),
          null,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          _property2,
          _property3);
      _unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition ("Schema", "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_unionViewDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_unionViewDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));
      Assert.That (_unionViewDefinition.UnionedEntities, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2 }));

      Assert.That (_unionViewDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_unionViewDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_unionViewDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_unionViewDefinition.Indexes, Is.EqualTo (_indexes));
      Assert.That (_unionViewDefinition.Synonyms, Is.EqualTo (_synonyms));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { _tableDefinition1, _tableDefinition2 },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (unionViewDefinition.ViewName, Is.Null);
    }

    [Test]
    public void Initialization_WithUnionedUnionEntity ()
    {
      new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { _unionViewDefinition },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Item 0 is of type 'Remotion.Data.DomainObjects.Persistence.Rdbms.Model.FilterViewDefinition', "
        + "but the unioned entities must either be a TableDefinitions or UnionViewDefinitions.\r\nParameter name: unionedEntities")]
    public void Initialization_WithInvalidUnionedEntity ()
    {
      var filterViewDefinition = new FilterViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "ViewName"),
          _tableDefinition1,
          new[] { "x" },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      new UnionViewDefinition (
          _storageProviderDefinition,
          null,
          new[] { filterViewDefinition },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
    }

    [Test]
    public void CalculateFullColumnList ()
    {
      var availableColumns = new[]
                             {
                                 ColumnDefinitionObjectMother.IDColumn, 
                                 ColumnDefinitionObjectMother.ClassIDColumn,
                                 ColumnDefinitionObjectMother.TimestampColumn, 
                                 _property3.ColumnDefinition, 
                                 ColumnDefinitionObjectMother.CreateColumn ("Test"), 
                                 _property1.ColumnDefinition
                             };

      var result = _unionViewDefinition.CalculateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (6));
      Assert.That (result[0], Is.SameAs (ColumnDefinitionObjectMother.IDColumn));
      Assert.That (result[1], Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (result[2], Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (result[3], Is.SameAs (_property1.ColumnDefinition));
      Assert.That (result[4], Is.Null);
      Assert.That (result[5], Is.SameAs (_property3.ColumnDefinition));
    }

    [Test]
    public void CalculateFullColumnList_MatchesByNameRatherThanIdentity ()
    {
      var availableColumns = new[] { ColumnDefinitionObjectMother.CreateColumn (_property3.ColumnDefinition.Name) };

      var result = _unionViewDefinition.CalculateFullColumnList (availableColumns).ToArray ();

      Assert.That (result.Length, Is.EqualTo (6));
      Assert.That (result[0], Is.Null);
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.Null);
      Assert.That (result[3], Is.Null);
      Assert.That (result[4], Is.Null);
      Assert.That (result[5], Is.SameAs (availableColumns[0]));
    }

    [Test]
    public void CalculateFullColumnList_DuplicateNames_FirstIsUsed ()
    {
      var availableColumns =
          new[]
          {
              ColumnDefinitionObjectMother.CreateColumn (_property3.ColumnDefinition.Name),
              ColumnDefinitionObjectMother.CreateColumn (_property3.ColumnDefinition.Name)
          };

      var result = _unionViewDefinition.CalculateFullColumnList (availableColumns).ToArray ();

      Assert.That (result.Length, Is.EqualTo (6));
      Assert.That (result[0], Is.Null);
      Assert.That (result[1], Is.Null);
      Assert.That (result[2], Is.Null);
      Assert.That (result[3], Is.Null);
      Assert.That (result[4], Is.Null);
      Assert.That (result[5], Is.SameAs (availableColumns[0]));
    }

    [Test]
    public void CalculateFullColumnList_OneColumnNotFound ()
    {
      var unionViewDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          new[] { _tableDefinition1, _tableDefinition2 },
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2 },
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var availableColumns = new[]
                             {
                                 ColumnDefinitionObjectMother.IDColumn,
                                 ColumnDefinitionObjectMother.ClassIDColumn,
                                 ColumnDefinitionObjectMother.TimestampColumn, 
                                 _property1.ColumnDefinition
                             };

      var result = unionViewDefinition.CalculateFullColumnList (availableColumns).ToArray();

      Assert.That (result.Length, Is.EqualTo (5));
      Assert.That (result[0], Is.SameAs (ColumnDefinitionObjectMother.IDColumn));
      Assert.That (result[1], Is.SameAs (ColumnDefinitionObjectMother.ClassIDColumn));
      Assert.That (result[2], Is.SameAs (ColumnDefinitionObjectMother.TimestampColumn));
      Assert.That (result[3], Is.SameAs (_property1.ColumnDefinition));
      Assert.That (result[4], Is.Null);
    }

    [Test]
    public void GetAllTables ()
    {
      var result = _unionViewDefinition.GetAllTables().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2 }));
    }

    [Test]
    public void GetAllTables_IndirectTables ()
    {
      var tableDefinition3 = TableDefinitionObjectMother.Create (_storageProviderDefinition);
      var baseUnionDefinition = new UnionViewDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "UnionView"),
          new IRdbmsStorageEntityDefinition[] { _unionViewDefinition, tableDefinition3 },
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);

      var result = baseUnionDefinition.GetAllTables().ToArray();

      Assert.That (result, Is.EqualTo (new[] { _tableDefinition1, _tableDefinition2, tableDefinition3 }));
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitUnionViewDefinition (_unionViewDefinition));
      visitorMock.Replay();

      _unionViewDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}