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
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public enum EventKind
  {
    Static,
    Instance
  }

  public class CustomEventEmitter : IAttributableEmitter
  {
    private readonly CustomClassEmitter _declaringType;
    private readonly EventBuilder _eventBuilder;

    private readonly string _name;
    private readonly EventKind _eventKind;
    private readonly Type _eventType;

    private IMethodEmitter _addMethod;
    private IMethodEmitter _removeMethod;

    public CustomEventEmitter (CustomClassEmitter declaringType, string name, EventKind eventKind, Type eventType, EventAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("eventType", eventType);

      _declaringType = declaringType;
      _eventBuilder = declaringType.TypeBuilder.DefineEvent (name, attributes, eventType);
      _name = name;
      _eventKind = eventKind;
      _eventType = eventType;
      declaringType.RegisterEventEmitter (this);
    }

    public IMethodEmitter AddMethod
    {
      get
      {
        if (_addMethod == null)
          CreateAddMethod ();

        return _addMethod;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException ("value", "Event accessors cannot be set to null.");

        if (_addMethod != null)
          throw new InvalidOperationException ("Add methods can only be assigned once.");

        _addMethod = value;
        _eventBuilder.SetAddOnMethod (_addMethod.MethodBuilder);
      }
    }

    public IMethodEmitter RemoveMethod
    {
      get
      {
        if (_removeMethod == null)
          CreateRemoveMethod ();

        return _removeMethod;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException ("value", "Event accessors cannot be set to null.");

        if (_removeMethod != null)
          throw new InvalidOperationException ("Remove methods can only be assigned once.");

        _removeMethod = value;
        _eventBuilder.SetRemoveOnMethod (_removeMethod.MethodBuilder);
      }
    }

    public string Name
    {
      get { return _name; }
    }

    public Type EventType
    {
      get { return _eventType; }
    }

    public EventKind EventKind
    {
      get { return _eventKind; }
    }

    public CustomClassEmitter DeclaringType
    {
      get { return _declaringType; }
    }

    public EventBuilder EventBuilder
    {
      get { return _eventBuilder; }
    }

    private void CreateAddMethod ()
    {
      Assertion.IsNull (_addMethod);

      MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
      if (EventKind == EventKind.Static)
        flags |= MethodAttributes.Static;
      IMethodEmitter method = _declaringType.CreateMethod ("add_" + Name, flags, typeof (void), new [] { EventType });
      AddMethod = method;
    }

    private void CreateRemoveMethod ()
    {
      Assertion.IsNull (_removeMethod);

      MethodAttributes flags = MethodAttributes.Public | MethodAttributes.SpecialName;
      if (EventKind == EventKind.Static)
        flags |= MethodAttributes.Static;
      IMethodEmitter method = _declaringType.CreateMethod ("remove_" + Name, flags, typeof (void), new [] { EventType });
      RemoveMethod = method;
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      _eventBuilder.SetCustomAttribute (customAttribute);
    }

    internal void EnsureValid ()
    {
      IMethodEmitter addMethod = AddMethod; // cause generation of default method if none has been assigned
      Assertion.IsNotNull (addMethod);

      IMethodEmitter removeMethod = RemoveMethod; // cause generation of default method if none has been assigned
      Assertion.IsNotNull (removeMethod);
    }
  }
}
