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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeIntroductionDefinitionBuilder
  {
    public AttributeIntroductionDefinitionBuilder (IAttributeIntroductionTarget target)
    {
      Target = target;
      Suppressors = new List<AttributeDefinition> ();
    }

    public List<AttributeDefinition> Suppressors { get; private set; }
    public IAttributeIntroductionTarget Target { get; private set; }

    public void AddPotentialSuppressors (IEnumerable<AttributeDefinition> attributes)
    {
      Suppressors.AddRange (attributes.Where (a => a.IsSuppressAttribute));
    }

    public void Apply (IAttributeIntroductionSource attributeSource)
    {
      foreach (AttributeDefinition attribute in attributeSource.CustomAttributes.Where (a => a.IsIntroducible))
      {
        AttributeDefinition suppressor = GetSuppressor (attribute);
        if (suppressor != null)
        {
          var suppressedDefinition = new SuppressedAttributeIntroductionDefinition (Target, attribute, suppressor);
          attributeSource.SuppressedAttributeIntroductions.Add (suppressedDefinition);
        }
        else if (IsImplicitlyExcluded (attribute))
        {
          var nonIntroductionDefinition = new NonAttributeIntroductionDefinition (attribute, false);
          attributeSource.NonAttributeIntroductions.Add (nonIntroductionDefinition);
        }
        else if (IsExplicitlyExcluded (attribute))
        {
          var nonIntroductionDefinition = new NonAttributeIntroductionDefinition (attribute, true);
          attributeSource.NonAttributeIntroductions.Add (nonIntroductionDefinition);
        }
        else
        {
          var introductionDefinition = new AttributeIntroductionDefinition (Target, attribute);
          Target.ReceivedAttributes.Add (introductionDefinition);
          attributeSource.AttributeIntroductions.Add (introductionDefinition);
        }
      }
    }

    public AttributeDefinition GetSuppressor (AttributeDefinition attribute)
    {
      ICustomAttributeProvider declaringEntity = attribute.DeclaringDefinition.CustomAttributeProvider;
      var suppressors = from s in Suppressors
                        let instance = (SuppressAttributesAttribute) s.Data.CreateInstance()
                        let suppressingEntity = s.DeclaringDefinition.CustomAttributeProvider
                        where instance.IsSuppressed (attribute.AttributeType, declaringEntity, suppressingEntity)
                        select s;
      return suppressors.FirstOrDefault ();
    }

    private bool IsImplicitlyExcluded (AttributeDefinition attribute)
    {
      return !AttributeUtility.IsAttributeAllowMultiple (attribute.AttributeType) && Target.CustomAttributes.ContainsKey (attribute.AttributeType);
    }

    private bool IsExplicitlyExcluded (AttributeDefinition attribute)
    {
      var excluders = from NonIntroducedAttribute excluder in attribute.DeclaringDefinition.CustomAttributeProvider.GetCustomAttributes (typeof (NonIntroducedAttribute), true)
                      where excluder.NonIntroducedType.IsAssignableFrom (attribute.AttributeType)
                      select attribute;
      return excluders.Any();
    }
  }
}
