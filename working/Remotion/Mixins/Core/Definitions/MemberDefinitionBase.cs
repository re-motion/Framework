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
  [DebuggerDisplay ("{MemberInfo}, DeclaringClass = {DeclaringClass.Type}")]
  public abstract class MemberDefinitionBase : IAttributeIntroductionTarget, IAttributeIntroductionSource
  {
    private IVisitableDefinition _parent;

    private IDefinitionCollection<Type, MemberDefinitionBase> _internalOverridesWrapper = null;

    protected MemberDefinitionBase (MemberInfo memberInfo, ClassDefinitionBase declaringClass)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("declaringClass", declaringClass);

      SuppressedReceivedAttributes = new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (a => a.AttributeType);
      ReceivedAttributes = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);
      CustomAttributes = new MultiDefinitionCollection<Type, AttributeDefinition> (a => a.AttributeType);

      SuppressedAttributeIntroductions = new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (a => a.AttributeType);
      NonAttributeIntroductions = new MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> (a => a.AttributeType);
      AttributeIntroductions = new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);

      MemberInfo = memberInfo;
      DeclaringClass = declaringClass;
      _parent = declaringClass;
    }

    public MultiDefinitionCollection<Type, AttributeDefinition> CustomAttributes { get; private set; }
    
    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> ReceivedAttributes { get; private set; }
    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedReceivedAttributes { get; private set; }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> AttributeIntroductions { get; private set; }
    public MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> NonAttributeIntroductions { get; private set; }
    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedAttributeIntroductions { get; private set; }

    public MemberInfo MemberInfo { get; private set; }
    public ClassDefinitionBase DeclaringClass { get; private set; }

    public MemberTypes MemberType
    {
      get { return MemberInfo.MemberType; }
    }

    public bool IsProperty
    {
      get { return MemberInfo.MemberType == MemberTypes.Property; }
    }

    public bool IsMethod
    {
      get { return MemberInfo.MemberType == MemberTypes.Method; }
    }

    public bool IsEvent
    {
      get { return MemberInfo.MemberType == MemberTypes.Event; }
    }

    public string Name
    {
      get { return MemberInfo.Name; }
    }

    public string FullName
    {
      get { return MemberInfo.DeclaringType.FullName + "." + Name; }
    }

    public abstract MemberDefinitionBase BaseAsMember { get; protected internal set; }

    public IVisitableDefinition Parent
    {
      get { return _parent; }
      internal set { _parent = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public ICustomAttributeProvider CustomAttributeProvider
    {
      get { return MemberInfo; }
    }

    public IDefinitionCollection<Type, MemberDefinitionBase> Overrides
    {
      get
      {
        if (_internalOverridesWrapper == null)
        {
          _internalOverridesWrapper = GetInternalOverridesWrapper();
        }
        return _internalOverridesWrapper;
      }
    }

    protected abstract IDefinitionCollection<Type, MemberDefinitionBase> GetInternalOverridesWrapper();

    internal abstract void AddOverride (MemberDefinitionBase member);

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ChildSpecificAccept (visitor);

      CustomAttributes.Accept (visitor);
      AttributeIntroductions.Accept (visitor);
      NonAttributeIntroductions.Accept (visitor);

      Assertion.IsTrue (SuppressedAttributeIntroductions.Count == 0, "Must be updated once we support suppressing attributes on members");
    }

    protected abstract void ChildSpecificAccept (IDefinitionVisitor visitor);
  }
}
