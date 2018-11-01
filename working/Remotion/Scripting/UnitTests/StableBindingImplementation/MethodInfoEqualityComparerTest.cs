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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class MethodInfoEqualityComparerTest
  {
    [Test]
    public void Equals_MethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (Proxied), "Sum");
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "Sum");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals(methodFromBaseType,method), Is.True);
    }


    public class CtorTest
    {
      public string Foo (string text)
      {
        return "CtorTest " + text;
      }
    }

    public interface ICtorTestFoo
    {
      string Foo (string text);
    }

    public class CtorTestChild : CtorTest, ICtorTestFoo
    {
      public new string Foo (string text)
      {
        return "CtorTestChild " + text;
      }
    }

    [Test]
    public void Ctor_Mask ()
    {
      var method = ScriptingHelper.GetInstanceMethod (typeof (CtorTest), "Foo", new[] { typeof (string) });
      var childMethod = ScriptingHelper.GetInstanceMethod (typeof (CtorTestChild), "Foo", new[] { typeof (string) });

      Assert.That (method.Attributes, Is.EqualTo (MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.HideBySig));
      const MethodAttributes childMethodAdditionalAttributes = MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.VtableLayoutMask;
      Assert.That (childMethod.Attributes, Is.EqualTo (method.Attributes | childMethodAdditionalAttributes));

      var comparerDefault = new MethodInfoEqualityComparer ();
      Assert.That (comparerDefault.Equals (method, childMethod), Is.False);

      var comparerNoVirtualNoFinal = new MethodInfoEqualityComparer (~childMethodAdditionalAttributes);
      Assert.That (comparerNoVirtualNoFinal.Equals (method, childMethod), Is.True);

      var comparerMethodFromBaseTypeAttributes = new MethodInfoEqualityComparer (method.Attributes);
      Assert.That (comparerMethodFromBaseTypeAttributes.Equals (method, childMethod), Is.True);
    }


    // Comparing MethodInfo between method and method hiding method through "new" works.
    [Test]
    public void Equals_MethodFromBaseTypeHiddenByInterfaceMethod ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (Proxied), "PrependName", new[] { typeof (string) });
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "PrependName", new[] { typeof (string) });
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));

      // Default comparison taking all method attributes into account fails, since IPrependName adds the Final, Virtual and 
      // VtableLayoutMask attributes.
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
      
      // Comparing ignoring method attributes coming from IPrependName works.
      var comparerNoVirtualNoFinal = new MethodInfoEqualityComparer (~(MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.VtableLayoutMask));
      Assert.That (comparerNoVirtualNoFinal.Equals (methodFromBaseType, method), Is.True);
    }


    [Test]
    public void Equals_InterfaceMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (Proxied), "GetName");
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "GetName");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_DifferingArgumentsMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChild), "BraKet", new[] { typeof (string), typeof (int) });
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "BraKet", new Type[0]);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }


    [Test]
    public void Equals_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (Proxied), "GenericToString");
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "GenericToString");
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 2);
      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_Differing_OverloadedGenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }

    [Test]
    public void Equals_GenericMethodFromBaseType_NonGenericOverloadInType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChild), "OverloadedGenericToString", new[] { typeof (int), typeof (int) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.False);
    }


    private class Test1
    {
      public string OverloadedGenericToString<Tx> (Tx tx)
      {
        return "Test1:" + tx;
      }
      
      public string OverloadedGenericToString<Tx, Ty> (Tx tx, Ty ty)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx,Ty> (string s, Tx tx, object o, Ty ty)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }

      public string ComplexGenericArgumentTest<Tx, Ty> (Dictionary<Tx,Ty> d)
      {
        return "Test1.ComplexGenericArgumentTest:" + d;
      }
    }

    private class Test2
    {
      public string OverloadedGenericToString<Tx, Ty> (Ty ty, Tx tx)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx, Ty> (string s, Tx tx, object o, Ty ty)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }
    }

    private class Test3
    {
      public string OverloadedGenericToString<Tx, Ty> (Ty ty, Tx tx)
      {
        return "Test1.OverloadedGenericToString:" + tx + ty;
      }

      public string MixedArgumentsTest<Tx, Ty> (string s, Ty ty, object o, Tx tx)
      {
        return "Test1.MixedArgumentsTest:" + tx + s + o + ty;
      }
    }

    [Test]
    public void Equals_GenericMethodFromNonRelatedTypes ()
    {
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Proxied), "OverloadedGenericToString", 2);
      var methodWithSameSignature = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test1), "OverloadedGenericToString", 2);
      var methodWithSwappedGenericArguments = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test2), "OverloadedGenericToString", 2);

      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSameSignature), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSwappedGenericArguments), Is.False);
    }

    [Test]
    public void Equals_MixedArgumentGenericMethodFromNonRelatedTypes ()
    {
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test1), "MixedArgumentsTest", 2);
      var methodWithSameSignature = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test2), "MixedArgumentsTest", 2);
      var methodWithSwappedGenericArguments = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (Test3), "MixedArgumentsTest", 2);

      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSameSignature), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (method, methodWithSwappedGenericArguments), Is.False);
    }


 

    [Test]
    public void Equals_GenericClass_MethodWithGenericClassArgumentsFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      var method = ScriptingHelper.GetInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    [Test]
    public void Equals_GenericClass_GenericMethodFromBaseType ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      var method = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, method), Is.True);
    }

    // FAILURE: Comparing MethodInfo between generic method and generic method hiding method through "new" does not work !
    [Test]
    [Explicit]
    public void Equals_GenericClass_GenericMethodFromBaseType2 ()
    {
      var methodFromBaseType = ScriptingHelper.GetAnyGenericInstanceMethod (typeof (ProxiedChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
      var methods = ScriptingHelper.GetAnyGenericInstanceMethodArray (typeof (ProxiedChildChildGeneric<int, string>), "ProxiedChildGenericToString", 2);
      Assert.That (methods.Length, Is.EqualTo (2));

      Console.WriteLine (methods[0].Name);
      Console.WriteLine (methods[1].Name);
      Console.WriteLine (methodFromBaseType.Name);

      Console.WriteLine (methods[0].ReturnType);
      Console.WriteLine (methods[1].ReturnType);
      Console.WriteLine (methodFromBaseType.ReturnType);

      Console.WriteLine (methods[0].Attributes);
      Console.WriteLine (methods[1].Attributes);
      Console.WriteLine (methodFromBaseType.Attributes);

      Console.WriteLine ("{" + string.Join (",", methods[0].GetParameters().Select (pi => pi.Attributes)) + "}");
      Console.WriteLine ("{" + string.Join (",", methods[1].GetParameters().Select (pi => pi.Attributes)) + "}");
      Console.WriteLine ("{" + string.Join (",", methodFromBaseType.GetParameters().Select (pi => pi.Attributes)) + "}");

      Console.WriteLine ("{" + string.Join (",", methods[0].GetParameters().Select (pi => pi.ParameterType)) + "}");
      Console.WriteLine ("{" + string.Join (",", methods[1].GetParameters().Select (pi => pi.ParameterType)) + "}");
      Console.WriteLine ("{" + string.Join (",", methodFromBaseType.GetParameters().Select (pi => pi.ParameterType)) + "}");

      var a0 = methods[0].GetParameters()[2];
      var a1 = methods[1].GetParameters()[2];
      var ax = methodFromBaseType.GetParameters()[2];

      var x = methods[0].GetParameters()[2];

      Assert.That (methodFromBaseType, Is.Not.EqualTo (methods[0]));
      Assert.That (methodFromBaseType, Is.Not.EqualTo (methods[1]));
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[1]), Is.True);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromBaseType, methods[0]), Is.True);
    }

  }

 
}
