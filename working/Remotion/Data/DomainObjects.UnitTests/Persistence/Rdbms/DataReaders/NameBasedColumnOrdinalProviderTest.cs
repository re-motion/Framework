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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DataReaders
{
  [TestFixture]
  public class NameBasedColumnOrdinalProviderTest
  {
    private ColumnDefinition _columnDefinition;
    private NameBasedColumnOrdinalProvider _nameBasedColumnOrdinalProvider;
    private IDataReader _dataReaderStub;

    [SetUp]
    public void SetUp ()
    {
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn("Testcolumn");
      _dataReaderStub = MockRepository.GenerateStub<IDataReader>();
      _nameBasedColumnOrdinalProvider = new NameBasedColumnOrdinalProvider();
    }

    [Test]
    public void GetOrdinal ()
    {
      _dataReaderStub.Stub (stub => stub.GetOrdinal (_columnDefinition.Name)).Return (5);

      var result = _nameBasedColumnOrdinalProvider.GetOrdinal (_columnDefinition, _dataReaderStub);

      Assert.That (result, Is.EqualTo (5));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException), ExpectedMessage = 
        "The column 'Testcolumn' was not found in the query result. The included columns are: Column 1, Column 2.")]
    public void GetOrdinal_IndexOutOfRange_ThrowsException ()
    {
      _dataReaderStub.Stub (stub => stub.GetOrdinal (_columnDefinition.Name)).Return(0).WhenCalled (mi => { throw new IndexOutOfRangeException(); });
      _dataReaderStub.Stub (stub => stub.FieldCount).Return (2);
      _dataReaderStub.Stub (stub => stub.GetName (0)).Return ("Column 1");
      _dataReaderStub.Stub (stub => stub.GetName (1)).Return ("Column 2");

      _nameBasedColumnOrdinalProvider.GetOrdinal (_columnDefinition, _dataReaderStub);
    }
  }
}