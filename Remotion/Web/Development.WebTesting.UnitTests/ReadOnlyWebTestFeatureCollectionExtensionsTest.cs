// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Remotion.Web.Development.WebTesting.UnitTests;

[TestFixture]
public class ReadOnlyWebTestFeatureCollectionExtensionsTest
{
  [Test]
  public void Contains_ExistingItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    var feature = new object();
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(true);

    var result = ReadOnlyWebTestFeatureCollectionExtensions.Contains<object>(featuresMock.Object);
    Assert.That(result, Is.True);
  }

  [Test]
  public void Contains_NonExistentItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    object feature = null;
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(false);

    var result = ReadOnlyWebTestFeatureCollectionExtensions.Contains<object>(featuresMock.Object);
    Assert.That(result, Is.False);
  }

  [Test]
  public void Get_ExistingItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    var feature = new object();
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(true);

    var result = ReadOnlyWebTestFeatureCollectionExtensions.Get<object>(featuresMock.Object);
    Assert.That(result, Is.SameAs(feature));
  }

  [Test]
  public void Get_NonExistentItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    object feature = null;
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(false);

    Assert.That(
        () => ReadOnlyWebTestFeatureCollectionExtensions.Get<object>(featuresMock.Object),
        Throws.TypeOf<KeyNotFoundException>()
            .With.Message.EqualTo("The specified feature 'System.Object' was not found in the feature collection."));
  }

  [Test]
  public void GetOrDefault_ExistingItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    var feature = new object();
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(true);

    var result = ReadOnlyWebTestFeatureCollectionExtensions.GetOrDefault<object>(featuresMock.Object);
    Assert.That(result, Is.SameAs(feature));
  }

  [Test]
  public void GetOrDefault_NonExistentItem ()
  {
    var featuresMock = new Mock<IReadOnlyWebTestFeatureCollection>();
    object feature = null;
    featuresMock.Setup(_ => _.TryGet<object>(out feature)).Returns(false);

    var result = ReadOnlyWebTestFeatureCollectionExtensions.GetOrDefault<object>(featuresMock.Object);
    Assert.That(result, Is.Null);
  }
}
