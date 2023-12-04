﻿using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class ISecurityPrincipalRepositoryTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var factory = _serviceLocator.GetInstance<ISecurityPrincipalRepository>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(SecurityPrincipalRepository)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<ISecurityPrincipalRepository>();
      var factory2 = _serviceLocator.GetInstance<ISecurityPrincipalRepository>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}
