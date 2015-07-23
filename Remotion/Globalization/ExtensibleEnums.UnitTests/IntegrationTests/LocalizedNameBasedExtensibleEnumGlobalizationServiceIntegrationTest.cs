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
using Remotion.Globalization.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.ExtensibleEnums.UnitTests.IntegrationTests
{
  [TestFixture]
  public class LocalizedNameBasedExtensibleEnumGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetExtensibleEnumValueDisplayName_IntegrationTest ()
    {
      string resourceValue;

      var service = SafeServiceLocator.Current.GetInstance<IExtensibleEnumGlobalizationService>();

      using (new CultureScope ("it-IT", "en-US"))
      {
        Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.DarkRed(), out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The en-US Dark Red"));
        Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.DarkRed()), Is.EqualTo ("The en-US Dark Red"));
        Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.DarkRed()), Is.EqualTo ("The en-US Dark Red"));
        Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.DarkRed()), Is.True);

        Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.DarkBlue(), out resourceValue), Is.False);
        Assert.That (resourceValue, Is.Null);
        Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.DarkBlue()), Is.EqualTo ("DarkBlue"));
        Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.DarkBlue()), Is.Null);
        Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.DarkBlue()), Is.False);
      }

      using (new CultureScope ("it-IT", "de-AT"))
      {
        Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.DarkRed(), out resourceValue), Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Dark Red"));
        Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.DarkRed()), Is.EqualTo ("The Invariant Dark Red"));
        Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.DarkRed()), Is.EqualTo ("The Invariant Dark Red"));
        Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.DarkRed()), Is.True);

        Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.DarkBlue(), out resourceValue), Is.False);
        Assert.That (resourceValue, Is.Null);
        Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.DarkBlue()), Is.EqualTo ("DarkBlue"));
        Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.DarkBlue()), Is.Null);
        Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.DarkBlue()), Is.False);
      }
    }
  }
}