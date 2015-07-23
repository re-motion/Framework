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
using Remotion.Mixins;
using Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.InterfaceProperties.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.InterfaceProperties
{
  [TestFixture]
  public class InterfacePropertiesTest : TestBase
  {
    [Test]
    public void ImplicitPropertyWithGetter_IsReadOnly ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetter");
      Assert.That (property.IsReadOnly (businessObject), Is.True);
    }

    [Test]
    public void ImplicitPropertyWithGetter_GetValue ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;

      obj.SetImplicitPropertyWithGetter ("value");
      Assert.That (businessObject.GetProperty ("ImplicitPropertyWithGetter"), Is.EqualTo ("value"));
    }

    [Test]
    public void ImplicitPropertyWithGetterAndSetter_IsNotReadOnly ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObject.BusinessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetterAndSetter");
      Assert.That (property.IsReadOnly (businessObject), Is.False);
    }

    [Test]
    public void ImplicitPropertyWithGetterAndSetter_SetValue ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;

      businessObject.SetProperty ("ImplicitPropertyWithGetterAndSetter", "value");
      Assert.That (obj.ImplicitPropertyWithGetterAndSetter, Is.EqualTo ("value"));
    }

    [Test]
    public void ImplicitPropertyWithGetterAndImplementationOnlySetter_IsNotReadOnly ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetterAndImplementationOnlySetter");
      Assert.That (property.IsReadOnly (businessObject), Is.False);
    }

    [Test]
    public void ImplicitPropertyWithGetterAndImplementationOnlySetter_SetValue ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;

      businessObject.SetProperty ("ImplicitPropertyWithGetterAndImplementationOnlySetter", "value");
      Assert.That (obj.ImplicitPropertyWithGetterAndImplementationOnlySetter, Is.EqualTo ("value"));
    }

    [Test]
    public void ExplicitPropertyWithGetter_IsReadOnly ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ExplicitPropertyWithGetter");
      Assert.That (property.IsReadOnly (businessObject), Is.True);
    }

    [Test]
    public void ExplicitPropertyWithGetter_GetValue ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;

      obj.SetExplicitPropertyWithGetter ("value");
      Assert.That (businessObject.GetProperty ("ExplicitPropertyWithGetter"), Is.EqualTo ("value"));
    }

    [Test]
    public void ExplicitPropertyWithGetterAndSetter_IsNotReadOnly ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ExplicitPropertyWithGetterAndSetter");
      Assert.That (property.IsReadOnly (businessObject), Is.False);
    }

    [Test]
    public void ExplicitPropertyWithGetterAndSetter_SetValue ()
    {
      var obj = ObjectFactory.Create<ClassWithInterface>();
      var businessObject = (IBusinessObject) obj;

      businessObject.SetProperty ("ExplicitPropertyWithGetterAndSetter", "value");
      Assert.That (((IInterface) obj).ExplicitPropertyWithGetterAndSetter, Is.EqualTo ("value"));
    }
  }
}