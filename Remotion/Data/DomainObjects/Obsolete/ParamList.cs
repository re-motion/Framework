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
using TypePipeParamList = Remotion.TypePipe.ParamList;

// ReSharper disable once CheckNamespace
namespace Remotion.Reflection
{
  /// <summary>
  /// Redirect for the <see cref="TypePipeParamList"/> located in the <b>Remotion.TypePipe</b> assembly.
  /// </summary>
  [Obsolete ("Moved to Remotion.TypePipe.dll. Add dependency on Remotion.TypePipe.dll and update using-directive from 'Remotion.Reflection' to 'Remotion.TypePipe'. (Version 1.15.7.0)")]
  public static class ParamList
  {
    public static TypePipeParamList Empty
    {
      get { return TypePipeParamList.Empty; }
    }

    public static TypePipeParamList Create<A1> (A1 a1)
    {
      return TypePipeParamList.Create (a1);
    }

    public static TypePipeParamList Create<A1, A2> (A1 a1, A2 a2)
    {
      return TypePipeParamList.Create (a1, a2);
    }

    public static TypePipeParamList Create<A1, A2, A3> (A1 a1, A2 a2, A3 a3)
    {
      return TypePipeParamList.Create (a1, a2, a3);
    }

    public static TypePipeParamList Create<A1, A2, A3, A4> (A1 a1, A2 a2, A3 a3, A4 a4)
    {
      return TypePipeParamList.Create (a1, a2, a3, a4);
    }

    public static TypePipeParamList Create<A1, A2, A3, A4, A5> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
    {
      return TypePipeParamList.Create (a1, a2, a3, a4, a5);
    }

    public static TypePipeParamList Create<A1, A2, A3, A4, A5, A6> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
    {
      return TypePipeParamList.Create (a1, a2, a3, a4, a5, a6);
    }

    public static TypePipeParamList Create<A1, A2, A3, A4, A5, A6, A7> (A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
    {
      return TypePipeParamList.Create (a1, a2, a3, a4, a5, a6, a7);
    }
  }
}