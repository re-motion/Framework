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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model
{
  [TestFixture]
  public class SqlIndexDefinitionTest
  {
    private ColumnDefinition[] _includedColumns;
    private SqlIndexedColumnDefinition[] _columns;
    private SqlIndexDefinition _sqlIndexDefinition;

    [SetUp]
    public void SetUp ()
    {
      _columns = new[] { new SqlIndexedColumnDefinition (ColumnDefinitionObjectMother.CreateColumn ("TestColumn1")) };
      _includedColumns = new[] { ColumnDefinitionObjectMother.CreateColumn ("TestColumn2") };

      _sqlIndexDefinition = new SqlIndexDefinition (
          "IndexName", _columns, _includedColumns, true, true, true, true, true, 5, true, true, true, true, true, 2);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_sqlIndexDefinition.IndexName, Is.EqualTo ("IndexName"));
      Assert.That (_sqlIndexDefinition.Columns, Is.EqualTo (_columns));
      Assert.That (_sqlIndexDefinition.IncludedColumns, Is.EqualTo (_includedColumns));
      Assert.That (_sqlIndexDefinition.IsClustered.Value, Is.True);
      Assert.That (_sqlIndexDefinition.IsUnique.Value, Is.True);
      Assert.That (_sqlIndexDefinition.IgnoreDupKey.Value, Is.True);
      Assert.That (_sqlIndexDefinition.Online.Value, Is.True);
      Assert.That (_sqlIndexDefinition.PadIndex.Value, Is.True);
      Assert.That (_sqlIndexDefinition.SortInDb.Value, Is.True);
      Assert.That (_sqlIndexDefinition.StatisticsNoReCompute.Value, Is.True);
      Assert.That (_sqlIndexDefinition.AllowPageLocks.Value, Is.True);
      Assert.That (_sqlIndexDefinition.AllowRowLocks.Value, Is.True);
      Assert.That (_sqlIndexDefinition.DropExisiting.Value, Is.True);
      Assert.That (_sqlIndexDefinition.FillFactor.Value, Is.EqualTo(5));
      Assert.That (_sqlIndexDefinition.MaxDop.Value, Is.EqualTo(2));
    }

    [Test]
    public void Initialization_NoIncludedColumns ()
    {
      _sqlIndexDefinition = new SqlIndexDefinition ("IndexName", _columns, null, true, true, true, true);

      Assert.That (_sqlIndexDefinition.IncludedColumns, Is.Null);
    }

    [Test]
    public void Accept_IndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IIndexDefinitionVisitor>();
      visitorMock.Replay();

      _sqlIndexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_SqlIndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ISqlIndexDefinitionVisitor>();
      visitorMock.Expect (mock => mock.VisitIndexDefinition (_sqlIndexDefinition));
      visitorMock.Replay();

      _sqlIndexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}