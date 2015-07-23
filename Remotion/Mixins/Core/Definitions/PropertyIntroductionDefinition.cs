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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class PropertyIntroductionDefinition : MemberIntroductionDefinitionBase<PropertyInfo, PropertyDefinition>
  {
    private readonly bool _introducesGetMethod;
    private readonly bool _introducesSetMethod;

    public PropertyIntroductionDefinition (InterfaceIntroductionDefinition declaringInterface, PropertyInfo interfaceMember, PropertyDefinition implementingMember, MemberVisibility visibility)
        : base (declaringInterface, interfaceMember, implementingMember, visibility)
    {
      _introducesGetMethod = interfaceMember.GetGetMethod() != null;
      _introducesSetMethod = interfaceMember.GetSetMethod () != null;
    }

    public bool IntroducesGetMethod
    {
      get { return _introducesGetMethod; }
    }

    public bool IntroducesSetMethod
    {
      get { return _introducesSetMethod; }
    }

    public override void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
