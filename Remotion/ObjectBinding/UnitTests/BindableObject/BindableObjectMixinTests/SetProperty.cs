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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class SetProperty : TestBase
  {
    private SimpleBusinessObjectClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty);
      _bindableObjectMixin = Mixin.Get<BindableObjectMixin>(_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void WithBusinessObjectProperty ()
    {
      _businessObject.SetProperty(_businessObject.BusinessObjectClass.GetPropertyDefinition("String"), "A String");

      Assert.That(_bindableObject.String, Is.EqualTo("A String"));
    }

    [Test]
    public void WithPropertyIdentifier ()
    {
      _businessObject.SetProperty("String", "A String");

      Assert.That(_bindableObject.String, Is.EqualTo("A String"));
    }

    [Test]
    [Ignore("TODO: discuss desired behavior")]
    public void WithoutSetter ()
    {
      IBusinessObject businessObject = Mixin.Get<BindableObjectMixin>(ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty));
      Assert.That(
          () => businessObject.SetProperty("StringWithoutSetter", null),
          Throws.InstanceOf<KeyNotFoundException>()
              .With.Message.EqualTo(
                  "The property 'StringWithoutSetter' was not found on business object class "
                  + "'Remotion.ObjectBinding.UnitTests.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests'."));
    }
  }
}
