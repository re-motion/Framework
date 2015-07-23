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
  public class SqlSecondaryXmlIndexDefinitionTest
  {
    private ColumnDefinition _xmlColumn;
    private SqlSecondaryXmlIndexDefinition _indexDefinition;

    [SetUp]
    public void SetUp ()
    {
      _xmlColumn = ColumnDefinitionObjectMother.CreateColumn ("XmlColumn");
      _indexDefinition = new SqlSecondaryXmlIndexDefinition (
          "IndexName", _xmlColumn, "PrimaryIndexName", SqlSecondaryXmlIndexKind.Property, true, 5, true, true, true, true, true, 2);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexDefinition.IndexName, Is.EqualTo ("IndexName"));
      Assert.That (_indexDefinition.PrimaryIndexName, Is.EqualTo ("PrimaryIndexName"));
      Assert.That (_indexDefinition.XmlColumn, Is.SameAs (_xmlColumn));
      Assert.That (_indexDefinition.Kind, Is.EqualTo (SqlSecondaryXmlIndexKind.Property));
      Assert.That (_indexDefinition.PadIndex.Value, Is.True);
      Assert.That (_indexDefinition.SortInDb.Value, Is.True);
      Assert.That (_indexDefinition.StatisticsNoReCompute.Value, Is.True);
      Assert.That (_indexDefinition.AllowPageLocks.Value, Is.True);
      Assert.That (_indexDefinition.AllowRowLocks.Value, Is.True);
      Assert.That (_indexDefinition.DropExisiting.Value, Is.True);
      Assert.That (_indexDefinition.FillFactor.Value, Is.EqualTo (5));
      Assert.That (_indexDefinition.MaxDop.Value, Is.EqualTo (2));
    }

    [Test]
    public void Accept_IndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<IIndexDefinitionVisitor>();
      visitorMock.Replay();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }

    [Test]
    public void Accept_SqlIndexDefinitionVisitor ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ISqlIndexDefinitionVisitor>();
      visitorMock.Expect (mock => mock.VisitSecondaryXmlIndexDefinition (_indexDefinition));
      visitorMock.Replay();

      _indexDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}