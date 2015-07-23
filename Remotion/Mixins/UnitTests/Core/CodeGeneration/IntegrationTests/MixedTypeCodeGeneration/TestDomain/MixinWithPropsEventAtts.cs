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

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain
{
  [Replicatable (4)]
  public class MixinWithPropsEventAtts : IMixinWithPropsEventsAtts
  {
    [Replicatable ("bla")]
    public int Property
    {
      [Replicatable (5, Named2 = 1.0)]
      get;
      [Replicatable (5, Named2 = 2.0)]
      set;
    }

    [Replicatable ("blo")]
    public event EventHandler Event
    {
      [Replicatable (1)]
      add { }
      [Replicatable (2)]
      remove { }
    }
  }
}
