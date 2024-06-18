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
using System.IO;
using System.Reflection;
using Remotion.Mixins.CrossReferencer.Reflectors;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Helpers
{
  public static class RemotionReflectorFactory
  {
    public static IRemotionReflector GetRemotionReflection ()
    {
      // TODO Replace with mock if possible
      return new RemotionReflector ("Remotion", new Version ("1.11.20"), new[] { Assembly.LoadFile (Path.GetFullPath ("Remotion.Mixins.CrossReferencer.Reflectors.dll")) }, ".");
    }
  }
}
