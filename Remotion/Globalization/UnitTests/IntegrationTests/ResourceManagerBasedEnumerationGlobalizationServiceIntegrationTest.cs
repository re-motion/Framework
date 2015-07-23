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
  public class ResourceManagerBasedEnumerationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetEnumerationValueDisplayName_IntegrationTest ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>();
      string resourceValue;

      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Value 1"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.Value1), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.Value2, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Value 2"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.Value2), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.ValueWithoutResource), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.False);

        Assert.That (service.TryGetEnumerationValueDisplayName ((EnumWithResources) 100, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName ((EnumWithResources) 100), Is.EqualTo ("100"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault ((EnumWithResources) 100), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName ((EnumWithResources) 100), Is.False);
      }

      var culture = new CultureInfo ("de-AT");
      using (new CultureScope (culture, culture))
      {
        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.Value1, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Wert 1"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Wert 1"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.Value1), Is.EqualTo ("Wert 1"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.Value1), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.Value2, out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Wert 2"));
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Wert 2"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.Value2), Is.EqualTo ("Wert 2"));
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.Value2), Is.True);

        Assert.That (service.TryGetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault (EnumWithResources.ValueWithoutResource), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.False);

        Assert.That (service.TryGetEnumerationValueDisplayName ((EnumWithResources) 100, out resourceValue), Is.False);
        Assert.That (service.GetEnumerationValueDisplayName ((EnumWithResources) 100), Is.EqualTo ("100"));
        Assert.That (service.GetEnumerationValueDisplayNameOrDefault ((EnumWithResources) 100), Is.Null);
        Assert.That (service.ContainsEnumerationValueDisplayName ((EnumWithResources) 100), Is.False);
      }
    }
  }
}