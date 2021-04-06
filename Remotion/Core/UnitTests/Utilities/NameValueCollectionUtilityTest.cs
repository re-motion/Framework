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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class NameValueCollectionUtilityTest
{
  // types

  // static members and constants

  // member fields

  private NameValueCollection _collection;
  private NameValueCollection _otherCollection;

  // construction and disposing

  public NameValueCollectionUtilityTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp()
  { 
    _collection = new NameValueCollection();
    _collection.Add ("FirstKey", "FirstValue");
    _collection.Add ("SecondKey", "SecondValue");
    _collection.Add ("ThirdKey", "ThirdValue");
    _collection.Add ("ThirdKey", "Other ThirdValue");

    _otherCollection = new NameValueCollection();
    _otherCollection.Add ("SecondKey", "Other SecondValue");
    _otherCollection.Add ("FourthKey", "FourthValue");
    _otherCollection.Add ("FifthKey", "FifthValue");
  }

  [Test]
  public void Clone()
  {
    NameValueCollection actual = NameValueCollectionUtility.Clone (_collection);

    Assert.That (actual, Is.Not.Null);
    Assert.That (ReferenceEquals (_collection, actual), Is.False);
    Assert.That (actual.Count, Is.EqualTo (3));

    Assert.That (actual.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (actual.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (actual.GetKey (2), Is.EqualTo ("ThirdKey"));

    Assert.That (actual["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (actual["SecondKey"], Is.EqualTo ("SecondValue"));
    Assert.That (actual["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
  }

  [Test]
  public void AppendWithNull()
  {
    NameValueCollectionUtility.Append (_collection, null);

    Assert.That (_collection.Count, Is.EqualTo (3));

    Assert.That (_collection.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (_collection.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (_collection.GetKey (2), Is.EqualTo ("ThirdKey"));

    Assert.That (_collection["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (_collection["SecondKey"], Is.EqualTo ("SecondValue"));
    Assert.That (_collection["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
  } 

  [Test]
  public void AppendWithEmptyCollection()
  {
    NameValueCollectionUtility.Append (_collection, new NameValueCollection());

    Assert.That (_collection.Count, Is.EqualTo (3));

    Assert.That (_collection.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (_collection.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (_collection.GetKey (2), Is.EqualTo ("ThirdKey"));

    Assert.That (_collection["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (_collection["SecondKey"], Is.EqualTo ("SecondValue"));
    Assert.That (_collection["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
  } 

  [Test]
  public void AppendWithOtherCollection()
  {
    NameValueCollectionUtility.Append (_collection, _otherCollection);

    Assert.That (_collection.Count, Is.EqualTo (5));

    Assert.That (_collection.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (_collection.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (_collection.GetKey (2), Is.EqualTo ("ThirdKey"));
    Assert.That (_collection.GetKey (3), Is.EqualTo ("FourthKey"));
    Assert.That (_collection.GetKey (4), Is.EqualTo ("FifthKey"));

    Assert.That (_collection["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (_collection["SecondKey"], Is.EqualTo ("Other SecondValue"));
    Assert.That (_collection["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
    Assert.That (_collection["FourthKey"], Is.EqualTo ("FourthValue"));
    Assert.That (_collection["FifthKey"], Is.EqualTo ("FifthValue"));
  } 

  [Test]
  public void MergeWithFirstNullAndSecondNull()
  {
    Assert.That (NameValueCollectionUtility.Merge (null, null), Is.Null);
  }

  [Test]
  public void MergeWithFirstValueAndSecondNull()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (_collection, null);

    Assert.That (actual, Is.Not.Null);
    Assert.That (ReferenceEquals (_collection, actual), Is.False);
    Assert.That (actual.Count, Is.EqualTo (3));

    Assert.That (actual.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (actual.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (actual.GetKey (2), Is.EqualTo ("ThirdKey"));

    Assert.That (actual["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (actual["SecondKey"], Is.EqualTo ("SecondValue"));
    Assert.That (actual["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
  }

  [Test]
  public void MergeWithFirstNullAndSecondValue()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (null, _collection);

    Assert.That (actual, Is.Not.Null);
    Assert.That (ReferenceEquals (_collection, actual), Is.False);
    Assert.That (actual.Count, Is.EqualTo (3));

    Assert.That (actual.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (actual.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (actual.GetKey (2), Is.EqualTo ("ThirdKey"));

    Assert.That (actual["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (actual["SecondKey"], Is.EqualTo ("SecondValue"));
    Assert.That (actual["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
  }

  [Test]
  public void MergeWithFirstValueAndSecondValue()
  {
    NameValueCollection actual = NameValueCollectionUtility.Merge (_collection, _otherCollection);

    Assert.That (actual, Is.Not.Null);
    Assert.That (ReferenceEquals (_collection, actual), Is.False);
    Assert.That (actual.Count, Is.EqualTo (5));

    Assert.That (actual.GetKey (0), Is.EqualTo ("FirstKey"));
    Assert.That (actual.GetKey (1), Is.EqualTo ("SecondKey"));
    Assert.That (actual.GetKey (2), Is.EqualTo ("ThirdKey"));
    Assert.That (actual.GetKey (3), Is.EqualTo ("FourthKey"));
    Assert.That (actual.GetKey (4), Is.EqualTo ("FifthKey"));

    Assert.That (actual["FirstKey"], Is.EqualTo ("FirstValue"));
    Assert.That (actual["SecondKey"], Is.EqualTo ("Other SecondValue"));
    Assert.That (actual["ThirdKey"], Is.EqualTo ("ThirdValue,Other ThirdValue"));
    Assert.That (actual["FourthKey"], Is.EqualTo ("FourthValue"));
    Assert.That (actual["FifthKey"], Is.EqualTo ("FifthValue"));
  } 
}

}
