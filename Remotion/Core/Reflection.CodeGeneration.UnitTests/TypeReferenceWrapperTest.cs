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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class TypeReferenceWrapperTest
  {
    [Test]
    public void TypeAndOwner ()
    {
      var ownerReference = new Mock<Reference> (MockBehavior.Strict);
      var wrappedReference = new Mock<Reference> (MockBehavior.Strict, ownerReference.Object);

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference.Object, typeof (int));
      Assert.That (tr.Type, Is.EqualTo (typeof (int)));
      Assert.That (tr.OwnerReference, Is.SameAs (ownerReference.Object));
    }

    [Test]
    public void LoadReference ()
    {
      var wrappedReference = new Mock<Reference> (MockBehavior.Strict);
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly().ManifestModule).GetILGenerator();

      // expect
      wrappedReference.Setup (_ => _.LoadReference (gen)).Verifiable();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference.Object, typeof (int));
      tr.LoadReference (gen);

      wrappedReference.Verify();
    }

    [Test]
    public void StoreReference ()
    {
      var wrappedReference = new Mock<Reference> (MockBehavior.Strict);
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly ().ManifestModule).GetILGenerator ();

      // expect
      wrappedReference.Setup (_ => _.StoreReference (gen)).Verifiable();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference.Object, typeof (int));
      tr.StoreReference (gen);

      wrappedReference.Verify();
    }

    [Test]
    public void LoadAddressReference ()
    {
      var wrappedReference = new Mock<Reference> (MockBehavior.Strict);
      ILGenerator gen = new DynamicMethod ("Foo", typeof (void), Type.EmptyTypes, AssemblyBuilder.GetExecutingAssembly ().ManifestModule).GetILGenerator ();

      // expect
      wrappedReference.Setup (_ => _.LoadAddressOfReference (gen)).Verifiable();

      TypeReferenceWrapper tr = new TypeReferenceWrapper (wrappedReference.Object, typeof (int));
      tr.LoadAddressOfReference (gen);

      wrappedReference.Verify();
    }
  }
}
