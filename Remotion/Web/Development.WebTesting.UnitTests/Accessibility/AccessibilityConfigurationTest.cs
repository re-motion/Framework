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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Accessibility;

namespace Remotion.Web.Development.WebTesting.UnitTests.Accessibility
{
  [TestFixture]
  public class AccessibilityConfigurationTest
  {
    [Test]
    public void Initialize_NegativeIFrameTimeout_ThrowsArgumentOutOfRangeException ()
    {
      Assert.That (
          () => new AccessibilityConfiguration (iframeTimeout: TimeSpan.FromSeconds (-5)),
          Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Initialize_WithoutParameters_DefaultValues ()
    {
      var configuration = new AccessibilityConfiguration();

      Assert.That (configuration.IFrameTimeout, Is.EqualTo (TimeSpan.FromSeconds (5)));
      Assert.That (configuration.IncludeIFrames, Is.True);
      Assert.That (configuration.EnableXPath, Is.True);
      Assert.That (configuration.EnableAbsolutePaths, Is.True);
      Assert.That (configuration.EnableScrollToInitialPosition, Is.True);
      Assert.That (configuration.ConformanceLevel, Is.EqualTo (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA));
    }

    [Test]
    public void Initialize_WithParameters_OverridesDefaults ()
    {
      var configuration = new AccessibilityConfiguration (includeIframes: false);

      Assert.That (configuration.IFrameTimeout, Is.EqualTo (TimeSpan.FromSeconds (5)));
      Assert.That (configuration.IncludeIFrames, Is.False);
      Assert.That (configuration.EnableXPath, Is.True);
      Assert.That (configuration.EnableAbsolutePaths, Is.True);
      Assert.That (configuration.EnableScrollToInitialPosition, Is.True);
      Assert.That (configuration.ConformanceLevel, Is.EqualTo (AccessibilityConformanceLevel.Wcag20_ConformanceLevelDoubleA));
    }
  }
}