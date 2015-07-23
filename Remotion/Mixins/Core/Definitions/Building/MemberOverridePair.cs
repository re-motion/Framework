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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  /// <summary>
  /// Represents an overridden method together with its overrider.
  /// </summary>
  public struct MemberOverridePair<TMember>
      where TMember : MemberDefinitionBase
  {
    public MemberOverridePair (TMember baseMember, TMember overrider)
        : this()
    {
      ArgumentUtility.CheckNotNull ("baseMember", baseMember);
      ArgumentUtility.CheckNotNull ("overrider", overrider);

      BaseMember = baseMember;
      Overrider = overrider;
    }

    public TMember BaseMember { get; private set; }
    public TMember Overrider { get; private set; }
  }
}
