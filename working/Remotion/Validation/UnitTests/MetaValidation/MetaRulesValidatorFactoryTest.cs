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
using NUnit.Framework;
using Remotion.Validation.MetaValidation;
using Remotion.Validation.Rules;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.MetaValidation
{
  [TestFixture]
  public class MetaRulesValidatorFactoryTest
  {
    [Test]
    public void CreateMetaRuleValidator ()
    {
      var systemMetaRulesProviderFactoryStub = MockRepository.GenerateStub<ISystemMetaValidationRulesProviderFactory>();

      var factory = new MetaRulesValidatorFactory (systemMetaRulesProviderFactoryStub);
      
      var result = factory.CreateMetaRuleValidator (new IAddingComponentPropertyMetaValidationRule[0]);

      Assert.That (result, Is.TypeOf (typeof (MetaRulesValidator)));
    }
  }
}