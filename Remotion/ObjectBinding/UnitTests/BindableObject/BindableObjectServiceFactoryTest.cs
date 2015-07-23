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
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectServiceFactoryTest
  {
    private IBusinessObjectServiceFactory _serviceFactory;
    private IBusinessObjectProviderWithIdentity _provider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _provider = MockRepository.GenerateStub<IBusinessObjectProviderWithIdentity>();
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_provider, typeof (IBusinessObjectStringFormatterService)),
          Is.InstanceOf (typeof (BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (IGetObjectService)), Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That (_serviceFactory.CreateService (_provider, typeof (ISearchAvailableObjectsService)), Is.Null);
    }
  }
}
