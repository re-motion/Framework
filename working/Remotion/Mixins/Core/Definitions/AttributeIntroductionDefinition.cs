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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{FullName} introduced via {Attribute.DeclaringDefinition.FullName}")]
  public class AttributeIntroductionDefinition : IVisitableDefinition
  {
    private readonly IAttributeIntroductionTarget _target;
    private readonly AttributeDefinition _attribute;

    public AttributeIntroductionDefinition (IAttributeIntroductionTarget target, AttributeDefinition attribute)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      _target = target;
      _attribute = attribute;
    }

    public Type AttributeType
    {
      get { return _attribute.AttributeType; }
    }

    public string FullName
    {
      get { return _attribute.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return _attribute.Parent; }
    }

    public IAttributeIntroductionTarget Target
    {
      get { return _target; }
    }

    public AttributeDefinition Attribute
    {
      get { return _attribute; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);
    }
  }
}
