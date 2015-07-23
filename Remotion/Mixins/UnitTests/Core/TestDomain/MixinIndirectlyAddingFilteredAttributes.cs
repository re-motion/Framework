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

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  [CopyCustomAttributes (typeof (AttributeSource), typeof (AttributeWithParameters2), typeof (AttributeWithParameters3))]
  public class MixinIndirectlyAddingFilteredAttributes : Mixin<object>
  {
    [AttributeWithParameters (1, "one", Property = 4, Field = 5)]
    [AttributeWithParameters2 (1, "one", Property = 4, Field = 5)]
    [AttributeWithParameters2 (1, "two", Property = 4, Field = 5)]
    [AttributeWithParameters3 (1, "two", Property = 4, Field = 5)]
    public class AttributeSource
    {
      [AttributeWithParameters (4)]
      [AttributeWithParameters2 (4)]
      [AttributeWithParameters2 (4)]
      [AttributeWithParameters3 (4)]
      public void AttributeSourceMethod ()
      {
      }
    }

    [OverrideTarget]
    [CopyCustomAttributes (typeof (AttributeSource), "AttributeSourceMethod", typeof (AttributeWithParameters2), typeof (AttributeWithParameters3))]
    public new string ToString ()
    {
      return "";
    }
  }
}
