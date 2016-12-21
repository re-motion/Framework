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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.UnitTests.Core.Validation
{
  [TestFixture]
  public class DelegateRuleDescriptionTest
  {
    private void NonDescribedSampleRule (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription]
    private void DescribedSampleRule_NoChanges (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [DelegateRuleDescription (RuleName = "Fritz", Message = "Echo?")]
    private void DescribedSampleRule_Changes (DelegateValidationRule<TargetClassDefinition>.Args args)
    {
    }

    [Test]
    public void DefaultDescription ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (NonDescribedSampleRule);
      Assert.That (rule.RuleName, Is.EqualTo ("Remotion.Mixins.UnitTests.Core.Validation.DelegateRuleDescriptionTest.NonDescribedSampleRule"));
      Assert.That (rule.Message, Is.EqualTo ("Non described sample rule"));
    }

    [Test]
    public void DescriptionAttribute_NoChanges ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_NoChanges);
      Assert.That (rule.RuleName, Is.EqualTo ("Remotion.Mixins.UnitTests.Core.Validation.DelegateRuleDescriptionTest.DescribedSampleRule_NoChanges"));
      Assert.That (rule.Message, Is.EqualTo ("Described sample rule_ no changes"));
    }

    [Test]
    public void DescriptionAttribute_Changes ()
    {
      IValidationRule rule = new DelegateValidationRule<TargetClassDefinition> (DescribedSampleRule_Changes);
      Assert.That (rule.RuleName, Is.EqualTo ("Fritz"));
      Assert.That (rule.Message, Is.EqualTo ("Echo?"));
    }
  }
}
