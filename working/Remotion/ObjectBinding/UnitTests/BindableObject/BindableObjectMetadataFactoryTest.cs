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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectMetadataFactoryTest
  {
    public class TestClass
    {
      public int Property
      {
        get { return 0; }
      }
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass (typeof (BindableObjectMetadataFactory)).AddMixin<MixinStub>().EnterScope())
      {
        BindableObjectMetadataFactory factory = BindableObjectMetadataFactory.Create();
        Assert.That (factory, Is.InstanceOf (typeof (BindableObjectMetadataFactory)));
        Assert.That (factory, Is.InstanceOf (typeof (IMixinTarget)));
      }
    }

    [Test]
    public void CreateClassReflector ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      IClassReflector classReflector = BindableObjectMetadataFactory.Create().CreateClassReflector (typeof (TestClass), provider);
      Assert.That (classReflector.TargetType, Is.SameAs (typeof (TestClass)));
      Assert.That (classReflector.BusinessObjectProvider, Is.SameAs (provider));
    }

    [Test]
    public void CreatePropertyFinder ()
    {
      IPropertyFinder finder = BindableObjectMetadataFactory.Create().CreatePropertyFinder (typeof (TestClass));
      Assert.That (finder.GetType(), Is.SameAs (typeof (ReflectionBasedPropertyFinder)));
      Assert.That (new List<IPropertyInformation> (finder.GetPropertyInfos())[0].DeclaringType, Is.SameAs (TypeAdapter.Create (typeof (TestClass))));
    }

    [Test]
    public void CreatePropertyReflector ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        PropertyInfo propertyInfo = typeof (TestClass).GetProperty ("Property");
        IPropertyInformation property = PropertyInfoAdapter.Create(propertyInfo);
        PropertyReflector propertyReflector =
            BindableObjectMetadataFactory.Create().CreatePropertyReflector (typeof (TestClass), property, new BindableObjectProvider());
        Assert.That (propertyReflector.GetType(), Is.SameAs (typeof (PropertyReflector)));
        Assert.That (propertyReflector.PropertyInfo, Is.SameAs (property));
      }
    }
  }
}
