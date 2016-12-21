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
using System.Diagnostics;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{FullName} introduced via {Implementer.FullName}")]
  public class InterfaceIntroductionDefinition : IVisitableDefinition
  {
    private readonly UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> _introducedMethods =
        new UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> (m => m.InterfaceMember);
    private readonly UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> _introducedProperties =
        new UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> (m => m.InterfaceMember);
    private readonly UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> _introducedEvents =
        new UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> (m => m.InterfaceMember);

    public InterfaceIntroductionDefinition (Type type, MixinDefinition implementer)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      InterfaceType = type;
      Implementer = implementer;
    }

    public Type InterfaceType { get; private set; }
    public MixinDefinition Implementer { get; private set; }

    public string FullName
    {
      get { return InterfaceType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return Implementer.TargetClass; }
    }

    public UniqueDefinitionCollection<EventInfo, EventIntroductionDefinition> IntroducedEvents
    {
      get { return _introducedEvents; }
    }

    public UniqueDefinitionCollection<PropertyInfo, PropertyIntroductionDefinition> IntroducedProperties
    {
      get { return _introducedProperties; }
    }

    public UniqueDefinitionCollection<MethodInfo, MethodIntroductionDefinition> IntroducedMethods
    {
      get { return _introducedMethods; }
    }

    public IEnumerable<IMemberIntroductionDefinition> GetIntroducedMembers ()
    {
      foreach (MethodIntroductionDefinition introducedMethod in _introducedMethods)
        yield return introducedMethod;
      foreach (PropertyIntroductionDefinition introducedProperty in _introducedProperties)
        yield return introducedProperty;
      foreach (EventIntroductionDefinition introducedEvent in _introducedEvents)
        yield return introducedEvent;
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
      _introducedMethods.Accept (visitor);
      _introducedProperties.Accept (visitor);
      _introducedEvents.Accept (visitor);
    }
  }
}
