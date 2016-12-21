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
using Remotion.Development.UnitTesting;

namespace Remotion.Reflection.CodeGeneration.UnitTests.TestDomain
{
  public class ClassWithAllKindsOfMembers
  {
    public virtual void Method ()
    {
      if (Event != null)
        Event (null, null);
    }

    public virtual void MethodWithOutRef (out string outP, ref int refP)
    {
      outP = refP.ToString ();
      ++refP;
    }

    public virtual int Property
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    public virtual string this[int i]
    {
      get { return i.ToString(); }
      set { Dev.Null = value; }
    }

    public virtual event EventHandler Event;
  }
}
