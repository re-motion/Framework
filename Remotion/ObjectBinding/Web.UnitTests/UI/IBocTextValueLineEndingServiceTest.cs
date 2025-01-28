// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI
{
  [TestFixture]
  public class IBocTextValueLineEndingServiceTest
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
      var instance = _serviceLocator.GetInstance<IBocTextValueLineEndingService>();

      Assert.That(instance, Is.Not.Null);
      Assert.That(instance, Is.TypeOf(typeof(DefaultBocTextValueLineEndingService)));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocTextValueLineEndingService>();
      var instance2 = _serviceLocator.GetInstance<IBocTextValueLineEndingService>();

      Assert.That(instance1, Is.SameAs(instance2));
    }
  }
}
