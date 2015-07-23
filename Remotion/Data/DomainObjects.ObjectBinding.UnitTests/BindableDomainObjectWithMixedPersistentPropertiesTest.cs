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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectWithMixedPersistentPropertiesTest : ObjectBindingTestBase
  {
    [Test]
    public void MixedProperty_Exists ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      Assert.That (boClass.GetPropertyDefinitions ().Select (p => p.Identifier ).ToArray(),
          Has.Member("MixedProperty"));
    }

    [Test]
    public void MixedProperty_DefaultValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.Null);
    }

    [Test]
    public void MixedProperty_NonDefaultValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      var dateTime = new DateTime(2008, 08, 01);
      ((IMixinAddingPersistentProperties) instance).MixedProperty = dateTime;
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedProperty_NonDefaultValue_WithUnchangedValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      var dateTime = ((IMixinAddingPersistentProperties) instance).MixedProperty;
      ((IMixinAddingPersistentProperties) instance).MixedProperty = dateTime;
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }
  }
}
