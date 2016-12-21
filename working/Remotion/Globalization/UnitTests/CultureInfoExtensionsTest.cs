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
using System.Globalization;
using NUnit.Framework;


namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class CultureInfoExtensionsTest
  {
    [Test]
    public void GetCultureHierarchy_WithPredefinedInvariant_ReturnsInvariant ()
    {
      var cultureInfo = CultureInfo.InvariantCulture;

      Assert.That (
          cultureInfo.GetCultureHierarchy(),
          Is.EqualTo (
              new[]
              {
                  new CultureInfo ("")
              }));
    }

    [Test]
    public void GetCultureHierarchy_WithNewInvariant_ReturnsInvariant ()
    {
      var cultureInfo = new CultureInfo ("");

      Assert.That (
          cultureInfo.GetCultureHierarchy(),
          Is.EqualTo (
              new[]
              {
                  new CultureInfo ("")
              }));
    }
    
    [Test]
    public void GetCultureHierarchy_WithNeutralCulture_ReturnsNeutralThenInvariant ()
    {
      var cultureInfo = new CultureInfo ("en");

      Assert.That (
          cultureInfo.GetCultureHierarchy(),
          Is.EqualTo (
              new[]
              {
                  new CultureInfo ("en"),
                  new CultureInfo ("")
              }));
    }
    
    [Test]
    public void GetCultureHierarchy_WithSpecificCulture_ReturnsSpecificThenNeutralThenInvariant ()
    {
      var cultureInfo = new CultureInfo ("en-US");

      Assert.That (
          cultureInfo.GetCultureHierarchy(),
          Is.EqualTo (
              new[]
              {
                  new CultureInfo ("en-US"),
                  new CultureInfo ("en"),
                  new CultureInfo ("")
              }));
    }
  }
}