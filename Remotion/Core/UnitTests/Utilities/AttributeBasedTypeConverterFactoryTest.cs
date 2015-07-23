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
using System.ComponentModel;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AttributeBasedTypeConverterFactoryTest
  {
    private class StubTypeConverter : TypeConverter
    {
    }

    [TypeConverter (typeof (StubTypeConverter))]
    private class BaseType
    {
    }

    private class DerivedType : BaseType
    {
    }

    private ITypeConverterFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new AttributeBasedTypeConverterFactory();
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithTypeConverter_ReturnsTypeConverter ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (BaseType));
      Assert.That (typeConverter, Is.TypeOf<StubTypeConverter>());
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithTypeConverterOnBaseType_ReturnsTypeConverter ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (DerivedType));
      Assert.That (typeConverter, Is.TypeOf<StubTypeConverter>());
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithOtherType_ReturnsNull ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Int32));
      Assert.That (typeConverter, Is.Null);
    }
  }
}