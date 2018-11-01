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
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class ForwardingProxyBuilderTest
  {
    private ModuleScope _moduleScope;
    private const BindingFlags _nonPublicInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;


    [TestFixtureTearDown]
    public void TestFixtureTearDown ()
    {
      if (_moduleScope != null)
      {
        if (_moduleScope.StrongNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (true);
        if (_moduleScope.WeakNamedModule != null)
          SaveAndVerifyModuleScopeAssembly (false);
      }
    }


    public ModuleScope ModuleScope
    {
      get
      {
        if (_moduleScope == null)
        {
          const string name = "Remotion.Scripting.CodeGeneration.Generated.ForwardingProxyBuilderTest";
          const string nameSigned = name + ".Signed";
          const string nameUnsigned = name + ".Unsigned";
          const string ext = ".dll";
          _moduleScope = new ModuleScope (true, false, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
        }
        return _moduleScope;
      }
    }


    [Test]
    public void BuildProxyType ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("ForwardingProxyBuilder_BuildProxyTypeTest", ModuleScope, typeof (Proxied), new Type[0]);
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      FieldInfo proxiedFieldInfo = proxy.GetType ().GetField ("_proxied", _nonPublicInstanceFlags);
      Assert.That (proxiedFieldInfo.GetValue (proxy), Is.EqualTo (proxied));
      Assert.That (proxiedFieldInfo.IsInitOnly, Is.False);
      Assert.That (proxiedFieldInfo.IsPrivate, Is.True);
    }


    [Test]
    public void AddForwardingExplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingExplicitInterfaceMethod",
        ModuleScope, typeof (ProxiedChild), new[] { typeof (IAmbigous1) });
      
      var methodInfo = typeof (IAmbigous1).GetMethod ("StringTimes");
      proxyBuilder.AddForwardingExplicitInterfaceMethod (methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChild ();
      object proxy = proxyBuilder.CreateInstance (proxied);

      Assert.That (((IAmbigous1) proxied).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
      Assert.That (((IAmbigous1) proxy).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
    }


    [Test]
    public void AddForwardingImplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingImplicitInterfaceMethod",
        ModuleScope, typeof (Proxied), new[] { typeof (IProxiedGetName) });
      proxyBuilder.AddForwardingImplicitInterfaceMethod (typeof (IProxiedGetName).GetMethod ("GetName"));
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxy.GetType ().GetInterfaces (), Is.EquivalentTo ((new[] { typeof (IProxiedGetName), typeof(IProxy) })));
      Assert.That (((IProxiedGetName) proxy).GetName (), Is.EqualTo ("Implementer.IProxiedGetName"));
      Assert.That (proxy.GetType ().GetMethod ("GetName").Invoke (proxy, new object[0]), Is.EqualTo ("Implementer.IProxiedGetName"));
    }


    [Test]
    public void AddForwardingMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethod", ModuleScope, typeof (Proxied), new Type[0]);
      var methodInfo = typeof (Proxied).GetMethod ("PrependName");
      var methodEmitter = proxyBuilder.AddForwardingMethod (methodInfo);

      Type proxyType = proxyBuilder.BuildProxyType ();

      // Added by FS
      AssertMethodInfoEqual (methodEmitter.MethodBuilder, methodInfo);

      var proxied = new Proxied ("The name");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Check calling proxied method through reflection
      Assert.That (methodInfo.Invoke (proxied, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));

      var proxyMethodInfo = proxy.GetType ().GetMethod ("PrependName");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));
    }

    // Added by FS
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot add a forwarding call to method "
        + "'Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes' because it is not public. If the method is an explicit interface implementation, use "
            + "AddForwardingMethodFromClassOrInterfaceMethodInfoCopy and supply the interface's MethodInfo.", MatchType = MessageMatch.Contains)]
    public void AddForwardingMethod_NonPublicMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethod_NonPublicMethod", ModuleScope, typeof (ProxiedChild), new Type[0]);
      var methodInfo = typeof (ProxiedChild).GetMethod (
          "Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes", _nonPublicInstanceFlags);
      try
      {
        proxyBuilder.AddForwardingMethod (methodInfo);
      }
      finally
      {
        proxyBuilder.BuildProxyType();
      }
    }

    [Test]
    public void AddForwardingMethod_MethodWithVariableNumberOfParameters ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_MethodWithVariableNumberOfParameters", ModuleScope, typeof (Proxied), new Type[0]);
      var methodInfo = typeof (Proxied).GetMethod ("Sum");

      var methodEmitter = proxyBuilder.AddForwardingMethod (methodInfo);

      Type proxyType = proxyBuilder.BuildProxyType ();

      // Added by FS
      AssertMethodInfoEqual (methodEmitter.MethodBuilder, methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ("ProxiedProxySumTest");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Check calling proxied method through reflection
      const string resultExpected = "ProxiedProxySumTest: 12";
      Assert.That (proxied.Sum (3, 4, 5), Is.EqualTo (resultExpected));
      Assert.That (methodInfo.Invoke (proxied, new object[] { new int[] { 3, 4, 5 } }), Is.EqualTo (resultExpected));

      var proxyMethodInfo = proxy.GetType ().GetMethod ("Sum");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 3, 4, 5 } }), Is.EqualTo (resultExpected));

      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { } }), Is.EqualTo ("ProxiedProxySumTest: 0"));
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 1 } }), Is.EqualTo ("ProxiedProxySumTest: 1"));
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { new int[] { 1000, 100, 10, 1 } }), Is.EqualTo ("ProxiedProxySumTest: 1111"));
    }


    [Test]
    public void AddForwardingMethod_GenericClass ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethod_GenericClass",
        ModuleScope, typeof (ProxiedChildGeneric<ProxiedChild, double>), new Type[0]);
      var methodInfo = typeof (ProxiedChildGeneric<ProxiedChild, double>).GetMethod ("ToStringKebap");
      var methodEmitter = proxyBuilder.AddForwardingMethod (methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildGeneric<ProxiedChild, double> ("PCG", new ProxiedChild ("PC"), 123.456);
      object proxy = proxyBuilder.CreateInstance (proxied);

      // Added by FS
      AssertMethodInfoEqual (methodEmitter.MethodBuilder, methodInfo);

      var proxyMethodInfo = proxy.GetType ().GetMethod ("ToStringKebap");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { 9800 }), Is.EqualTo ("ProxiedChild: PCG_[Proxied: PC]_123.456_9800"));
    }


    [Test]
    public void AddForwardingProperty ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty",
        ModuleScope, typeof (ProxiedChildGeneric<ProxiedChild, double>), new Type[0]);
      var propertyInfo = typeof (ProxiedChildGeneric<ProxiedChild, double>).GetProperty ("MutableName");
      var propertyEmitter = proxyBuilder.AddForwardingProperty (propertyInfo);

      // Added by FS
      Assert.That (propertyEmitter.Name, Is.EqualTo (propertyInfo.Name));
      Assert.That (propertyEmitter.PropertyType, Is.SameAs (propertyInfo.PropertyType));

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildGeneric<ProxiedChild, double> ("PCG", new ProxiedChild ("PC"), 123.456);
      
      //object proxy = proxyBuilder.CreateInstance (proxied);
      var proxyType = proxyBuilder.BuildProxyType ();
      //object proxy = proxyBuilder.CreateInstance (proxied);
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.MutableName, Is.EqualTo ("ProxiedChild: PCG"));
      
      //var proxyPropertyInfo = proxy.GetType ().GetProperty ("MutableName");
      var proxyPropertyInfo = proxyType.GetProperty ("MutableName");
      
      AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild: PCG"));

      proxied.MutableName = "PCG_Changed";
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild: PCG_Changed"));

      proxyPropertyInfo.SetValue (proxy, "PCG_Changed_Proxy", null);
      Assert.That (proxied.MutableName, Is.EqualTo ("ProxiedChild: PCG_Changed_Proxy"));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild: PCG_Changed_Proxy"));
    }



    [Test]
    public void AddForwardingProperty_ReadOnlyProperty ()
    {
      Type proxiedType = typeof (Proxied);
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_ReadOnlyProperty", ModuleScope, proxiedType, new Type[0]);
      
      var propertyInfo = proxiedType.GetProperty ("ReadonlyName");
      var propertyEmitter = proxyBuilder.AddForwardingProperty (propertyInfo);

      // Added by FS
      Assert.That (propertyEmitter.Name, Is.EqualTo (propertyInfo.Name));
      Assert.That (propertyEmitter.PropertyType, Is.SameAs (propertyInfo.PropertyType));

      Type proxyType = proxyBuilder.BuildProxyType ();
      var proxyPropertyInfo = proxyType.GetProperty ("ReadonlyName");
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
    }


    [Test]
    public void AddForwardingProperty_WriteOnlyProperty ()
    {
      Type proxiedType = typeof (Proxied);
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingProperty_WriteOnlyProperty", ModuleScope, proxiedType, new Type[0]);
      
      var propertyInfo = proxiedType.GetProperty ("WriteonlyName");
      var propertyEmitter = proxyBuilder.AddForwardingProperty (propertyInfo);
      
      // Added by FS
      Assert.That (propertyEmitter.Name, Is.EqualTo (propertyInfo.Name));
      Assert.That (propertyEmitter.PropertyType, Is.SameAs (propertyInfo.PropertyType));
      
      Type proxyType = proxyBuilder.BuildProxyType ();
      var proxyPropertyInfo = proxyType.GetProperty ("WriteonlyName");
      Assert.That (proxyPropertyInfo.CanRead, Is.False);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
    }







    [Test]
    public void AddForwardingMethodFromMethodInfoCopy_ExplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethodFromMethodInfoCopy_ExplicitInterfaceMethod",
        ModuleScope, typeof (ProxiedChild), new[] { typeof (IAmbigous1) });

      var methodInfo = typeof (IAmbigous1).GetMethod ("StringTimes");
      proxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (methodInfo);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChild ();
      object proxy = proxyBuilder.CreateInstance (proxied);

      Assert.That (((IAmbigous1) proxied).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
      Assert.That (((IAmbigous1) proxy).StringTimes ("aBc", 4), Is.EqualTo ("aBcaBcaBcaBc"));
    }


    [Test]
    public void AddForwardingMethodFromMethodInfoCopy_ImplicitInterfaceMethod ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethodFromMethodInfoCopy_ImplicitInterfaceMethod",
        ModuleScope, typeof (Proxied), new[] { typeof (IProxiedGetName) });
      proxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (typeof (IProxiedGetName).GetMethod ("GetName"));
      Type proxyType = proxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new Proxied ();
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxy.GetType ().GetInterfaces (), Is.EquivalentTo ((new[] { typeof (IProxiedGetName), typeof (IProxy) })));
      Assert.That (((IProxiedGetName) proxy).GetName (), Is.EqualTo ("Implementer.IProxiedGetName"));
      Assert.That (proxy.GetType ().GetMethod ("GetName").Invoke (proxy, new object[0]), Is.EqualTo ("Implementer.IProxiedGetName"));
    }


    [Test]
    public void AddForwardingMethodFromMethodInfoCopy_Method ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("AddForwardingMethodFromMethodInfoCopy_Method", ModuleScope, typeof (Proxied), new Type[0]);
      var methodInfo = typeof (Proxied).GetMethod ("PrependName");
      proxyBuilder.AddForwardingMethodFromClassOrInterfaceMethodInfoCopy (methodInfo);
      Type proxyType = proxyBuilder.BuildProxyType ();

      var proxied = new Proxied ("The name");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Check calling proxied method through reflection
      Assert.That (methodInfo.Invoke (proxied, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));

      var proxyMethodInfo = proxy.GetType ().GetMethod ("PrependName");
      AssertMethodInfoEqual (proxyMethodInfo, methodInfo);
      Assert.That (proxyMethodInfo.Invoke (proxy, new object[] { "is Smith" }), Is.EqualTo ("The name is Smith"));
    }



    [Test]
    public void IProxy ()
    {
      var proxyBuilder = new ForwardingProxyBuilder ("IProxy", ModuleScope, typeof (Proxied), new Type[0]);
      Type proxyType = proxyBuilder.BuildProxyType ();

      var proxied = new Proxied ("AAA");
      object proxy = Activator.CreateInstance (proxyType, proxied);
      Assert.That (ScriptingHelper.GetProxiedFieldValue (proxy), Is.SameAs (proxied));

       var proxied2 = new Proxied ("BBB");
      ((IProxy) proxy).SetProxied (proxied2);

      Assert.That (ScriptingHelper.GetProxiedFieldValue (proxy), Is.SameAs (proxied2));
    }


    public void AssertMethodInfoEqual (MethodInfo methodInfo0, MethodInfo methodInfo1)
    {
      Assert.That (methodInfo0, Is.Not.Null);
      Assert.That (methodInfo1, Is.Not.Null);
      Assert.That (methodInfo0.Name, Is.EqualTo (methodInfo1.Name));
      Assert.That (methodInfo0.ReturnType, Is.EqualTo (methodInfo1.ReturnType));
      var parameterTypes0 = methodInfo0.GetParameters ().Select (pi => pi.ParameterType);
      var parameterTypes1 = methodInfo1.GetParameters ().Select (pi => pi.ParameterType);
      Assert.That (parameterTypes0.ToList (), Is.EquivalentTo (parameterTypes1.ToList ()));
    }

    public void AssertPropertyInfoEqual (PropertyInfo propertyInfo0, PropertyInfo propertyInfo1)
    {
      Assert.That (propertyInfo0, Is.Not.Null);
      Assert.That (propertyInfo1, Is.Not.Null);
      Assert.That (propertyInfo0.Name, Is.EqualTo (propertyInfo1.Name));
      Assert.That (propertyInfo0.CanRead, Is.EqualTo (propertyInfo1.CanRead));
      Assert.That (propertyInfo0.CanWrite, Is.EqualTo (propertyInfo1.CanWrite));
      Assert.That (propertyInfo0.PropertyType, Is.EqualTo (propertyInfo1.PropertyType));
    }


    private void SaveAndVerifyModuleScopeAssembly (bool strongNamed)
    {
      var path = _moduleScope.SaveAssembly (strongNamed);
      PEVerifier.CreateDefault().VerifyPEFile (path);
    }
  }
}
