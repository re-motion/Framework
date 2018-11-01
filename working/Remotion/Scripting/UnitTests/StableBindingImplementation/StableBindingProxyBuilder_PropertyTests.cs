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
using NUnit.Framework;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class StableBindingProxyBuilder_PropertyTests : StableBindingProxyBuilderTest
  {
    private const BindingFlags _nonPublicInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags _publicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
    private const BindingFlags _allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;


    private readonly ScriptContext _scriptContext = ScriptContext.Create ("rubicon.eu.Remotion.Scripting.StableBindingProxyBuilder_PropertyTests",
      new TypeLevelTypeFilter (new[] { typeof (StableBindingProxyProviderPerformanceTests.ICascade1) }));



    [Test]
    public void BuildProxyType_PublicProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChildChild) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::NameProperty PC"));
    }

    [Test]
    public void BuildProxyType_PublicProperty_WithSetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty XXyyZZ-ProxiedChild::NameProperty"));
    }



    [Test]
    public void BuildProxyType_PublicProperty_WithNonPublicGetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownTypes = knownBaseTypes; 
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

     //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("PropertyWithNonPublicGetter", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.False);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      //Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
    }


    [Test]
    public void BuildProxyType_PublicProperty_WithNonPublicSetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownTypes = knownBaseTypes;
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("PropertyWithNonPublicSetter", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("Proxied::PropertyWithNonPublicSetter PC"));
    }


    [Test]
    public void BuildProxyType_VirtualPublicProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NamePropertyVirtual, Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NamePropertyVirtual", _publicInstanceFlags);


      // Note: Virtual properties remain virtual, so the proxy calls go to ProxiedChildChildChild, not the first known base type of ProxiedChild.

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual PC"));
    }

    [Test]
    public void BuildProxyType_VirtualPublicProperty_WithSetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.MutableNamePropertyVirtual, Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("MutableNamePropertyVirtual", _publicInstanceFlags);

      
      // Note: Virtual properties remain virtual, so the proxy calls go to ProxiedChildChild, not the first known base type of ProxiedChild 
      // (ProxiedChildChildChild does not override MutableNamePropertyVirtual).

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual XXyyZZ-ProxiedChildChild::MutableNamePropertyVirtual"));
    }


    [Test]
    public void BuildProxyType_ExplicitInterfaceProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      const string expectedPropertyValue = "ProxiedChild::IAmbigous1::MutableNameProperty PC";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IProperty.MutableNameProperty", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IProperty) proxied).MutableNameProperty = "aBc";
      const string expectedPropertyValue2 = "ProxiedChild::IAmbigous1::MutableNameProperty aBc-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy,"XXyyZZ" ,null);
      const string expectedPropertyValue3 = "ProxiedChild::IAmbigous1::MutableNameProperty XXyyZZ-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));
    }


    [Test]
    [ExpectedException (typeof (MissingMemberException), ExpectedMessage = "'ProxiedChild' object has no attribute 'PropertyAmbigous'")]
    public void AmbigousExplicitInterfaceProperties_Without_Proxy ()
    {
      var proxied = new ProxiedChild ("PC");
      ExecuteScriptAccessPropertyAmbigous(proxied);
    }

    private void ExecuteScriptAccessPropertyAmbigous (Object obj)
    {
      ScriptingHelper.ExecuteScriptExpression<string> ("p0.PropertyAmbigous", obj);
    }

    [Test]
    public void AmbigousExplicitInterfaceProperties_With_Proxy ()
    {
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (
          proxiedType, new TypeLevelTypeFilter (new[] { typeof (IPropertyAmbigous2) }), CreateModuleScope ("AmbigousExplicitInterfaceProperties_With_Proxy"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType();

      var proxied = new ProxiedChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Proxy works, since only IPropertyAmbigous2 is exposed.
      ExecuteScriptAccessPropertyAmbigous (proxy);

      // Proxied fails, since call to PropertyAmbigous is ambigous.
      try
      {
        ExecuteScriptAccessPropertyAmbigous (proxied);
      }
      catch (MissingMemberException e)
      {
        Assert.That (e.Message, Is.EqualTo ("'ProxiedChild' object has no attribute 'PropertyAmbigous'"));
      }
    }


    [Test]
    public void BuildProxyType_AmbigousExplicitInterfaceProperties ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty), typeof (IPropertyAmbigous2) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      Assert.That (proxyType.GetInterface ("IPropertyAmbigous2"), Is.Not.Null);
      Assert.That (proxyType.GetInterface ("IPropertyAmbigous1"), Is.Null);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);


      const string expectedPropertyValue = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous PC";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue));

      // Script call to proxy is not ambigous
      Assert.That (ScriptingHelper.ExecuteScriptExpression<string> ("p0.PropertyAmbigous", proxy), Is.EqualTo (expectedPropertyValue));
      
      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous2.PropertyAmbigous", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IPropertyAmbigous2) proxied).PropertyAmbigous = "aBc";
      const string expectedPropertyValue2 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous aBc-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      const string expectedPropertyValue3 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous XXyyZZ-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));

      var proxyPropertyInfo2 = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous1.PropertyAmbigous", _nonPublicInstanceFlags);
      Assert.That (proxyPropertyInfo2, Is.Null);
    }

    [Test]
    public void BuildProxyType_AmbigousExplicitInterfaceProperties_With_PublicInterfaceImplementation ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChildChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty), typeof (IPropertyAmbigous2) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      Assert.That (proxyType.GetInterface ("IPropertyAmbigous2"), Is.Not.Null);
      Assert.That (proxyType.GetInterface ("IPropertyAmbigous1"), Is.Null);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      const string expectedPropertyValue = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous PC";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());


      var proxyPropertyInfoPublicProperty = proxyType.GetProperty ("PropertyAmbigous", _publicInstanceFlags);
      Assert.That (proxyPropertyInfoPublicProperty, Is.Not.Null);

      Assert.That (ScriptingHelper.ExecuteScriptExpression<string> ("p0.PropertyAmbigous", proxy), Is.EqualTo ("ProxiedChildChild::PropertyAmbigous PC"));

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous2.PropertyAmbigous", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IPropertyAmbigous2) proxied).PropertyAmbigous = "aBc";
      const string expectedPropertyValue2 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous aBc-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      const string expectedPropertyValue3 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous XXyyZZ-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));

      var proxyPropertyInfo2 = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous1.PropertyAmbigous", _nonPublicInstanceFlags);
      Assert.That (proxyPropertyInfo2, Is.Null);

    }

    // TODO: Remove dependency on StableBindingProxyProviderPerformanceTests.CascadeStableBinding, turn into unit test
    [Test]
    public void OnlyKnownInterface_PublicProperty ()
    {
      const string scriptFunctionSourceCode = @"
import clr

def Dir(x) :
  return dir(x)

def PropertyPathAccess(cascade) :
  # return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
  return cascade.Child.Name
";

      const int numberChildren = 10;
      var cascadeStableBinding = new StableBindingProxyProviderPerformanceTests.CascadeStableBinding (numberChildren);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import (typeof (Cascade).Assembly.GetName().Name, typeof (Cascade).Namespace, typeof (Cascade).Name);

      //var dirScript = new ScriptFunction<object, object> (
      //  _scriptContext, ScriptLanguageType.Python,
      //  scriptFunctionSourceCode, privateScriptEnvironment, "Dir"
      //);
      //To.ConsoleLine.e (dirScript.Execute (cascadeStableBinding)).nl ().e (dirScript.Execute (cascadeStableBinding.Child)).nl ().e (dirScript.Execute (cascadeStableBinding.Child.Child));

      var propertyPathAccessScript = new ScriptFunction<StableBindingProxyProviderPerformanceTests.Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );


      propertyPathAccessScript.Execute (cascadeStableBinding);
    }

  }
}
