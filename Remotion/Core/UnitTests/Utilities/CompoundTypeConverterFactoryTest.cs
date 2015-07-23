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
using System.ComponentModel;
using NUnit.Framework;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CompoundTypeConverterFactoryTest
  {
    [Test]
    public void Initialize ()
    {
      var factories = new[] { MockRepository.GenerateStub<ITypeConverterFactory>(), MockRepository.GenerateStub<ITypeConverterFactory>() };

      var compoundFactory = new CompoundTypeConverterFactory (factories);

      Assert.That (compoundFactory.TypeConverterFactories, Is.Not.SameAs (factories));
      Assert.That (compoundFactory.TypeConverterFactories, Is.EqualTo (factories));
    }

    [Test]
    public void CreateTypeConverterOrDefault ()
    {
      var intTypeConverter = new TypeConverter();
      var doubleTypeConverter = new TypeConverter();

      var factories = new[] { MockRepository.GenerateStub<ITypeConverterFactory>(), MockRepository.GenerateStub<ITypeConverterFactory>() };
      factories[0].Stub (_ => _.CreateTypeConverterOrDefault (typeof (int))).Return (intTypeConverter);
      factories[1].Stub (_ => _.CreateTypeConverterOrDefault (typeof (double))).Return (doubleTypeConverter);

      var compoundFactory = new CompoundTypeConverterFactory (factories);

      Assert.That (compoundFactory.CreateTypeConverterOrDefault (typeof (int)), Is.SameAs (intTypeConverter));
      Assert.That (compoundFactory.CreateTypeConverterOrDefault (typeof (double)), Is.SameAs (doubleTypeConverter));
      Assert.That (compoundFactory.CreateTypeConverterOrDefault (typeof (object)), Is.Null);
    }
  }
}