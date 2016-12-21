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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A common base class for <see cref="IMixinContextSerializer"/> implementations.
  /// </summary>
  public class MixinContextSerializerBase : IMixinContextSerializer
  {
    private Type _mixinType;
    private MixinKind _mixinKind;
    private MemberVisibility _introducedMemberVisibility;
    private IEnumerable<Type> _explicitDependencies;
    private MixinContextOrigin _origin;

    public Type MixinType
    {
      get { return _mixinType; }
    }

    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    public MemberVisibility IntroducedMemberVisibility
    {
      get { return _introducedMemberVisibility; }
    }

    public IEnumerable<Type> ExplicitDependencies
    {
      get { return _explicitDependencies; }
    }

    public MixinContextOrigin Origin
    {
      get { return _origin; }
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _mixinType = mixinType;
    }

    public void AddMixinKind (MixinKind mixinKind)
    {
      _mixinKind = mixinKind;
    }

    public void AddIntroducedMemberVisibility (MemberVisibility introducedMemberVisibility)
    {
      _introducedMemberVisibility = introducedMemberVisibility;
    }

    public void AddExplicitDependencies (IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);

      _explicitDependencies = explicitDependencies;
    }

    public void AddOrigin (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);

      _origin = origin;
    }
  }
}