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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class CompoundMemberInformationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithResources)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Resource-based Type ID"));
      }
    }

    [Test]
    public void TryGetPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithResources)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Resource-based Property ID"));
      }
    }
  }
}