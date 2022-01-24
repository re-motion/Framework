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
using System.Threading;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public partial class SafeContextTest
  {
    [Test]
    public void Instance_AutoInitialization ()
    {
      ISafeContextStorageProvider instance = SafeContext.Instance;
      Assert.That(instance, Is.Not.Null);
      Assert.That(SafeContext.Instance, Is.SameAs(instance));
    }

    private static IDisposable SetupImplicitSafeContextStorageProvider (ISafeContextStorageProvider safeContextStorageProvider)
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => safeContextStorageProvider);

      return new ServiceLocatorScope(serviceLocator);
    }

    private static void ResetSafeContextStorageProvider ()
    {
      PrivateInvoke.SetNonPublicStaticField(
          typeof(SafeContext),
          "s_instance",
          new Lazy<ISafeContextStorageProvider>(
                  () => SafeServiceLocator.Current.GetInstance<ISafeContextStorageProvider>(),
                  LazyThreadSafetyMode.ExecutionAndPublication));
    }
  }
}
