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
  public class QueryParameterTest : StandardMappingTest
  {
    private QueryParameter _parameter;

    public override void SetUp ()
    {
      base.SetUp ();

      _parameter = new QueryParameter ("name", "value", QueryParameterType.Value);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_parameter.Name, Is.EqualTo ("name"));
      Assert.That (_parameter.Value, Is.EqualTo ("value"));
      Assert.That (_parameter.ParameterType, Is.EqualTo (QueryParameterType.Value));
    }

    [Test]
    public void Equals_EqualParameterWithAllMembers_ValueIsReferenceType ()
    {
      var equalParameter = new QueryParameter ("name", "value", QueryParameterType.Value);
      Assert.That (_parameter.Equals(equalParameter), Is.True);
    }

    [Test]
    public void Equals_EqualParameterWithAllMembers_ValueIsValueTypee ()
    {
      var parameter1 = new QueryParameter ("name", 5, QueryParameterType.Value);
      var parameter2 = new QueryParameter ("name", 5, QueryParameterType.Value);
      Assert.That (parameter1.Equals (parameter2), Is.True);
    }

    [Test]
    public void Equals_EqualParameterWithMandatoryMembers ()
    {
      var parameter1 = new QueryParameter ("name", null, QueryParameterType.Text);
      var parameter2 = new QueryParameter ("name", null, QueryParameterType.Text);
      Assert.That (parameter1.Equals (parameter2), Is.True);
    }

    [Test]
    public void Equal_DifferentParameterName ()
    {
      var equalParameter = new QueryParameter ("name2", "value", QueryParameterType.Value);
      Assert.That (_parameter.Equals (equalParameter), Is.False);
    }

    [Test]
    public void Equal_DifferentParameterValue ()
    {
      var equalParameter = new QueryParameter ("name", "value2", QueryParameterType.Value);
      Assert.That (_parameter.Equals (equalParameter), Is.False);
    }

    [Test]
    public void Equal_DifferentParameterType ()
    {
      var equalParameter = new QueryParameter ("name", "value", QueryParameterType.Text);
      Assert.That (_parameter.Equals (equalParameter), Is.False);
    }

    [Test]
    public void Equals_ObjectIsNull ()
    {
      Assert.That (_parameter.Equals (null), Is.False);
    }

    [Test]
    public void Equals_ObjectIsNoQueryParameter ()
    {
      Assert.That (_parameter.Equals (new object()), Is.False);
    }

    [Test]
    public void Equals_AssertPropertyCount ()
    {
      Assert.That (typeof (QueryParameter).GetProperties ().Length, Is.EqualTo (3), "The implementation of Equals and GetHashCode has to be adapted.");
    }

    [Test]
    public void GetHashCode_EqualQueryParameterWithAllMembers ()
    {
      var equalParameter = new QueryParameter ("name", "value", QueryParameterType.Value);
      Assert.That (_parameter.GetHashCode (), Is.EqualTo (equalParameter.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_EqualQueryParameterWithMandatoryMembers ()
    {
      var parameter1 = new QueryParameter ("name", null, QueryParameterType.Text);
      var parameter2 = new QueryParameter ("name", null, QueryParameterType.Text);
      Assert.That (parameter1.GetHashCode (), Is.EqualTo (parameter2.GetHashCode ()));
    }

  }
}
