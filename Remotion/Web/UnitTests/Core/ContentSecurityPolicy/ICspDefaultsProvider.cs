// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Web;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy
{
  [TestFixture]
  public class ICspDefaultsProviderTest
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
      var factory = _serviceLocator.GetInstance<ICspDefaultsProvider>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(CspDefaultsProvider)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var factory1 = _serviceLocator.GetInstance<ICspDefaultsProvider>();
      var factory2 = _serviceLocator.GetInstance<ICspDefaultsProvider>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
