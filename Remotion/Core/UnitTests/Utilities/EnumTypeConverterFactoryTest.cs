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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class EnumTypeConverterFactoryTest
  {
    // ReSharper disable EnumUnderlyingTypeIsInt
    public enum Int32Enum : int
    {
    }

    // ReSharper restore EnumUnderlyingTypeIsInt

    private ITypeConverterFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new EnumTypeConverterFactory();
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithEnum_ReturnsAdvancedEnumConverter ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Int32Enum));
      Assert.That (typeConverter, Is.TypeOf<AdvancedEnumConverter>());
      Assert.That (((AdvancedEnumConverter) typeConverter).EnumType, Is.EqualTo (typeof (Int32Enum)));
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithNullableEnum_ReturnsAdvancedEnumConverter ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Int32Enum?));
      Assert.That (typeConverter, Is.TypeOf<AdvancedEnumConverter>());
      Assert.That (((AdvancedEnumConverter) typeConverter).EnumType, Is.EqualTo (typeof (Int32Enum?)));
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithOtherType_ReturnsNull ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Int32));
      Assert.That (typeConverter, Is.Null);
    }
  }
}