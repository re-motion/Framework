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
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using NUnit.Framework;
using Rhino.Mocks;
using MixinXRef.UnitTests.TestDomain.Reflection;
using System.Runtime.InteropServices;
using MixinXRef.UnitTests.Stub;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class RemotionReflectorTest
  {
    [Test]
    public void CompatibleMethodCall ()
    {
      var assembly1 = MockRepository.GenerateStub<_Assembly> ();
      assembly1.Stub (a => a.GetExportedTypes ())
        .Return (new[] { typeof (ClassImplementingRemotionReflectorV1_13_32), typeof (ClassImplementingRemotionReflectorV1_13_141) });

      var reflector = new RemotionReflectorStub ("Remotion", new Version (1, 13, 150), new[] { assembly1 }, ".");
      reflector.CallMethod (typeof (IRemotionReflector).GetMethod ("IsInfrastructureType", new[] { typeof (Type) }));

      Assert.That (reflector.ReceivedReflector, Is.InstanceOfType (typeof (ClassImplementingRemotionReflectorV1_13_141)));
    }

    [Test]
    public void IncompatibleMethodCall ()
    {
      var assembly1 = MockRepository.GenerateStub<_Assembly> ();
      assembly1.Stub (a => a.GetExportedTypes ()).Return (new[] { typeof (ClassImplementingRemotionReflectorV1_13_32) });

      try
      {
        var remotionReflector = new RemotionReflector ("Remotion", new Version (1, 13, 32), new[] { assembly1 }, ".");
        remotionReflector.IsInheritedFromMixin (typeof (int));

        Assert.Fail ("Expected exception {0} was not thrown", typeof (NotSupportedException));
      }
      catch (NotSupportedException)
      { }
    }

    [Test]
    public void MissingMethodCall ()
    {
      var assembly1 = MockRepository.GenerateStub<_Assembly> ();
      assembly1.Stub (a => a.GetExportedTypes ()).Return (new[] { typeof (ClassImplementingRemotionReflectorV1_13_32) });

      try
      {
        var remotionReflector = new RemotionReflector ("Remotion", new Version (1, 13, 32), new[] { assembly1 }, ".");
        remotionReflector.IsInheritedFromMixin (typeof (int));

        Assert.Fail ("Expected exception {0} was not thrown", typeof (NotSupportedException));
      }
      catch (NotSupportedException)
      { }
    }

    [Test]
    public void AmbiguousMatch ()
    {
      var assembly1 = MockRepository.GenerateStub<_Assembly> ();
      assembly1.Stub (a => a.GetExportedTypes ()).Return (new[] { typeof (ClassWithAmbiguousMethod1), typeof (ClassWithAmbiguousMethod2) });

      try
      {
        var remotionReflector = new RemotionReflector ("Remotion", new Version (1, 11, 20), new[] { assembly1 }, ".");
        remotionReflector.IsInfrastructureType (typeof (int));

        Assert.Fail ("Expected exception {0} was not thrown", typeof (AmbiguousMatchException));
      }
      catch (AmbiguousMatchException)
      { }
    }

    [Test]
    public void AmbiguousMatchIrrelevant ()
    {
      var assembly1 = MockRepository.GenerateStub<_Assembly> ();
      assembly1.Stub (a => a.GetExportedTypes ()).Return (new[] { typeof (ClassImplementingRemotionReflectorV1_13_32), typeof (ClassWithAmbiguousMethod1), typeof (ClassWithAmbiguousMethod2) });

      var reflector = new RemotionReflectorStub ("Remotion", new Version (1, 13, 32), new[] { assembly1 }, ".");
      reflector.CallMethod (typeof (IRemotionReflector).GetMethod ("IsInfrastructureType", new[] { typeof (Type) }));

      Assert.That (reflector.ReceivedReflector, Is.InstanceOfType (typeof (ClassImplementingRemotionReflectorV1_13_32)));
    }
  }
}
