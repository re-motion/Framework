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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class MultiLingualNameBasedEnumerationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetEnumerationValueDisplayName_IntegrationTest ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>();
      string resourceValue;

      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Name"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.EqualTo ("The Invariant Name"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.EqualTo ("The Invariant Name"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.EqualTo ("ValueWithoutLocalizedName"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.False);

        Assert.That (service.TryGetEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100), Is.EqualTo ("100"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault ((EnumWithMultiLingualNameAttribute) 100), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100), Is.False);
      }

      var culture = new CultureInfo ("en-US");
      using (new CultureScope (culture, culture))
      {
        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The en-US Name"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.EqualTo ("The en-US Name"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.EqualTo ("The en-US Name"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithLocalizedName), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.EqualTo ("ValueWithoutLocalizedName"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithMultiLingualNameAttribute.ValueWithoutLocalizedName), Is.False);

        Assert.That (service.TryGetEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100), Is.EqualTo ("100"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault ((EnumWithMultiLingualNameAttribute) 100), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName ((EnumWithMultiLingualNameAttribute) 100), Is.False);
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_EnumDescriptionIntegrationTest ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService> ();
      string resourceValue;

      Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithDescription.Value1, out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Value I"));
      Assert.That (service.GetEnumerationValueDisplayName (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithDescription.Value1), Is.True);

      Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithDescription.Value2, out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Value II"));
      Assert.That (service.GetEnumerationValueDisplayName (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithDescription.Value2), Is.True);

      Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription, out resourceValue), Is.False);
      Assert.That (service.GetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription), Is.EqualTo ("ValueWithoutDescription"));
      Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithDescription.ValueWithoutDescription), Is.Null);
      Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription), Is.False);

      Assert.That (service.TryGetEnumerationValueDisplayName ((EnumWithDescription) 100, out resourceValue), Is.False);
      Assert.That (service.GetEnumerationValueDisplayName ((EnumWithDescription) 100), Is.EqualTo ("100"));
      Assert.That (service.GetEnumerationValueDisplayNameOrDefault ((EnumWithDescription) 100), Is.Null);
      Assert.That (service.ContainsEnumerationValueDisplayName ((EnumWithDescription) 100), Is.False);
    }
  }
}