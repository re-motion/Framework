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

namespace Remotion.Globalization.ExtensibleEnums.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ResourceManagerBasedExtensibleEnumGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetExtensibleEnumValueDisplayName_IntegrationTest ()
    {
      string resourceValue;

      var service = SafeServiceLocator.Current.GetInstance<IExtensibleEnumGlobalizationService> ();

      Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.Red (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Rot"));
      Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.Red ()), Is.EqualTo ("Rot"));
      Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.Red ()), Is.EqualTo ("Rot"));
      Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.Red ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.Green (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Grün"));
      Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.Green ()), Is.EqualTo ("Grün"));
      Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.Green ()), Is.EqualTo ("Grün"));
      Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.Green ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.RedMetallic (), out resourceValue), Is.False);
      Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.RedMetallic ()), Is.EqualTo ("RedMetallic"));
      Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.RedMetallic ()), Is.Null);
      Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.RedMetallic ()), Is.False);

      Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.LightRed (), out resourceValue), Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Hellrot"));
      Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.LightRed ()), Is.EqualTo ("Hellrot"));
      Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.LightRed ()), Is.EqualTo ("Hellrot"));
      Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.LightRed ()), Is.True);

      Assert.That (service.TryGetExtensibleEnumValueDisplayName (Color.Values.LightBlue (), out resourceValue), Is.False);
      Assert.That (resourceValue, Is.Null);
      Assert.That (service.GetExtensibleEnumValueDisplayName (Color.Values.LightBlue ()), Is.EqualTo ("LightBlue"));
      Assert.That (service.GetExtensibleEnumValueDisplayNameOrDefault (Color.Values.LightBlue ()), Is.Null);
      Assert.That (service.ContainsExtensibleEnumValueDisplayName (Color.Values.LightBlue ()), Is.False);
    }
  }
}