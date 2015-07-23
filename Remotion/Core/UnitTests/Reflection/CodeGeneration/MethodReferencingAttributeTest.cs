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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Reflection.CodeGeneration.TestDomain;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class MethodReferencingAttributeTest
  {
    [Test]
    public void ResolveReferencedMethod()
    {
      var wrappedMethod = typeof (DateTime).GetMethod ("get_Now");
      var attribute = new TestMethodReferencingAttribute (typeof (DateTime), "get_Now", wrappedMethod.ToString());
      
      var resolvedMethod = attribute.ResolveReferencedMethod ();
      
      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveReferencedMethod_GenType_RefType ()
    {
      MethodInfo wrappedMethod = typeof (List<string>).GetMethod ("Add");
      var attribute = new TestMethodReferencingAttribute (typeof (List<string>), "Add", wrappedMethod.ToString());
      
      var resolvedMethod = attribute.ResolveReferencedMethod ();

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveReferencedMethod_GenType_ValueType ()
    {
      MethodInfo wrappedMethod = typeof (List<int>).GetMethod ("Add");
      var attribute = new TestMethodReferencingAttribute (typeof (List<int>), "Add", wrappedMethod.ToString());
      
      var resolvedMethod = attribute.ResolveReferencedMethod ();

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveReferencedMethod_GenMethod ()
    {
      MethodInfo wrappedMethod = typeof (ClassWithConstrainedGenericMethod).GetMethod ("GenericMethod");
      var attribute = new TestMethodReferencingAttribute (typeof (ClassWithConstrainedGenericMethod), "GenericMethod", wrappedMethod.ToString());
      
      var resolvedMethod = attribute.ResolveReferencedMethod ();

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }

    [Test]
    public void ResolveReferencedMethod_GenMethod_GenType ()
    {
      var genericType = typeof (GenericClassWithGenericMethod<IConvertible, List<string>, DateTime, object, IConvertible,  List<List<IConvertible[]>>>);
      MethodInfo wrappedMethod = genericType.GetMethod ("GenericMethod");
      var attribute = new TestMethodReferencingAttribute (genericType, "GenericMethod", wrappedMethod.ToString());
      
      var resolvedMethod = attribute.ResolveReferencedMethod ();

      Assert.That (resolvedMethod, Is.EqualTo (wrappedMethod));
    }
  }
}
