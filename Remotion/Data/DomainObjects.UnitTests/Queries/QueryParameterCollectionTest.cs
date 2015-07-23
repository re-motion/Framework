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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class QueryParameterCollectionTest : StandardMappingTest
  {
    private QueryParameterCollection _collection;
    private QueryParameter _parameter;

    public override void SetUp ()
    {
      base.SetUp ();

      _parameter = new QueryParameter ("name", "value");
      _collection = new QueryParameterCollection ();
    }

    [Test]
    public void Add ()
    {
      _collection.Add (_parameter);
      Assert.That (_collection.Count, Is.EqualTo (1));
    }

    [Test]
    public void QueryParameterIndexer ()
    {
      _collection.Add (_parameter);
      Assert.That (_collection[_parameter.Name], Is.SameAs (_parameter));
    }

    [Test]
    public void NumericIndexer ()
    {
      _collection.Add (_parameter);
      Assert.That (_collection[0], Is.SameAs (_parameter));
    }

    [Test]
    public void ContainsParameterNameTrue ()
    {
      _collection.Add (_parameter);
      Assert.That (_collection.Contains (_parameter.Name), Is.True);
    }

    [Test]
    public void ContainsParameterNameFalse ()
    {
      Assert.That (_collection.Contains (_parameter.Name), Is.False);
    }

    [Test]
    public void CopyConstructor ()
    {
      _collection.Add (_parameter);

      QueryParameterCollection copiedCollection = new QueryParameterCollection (_collection, false);

      Assert.That (copiedCollection.Count, Is.EqualTo (1));
      Assert.That (copiedCollection[0], Is.SameAs (_parameter));
    }

    [Test]
    public void ContainsParameterTrue ()
    {
      _collection.Add (_parameter);
      Assert.That (_collection.Contains (_parameter), Is.True);
    }

    [Test]
    public void ContainsParameterFalse ()
    {
      _collection.Add (_parameter);
      QueryParameter param = new QueryParameter ("Test", "Test", QueryParameterType.Text);
      Assert.That (_collection.Contains (param), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryParameter ()
    {
      _collection.Contains ((QueryParameter) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void ContainsNullQueryParameterName ()
    {
      _collection.Contains ((string) null);
    }

    [Test]
    public void AddShorthand1 ()
    {
      _collection.Add (_parameter.Name, _parameter.Value, _parameter.ParameterType);
      Assert.That (_collection.Count, Is.EqualTo (1));
      Assert.That (_collection[0].Name, Is.EqualTo (_parameter.Name));
      Assert.That (_collection[0].Value, Is.EqualTo (_parameter.Value));
      Assert.That (_collection[0].ParameterType, Is.EqualTo (_parameter.ParameterType));
    }

    [Test]
    public void AddShorthand2 ()
    {
      _collection.Add (_parameter.Name, _parameter.Value);
      Assert.That (_collection.Count, Is.EqualTo (1));
      Assert.That (_collection[0].Name, Is.EqualTo (_parameter.Name));
      Assert.That (_collection[0].Value, Is.EqualTo (_parameter.Value));
      Assert.That (_collection[0].ParameterType, Is.EqualTo (QueryParameterType.Value));
    }
  }
}
