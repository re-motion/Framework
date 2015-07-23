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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class ExtensibleEnumsTest : IntegrationTestBase
  {
    [Test]
    public void ExtensibleEnums_InWhereClause ()
    {
      var query1 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                  where cwadt.ExtensibleEnumProperty == Color.Values.Red ()
                  select cwadt;

      var result1 = query1.ToArray ();
      Assert.That (result1, Is.EqualTo (new[] { DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> () }));

      var query2 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                  where cwadt.ExtensibleEnumProperty.Equals (Color.Values.Red())
                  select cwadt;

      var result2 = query2.ToArray ();
      Assert.That (result2, Is.EqualTo (new[] { DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> () }));

      var query3 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                   where new[] { Color.Values.Red (), Color.Values.Blue () }.Contains (cwadt.ExtensibleEnumProperty)
                   select cwadt;

      var result3 = query3.ToArray ();
      Assert.That (
          result3,
          Is.EquivalentTo (
              new[]
              {
                  DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
                  DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ()
              }));
    }

    [Test]
    public void ExtensibleEnums_AsSingleResult ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes>()
                  where cwadt.ID == DomainObjectIDs.ClassWithAllDataTypes1
                  select cwadt.ExtensibleEnumProperty;

      var result = query.Single();

      Assert.That (result, Is.EqualTo (Color.Values.Red()));
    }

    [Test]
    public void ExtensibleEnums_InOrderByClause ()
    {
      var query1 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                   orderby cwadt.ExtensibleEnumProperty
                   select cwadt;

      var result1 = query1.ToArray ();
      // This is in alphabetic order
      Assert.That (result1, Is.EqualTo (
          new[]
          {
              DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> (), 
              DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ()
          }));

      var query2 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                   where cwadt.ExtensibleEnumProperty.Equals (Color.Values.Red ())
                   select cwadt;

      var result2 = query2.ToArray ();
      Assert.That (result2, Is.EqualTo (new[] { DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> () }));

      var query3 = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> ()
                   where new[] { Color.Values.Red (), Color.Values.Blue () }.Contains (cwadt.ExtensibleEnumProperty)
                   select cwadt;

      var result3 = query3.ToArray ();
      Assert.That (
          result3,
          Is.EquivalentTo (
              new[]
              {
                  DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
                  DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ()
              }));
    }
  }
}