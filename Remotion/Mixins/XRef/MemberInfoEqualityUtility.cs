// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Reflection;

namespace Remotion.Mixins.XRef
{
  public class MemberInfoEqualityUtility
  {
    public static bool MemberEquals (object obj1, object obj2)
    {
      ArgumentUtility.CheckNotNull("obj1", obj1);
      ArgumentUtility.CheckNotNull("obj2", obj2);

      var memberInfo1 = obj1 as MemberInfo;
      var memberInfo2 = obj2 as MemberInfo;

      return memberInfo1 != null
             && memberInfo2 != null
             && memberInfo1.DeclaringType == memberInfo2.DeclaringType
             && memberInfo1.MetadataToken == memberInfo2.MetadataToken
          ;
    }
  }
}
