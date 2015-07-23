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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Model
{
  [TestFixture]
  public class SqlIndexedColumnDefinitionTest
  {
    private ColumnDefinition _innerColumn;
    private SqlIndexedColumnDefinition _indexedColumn;

    [SetUp]
    public void SetUp ()
    {
      _innerColumn = ColumnDefinitionObjectMother.CreateColumn ("InnerColumn");
      _indexedColumn = new SqlIndexedColumnDefinition (_innerColumn, IndexOrder.Desc);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_indexedColumn.Columnn, Is.SameAs (_innerColumn));
      Assert.That (_indexedColumn.IndexOrder, Is.EqualTo (IndexOrder.Desc));
    }

    [Test]
    public void Initialization_WithNullIndexOrder ()
    {
      var indexedColumn = new SqlIndexedColumnDefinition (_innerColumn);

      Assert.That (indexedColumn.IndexOrder, Is.Null);
    }
    
  }
}