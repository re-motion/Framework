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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMember}")]
  public abstract class MemberIntroductionDefinitionBase<TMemberInfo, TMemberDefinition>: IMemberIntroductionDefinition where TMemberInfo : MemberInfo
      where TMemberDefinition : MemberDefinitionBase
  {
    private readonly InterfaceIntroductionDefinition _declaringInterface;
    private readonly TMemberInfo _interfaceMember;
    private readonly TMemberDefinition _implementingMember;
    private readonly MemberVisibility _visibility;

    protected MemberIntroductionDefinitionBase (
        InterfaceIntroductionDefinition declaringInterface, TMemberInfo interfaceMember, TMemberDefinition implementingMember, MemberVisibility visibility)
    {
      ArgumentUtility.CheckNotNull ("interfaceMember", interfaceMember);
      ArgumentUtility.CheckNotNull ("declaringInterface", declaringInterface);
      ArgumentUtility.CheckNotNull ("implementingMember", implementingMember);

      _declaringInterface = declaringInterface;
      _implementingMember = implementingMember;
      _interfaceMember = interfaceMember;
      _visibility = visibility;
    }

    public InterfaceIntroductionDefinition DeclaringInterface
    {
      get { return _declaringInterface; }
    }

    public string Name
    {
      get { return InterfaceMember.Name; }
    }

    public TMemberInfo InterfaceMember
    {
      get { return _interfaceMember; }
    }

    public TMemberDefinition ImplementingMember
    {
      get { return _implementingMember; }
    }

    public string FullName
    {
      get { return DeclaringInterface.FullName + "." + InterfaceMember.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return DeclaringInterface; }
    }

    public MemberVisibility Visibility
    {
      get { return _visibility; }
    }

    public abstract void Accept (IDefinitionVisitor visitor);
  }
}
