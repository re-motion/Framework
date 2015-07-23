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
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class StableBindingProxyProviderTest
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("Remotion.Scripting.StableBindingProxyProviderTest",
      new TypeLevelTypeFilter (new[] { typeof (ProxiedChildChildChild) }));


    [Test]
    public void BuildProxyType ()
    {
      var typeFilter = new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) });
      var provider = new StableBindingProxyProvider (
        typeFilter, CreateModuleScope ("GetTypeMemberProxy"));

      Assert.That (provider.TypeFilter, Is.SameAs (typeFilter));

      var proxied = new ProxiedChildChildChild ("abrakadava");
      const string attributeName = "PrependName";

      var proxyType = provider.BuildProxyType (proxied.GetType());

      var proxyMethod = proxyType.GetAllInstanceMethods(attributeName,typeof(string)).Single();

      Assert.That (proxyMethod, Is.Not.Null);
    }


    [Test]
    public void GetTypeMemberProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), CreateModuleScope ("GetTypeMemberProxy"));

      var proxied = new ProxiedChildChildChild ("abrakadava");
      const string attributeName = "PrependName";

      var typeMemberProxy = provider.GetAttributeProxy (proxied, attributeName);

      var customMemberTester = new GetCustomMemberTester (typeMemberProxy);

      var result = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester);
      Assert.That (result, Is.EqualTo ("ProxiedChild ProxiedChild: abrakadava simsalbum, THE NUMBER=2"));
    }

    [Test]
    public void BuildProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), CreateModuleScope ("BuildProxy"));

      var proxied = new ProxiedChildChildChild ("abrakadava");

      var proxy = provider.BuildProxy (proxied);
      // Necessary since a newly built proxy has an empty proxied field
      // TODO: Introduce BuildProxyFromType(proxiedType)
      ScriptingHelper.SetProxiedFieldValue (proxy, proxied); 
   
      Assert.That (proxy, Is.Not.Null);

      var result = ScriptingHelper.ExecuteScriptExpression<string> ("p0.PrependName('simsalbum',2)", proxy);
      Assert.That (result, Is.EqualTo ("ProxiedChild ProxiedChild: abrakadava simsalbum, THE NUMBER=2"));
    }

    [Test]
    public void GetProxyType_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), CreateModuleScope ("BuildProxy"));

      var proxied = new GetProxyTypeIsCachedTest ("abrakadava");

      var proxyType =  provider.GetProxyType (proxied.GetType());
      Assert.That (proxyType, Is.Not.Null);
      Assert.That (provider.GetProxyType (proxied.GetType ()), Is.SameAs (proxyType));
    }

    [Test]
    public void GetProxy_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), CreateModuleScope ("BuildProxy"));

      var proxied0 = new GetProxyTypeIsCachedTest ("abrakadava");
      var proxied1 = new GetProxyTypeIsCachedTest ("simsalsabum");

      var proxy0 = provider.GetProxy (proxied0);
      Assert.That (proxy0, Is.Not.Null);
      var proxy1 = provider.GetProxy (proxied1);
      Assert.That (proxy0, Is.SameAs (proxy1));
    }

    [Test]
    public void GetProxy_IsCachedAndProxiedIsSet ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), CreateModuleScope ("GetProxy_IsCachedAndProxiedSet"));

      var proxied0 = new GetProxyTypeIsCachedTest ("abrakadava");
      var proxied1 = new GetProxyTypeIsCachedTest ("simsalsabum");

      var proxy0 = provider.GetProxy (proxied0);
      Assert.That (proxy0, Is.Not.Null);

      var proxiedFieldValue0 = ScriptingHelper.GetProxiedFieldValue (proxy0);
      Assert.That (proxiedFieldValue0, Is.SameAs (proxied0));
      var proxy1 = provider.GetProxy (proxied1);
      Assert.That (proxy0, Is.SameAs (proxy1));
      Assert.That (ScriptingHelper.GetProxiedFieldValue (proxy1), Is.SameAs (proxied1));
    }

    [Test]
    public void GetAttributeProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), CreateModuleScope ("GetTypeMemberProxy"));

      var proxied0 = new ProxiedChildChildChild ("ABCccccccccccccccccc");
      var proxied1 = new ProxiedChildChildChild ("xyzzzzzzzzz");

      var typeMemberProxy0 = provider.GetAttributeProxy (proxied0, "PrependName");
      var customMemberTester0 = new GetCustomMemberTester (typeMemberProxy0);
      var result0 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester0);
      Assert.That (result0, Is.EqualTo ("ProxiedChild ProxiedChild: ABCccccccccccccccccc simsalbum, THE NUMBER=2"));


      var typeMemberProxy1 = provider.GetAttributeProxy (proxied1, "NamePropertyVirtual");

      var customMemberTester1 = new GetCustomMemberTester (typeMemberProxy1);
      var result1 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.ABCDEFG", customMemberTester1);
      Assert.That (result1, Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual xyzzzzzzzzz"));

    }

    [Test]
    public void GetAttributeProxy_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), CreateModuleScope ("GetTypeMemberProxy"));

      var proxied0 = new ProxiedChildChildChild ("ABCccccccccccccccccc");
      var proxied1 = new ProxiedChildChildChild ("xyzzzzzzzzz");

      const string attributeName = "PrependName";
      var typeMemberProxy0 = provider.GetAttributeProxy (proxied0, attributeName);
      var customMemberTester0 = new GetCustomMemberTester (typeMemberProxy0);
      var result0 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester0);
      Assert.That (result0, Is.EqualTo ("ProxiedChild ProxiedChild: ABCccccccccccccccccc simsalbum, THE NUMBER=2"));


      var typeMemberProxy1 = provider.GetAttributeProxy (proxied1, attributeName);

      Assert.That (typeMemberProxy0, Is.SameAs (typeMemberProxy1));

      var customMemberTester1 = new GetCustomMemberTester (typeMemberProxy1);
      var result1 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.ABCDEFG('Schlaf')", customMemberTester1);
      Assert.That (result1, Is.EqualTo ("xyzzzzzzzzz Schlaf"));

    }

    public static ModuleScope CreateModuleScope (string namePostfix)
    {
      string name = "Remotion.Scripting.CodeGeneration.Generated.Test." + namePostfix;
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (true, false, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }
  }




  /// <summary>
  /// Stores a pythonScriptEngine.Operations.GetMember(proxy, attributeName)-wrapper-object and returns it 
  /// in calls to GetCustomMember.
  /// </summary>
  public class GetCustomMemberTester
  {
    private readonly object _typeMemberProxy;

    public GetCustomMemberTester (Object typeMemberProxy)
    {
      _typeMemberProxy = typeMemberProxy;
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return _typeMemberProxy;
    }
  }


  public class GetProxyTypeIsCachedTest : ProxiedChildChildChild
  {
    public GetProxyTypeIsCachedTest (string name)
     : base (name)
    {
    }

  }
}
