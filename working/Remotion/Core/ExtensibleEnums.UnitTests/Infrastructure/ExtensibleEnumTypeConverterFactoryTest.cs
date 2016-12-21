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
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.UnitTests.Infrastructure
{
  [TestFixture]
  public class ExtensibleEnumTypeConverterFactoryTest
  {
    private ITypeConverterFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _factory = new ExtensibleEnumTypeConverterFactory();
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithExtensibleEnum_ReturnsExtensibleEnumConverter ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Color));
      Assert.That (typeConverter, Is.TypeOf<ExtensibleEnumConverter>());
      Assert.That (((ExtensibleEnumConverter) typeConverter).ExtensibleEnumType, Is.EqualTo (typeof (Color)));
    }

    [Test]
    public void CreateTypeConverterOrDefault_WithOtherType_ReturnsNull ()
    {
      var typeConverter = _factory.CreateTypeConverterOrDefault (typeof (Int32));
      Assert.That (typeConverter, Is.Null);
    }
  }
}