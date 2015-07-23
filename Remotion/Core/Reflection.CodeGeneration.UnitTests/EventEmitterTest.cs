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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class EventEmitterTest : CodeGenerationTestBase
  {
    private CustomClassEmitter _classEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = new CustomClassEmitter (Scope, UniqueName, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true); // force unsigned because we use SimpleAttribute below
    }

    public override void TearDown ()
    {
      if (!_classEmitter.HasBeenBuilt)
        _classEmitter.BuildType();

      base.TearDown();
    }

    private object BuildInstance ()
    {
      return Activator.CreateInstance (_classEmitter.BuildType ());
    }

    private void AddEventMethod (object instance, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent(instance, eventEmitter).GetAddMethod(true).Invoke (instance, new[] {method});
    }

    private void AddEventMethod (Type type, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (type, eventEmitter).GetAddMethod (true).Invoke (null, new object[] { method });
    }

    private void RemoveEventMethod (object instance, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (instance, eventEmitter).GetRemoveMethod (true).Invoke (instance, new object[] { method });
    }

    private void RemoveEventMethod (Type type, CustomEventEmitter eventEmitter, object method)
    {
      GetEvent (type, eventEmitter).GetRemoveMethod (true).Invoke (null, new object[] { method });
    }

    private EventInfo GetEvent (Type builtType, CustomEventEmitter eventEmitter)
    {
      return builtType.GetEvent (eventEmitter.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private EventInfo GetEvent (object instance, CustomEventEmitter eventEmitter)
    {
      return GetEvent (instance.GetType (), eventEmitter);
    }

    private void ImplementEventAddMethod (CustomEventEmitter eventEmitter)
    {
      FieldReference field;
      if (eventEmitter.EventKind == EventKind.Static)
        field = _classEmitter.CreateStaticField ("AddCalled", typeof (bool));
      else
        field = _classEmitter.CreateField ("AddCalled", typeof (bool));

      eventEmitter.AddMethod.AddStatement (new AssignStatement (field, new ConstReference (true).ToExpression ()));
      eventEmitter.AddMethod.ImplementByReturningVoid ();
    }

    private void ImplementEventRemoveMethod (CustomEventEmitter eventEmitter)
    {
      FieldReference field;
      if (eventEmitter.EventKind == EventKind.Static)
        field = _classEmitter.CreateStaticField ("RemoveCalled", typeof (bool));
      else
        field = _classEmitter.CreateField ("RemoveCalled", typeof (bool));

      eventEmitter.RemoveMethod.AddStatement (new AssignStatement (field, new ConstReference (true).ToExpression ()));
      eventEmitter.RemoveMethod.ImplementByReturningVoid ();
    }

    private bool AddCalled (object instance)
    {
      FieldInfo field = instance.GetType().GetField ("AddCalled");
      return (bool) field.GetValue (instance);
    }

    private bool AddCalled (Type type)
    {
      FieldInfo field = type.GetField ("AddCalled");
      return (bool) field.GetValue (null);
    }

    private bool RemoveCalled (object instance)
    {
      FieldInfo field = instance.GetType ().GetField ("RemoveCalled");
      return (bool) field.GetValue (instance);
    }

    private bool RemoveCalled (Type type)
    {
      FieldInfo field = type.GetField ("RemoveCalled");
      return (bool) field.GetValue (null);
    }

    [Test]
    public void SimpleEvent ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("SimpleEvent", EventKind.Instance, typeof (EventHandler));

      Assert.That (eventEmitter.Name, Is.EqualTo ("SimpleEvent"));
      Assert.That (eventEmitter.EventType, Is.EqualTo (typeof (EventHandler)));
      Assert.That (eventEmitter.EventKind, Is.EqualTo (EventKind.Instance));

      ImplementEventAddMethod (eventEmitter);
      ImplementEventRemoveMethod (eventEmitter);

      object instance = BuildInstance ();
      Assert.That (AddCalled (instance), Is.False);
      Assert.That (RemoveCalled (instance), Is.False);

      AddEventMethod (instance, eventEmitter, (EventHandler) delegate { });

      Assert.That (AddCalled (instance), Is.True);
      Assert.That (RemoveCalled (instance), Is.False);

      RemoveEventMethod (instance, eventEmitter, (EventHandler) delegate { });

      Assert.That (AddCalled (instance), Is.True);
      Assert.That (RemoveCalled (instance), Is.True);
    }

    [Test]
    public void StaticEvent ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("StaticEvent", EventKind.Static, typeof (Func<string>));

      Assert.That (eventEmitter.Name, Is.EqualTo ("StaticEvent"));
      Assert.That (eventEmitter.EventType, Is.EqualTo (typeof (Func<string>)));
      Assert.That (eventEmitter.EventKind, Is.EqualTo (EventKind.Static));

      ImplementEventAddMethod (eventEmitter);
      ImplementEventRemoveMethod (eventEmitter);

      Type type = _classEmitter.BuildType ();

      Assert.That (AddCalled (type), Is.False);
      Assert.That (RemoveCalled (type), Is.False);

      AddEventMethod (type, eventEmitter, (Func<string>) delegate { return null; });

      Assert.That (AddCalled (type), Is.True);
      Assert.That (RemoveCalled (type), Is.False);

      RemoveEventMethod (type, eventEmitter, (Func<string>) delegate { return null; });

      Assert.That (AddCalled (type), Is.True);
      Assert.That (RemoveCalled (type), Is.True);
    }

    [Test]
    public void DefaultAddMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("DefaultAddMethod", EventKind.Static, typeof (EventHandler));
      Assert.That (eventEmitter.AddMethod, Is.Not.Null);
      Type type = _classEmitter.BuildType ();
      Assert.That (GetEvent (type, eventEmitter).GetAddMethod (), Is.Not.Null);
    }

    [Test]
    public void DefaultRemoveMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("DefaultRemoveMethod", EventKind.Static, typeof (EventHandler));
      Assert.That (eventEmitter.RemoveMethod, Is.Not.Null);
      Type type = _classEmitter.BuildType ();
      Assert.That (GetEvent (type, eventEmitter).GetRemoveMethod (), Is.Not.Null);
    }

    [Test]
    public void CustomAddMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("CustomAddMethod", EventKind.Static, typeof (EventHandler));
      eventEmitter.AddMethod = _classEmitter.CreateMethod (
          "CustomAdd", MethodAttributes.Public | MethodAttributes.Static, typeof (void), new [] { typeof (EventHandler) });
    }

    [Test]
    public void CustomRemoveMethod ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("CustomRemoveMethod", EventKind.Static, typeof (EventHandler));
      eventEmitter.RemoveMethod = _classEmitter.CreateMethod (
          "CustomRemove", MethodAttributes.Public | MethodAttributes.Static, typeof (void), new[] { typeof (EventHandler) });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Add methods can only be assigned once.")]
    public void AddMethodCannotBeSetTwice ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetTwice", EventKind.Static, typeof (EventHandler));
      Dev.Null = eventEmitter.AddMethod;
      eventEmitter.AddMethod = _classEmitter.CreateMethod (
          "invalid", MethodAttributes.Public | MethodAttributes.Static, typeof (void), new[] { typeof (EventHandler) });
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Remove methods can only be assigned once.")]
    public void RemoveMethodCannotBeSetTwice ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetTwice", EventKind.Static, typeof (EventHandler));
      Dev.Null = eventEmitter.RemoveMethod;
      eventEmitter.RemoveMethod = _classEmitter.CreateMethod (
          "invalid", MethodAttributes.Public | MethodAttributes.Static, typeof (void), new[] { typeof (EventHandler) });
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
        ExpectedMessage = "Event accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void AddMethodCannotBeSetToNull()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddMethodCannotBeSetToNull", EventKind.Static, typeof (string));
      eventEmitter.AddMethod = null;
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException),
       ExpectedMessage = "Event accessors cannot be set to null.", MatchType = MessageMatch.Contains)]
    public void RemoveMethodCannotBeSetToNull ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("RemoveMethodCannotBeSetToNull", EventKind.Static, typeof (string));
      eventEmitter.RemoveMethod = null;
    }

    [Test]
    public void AddCustomAttribute ()
    {
      CustomEventEmitter eventEmitter = _classEmitter.CreateEvent ("AddCustomAttribute", EventKind.Static, typeof (string));
      eventEmitter.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      Type type = _classEmitter.BuildType ();
      Assert.That (GetEvent (type, eventEmitter).IsDefined (typeof (SimpleAttribute), false), Is.True);
      Assert.That (GetEvent (type, eventEmitter).GetCustomAttributes (false).Length, Is.EqualTo (1));
      Assert.That (GetEvent (type, eventEmitter).GetCustomAttributes (false)[0], Is.EqualTo (new SimpleAttribute()));
    }
  }
}
