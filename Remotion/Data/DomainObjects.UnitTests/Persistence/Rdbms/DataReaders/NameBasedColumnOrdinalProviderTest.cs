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
using System.Data;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class NameBasedColumnOrdinalProviderTest
  {
    private ColumnDefinition _columnDefinition;
    private NameBasedColumnOrdinalProvider _nameBasedColumnOrdinalProvider;
    private Mock<IDataReader> _dataReaderStub;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn("Testcolumn");
      _dataReaderStub = new Mock<IDataReader>();
      _nameBasedColumnOrdinalProvider = new NameBasedColumnOrdinalProvider();
    }

    [Test]
    public void GetOrdinal ()
    {
      _dataReaderStub.Setup (stub => stub.GetOrdinal (_columnDefinition.Name)).Returns (5);

      var result = _nameBasedColumnOrdinalProvider.GetOrdinal(_columnDefinition, _dataReaderStub.Object);

      Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public void GetOrdinal_IndexOutOfRange_ThrowsException ()
    {
      _dataReaderStub.Setup (stub => stub.GetOrdinal (_columnDefinition.Name)).Returns (0).Callback ((string name) => { throw new IndexOutOfRangeException(); });
      _dataReaderStub.Setup (stub => stub.FieldCount).Returns (2);
      _dataReaderStub.Setup (stub => stub.GetName (0)).Returns ("Column 1");
      _dataReaderStub.Setup (stub => stub.GetName (1)).Returns ("Column 2");
      Assert.That(
          () => _nameBasedColumnOrdinalProvider.GetOrdinal(_columnDefinition, _dataReaderStub.Object),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("The column 'Testcolumn' was not found in the query result. The included columns are: Column 1, Column 2."));
    }
  }
}
