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
  public class TableDefinitionTest
  {
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    private ObjectIDStoragePropertyDefinition _objectIDProperty;
    private SimpleStoragePropertyDefinition _timestampProperty;
    private SimpleStoragePropertyDefinition _property1;
    private SimpleStoragePropertyDefinition _property2;
    private SimpleStoragePropertyDefinition _property3;

    private PrimaryKeyConstraintDefinition[] _constraints;
    private IIndexDefinition[] _indexes;
    private EntityNameDefinition[] _synonyms;

    private TableDefinition _tableDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");

      _objectIDProperty = ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty;
      _timestampProperty = SimpleStoragePropertyDefinitionObjectMother.TimestampProperty;
      _property1 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column1");
      _property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column2");
      _property3 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("Column3");
     
      _constraints = new[]
                     { new PrimaryKeyConstraintDefinition ("PK_Table", true, new[] { ColumnDefinitionObjectMother.IDColumn }) };
      _indexes = new[] { MockRepository.GenerateStub<IIndexDefinition>() };
      _synonyms = new[] { new EntityNameDefinition (null, "Test") };

      _tableDefinition = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition ("TableSchema", "TableTest"),
          new EntityNameDefinition ("Schema", "Test"),
          _objectIDProperty,
          _timestampProperty,
          new[] { _property1, _property2, _property3 },
          _constraints,
          _indexes,
          _synonyms);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_tableDefinition.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
      Assert.That (_tableDefinition.ViewName, Is.EqualTo (new EntityNameDefinition ("Schema", "Test")));
      Assert.That (_tableDefinition.TableName, Is.EqualTo (new EntityNameDefinition ("TableSchema", "TableTest")));

      Assert.That (_tableDefinition.ObjectIDProperty, Is.SameAs (_objectIDProperty));
      Assert.That (_tableDefinition.TimestampProperty, Is.SameAs (_timestampProperty));
      Assert.That (_tableDefinition.DataProperties, Is.EqualTo (new[] { _property1, _property2, _property3 }));

      Assert.That (_tableDefinition.Indexes, Is.EqualTo (_indexes));
      Assert.That (_tableDefinition.Synonyms, Is.EqualTo (_synonyms));
      Assert.That (_tableDefinition.Constraints, Is.EqualTo (_constraints));
    }

    [Test]
    public void Initialization_ViewNameNull ()
    {
      var tableDefinition = new TableDefinition (
          _storageProviderDefinition,
          new EntityNameDefinition (null, "Test"),
          null,
          _objectIDProperty,
          _timestampProperty,
          new SimpleStoragePropertyDefinition[0],
          _constraints,
          new IIndexDefinition[0],
          new EntityNameDefinition[0]);
      Assert.That (tableDefinition.ViewName, Is.Null);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IRdbmsStorageEntityDefinitionVisitor>();

      visitorMock.Expect (mock => mock.VisitTableDefinition (_tableDefinition));
      visitorMock.Replay();

      _tableDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void CalculateAdjustedColumnList ()
    {
      var fullColumnList = 
          new[]
          {
              _property3.ColumnDefinition,
              _property1.ColumnDefinition,
              ColumnDefinitionObjectMother.CreateColumn()
          };
      
      var adjustedColumnList = _tableDefinition.CalculateAdjustedColumnList (fullColumnList);

      Assert.That (adjustedColumnList, Is.EqualTo (new[] { _property3.ColumnDefinition, _property1.ColumnDefinition, null }));
    }
  }
}