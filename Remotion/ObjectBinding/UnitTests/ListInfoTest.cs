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
using System.Collections;
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class ListInfoTest
  {
    [Test]
    public void GetPropertyType ()
    {
      ListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.PropertyType, Is.SameAs (typeof (string[])));
    }

    [Test]
    public void GetItemType ()
    {
      IListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.ItemType, Is.SameAs (typeof (string)));
    }

    [Test]
    public void GetRequiresWriteBack_WithArray ()
    {
      IListInfo listInfo = new ListInfo (typeof (string[]), typeof (string));
      Assert.That (listInfo.RequiresWriteBack, Is.True);
    }

    [Test]
    public void GetRequiresWriteBack_WithIList ()
    {
      IListInfo listInfo = new ListInfo (typeof (IList), typeof (string));
      Assert.That (listInfo.RequiresWriteBack, Is.False);
    }

    [Test]
    public void CreateList_ReferenceType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleReferenceType[]), typeof (SimpleReferenceType));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleReferenceType[1]));
    }

    [Test]
    public void CreateList_ValueType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleValueType[]), typeof (SimpleValueType));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleValueType[1]));
    }

    [Test]
    public void CreateList_NullableValueType ()
    {
      IListInfo listInfo = new ListInfo (typeof (SimpleValueType?[]), typeof (SimpleValueType?));
      Assert.That (listInfo.CreateList (1), Is.EquivalentTo (new SimpleValueType?[1]));
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void InsertItem ()
    {      
    }

    [Test]
    [Ignore ("TODO: implement and integrate with BocList")]
    public void RemoveItem ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void Initialize_WithMismatchedItemType ()
    {
    }
  }
}
