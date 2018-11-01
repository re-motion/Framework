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
  [DebuggerDisplay ("{FullName}, not introduced by {Implementer.FullName}")]
  public class NonInterfaceIntroductionDefinition : IVisitableDefinition
  {
    public NonInterfaceIntroductionDefinition (Type type, MixinDefinition implementer, bool explicitSuppression)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      InterfaceType = type;
      Implementer = implementer;
      IsExplicitlySuppressed = explicitSuppression;
    }

    public Type InterfaceType { get; private set; }
    public MixinDefinition Implementer { get; private set; }
    public bool IsExplicitlySuppressed { get; private set; }

    public bool IsShadowed
    {
      get { return !IsExplicitlySuppressed; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return InterfaceType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }
  }
}
