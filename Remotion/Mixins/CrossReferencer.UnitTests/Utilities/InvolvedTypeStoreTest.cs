// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class InvolvedTypeStoreTest
  {
    private InvolvedTypeStore _involvedTypeStore;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeStore = new InvolvedTypeStore();
    }

    [Test]
    public void GetOrCreateValue_EmptyStore ()
    {
      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof (object));

      var expectedInvolvedType = new InvolvedType (typeof (object));
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }

    [Test]
    public void GetOrCreateValue_NonEmptyStore ()
    {
      _involvedTypeStore.GetOrCreateValue (typeof (object));

      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof (object));

      var expectedInvolvedType = new InvolvedType (typeof (object));
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }

    [Test]
    public void ToSortedArray_EmptyStore ()
    {
      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (new InvolvedType[0]));
    }

    [Test]
    public void ToSortedArray_NonEmptyStore ()
    {
      var involvedType1 = _involvedTypeStore.GetOrCreateValue (typeof (object));
      var involvedType2 = _involvedTypeStore.GetOrCreateValue (typeof (string));

      var expectedTypes = new[] { involvedType1, involvedType2 };
      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (expectedTypes));
    }

    [Test]
    public void ToSortedArray_OrderByFullName ()
    {
      var involvedType1 = _involvedTypeStore.GetOrCreateValue (typeof (System.String));      // 3
      var involvedType2 = _involvedTypeStore.GetOrCreateValue (typeof (System.Object));      // 2
      var involvedType3 = _involvedTypeStore.GetOrCreateValue (typeof (System.IDisposable)); // 1

      var expectedTypesInOrder = new[] { involvedType3, involvedType2, involvedType1 };
      Assert.That (_involvedTypeStore.ToArray (), Is.EqualTo (expectedTypesInOrder));
    }
  }
}