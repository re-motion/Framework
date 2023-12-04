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
using Moq;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  public abstract class SafeContextTestBase
  {
    protected Mock<ISafeContextStorageProvider> SafeContextProviderMock { get; private set; }

    [SetUp]
    public void SetUp ()
    {
      ResetSafeContextStorageProvider();

      SafeContextProviderMock = new Mock<ISafeContextStorageProvider>(MockBehavior.Strict);
      SafeContextProviderMock.Setup(e => e.OpenSafeContextBoundary()).Returns((SafeContextBoundary)default);
    }

    [TearDown]
    public void TearDown ()
    {
      ResetSafeContextStorageProvider();
    }

    protected static IDisposable SetupImplicitSafeContextStorageProvider (ISafeContextStorageProvider safeContextStorageProvider)
    {
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => safeContextStorageProvider);

      return new ServiceLocatorScope(serviceLocator);
    }

    private static void ResetSafeContextStorageProvider ()
    {
      var fields = PrivateInvoke.GetNonPublicStaticField(typeof(SafeContext), "s_fields");
      Assertion.IsNotNull(fields);
      PrivateInvoke.SetPublicField(
          fields,
          "SafeContextStorageProvider",
          new Lazy<ISafeContextStorageProvider>(
              () => SafeServiceLocator.Current.GetInstance<ISafeContextStorageProvider>(),
              LazyThreadSafetyMode.ExecutionAndPublication));
    }
  }
}
