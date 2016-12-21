﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class IEnumerationGlobalizationServiceTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var service = _serviceLocator.GetInstance<IEnumerationGlobalizationService> ();

      Assert.That (service, Is.Not.Null);
      Assert.That (service, Is.TypeOf (typeof (CompoundEnumerationGlobalizationService)));
      Assert.That (
          ((CompoundEnumerationGlobalizationService) service).EnumerationGlobalizationServices.Select (s => s.GetType()),
          Is.EqualTo (
              new[]
              {
                  typeof (ResourceManagerBasedEnumerationGlobalizationService),
                  typeof (MultiLingualNameBasedEnumerationGlobalizationService)
              }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var service1 = _serviceLocator.GetInstance<IEnumerationGlobalizationService> ();
      var service2 = _serviceLocator.GetInstance<IEnumerationGlobalizationService> ();

      Assert.That (service1, Is.SameAs (service2));
    }
  }
}