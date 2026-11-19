// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

[TestFixture]
public class ISortingOptimizationNodeFactoryTest
{
  private DefaultServiceLocator _serviceLocator;

  [SetUp]
  public void SetUp ()
  {
    _serviceLocator = DefaultServiceLocator.Create();
  }


  [Test]
  public void ResolvingNodeFactoryFromServiceLocator_GetInstance_Once ()
  {
    _serviceLocator = DefaultServiceLocator.Create();
    var sortingOptimizationNodeFactory = _serviceLocator.GetInstance<ISortingOptimizationNodeFactory>();

    Assert.That(sortingOptimizationNodeFactory, Is.TypeOf<SortingOptimizationNodeFactory>());
  }

  [Test]
  public void ResolvingNodeFactoryFromServiceLocator_GetInstance_Twice_ReturnsSameInstance ()
  {
    var propertyDefaultValueProvider1 = _serviceLocator.GetInstance<ISortingOptimizationNodeFactory>();
    var propertyDefaultValueProvider2 = _serviceLocator.GetInstance<ISortingOptimizationNodeFactory>();

    Assert.That(propertyDefaultValueProvider1, Is.SameAs(propertyDefaultValueProvider2));
  }
}
