﻿using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessEvaluation
{
  [TestFixture]
  public class ISecurityTokenBuilderTest
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
      var factory = _serviceLocator.GetInstance<ISecurityTokenBuilder>();

      Assert.That(factory, Is.Not.Null);
      Assert.That(factory, Is.TypeOf(typeof(SecurityTokenBuilder)));
    }

    [Test]
    public void GetInstance_Twice ()
    {
      var factory1 = _serviceLocator.GetInstance<ISecurityTokenBuilder>();
      var factory2 = _serviceLocator.GetInstance<ISecurityTokenBuilder>();

      Assert.That(factory1, Is.SameAs(factory2));
    }
  }
}