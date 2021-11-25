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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class FlattenedSerializationWriterTest
  {
    [Test]
    public void InitialWriter ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      int[] data = writer.GetData();
      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.Empty);
    }

    [Test]
    public void AddSimpleValue ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      writer.AddSimpleValue(1);
      int[] data = writer.GetData();
      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(new int[] { 1 }));
    }

    [Test]
    public void AddSimpleValue_Twice ()
    {
      FlattenedSerializationWriter<int> writer = new FlattenedSerializationWriter<int>();
      writer.AddSimpleValue(1);
      writer.AddSimpleValue(2);
      int[] data = writer.GetData();
      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(new int[] { 1, 2 }));
    }

    [Test]
    public void AddSimpleValue_WithParameterValueIsNull ()
    {
      FlattenedSerializationWriter<object> writer = new FlattenedSerializationWriter<object>();
      writer.AddSimpleValue(null);
      object[] data = writer.GetData();
      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(new object[] { null }));
    }

    [Test]
    public void AddSimpleValue_WithParameterValueIsType_ThrowsArgumentException ()
    {
#if !DEBUG
      Assert.Ignore("The tested functionality is only available in debug builds.");
#endif

      FlattenedSerializationWriter<object> writer = new FlattenedSerializationWriter<object>();
      Assert.That(
          () => writer.AddSimpleValue(typeof (DomainObject)),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Cannot serialize values of type 'System.RuntimeType'.",
              "value"));
    }

    [Test]
    public void AddSimpleValue_WithParameterValueIsDelegate_ThrowsArgumentException ()
    {
#if !DEBUG
      Assert.Ignore("The tested functionality is only available in debug builds.");
#endif

      Func<bool> value = () => true;
      FlattenedSerializationWriter<object> writer = new FlattenedSerializationWriter<object>();
      Assert.That(
          () => writer.AddSimpleValue(value),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Cannot serialize values of type 'System.Delegate'.",
              "value"));
    }

    [Test]
    public void AddSimpleValue_WithParameterValueIsNotSerializable_ThrowsArgumentException ()
    {
#if !DEBUG
      Assert.Ignore("The tested functionality is only available in debug builds.");
#endif

      FlattenedSerializationWriter<object> writer = new FlattenedSerializationWriter<object>();
      Assert.That(
          () => writer.AddSimpleValue(this),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Cannot serialize values of type 'Remotion.Data.DomainObjects.UnitTests.Serialization.FlattenedSerializationWriterTest'.",
              "value"));
    }
  }
}
