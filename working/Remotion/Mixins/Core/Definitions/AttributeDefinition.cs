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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay("{AttributeType}")]
  public class AttributeDefinition: IVisitableDefinition
  {
    private readonly IAttributableDefinition _declaringDefinition;
    private readonly ICustomAttributeData _data;
    private readonly bool _isCopyTemplate;

    public AttributeDefinition (IAttributableDefinition declaringDefinition, ICustomAttributeData data, bool isCopyTemplate)
    {
      _declaringDefinition = declaringDefinition;
      _data = data;
      _isCopyTemplate = isCopyTemplate;
    }

    public ICustomAttributeData Data
    {
      get { return _data;}
    }

    public Type AttributeType
    {
      get { return _data.Constructor.DeclaringType; }
    }

    public bool IsIntroducible
    {
      get { return AttributeUtility.IsAttributeInherited (AttributeType) || IsCopyTemplate; }
    }

    public bool IsSuppressAttribute
    {
      get { return typeof (SuppressAttributesAttribute).IsAssignableFrom (AttributeType); }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get
      {
        var declaringType = _data.Constructor.DeclaringType;
        Assertion.IsNotNull (declaringType);
        return declaringType.FullName;
      }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringDefinition as IVisitableDefinition; }
    }

    public IAttributableDefinition DeclaringDefinition
    {
      get { return _declaringDefinition; }
    }

    public bool IsCopyTemplate
    {
      get { return _isCopyTemplate; }
    }
  }
}
