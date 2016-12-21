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
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class StableBindingProxyBuilderTest
  {
    private readonly MethodInfoEqualityComparer _methodInfoEqualityComparerIgnoreVirtual = new MethodInfoEqualityComparer(~(MethodAttributes.Virtual | MethodAttributes.ReservedMask | MethodAttributes.VtableLayoutMask));


    [Test]
    public void Ctor ()
    {
      var typeFilter = new TypeLevelTypeFilter(new Type[0]);
      var proxiedType = typeof (string);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("Ctor"));
      Assert.That (stableBindingProxyBuilder.ProxiedType, Is.EqualTo (proxiedType));
      Assert.That (stableBindingProxyBuilder.GetTypeFilter (), Is.EqualTo (typeFilter));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_OneExplicitInterfaceMethod ()
    {
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeFilter = new TypeLevelTypeFilter (new[] { typeIAmbigous2 });
      // Note: ProxiedChild implements IAmbigous1 and IAmbigous2
      var proxiedType = typeof(ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_OneExplicitInterfaceMethod"));

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      //To.ConsoleLine.e (classMethodToInterfaceMethodsMap);

      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod(proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");

      //To.ConsoleLine.e (stringTimesMethod).nl ().e (stringTimesIAmbigous2InterfaceMethod);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (1));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous2ClassMethod].ToList(), Is.EquivalentTo (ListObjectMother.New(stringTimesIAmbigous2InterfaceMethod)));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_TwoExplicitInterfaceMethods ()
    {
      var typeIAmbigous1 = typeof (IAmbigous1);
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeFilter = new TypeLevelTypeFilter (new[] { typeIAmbigous2, typeof (Proxied), typeIAmbigous1 });
      // Note: ProxiedChild implements IAmbigous1 and IAmbigous2
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_TwoExplicitInterfaceMethods"));

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      var stringTimesIAmbigous1ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes");
      Assert.That (stringTimesIAmbigous1ClassMethod, Is.Not.Null);
      var stringTimesIAmbigous1InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous1, "StringTimes");
      Assert.That (stringTimesIAmbigous1InterfaceMethod, Is.Not.Null);

      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      Assert.That (stringTimesIAmbigous2ClassMethod, Is.Not.Null);
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");
      Assert.That (stringTimesIAmbigous2InterfaceMethod, Is.Not.Null);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (2));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous1ClassMethod].ToList (), Is.EquivalentTo (ListObjectMother.New (stringTimesIAmbigous1InterfaceMethod)));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous2ClassMethod].ToList (), Is.EquivalentTo (ListObjectMother.New (stringTimesIAmbigous2InterfaceMethod)));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_TwoInterfaceMethodsImplementedAsOnePublicMethod ()
    {
      var typeIProcessText1 = typeof (IProcessText1);
      var typeIProcessText2 = typeof (IProcessText2);
      var typeFilter = new TypeLevelTypeFilter (new[] { typeIProcessText2, typeof (Proxied), typeIProcessText1 });
      // Note: ProxiedChildChild implements IProcessText1 and IProcessText2 through one public member
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_TwoInterfaceMethodsImplementedAsOnePublicMethod"));

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      var stringTimesIProcessText1ClassMethod = GetAnyInstanceMethod (proxiedType, "ProcessText");
      Assert.That (stringTimesIProcessText1ClassMethod, Is.Not.Null);

      var stringTimesIProcessText1InterfaceMethod = GetAnyInstanceMethod (typeIProcessText1, "ProcessText");
      Assert.That (stringTimesIProcessText1InterfaceMethod, Is.Not.Null);
      var stringTimesIProcessText2InterfaceMethod = GetAnyInstanceMethod (typeIProcessText2, "ProcessText");
      Assert.That (stringTimesIProcessText2InterfaceMethod, Is.Not.Null);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (1));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIProcessText1ClassMethod].ToList (), Is.EquivalentTo (ListObjectMother.New (stringTimesIProcessText1InterfaceMethod, stringTimesIProcessText2InterfaceMethod)));
    }


    [Test]
    public void GetInterfaceMethodsToClassMethod ()
    {
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeFilter = new TypeLevelTypeFilter (new[] { typeIAmbigous2 });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("GetInterfaceMethodsToClassMethod"));

      var stringTimesIAmbigous1ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes");
      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");
     
      Assert.That (stableBindingProxyBuilder.GetInterfaceMethodsToClassMethod (stringTimesIAmbigous1ClassMethod).ToList (), Is.Empty);
      Assert.That (stableBindingProxyBuilder.GetInterfaceMethodsToClassMethod (stringTimesIAmbigous2ClassMethod).ToList (), Is.EquivalentTo (ListObjectMother.New (stringTimesIAmbigous2InterfaceMethod)));
    }


    [Test]
    public void GetFirstKnownBaseType ()
    {
      AssertGetFirstKnownBaseType( typeof (ProxiedChildChild), new TypeLevelTypeFilter (new[] { typeof (Proxied) }), typeof (Proxied));
      AssertGetFirstKnownBaseType (typeof (ProxiedChildChild), 
        new TypeLevelTypeFilter (new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild) }), typeof (ProxiedChildChild));
      AssertGetFirstKnownBaseType (typeof (ProxiedChildChild), new TypeLevelTypeFilter (new[] { typeof (string), typeof (Object), typeof (Type) }), typeof (Object));
    }

    private void AssertGetFirstKnownBaseType (Type proxiedType, TypeLevelTypeFilter typeFilter, Type expectedType)
    {
      var moduleScopeStub = MockRepository.GenerateStub<ModuleScope>();
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, moduleScopeStub);
      Assert.That (stableBindingProxyBuilder.GetFirstKnownBaseType (), Is.EqualTo (expectedType));
    }


    [Test]
    public void IsMethodKnownInClass_MethodNotHiddenThroughNew ()
    {
      //var moduleScopeStub = MockRepository.GenerateStub<ModuleScope> ();
      var baseType = typeof (Proxied);
      //var typeFilter = new TypeLevelTypeFilter (new[] { baseType });
      var proxiedType = typeof (ProxiedChildChild);
      //var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, moduleScopeStub);

      var methodFromBaseType = GetAnyInstanceMethod (baseType, "Sum");
      var method = GetAnyInstanceMethod (proxiedType, "Sum");

      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      
      // Shows methodInfo.GetBaseDefinition()-bug: Results should be equal.
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));

      // "Sum" is defined in Proxied
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (baseType, "Sum"), Is.True);
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "Sum"), Is.True);
      AssertIsMethodKnownInBaseType (baseType, method, Is.True);

      // "OverrideMe" is overridden in ProxiedChild
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (baseType, "OverrideMe"), Is.True);
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "OverrideMe"), Is.True);
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (proxiedType, "OverrideMe"), Is.True);

      // "PrependName" is redefined with new in ProxiedChildChild
      AssertIsMethodKnownInBaseType (baseType, GetAnyInstanceMethod (baseType, "PrependName"), Is.True);
      //AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "PrependName"), Is.True);
      //AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (proxiedType, "PrependName"), Is.False);
      AssertIsMethodKnownInBaseType (baseType, 
        ScriptingHelper.GetInstanceMethod (typeof (ProxiedChild), "PrependName",new[] {typeof(String)}), Is.True);
      AssertIsMethodKnownInBaseType (baseType, 
        ScriptingHelper.GetInstanceMethod (proxiedType, "PrependName", new[] { typeof (String) }), Is.False);
    }

    private void AssertIsMethodKnownInBaseType (Type baseType, MethodInfo method, Constraint constraint)
    {
      bool isKnownInBaseType = method.GetBaseDefinition().DeclaringType.IsAssignableFrom (baseType);
      Assert.That (isKnownInBaseType, constraint);
    }


    [Test]
    public void IsMethodEqualToBaseTypeMethod ()
    {
      var moduleScopeStub = MockRepository.GenerateStub<ModuleScope> ();

      var baseType = typeof (ProxiedChildGeneric<int, string>);
      var proxiedType = typeof (ProxiedChildChildGeneric<int, string>);

      var typeFilter = new TypeLevelTypeFilter (new[] { baseType });
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, moduleScopeStub);

      var methodFromBaseType = ScriptingHelper.GetInstanceMethod (baseType, "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });
      var method = ScriptingHelper.GetInstanceMethod (proxiedType, "ProxiedChildGenericToString", new[] { typeof (int), typeof (string) });

      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));

      // Shows methodInfo.GetBaseDefinition()-bug: Results should be equal.
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));

      Assert.That (methodFromBaseType.ReflectedType, Is.EqualTo (baseType));

      Assert.That (stableBindingProxyBuilder.IsMethodEqualToBaseTypeMethod (method, method), Is.True);
      Assert.That (stableBindingProxyBuilder.IsMethodEqualToBaseTypeMethod (method, methodFromBaseType), Is.True);
      
      var sumMethod = proxiedType.GetMethod ("Sum", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      Assert.That (stableBindingProxyBuilder.IsMethodEqualToBaseTypeMethod (sumMethod, methodFromBaseType), Is.False);
    }







    [Test]
    public void BuildProxyType_ObjectMethods ()
    {
      var knownBaseType = typeof (Proxied);
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ObjectMethods"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var proxied = new ProxiedChild ();
      var proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (InvokeMethod (proxy, "ToString"), Is.EqualTo (proxied.ToString ()));
      Assert.That (InvokeMethod (proxy, "GetType"), Is.EqualTo (proxied.GetType ()));
      Assert.That (InvokeMethod (proxy, "GetHashCode"), Is.EqualTo (proxied.GetHashCode ()));
      Assert.That (InvokeMethod (proxy, "Equals", proxied), Is.True);
      Assert.That (InvokeMethod (proxy, "Equals", proxy), Is.False);
    }




    [Test]
    public void BuildProxyType_CheckMembers ()
    {
      var knownBaseType = typeof (Proxied);
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType();

      var knownBaseTypeMethods = knownBaseType.GetMethods ().Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods ().Where (m => m.IsSpecialName == false && m.DeclaringType != typeof(Object));

      Assert.That (knownBaseTypeMethods.Count (), Is.EqualTo (proxyMethods.Count ()));

      AssertHasSameMethod (knownBaseType, proxyType, "GetName");
      AssertHasSameMethod (knownBaseType, proxyType, "Sum", typeof(Int32[]));
      AssertHasSameMethod (knownBaseType, proxyType, "PrependName", typeof (String));
      AssertHasSameMethod (knownBaseType, proxyType, "SumMe", typeof(Int32[]));
      
      AssertHasSameMethod (knownBaseType, proxyType, "OverrideMe", typeof (String));

      AssertHasSameGenericMethod (knownBaseType, proxyType, "GenericToString", 2);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 1);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 2);
    }




    [Test]
    public void BuildProxyType_MethodHiddenWithNew ()
    {
      AssertBuildProxyType_MethodHiddenWithNew (typeof (Proxied), typeof (ProxiedChildChild),true, "PrependName");
      AssertBuildProxyType_MethodHiddenWithNew (typeof (Proxied), typeof (ProxiedChild), false, "PrependName");
      AssertBuildProxyType_MethodHiddenWithNew (typeof (Proxied), typeof (Proxied), false, "PrependName");

      AssertBuildProxyType_MethodHiddenWithNew (typeof (ProxiedChild), typeof (ProxiedChildChild), true, "PrependName");
      AssertBuildProxyType_MethodHiddenWithNew (typeof (Proxied), typeof (ProxiedChild), false, "PrependName");
    }

    [Test]
    public void BuildProxyType_MethodHiddenWithNew_ProxiedTypeIsKnownBaseType ()
    {
      AssertBuildProxyType_MethodHiddenWithNew (typeof (ProxiedChildChild), typeof (ProxiedChildChildChild), false, "PrependName");
      AssertBuildProxyType_MethodHiddenWithNew (typeof (ProxiedChildChild), typeof (ProxiedChildChild), false, "PrependName");
    }


    private void AssertBuildProxyType_MethodHiddenWithNew (Type knownBaseType, Type proxiedType, bool expectProxiedCallDifferent, string methodName)
    {
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType });
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("AssertBuildProxyType_MethodHiddenWithNew"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var proxiedTypeInstance = Activator.CreateInstance (proxiedType, "Peter");
      var proxy = Activator.CreateInstance (proxyType, proxiedTypeInstance);
      const string argument = "Fox";

      // Test that call to proxy method is not ambigous
      PrivateInvoke.InvokePublicMethod (proxy, methodName, argument);
    }


    [Test]
    public void BuildProxyType_MethodOverridden ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChildChild), typeof (ProxiedChildChildChild), "PrependNameVirtual", true);
    }

    [Test]
    public void BuildProxyType_MethodOverridden_ProxiedTypeIsKnownBaseType1 ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChildChildChild), typeof (ProxiedChildChildChild), "PrependNameVirtual", false);
    }

    [Test]
    public void BuildProxyType_MethodOverridden_ProxiedTypeIsKnownBaseType2 ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChildChild), typeof (ProxiedChildChild), "PrependNameVirtual", false);

    }


    [Test]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsKnown_CallIsVirtual_ProxiedChildChildChild ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChild), typeof (ProxiedChildChildChild), "VirtualMethodNotInBaseType", true);
    }

    [Test]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsKnown_CallIsVirtual_ProxiedChildChildChild2 ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChildChildChild), typeof (ProxiedChildChildChild), "VirtualMethodNotInBaseType", false);
    }

    [Test]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsKnown_CallIsVirtual_ProxiedChild ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChild), typeof (ProxiedChild), "VirtualMethodNotInBaseType", false);
    }

    [Test]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsKnown_CallIsVirtual_ProxiedChildChild ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (ProxiedChild), typeof (ProxiedChildChild), "VirtualMethodNotInBaseType", false);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Method 'ProxiedChild.VirtualMethodNotInBaseType' not found.")]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsNotKnown_ProxiedChild ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (Proxied), typeof (ProxiedChild), "VirtualMethodNotInBaseType", true);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Method 'ProxiedChildChildChild.VirtualMethodNotInBaseType' not found.")]
    public void BuildProxyType_VirtualMethodNotInBaseType_IsNotKnown_ProxiedChildChildChild ()
    {
      AssertBuildProxyType_MethodOverridden (
        typeof (Proxied), typeof (ProxiedChildChildChild), "VirtualMethodNotInBaseType", true);
    }


    private void AssertBuildProxyType_MethodOverridden (Type knownBaseType, Type proxiedType, string methodName, bool expectKnownBaseTypeCallDifferent)
    {
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType });
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("AssertBuildProxyType_MethodOverridden_ProxiedTypeIsKnownBaseType"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var proxiedTypeInstance = Activator.CreateInstance (proxiedType, "Peter");
      var knownBaseTypeInstance = Activator.CreateInstance (knownBaseType, "Peter");
      var proxy = Activator.CreateInstance (proxyType, proxiedTypeInstance);
      const string argument = "Fox";

      var proxyResult = InvokeMethod (proxy, methodName, argument);

      // We expect the call to be virtual.
      Assert.That (proxyResult, Is.EqualTo (InvokeMethod (proxiedTypeInstance, methodName, argument)));
      if (expectKnownBaseTypeCallDifferent)
      {
        Assert.That (proxyResult, Is.Not.EqualTo (InvokeMethod (knownBaseTypeInstance, methodName, argument)));
      }
    }


    [Test]
    public void BuildProxyType_CheckInterfaceMembers ()
    {
      var knownBaseType = typeof (Proxied);
      var knownInterfaceType = typeof (IAmbigous2);
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType, knownInterfaceType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_CheckInterfaceMembers"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var knownBaseTypeMethods = knownBaseType.GetMethods ().Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object));
      var interfaceMethods = knownInterfaceType.GetMethods (BindingFlags.Instance | BindingFlags.Public); 

      //To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (proxyMethods).nl (4);
      //To.ConsoleLine.e (knownInterfaceType.GetMethods()).nl (3).e (interfaceProxyMethods).nl (3);

      Assert.That (knownBaseTypeMethods.Count () + interfaceMethods.Count () + typeof(IProxy).GetMethods().Length, Is.EqualTo (proxyMethods.Count ()));

      //---------------------------------------------------------------------------------------------------------------------------------
      // Adding IAmbigous2 interface adds StringTimes(string,int) explicit interface implementation.
      // Note: StringTimes(string,int) exists 3x in ProxiedChild, as a regular member and as an explicit interface implementation from
      // IAmbigous1 and IAmbigous2. Since the first is private and the second is not known, only the StringTimes coming from 
      // IAmbigous2 is exposed.
      //---------------------------------------------------------------------------------------------------------------------------------
      AssertHasSameExplicitInterfaceMethod (knownInterfaceType, proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));

      // Methods coming from knownBaseType Proxied are still here:
      AssertHasSameMethod (knownBaseType, proxyType, "GetName");
      AssertHasSameMethod (knownBaseType, proxyType, "Sum", typeof (Int32[]));
      AssertHasSameMethod (knownBaseType, proxyType, "PrependName", typeof (String));
      AssertHasSameMethod (knownBaseType, proxyType, "SumMe", typeof (Int32[]));
      AssertHasSameMethod (knownBaseType, proxyType, "OverrideMe", typeof (String));
      AssertHasSameGenericMethod (knownBaseType, proxyType, "GenericToString", 2);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 1);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 2);
    }

    [Test]
    public void BuildProxyType_MultipleKnownInterfaces ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IAmbigous2), typeof(IAmbigous1) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_MultipleKnownInterfaces"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var knownBaseTypeMethods = knownBaseTypes.SelectMany(t => t.GetMethods ()).Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object));
      var interfaceMethods = knownInterfaceTypes.SelectMany(t => t.GetMethods (BindingFlags.Instance | BindingFlags.Public));

      //To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (interfaceMethods).nl (3).e (proxyMethods).nl (3);

      Assert.That (knownBaseTypeMethods.Count () + interfaceMethods.Count () + typeof (IProxy).GetMethods ().Length, // + typeof (IProxy).GetMethods ().Length, 
        Is.EqualTo (proxyMethods.Count ()));

      // Adding IAmbigous2 interface adds StringTimes(string,int) explicit interface implementation
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous1), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous2), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
    }


    [Test]
    public void BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation ()
    {
      Assert_BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation(
        new[] { typeof (ProxiedChild) }, 
        new[] { typeof (IAmbigous2), typeof(IAmbigous1), typeof (IAmbigous3), typeof (IAmbigous4) }, 
        typeof (ProxiedChild));

      Assert_BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation (
        new[] { typeof (ProxiedChild) },
        new[] { typeof (IAmbigous2), typeof (IAmbigous1), typeof (IAmbigous3), typeof (IAmbigous4) },
        typeof (ProxiedChildChild));

      Assert_BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation (
        new[] { typeof (ProxiedChild) },
        new[] { typeof (IAmbigous2), typeof (IAmbigous1), typeof (IAmbigous3), typeof (IAmbigous4) },
        typeof (ProxiedChildChildGeneric<string,object>));
    }

    private void Assert_BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation (
      Type[] knownBaseTypes, Type[] knownInterfaceTypes, Type proxiedType)
    {
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var knownBaseTypeMethods = knownBaseTypes.SelectMany (t => t.GetMethods ()).Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object));
      var interfaceMethods = knownInterfaceTypes.SelectMany (t => t.GetMethods (BindingFlags.Instance | BindingFlags.Public));

      //To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (interfaceMethods).nl (3).e (proxyMethods).nl (3);

      Assert.That (knownBaseTypeMethods.Count () + interfaceMethods.Count () + typeof (IProxy).GetMethods ().Length, // + typeof (IProxy).GetMethods ().Length, 
        Is.EqualTo (proxyMethods.Count ()));

      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous3), proxiedType, proxyType, "StringTimes2", typeof (String), typeof (Int32));
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous4), proxiedType, proxyType, "StringTimes2", typeof (String), typeof (Int32));
      AssertHasSameMethod (typeof (ProxiedChild), proxyType, "StringTimes2", typeof (String), typeof (Int32));

      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous3), proxiedType, proxyType, "AnotherMethod", typeof (String), typeof (Int32));
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous4), proxiedType, proxyType, "AnotherMethod", typeof (String), typeof (Int32));
      AssertHasSameMethod (typeof (ProxiedChild), proxyType, "AnotherMethod", typeof (String), typeof (Int32));

      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous1), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
      AssertHasSameExplicitInterfaceMethod (typeof (IAmbigous2), proxiedType, proxyType, "StringTimes", typeof (String), typeof (Int32));
    }





    [Test]
    public void BuildProxyType_GenericClassesAndMethods ()
    {
      Assert_BuildProxyType_GenericClassesAndMethods (
        typeof (ProxiedChildGeneric<int,string>),
        new Type[0], // { typeof (IAmbigous2), typeof (IAmbigous1), typeof (IAmbigous3), typeof (IAmbigous4) },
        typeof (ProxiedChildChildGeneric<int, string>));
    }

    private void Assert_BuildProxyType_GenericClassesAndMethods (
      Type knownBaseType, Type[] knownInterfaceTypes, Type proxiedType)
    {
      var knownTypes = knownInterfaceTypes.Concat (new[] { knownBaseType }).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_MultipleKnownInterfaces_PublicAndExplicitInterfaceImplementation"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var knownBaseTypeMethods = knownBaseType.GetMethods ().Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where (m => m.IsSpecialName == false && m.DeclaringType != typeof (Object)).ToArray();
      var interfaceMethods = knownInterfaceTypes.SelectMany (t => t.GetMethods (BindingFlags.Instance | BindingFlags.Public));

      //To.ConsoleLine.e (knownBaseTypeMethods).nl (3).e (interfaceMethods).nl (3).e (proxyMethods).nl (3);

      var numberKnownBaseTypeMethods = knownBaseTypeMethods.Count ();
      var numberInterfaceMethods = interfaceMethods.Count ();
      var numberProxyMethods = proxyMethods.Count ();
     
      #if(true)
      Assert.That (numberKnownBaseTypeMethods + numberInterfaceMethods + typeof (IProxy).GetMethods ().Length, // + typeof (IProxy).GetMethods ().Length,
        Is.EqualTo (numberProxyMethods));
      #endif

      AssertHasSameMethod (knownBaseType, proxyType, "ProxiedChildGenericToString", typeof (int), typeof (string));
    }




    [Test]
    public void BuildProxyType_ComplexGenericArguments_New ()
    {
      var knownBaseType = typeof (ProxiedChildGeneric<Proxied,Document>);
      var knownInterfaceType = typeof (IAmbigous2);
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType, knownInterfaceType });
      var proxiedType = typeof (ProxiedChildChildGeneric<Proxied, Document>);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_CheckInterfaceMembers"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      const string methodName = "ProxiedChildGeneric_ComplexGenericArguments_New";
      AssertHasSameGenericMethod (knownBaseType, proxyType, methodName, 2);

      var proxied = new ProxiedChildChildGeneric<Proxied, Document>();

      var proxyMethodGeneric = GetGenericMethod (proxyType, methodName, 2);
      Assert.That (proxyMethodGeneric.IsGenericMethod, Is.True);

      var proxyMethod = proxyMethodGeneric.MakeGenericMethod (typeof (string), typeof (int));

      var proxy = Activator.CreateInstance (proxyType,proxied);

      var p0 = new Proxied ("Hello");
      var p1 = new Document ("Doc");
      const string p2 = "xyz";
      const int p3 = 1234567;

      var methodArguments = Create_ProxiedChildGeneric_ComplexGenericArguments_Parameters (p0, p1,p2,p3);

      var resultProxy = (string) proxyMethod.Invoke (proxy, methodArguments);

      //To.ConsoleLine.e (() => resultProxy);
      StringAssert.StartsWith ("ProxiedChildGeneric::ProxiedChildGeneric_ComplexGenericArguments_New", resultProxy);
    }

    [Test]
    public void BuildProxyType_ComplexGenericArguments_Virtual ()
    {
      var knownBaseType = typeof (ProxiedChildGeneric<Proxied, Document>);
      var typeFilter = new TypeLevelTypeFilter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChildChildGeneric<Proxied, Document>);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_CheckInterfaceMembers"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      const string methodName = "ProxiedChildGeneric_ComplexGenericArguments_Virtual";
      AssertHasSameGenericMethod (knownBaseType, proxyType, methodName, 2);

      var proxied = new ProxiedChildChildGeneric<Proxied, Document> ();

      var proxyMethodGeneric = GetGenericMethod (proxyType, methodName, 2);
      Assert.That (proxyMethodGeneric.IsGenericMethod, Is.True);

      var proxyMethod = proxyMethodGeneric.MakeGenericMethod (typeof (string), typeof (int));

      var proxy = Activator.CreateInstance (proxyType, proxied);

      var p0 = new Proxied ("Hello");
      var p1 = new Document ("Doc");
      const string p2 = "xyz";
      const int p3 = 1234567;

      var methodArguments = Create_ProxiedChildGeneric_ComplexGenericArguments_Parameters (p0, p1, p2, p3);

      var proxyResult = (string) proxyMethod.Invoke (proxy, methodArguments);
      StringAssert.StartsWith ("ProxiedChildGeneric::ProxiedChildGeneric_ComplexGenericArguments_Virtual", proxyResult);
    }

    public object[] Create_ProxiedChildGeneric_ComplexGenericArguments_Parameters<T0,T1,T2,T3> (T0 t0, T1 t1,
      T2 t2, T3 t3)
    {
      var p0 = new List<List<T0>> { new List<T0> {t0}};
      var p1 = new Dictionary<T2, T1> { {t2,t1}};
      var p2 = new List<GenericWrapper<T2>> { new GenericWrapper<T2> (t2) };
      var p3 = new GenericWrapper<T3> (t3);

      return new object[] { p0,p1,p2,p3};
    }



    [Test]
    public void BuildProxyType_NoKnownBaseType ()
    {
      var typeFilter = new TypeLevelTypeFilter (new[]{typeof(IAmbigous1)});
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_NoKnownBaseType"));
      // Having no known base type must not throw
      stableBindingProxyBuilder.BuildProxyType ();
    }

    [Test]
    public void BuildProxyType_NoKnownBaseTypeAndNoKnownInterfaces ()
    {
      var typeFilter = new TypeLevelTypeFilter (new Type[0]);
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_NoKnownBaseType"));
      // Having no known base type and no known interfaces must not throw
      stableBindingProxyBuilder.BuildProxyType ();
    }


    private void AssertHasSameMethod (Type type0, Type type1, string methodName, params Type[] parameterTypes)
    {
      var methodFromType0 = type0.GetMethod (methodName, parameterTypes);
      Assert.That (methodFromType0, Is.Not.Null);
      var methodFromType1 = type1.GetMethod (methodName, parameterTypes);
      Assert.That (methodFromType1, Is.Not.Null);
      Assert.That (_methodInfoEqualityComparerIgnoreVirtual.Equals (methodFromType0, methodFromType1), Is.True);
    }

    protected void AssertHasSameExplicitInterfaceMethod (Type interfaceType, Type type0, Type type1, string methodName, params Type[] parameterTypes)
    {
      var methodFromType0 = ScriptingHelper.GetExplicitInterfaceMethod (interfaceType, type0, methodName, parameterTypes);
      Assert.That (methodFromType0, Is.Not.Null);
      var methodFromType1 = ScriptingHelper.GetExplicitInterfaceMethod (interfaceType, type1, methodName, parameterTypes);
      Assert.That (methodFromType1, Is.Not.Null);
      Assert.That (_methodInfoEqualityComparerIgnoreVirtual.Equals (methodFromType0, methodFromType1), Is.True);
    }

    private void AssertHasSameGenericMethod (Type type0, Type type1, string methodName, int numberOfGenericArguments)
    {
      var methodFromType0 = GetGenericMethod(type0, methodName, numberOfGenericArguments);
      Assert.That (methodFromType0, Is.Not.Null);
      var methodFromType1 = GetGenericMethod (type1, methodName, numberOfGenericArguments);
      Assert.That (methodFromType1, Is.Not.Null);
      Assert.That (_methodInfoEqualityComparerIgnoreVirtual.Equals (methodFromType0, methodFromType1), Is.True);
    }

    public static MethodInfo GetGenericMethod (Type type, string methodName, int numberOfGenericArguments)
    {
      return GetGenericMethods (type, methodName, numberOfGenericArguments).Single();
    } 
    
    public static IEnumerable<MethodInfo> GetGenericMethods (Type type, string methodName, int numberOfGenericArguments)
    {
      return type.GetMethods().Where (m => m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == numberOfGenericArguments);
    }

 
    public ModuleScope CreateModuleScope(string namePostfix)
    {
      string name = "Remotion.Scripting.CodeGeneration.Generated.StableBindingProxyBuilderTest" + namePostfix;
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (true, false, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }

    private void SaveAndVerifyModuleScopeAssembly (ModuleScope moduleScope)
    {
      ArgumentUtility.CheckNotNull ("moduleScope", moduleScope);
      if (moduleScope.StrongNamedModule != null)
        SaveAndVerifyModuleScopeAssembly (moduleScope, true);
      if (moduleScope.WeakNamedModule != null)
        SaveAndVerifyModuleScopeAssembly (moduleScope, false);
    }

    private void SaveAndVerifyModuleScopeAssembly (ModuleScope moduleScope, bool strongNamed)
    {
      var path = moduleScope.SaveAssembly (strongNamed);
      PEVerifier.CreateDefault ().VerifyPEFile (path);
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    public Object InvokeMethod (object instance, string name, params object[] arguments)
    {
      return instance.GetType ().InvokeMember (
          name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, instance, arguments);
    }
  }
}
