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
using Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.MixedProperties.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.MixedProperties
{
  [TestFixture]
  public class PubliclyIntroducedMixedPropertiesTest : TestBase
  {
    [Test]
    public void ImplicitPropertyWithGetter_IsReadOnly ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPubliclyIntroducedMembers>();
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetter");
      Assert.That (property.IsReadOnly (businessObject), Is.True);
    }

    [Test]
    public void ImplicitPropertyWithGetter_GetValue ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var mixin = Mixin.Get<MixinClass> (businessObject);

      mixin.SetImplicitPropertyWithGetter ("value");
      Assert.That (businessObject.GetProperty ("ImplicitPropertyWithGetter"), Is.EqualTo ("value"));
    }

    [Test]
    public void ImplicitPropertyWithGetterAndSetter_IsNotReadOnly ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetterAndSetter");
      Assert.That (property.IsReadOnly (businessObject), Is.False);
    }

    [Test]
    public void ImplicitPropertyWithGetterAndSetter_SetValue ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var mixin = Mixin.Get<MixinClass> (businessObject);

      businessObject.SetProperty ("ImplicitPropertyWithGetterAndSetter", "value");
      Assert.That (mixin.ImplicitPropertyWithGetterAndSetter, Is.EqualTo ("value"));
    }

    [Test]
    public void ImplicitPropertyWithGetterAndImplementationOnlySetter_IsReadOnly ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ImplicitPropertyWithGetterAndImplementationOnlySetter");
      Assert.That (property.IsReadOnly (businessObject), Is.True);
    }

    [Test]
    public void ImplicitPropertyWithGetterAndImplementationOnlySetter_SetValueThrowsInvalidOperationException ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();

      Assert.That (
          () => businessObject.SetProperty ("ImplicitPropertyWithGetterAndImplementationOnlySetter", "value"),
          Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void ExplicitPropertyWithGetter_IsReadOnly ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ExplicitPropertyWithGetter");
      Assert.That (property.IsReadOnly (businessObject), Is.True);
    }

    [Test]
    public void ExplicitPropertyWithGetter_GetValue ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var mixin = Mixin.Get<MixinClass> (businessObject);

      mixin.SetExplicitPropertyWithGetter ("value");
      Assert.That (businessObject.GetProperty ("ExplicitPropertyWithGetter"), Is.EqualTo ("value"));
    }

    [Test]
    public void ExplicitPropertyWithGetterAndSetter_IsNotReadOnly ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var businessObjectClass = businessObject.BusinessObjectClass;

      var property = businessObjectClass.GetPropertyDefinition ("ExplicitPropertyWithGetterAndSetter");
      Assert.That (property.IsReadOnly (businessObject), Is.False);
    }

    [Test]
    public void ExplicitPropertyWithGetterAndSetter_SetValue ()
    {
      var businessObject = (IBusinessObject) ObjectFactory.Create<TargetClassWithPrivatelyIntroducedMembers>();
      var mixin = Mixin.Get<MixinClass> (businessObject);

      businessObject.SetProperty ("ExplicitPropertyWithGetterAndSetter", "value");
      Assert.That (((IMixinInterface) mixin).ExplicitPropertyWithGetterAndSetter, Is.EqualTo ("value"));
    }
  }
}