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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class StoragePropertyDefinitionUnificationUtilityTest
  {
    [Test]
    public void CheckAndConvertEquivalentProperty_ReturnsTypedActualValue ()
    {
      var expected = new DateTime (2013, 04, 10);
      object actual = new DateTime (2013, 04, 10, 10, 28, 10);

      var result = StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
          expected, 
          actual, 
          "param", 
          item => Tuple.Create<string, object> ("Year", item.Year));

      Assert.That (VariableTypeInferrer.GetVariableType (result), Is.SameAs (typeof (DateTime)));
      Assert.That (result, Is.EqualTo (actual));
    }

    [Test]
    public void CheckAndConvertEquivalentProperty_ThrowsOnIncompatibleType ()
    {
      var expected = new DateTime (2013, 04, 10);
      object actual = "hello";

      Assert.That (
          () => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
              expected,
              actual,
              "param",
              item => Tuple.Create<string, object> ("Year", item.Year)),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'DateTime', and the given property has type 'String'.\r\n"
              + "Parameter name: param"));
    }

    [Test]
    public void CheckAndConvertEquivalentProperty_ThrowsOnPropertyMismatch ()
    {
      var expected = new DateTime (2013, 04, 10);
      object actual = new DateTime (2014, 04, 10, 10, 28, 10);

      Assert.That (
          () => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
              expected,
              actual,
              "param",
              item => Tuple.Create<string, object> ("Year", item.Year)),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has Year '2013', and the given property has Year '2014'.\r\n"
              + "Parameter name: param"));
    }
  }
}