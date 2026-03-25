// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Remotion.Web.Development.WebTesting.UnitTests;

[TestFixture]
public class WebTestFeatureCollectionTest
{
  private interface ITestFeature;

  private class TestFeature : ITestFeature;

  [Test]
  public void Remove ()
  {
    var features = new WebTestFeatureCollection([(typeof(ITestFeature), new TestFeature())]);
    features.Remove<ITestFeature>();

    Assert.That(features, Is.Empty);
  }

  [Test]
  public void Remove_WithNoElements_DoesNothing ()
  {
    var features = new WebTestFeatureCollection();
    features.Remove<ITestFeature>();

    Assert.That(features, Is.Empty);
  }

  [Test]
  public void Set_NonexistentElement ()
  {
    var features = new WebTestFeatureCollection();

    var instance = new TestFeature();
    features.Set<ITestFeature>(instance);

    AssertCollectionItems(features, [(typeof(ITestFeature), instance)]);
  }

  [Test]
  public void Set_ExistentElement_NewElementReplacesOriginal ()
  {
    var instance1 = new TestFeature();
    var features = new WebTestFeatureCollection([(typeof(ITestFeature), instance1)]);

    var instance2 = new TestFeature();
    features.Set<ITestFeature>(instance2);

    AssertCollectionItems(features, [(typeof(ITestFeature), instance2)]);
  }

  [Test]
  public void TryAdd_NonexistentElement_NewElementIsAdded ()
  {
    var features = new WebTestFeatureCollection();

    var instance = new TestFeature();
    var addSuccessful = features.TryAdd<ITestFeature>(instance);

    Assert.That(addSuccessful, Is.True);
    AssertCollectionItems(features, [(typeof(ITestFeature), instance)]);
  }

  [Test]
  public void TryAdd_ExistentElement_DoesNothingAndOriginalIsUntouched ()
  {
    var instance1 = new TestFeature();
    var features = new WebTestFeatureCollection([(typeof(ITestFeature), instance1)]);

    var instance2 = new TestFeature();
    var addSuccessful = features.TryAdd<ITestFeature>(instance2);

    Assert.That(addSuccessful, Is.False);
    AssertCollectionItems(features, [(typeof(ITestFeature), instance1)]);
  }

  [Test]
  public void TryGet_NonexistentElement_ReturnsFalse ()
  {
    var features = new WebTestFeatureCollection();

    var getSuccessful = features.TryGet<ITestFeature>(out var result);

    Assert.That(getSuccessful, Is.False);
    Assert.That(result, Is.Null);
  }

  [Test]
  public void TryGet_ExistentElement_ReturnsElement ()
  {
    var instance1 = new TestFeature();
    var features = new WebTestFeatureCollection([(typeof(ITestFeature), instance1)]);

    var getSuccessful = features.TryGet<ITestFeature>(out var result);

    Assert.That(getSuccessful, Is.True);
    Assert.That(result, Is.SameAs(instance1));
  }

  [Test]
  public void Dispose_DisposesItems ()
  {
    var disposableMock = new Mock<IDisposable>();

    var features = new WebTestFeatureCollection([(typeof(ITestFeature), new TestFeature()), (typeof(Stream), disposableMock.Object)]);
    features.Dispose();

    disposableMock.Verify(_ => _.Dispose(), Times.Once);
  }

  [Test]
  public void GetEnumerator ()
  {
    var items = new List<(Type, object)>()
                {
                    (typeof(ITestFeature), new TestFeature()),
                    (typeof(object), new object())
                };

    var features = new WebTestFeatureCollection(items);
    AssertCollectionItems(features, items);
  }

  [Test]
  public void InitializeFeatures ()
  {
    var webTestFeatureMock = new Mock<ILifecycleWebTestFeature>();
    var items = new List<(Type, object)>
                {
                    (typeof(ILifecycleWebTestFeature), webTestFeatureMock.Object)
                };

    var features = new WebTestFeatureCollection(items);

    features.InitializeFeatures();

    webTestFeatureMock.Verify(f => f.Initialize(), Times.Once);
  }

  [Test]
  public void InitializeFeatures_ThrowsAggregateException ()
  {
    var webTestFeatureMock1 = new Mock<ILifecycleWebTestFeature>();
    var webTestFeatureMock2 = new Mock<ILifecycleWebTestFeature>();

    Exception[] expectedExceptions = [new("Feature 1 failed")];
    webTestFeatureMock1.Setup(f => f.Initialize()).Throws(expectedExceptions[0]);

    var items = new List<(Type, object)>
                {
                    (typeof(ILifecycleWebTestFeature), webTestFeatureMock1.Object),
                    (typeof(ITestFeature), webTestFeatureMock2.Object)
                };
    var features = new WebTestFeatureCollection(items);

    var ex = Assert.Throws<AggregateException>(() => features.InitializeFeatures());

    Assert.That(ex!.InnerExceptions, Is.EquivalentTo(expectedExceptions));

    webTestFeatureMock1.Verify(f => f.Initialize(), Times.Once);
    webTestFeatureMock2.Verify(f => f.Initialize(), Times.Once);
  }

  private void AssertCollectionItems (
      WebTestFeatureCollection features,
      IEnumerable<(Type, object)> expectedItems)
  {
    Assert.That(
        features,
        Is.EquivalentTo(expectedItems.Select(e => new KeyValuePair<Type, object>(e.Item1, e.Item2))));
  }
}
