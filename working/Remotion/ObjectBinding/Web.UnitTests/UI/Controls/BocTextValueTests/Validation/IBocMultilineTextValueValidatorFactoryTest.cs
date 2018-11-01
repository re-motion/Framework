﻿using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests.Validation
{
  [TestFixture]
  public class IBocMultilineTextValueValidatorFactoryTest
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
      var instance = _serviceLocator.GetInstance<IBocMultilineTextValueValidatorFactory>();

      Assert.That (instance, Is.InstanceOf<CompoundValidatorFactory<IBocMultilineTextValue>>());

      var factories = ((CompoundValidatorFactory<IBocMultilineTextValue>) instance).VlidatorFactories;
      Assert.That (factories.Select (f => f.GetType()), Has.Member (typeof (BocMultilineTextValueValidatorFactory)));
      Assert.That (factories.Count, Is.EqualTo (1));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var instance1 = _serviceLocator.GetInstance<IBocMultilineTextValueValidatorFactory>();
      var instance2 = _serviceLocator.GetInstance<IBocMultilineTextValueValidatorFactory>();

      Assert.That (instance1, Is.InstanceOf<CompoundValidatorFactory<IBocMultilineTextValue>>());
      Assert.That (instance1, Is.SameAs (instance2));
    }
  }
}