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
using Remotion.Mixins.Context.Serialization;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class FlatMixinContextOriginSerializationTest
  {
    private FlatMixinContextOriginSerializer _serializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new FlatMixinContextOriginSerializer();
    }

    [Test]
    public void AddKind ()
    {
      _serializer.AddKind ("some kind");
      Assert.That (_serializer.Values[0], Is.EqualTo ("some kind"));
      
      var deserializer = new FlatMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetKind (), Is.EqualTo ("some kind"));
    }

    [Test]
    public void AddAssembly ()
    {
      var someAssembly = GetType().Assembly;
      _serializer.AddAssembly (someAssembly);
      Assert.That (_serializer.Values[1], Is.EqualTo (someAssembly.FullName));

      var deserializer = new FlatMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetAssembly (), Is.EqualTo (someAssembly));
    }

    [Test]
    public void AddLocation ()
    {
      _serializer.AddLocation ("some location");
      Assert.That (_serializer.Values[2], Is.EqualTo ("some location"));

      var deserializer = new FlatMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetLocation (), Is.EqualTo ("some location"));
    }
  }
}
