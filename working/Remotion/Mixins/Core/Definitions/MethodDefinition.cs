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
  public class MethodDefinition : MemberDefinitionBase
  {
    private readonly UniqueDefinitionCollection<Type, MethodDefinition> _overrides =
        new UniqueDefinitionCollection<Type, MethodDefinition> (m => m.DeclaringClass.Type);

    private MethodDefinition _base;

    public MethodDefinition (MethodInfo memberInfo, ClassDefinitionBase declaringClass)
        : base (memberInfo, declaringClass)
    {
    }

    public MethodInfo MethodInfo
    {
      get { return (MethodInfo) MemberInfo; }
    }

    public override MemberDefinitionBase BaseAsMember
    {
      get { return _base; }
      protected internal set
      {
        if (value == null || value is MethodDefinition)
          _base = (MethodDefinition) value;
        else
          throw new ArgumentException ("Base must be MethodDefinition or null.", "value");
      }
    }

    public MethodDefinition Base
    {
      get { return _base; }
      protected internal set { BaseAsMember = value; }
    }

    public bool IsAbstract
    {
      get { return MethodInfo.IsAbstract; }
    }

    public new UniqueDefinitionCollection<Type, MethodDefinition> Overrides
    {
      get { return _overrides; }
    }

    protected override IDefinitionCollection<Type, MemberDefinitionBase> GetInternalOverridesWrapper()
    {
      return new CovariantDefinitionCollectionWrapper<Type, MethodDefinition, MemberDefinitionBase>(_overrides);
    }

    internal override void AddOverride (MemberDefinitionBase member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var method = member as MethodDefinition;
      if (method == null)
      {
        string message = string.Format ("Member {0} cannot override method {1} - it is not a method.", member.FullName, FullName);
        throw new ArgumentException (message);
      }

      _overrides.Add (method);
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}
