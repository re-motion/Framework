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
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Rendering
{
  [TestFixture]
  public class RendererBaseTest : RendererTestBase
  {
    [Test]
    public void GetResourceManager_WithSameArguments_ReturnsSameInstance ()
    {
      var resourceType = typeof (string);
      var resourceManager = new FakeResourceManager();
      var renderer = new TestableRendererBase(Mock.Of<IResourceUrlFactory>(), GlobalizationService, Mock.Of<IRenderingFeatures>());

      var instance1 = renderer.GetResourceManager(resourceType, resourceManager);
      var instance2 = renderer.GetResourceManager(resourceType, resourceManager);

      Assert.That(instance1, Is.Not.Null);
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void GetResourceManager_DoesNotKeepResourceManagerAlive ()
    {
      var renderer = new TestableRendererBase(Mock.Of<IResourceUrlFactory>(), GlobalizationService, Mock.Of<IRenderingFeatures>());

      var resourceManagerWeakReference = CallGetResourceManagerWithManagerAndReturnItsWeakReference(renderer);

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      Assert.That(resourceManagerWeakReference.IsAlive, Is.False);
    }

    private static WeakReference CallGetResourceManagerWithManagerAndReturnItsWeakReference (TestableRendererBase renderer)
    {
      var resourceManager = new FakeResourceManager();
      var resourceManagerReference = new WeakReference(resourceManager);

      _ = renderer.GetResourceManager(typeof (string), resourceManager);

      return resourceManagerReference;
    }

    private class FakeResourceManager : IResourceManager
    {
      public bool IsNull => false;

      public string Name => nameof(FakeResourceManager);

      public IReadOnlyDictionary<string, string> GetAllStrings (string prefix) => new Dictionary<string, string>();

      public IReadOnlyDictionary<CultureInfo, string> GetAvailableStrings (string id) => new Dictionary<CultureInfo, string>();

      public bool TryGetString (string id, out string value)
      {
        value = null;
        return false;
      }
    }

    private interface IDummyControl : IStyledControl, IControlWithDiagnosticMetadata
    {
    }

    private class TestableRendererBase : RendererBase<IDummyControl>
    {
      public TestableRendererBase (
          [NotNull] IResourceUrlFactory resourceUrlFactory,
          [NotNull] IGlobalizationService globalizationService,
          [NotNull] IRenderingFeatures renderingFeatures)
          : base (resourceUrlFactory, globalizationService, renderingFeatures)
      {
      }

      public new IResourceManager GetResourceManager (Type localResourcesType, IResourceManager controlResourceManager)
      {
        return base.GetResourceManager(localResourcesType, controlResourceManager);
      }
    }
  }
}