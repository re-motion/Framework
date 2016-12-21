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
using Remotion.ObjectBinding.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation
{
  [TestFixture]
  public class BocListRowTest
  {
    [Test]
    public void Equal_SameBusinessObject_SameIndex_AreEqual ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      var index = 5;
      var row1 = new BocListRow (index, businessObject);
      var row2 = new BocListRow (index, businessObject);

      Assert.That (row1.Equals (row2), Is.True);
      Assert.That (row2.Equals (row1), Is.True);
    }

    [Test]
    public void IEquatable_SameBusinessObject_SameIndex_AreEqual ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      var index = 5;
      IEquatable<BocListRow> row1 = new BocListRow (index, businessObject);
      IEquatable<BocListRow> row2 = new BocListRow (index, businessObject);

      Assert.That (row1.Equals (row2), Is.True);
      Assert.That (row2.Equals (row1), Is.True);
    }

    [Test]
    public void IEquatable_OtherBusinessObject_SameIndex_AreNotEqual ()
    {
      var index = 5;
      IEquatable<BocListRow> row1 = new BocListRow (index, MockRepository.GenerateStub<IBusinessObject>());
      IEquatable<BocListRow> row2 = new BocListRow (index, MockRepository.GenerateStub<IBusinessObject>());

      Assert.That (row1.Equals (row2), Is.False);
      Assert.That (row2.Equals (row1), Is.False);
    }

    [Test]
    public void IEquatable_SameBusinessObject_OtherIndex_AreNotEqual ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      IEquatable<BocListRow> row1 = new BocListRow (4, businessObject);
      IEquatable<BocListRow> row2 = new BocListRow (6, businessObject);

      Assert.That (row1.Equals (row2), Is.False);
      Assert.That (row2.Equals (row1), Is.False);
    }

    [Test]
    public void Equal_OtherRowNull_AreNotEqual ()
    {
      var row = new BocListRow (5, MockRepository.GenerateStub<IBusinessObject>());

      Assert.That (row.Equals (null), Is.False);
    }

    [Test]
    public void IEquatable_OtherRowNull_AreNotEqual ()
    {
      IEquatable<BocListRow> row = new BocListRow (5, MockRepository.GenerateStub<IBusinessObject>());

      Assert.That (row.Equals (null), Is.False);
    }

    [Test]
    public void Equal_OtherType_AreNotEqual ()
    {
      var row = new BocListRow (5, MockRepository.GenerateStub<IBusinessObject>());

      Assert.That (row.Equals (new object()), Is.False);
    }

    [Test]
    public void OperatorEqual_SameBusinessObject_SameIndex_AreEqual ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      var index = 5;
      var row1 = new BocListRow (index, businessObject);
      var row2 = new BocListRow (index, businessObject);

      Assert.That (row1 == row2, Is.True);
      Assert.That (row2 == row1, Is.True);
    }

    [Test]
    public void OperatorNotEqual_SameBusinessObject_OtherIndex_AreNotEqual ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      var row1 = new BocListRow (4, businessObject);
      var row2 = new BocListRow (6, businessObject);

      Assert.That (row1 != row2, Is.True);
      Assert.That (row2 != row1, Is.True);
    }

    [Test]
    public void GetHashcode_SameBusinessObject_SameIndex_SameHashcode ()
    {
      var businessObject = MockRepository.GenerateStub<IBusinessObject>();
      var index = 5;
      var row1 = new BocListRow (index, businessObject);
      var row2 = new BocListRow (index, businessObject);

      Assert.That (row1.GetHashCode(), Is.EqualTo(row2.GetHashCode()));
    }
  }
}