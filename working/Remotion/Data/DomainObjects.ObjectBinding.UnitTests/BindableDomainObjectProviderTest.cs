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
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectProviderTest
  {
    [Test]
    public void Instantiate_WithDefaultValues ()
    {
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider();
      Assert.IsInstanceOf (typeof (BindableDomainObjectMetadataFactory), provider.MetadataFactory);
      Assert.IsInstanceOf (typeof (BindableObjectServiceFactory), provider.ServiceFactory);
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (BindableDomainObjectMetadataFactory)).AddMixin<MixinStub> ().EnterScope ())
      {
        BindableDomainObjectProvider provider = new BindableDomainObjectProvider ();
        Assert.That (provider.MetadataFactory, Is.InstanceOf (typeof (BindableDomainObjectMetadataFactory)));
        Assert.That (provider.MetadataFactory, Is.InstanceOf (typeof (IMixinTarget)));
        Assert.That (provider.ServiceFactory, Is.InstanceOf (typeof (BindableObjectServiceFactory)));
      }
    }

    [Test]
    public void Instantiate_WithCustomValues ()
    {
      IMetadataFactory metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      IBusinessObjectServiceFactory serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider (metadataFactoryStub, serviceFactoryStub);

      Assert.That (provider.MetadataFactory, Is.SameAs (metadataFactoryStub));
      Assert.That (provider.ServiceFactory, Is.SameAs (serviceFactoryStub));
    }
  }
}
