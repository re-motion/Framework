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
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class TypeConversionProviderTest
  {
    private readonly Type _string = typeof (string);
    private ITypeConversionProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();
    }

    [Test]
    public void CanConvert_FromExtensibleEnum_ToString ()
    {
      Assert.That (_provider.CanConvert (typeof (Color), _string), Is.True);
    }

    [Test]
    public void CanConvert_FromString_ToExtensibleEnum ()
    {
      Assert.That (_provider.CanConvert (_string, typeof (Color)), Is.True);
    }

    [Test]
    public void Convert_FromExtensibleEnum_ToString ()
    {
      Assert.That (_provider.Convert (typeof (Color), typeof (string), Color.Values.Red()), Is.EqualTo ("Red"));
    }

    [Test]
    public void Convert_FromString_ToExtensibleEnum ()
    {
      Assert.That (_provider.Convert (typeof (string), typeof (Color), "Red"), Is.EqualTo (Color.Values.Red()));
    }

    [Test]
    public void GetTypeConverter_FromExtensibleEnum_ToString ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (typeof (Color), _string);
      Assert.That (converterResult.TypeConverterType, Is.EqualTo (TypeConverterType.SourceTypeConverter));
      Assert.That (converterResult.TypeConverter, Is.InstanceOf (typeof (ExtensibleEnumConverter)));
      Assert.That (((ExtensibleEnumConverter) converterResult.TypeConverter).ExtensibleEnumType, Is.SameAs (typeof (Color)));
    }

    [Test]
    public void GetTypeConverter_FromString_ToExtensibleEnum ()
    {
      TypeConverterResult converterResult = _provider.GetTypeConverter (_string, typeof (Color));
      Assert.That (converterResult.TypeConverterType, Is.EqualTo (TypeConverterType.DestinationTypeConverter));
      Assert.That (converterResult.TypeConverter, Is.InstanceOf (typeof (ExtensibleEnumConverter)));
      Assert.That (((ExtensibleEnumConverter) converterResult.TypeConverter).ExtensibleEnumType, Is.SameAs (typeof (Color)));
    }

    [Test]
    public void GetTypeConverter_ForExtensibleEnum ()
    {
      TypeConverter converterFirstRun = _provider.GetTypeConverter (typeof (Color));
      TypeConverter converterSecondRun = _provider.GetTypeConverter (typeof (Color));
      Assert.That (converterFirstRun, Is.Not.Null, "TypeConverter from first run is null.");
      Assert.That (converterSecondRun, Is.Not.Null, "TypeConverter from second run is null.");
      Assert.That (converterSecondRun, Is.SameAs (converterFirstRun));
      Assert.That (converterFirstRun, Is.InstanceOf (typeof (ExtensibleEnumConverter)));
      Assert.That (((ExtensibleEnumConverter) converterFirstRun).ExtensibleEnumType, Is.SameAs (typeof (Color)));
    }
  }
}