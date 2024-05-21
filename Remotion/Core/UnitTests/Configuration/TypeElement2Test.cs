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
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using TypeNameConverter = Remotion.Utilities.TypeNameConverter;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElement2Test
  {
    [Test]
    public void Initialize ()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection)PrivateInvoke.GetNonPublicProperty(typeElement, "Properties");
      Assert.That(properties, Is.Not.Null);
      ConfigurationProperty property = properties["type"];
      Assert.That(property, Is.Not.Null);
      Assert.That(property.DefaultValue, Is.EqualTo(typeof(DerivedSampleType)));
      Assert.That(property.Converter, Is.InstanceOf(typeof(TypeNameConverter)));
      Assert.That(property.Validator, Is.InstanceOf(typeof(SubclassTypeValidator)));
      Assert.That(property.IsRequired, Is.True);
    }

    [Test]
    public void GetType_WithDefaultValue ()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.That(typeElement.Type, Is.EqualTo(typeof(DerivedSampleType)));
    }

    [Test]
    public void CreateInstance_WithoutType ()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      Assert.That(typeElement.CreateInstance(), Is.InstanceOf(typeof(DerivedSampleType)));
    }

    [Test]
    public void Deserialize_WithValidType ()
    {
      TypeElement<SampleType, DerivedSampleType> typeElement = new TypeElement<SampleType, DerivedSampleType>();

      string xmlFragment = @"<theElement type=""Remotion.UnitTests::Configuration.SampleType"" />";
      ConfigurationHelper.DeserializeElement(typeElement, xmlFragment);

      Assert.That(typeElement.Type, Is.EqualTo(typeof(SampleType)));
    }
  }
}
