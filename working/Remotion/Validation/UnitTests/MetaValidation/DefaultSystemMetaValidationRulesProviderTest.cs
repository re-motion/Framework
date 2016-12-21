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
using Remotion.Reflection;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class DefaultSystemMetaValidationRulesProviderTest
  {
    private DefaultSystemMetaValidationRulesProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new DefaultSystemMetaValidationRulesProvider (PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName")));
    }

    [Test]
    public void GetSystemMetaValidationRules ()
    {
      var result = _provider.GetSystemMetaValidationRules();

      Assert.That (result.Any(), Is.True);
    }
  }
}