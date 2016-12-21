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
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class PropertyIntegrationTest : ObjectBindingTestBase
  {
    [Test]
    public void TestPropertyAccess ()
    {
      SampleBindableMixinDomainObject instance = SampleBindableMixinDomainObject.NewObject();
      var instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.Null);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (instance.Int32, Is.EqualTo (0));
        Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (0));
      }

      instanceAsIBusinessObject.SetProperty ("Int32", 1);
      Assert.That (instance.Int32, Is.EqualTo (1));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (instance.Int32, Is.EqualTo (1));
        Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (1));
      }

      instance.Int32 = 2;
      Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (2));
      Assert.That (instanceAsIBusinessObject.GetPropertyString ("Int32"), Is.EqualTo ("2"));

      instance.Int32 = 1;
      Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (1));

      instance.Int32 = 0;
      Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (0));

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (instance.Int32, Is.EqualTo (0));
        Assert.That (instanceAsIBusinessObject.GetProperty ("Int32"), Is.EqualTo (0));
      }
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableMixinDomainObject));
      var bindableObjectClass = provider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));

      var properties = bindableObjectClass.GetPropertyDefinitions();

      var propertiesByName = Array.ConvertAll (properties, property => property.Identifier);
      Assert.That (propertiesByName, Is.EquivalentTo (new[] { "List", "Relation", "Name", "Int32" }));
    }
  }
}