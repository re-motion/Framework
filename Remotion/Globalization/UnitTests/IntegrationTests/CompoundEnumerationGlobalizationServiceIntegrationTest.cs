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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class CompoundEnumerationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IEnumerationGlobalizationService>();

      string resourceValue;
      Assert.That (
          service.TryGetEnumerationValueDisplayName (
              EnumWithResourcesAndAttribute.ValueWithAttribute,
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Attribute"));

      Assert.That (
          service.TryGetEnumerationValueDisplayName (
              EnumWithResourcesAndAttribute.ValueWithResource,
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Resource"));

      Assert.That (
          service.TryGetEnumerationValueDisplayName (
              EnumWithResourcesAndAttribute.ValueWithResourceAndAttribute,
              out resourceValue),
          Is.True);
      Assert.That (resourceValue, Is.EqualTo ("Resource"));

      Assert.That (
          service.TryGetEnumerationValueDisplayName (
              EnumWithResourcesAndAttribute.ValueWithoutResourceOrAttribute,
              out resourceValue),
          Is.False);
      Assert.That (resourceValue, Is.Null);
    }
  }
}