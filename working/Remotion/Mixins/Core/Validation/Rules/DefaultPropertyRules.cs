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
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class DefaultPropertyRules : RuleSetBase
  {
    public override void Install (ValidatingVisitor visitor)
    {
      visitor.PropertyRules.Add (new DelegateValidationRule<PropertyDefinition> (NewMemberAddedByOverride));
    }

    [DelegateRuleDescription (Message = "A property override adds a new accessor method to the property; this method won't be accessible from the "
        + "mixed instance.")]
    private void NewMemberAddedByOverride (DelegateValidationRule<PropertyDefinition>.Args args)
    {
      SingleShould (args.Definition.Base != null ? (args.Definition.GetMethod == null || args.Definition.GetMethod.Base != null)
          && (args.Definition.SetMethod == null || args.Definition.SetMethod.Base != null)
          : true,
          args.Log,
          args.Self);
    }
  }
}
