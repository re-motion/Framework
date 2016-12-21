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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class BindableObjectWithoutSecurityTest : BindableObjectTestBase
  {
    private ServiceLocatorScope _serviceLocatorScope;
    private bool _disableAccessChecksBackup;

    [SetUp]
    public void SetUp ()
    {
      var bindablePropertyReadAccessStrategy =
          new CompundBindablePropertyReadAccessStrategy (
              new IBindablePropertyReadAccessStrategy[] { new BindableDomainObjectPropertyReadAccessStrategy() });

      var bindablePropertyWriteAccessStrategy =
          new CompundBindablePropertyWriteAccessStrategy (
              new IBindablePropertyWriteAccessStrategy[] { new BindableDomainObjectPropertyWriteAccessStrategy() });

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IBindablePropertyReadAccessStrategy> (() => bindablePropertyReadAccessStrategy);
      serviceLocator.RegisterSingle<IBindablePropertyWriteAccessStrategy> (() => bindablePropertyWriteAccessStrategy);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _disableAccessChecksBackup = SecurityConfiguration.Current.DisableAccessChecks;
      SecurityConfiguration.Current.DisableAccessChecks = true;
      ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [TearDown]
    public void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);

      ClientTransactionScope.ResetActiveScope ();
      SecurityConfiguration.Current.DisableAccessChecks = _disableAccessChecksBackup;
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public override void BusinessObject_Property_IsAccessible ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_Property_IsAccessible on reference system: ~0.08탎 (release build), ~0.66 탎 (debug build)");

      base.BusinessObject_Property_IsAccessible ();

      Console.WriteLine ();
    }

    [Test]
    public override void BusinessObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_GetProperty on reference system: ~1.4 탎 (release build), ~3.8 탎 (debug build)");

      base.BusinessObject_GetProperty ();
      
      Console.WriteLine ();
    }

    [Test]
    public override void DynamicMethod_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DynamicMethod_GetProperty on reference system: ~0.8 탎 (release build), ~2.0 탎 (debug build)");

      base.DynamicMethod_GetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_GetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DomainObject_GetProperty on reference system: ~0.8 탎 (release build), ~2.0 탎 (debug build)");

      base.DomainObject_GetProperty ();

      Console.WriteLine ();
    }
    [Test]
    public override void BusinessObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for BusinessObject_SetProperty on reference system: ~1.4 탎 (release build), ~3.3 탎 (debug build)");

      base.BusinessObject_SetProperty ();

      Console.WriteLine ();
    }

    [Test]
    public override void DomainObject_SetProperty ()
    {
      Console.WriteLine (
          "Expected average duration of BindableObjectWithoutSecurityTest for DomainObject_SetProperty on reference system: ~1.3 탎 (release build), ~3.0 탎 (debug build)");

      base.DomainObject_SetProperty ();

      Console.WriteLine ();
    }
  }
}