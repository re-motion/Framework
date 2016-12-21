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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain
{
  [Uses (typeof (MixinAddingUnidirectionalRelation1))]
  [DBTable ("MixedDomains_TargetWithUnidirectionalMixin1")]
  [TestDomain]
  public class TargetClassWithUnidirectionalMixin1 : DomainObject, ISupportsGetObject
  {
    public static TargetClassWithUnidirectionalMixin1 NewObject ()
    {
      return NewObject<TargetClassWithUnidirectionalMixin1>();
    }

    [StorageClassNone]
    public Computer RedirectedComputer
    {
      [LinqPropertyRedirection (typeof (MixinAddingUnidirectionalRelation1), "Computer")] get { throw new NotImplementedException(); }
    }
  }
}