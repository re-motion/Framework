﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class IUserControlBindingValidatorFactoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var instance = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());

      var factories = ((CompoundValidatorFactory<UserControlBinding>) instance).VlidatorFactories;
      Assert.That (factories.Select (f => f.GetType()), Has.Member (typeof (UserControlBindingValidatorFactory)));
      Assert.That (factories.Count, Is.EqualTo (1));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IUserControlBindingValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<UserControlBinding>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }
  }
}