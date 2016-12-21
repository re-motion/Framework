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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class PropertyReferenceTest : SnippetGenerationBaseTest
  {
    [Test]
    public void InstancePropertyReference ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));
      propertyEmitter.CreateGetMethod();
      propertyEmitter.CreateSetMethod();
      propertyEmitter.ImplementWithBackingField ();

      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);
      
      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);
      Assert.That (propertyWithSelfOwner.Type, Is.EqualTo (typeof (string)));

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithSelfOwner.ToExpression()));
      methodEmitter.AddStatement (new AssignStatement (propertyWithSelfOwner, new ConstReference ("New").ToExpression()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      object instance = GetBuiltInstance ();
      PrivateInvoke.SetPublicProperty (instance, "Property", "Old");
      Assert.That (InvokeMethod(), Is.EqualTo ("Old"));
      Assert.That (PrivateInvoke.GetPublicProperty (instance, "Property"), Is.EqualTo ("New"));
    }

    [Test]
    public void StaticPropertyReference ()
    {
      CustomPropertyEmitter propertyEmitter = ClassEmitter.CreateProperty ("Property", PropertyKind.Static, typeof (string));
      propertyEmitter.CreateGetMethod ();
      propertyEmitter.CreateSetMethod ();
      propertyEmitter.ImplementWithBackingField ();

      var methodEmitter = GetMethodEmitter (true, typeof (string), new Type[0]);

      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithNoOwner = new PropertyReference (null, propertyEmitter.PropertyBuilder);
      Assert.That (propertyWithNoOwner.Type, Is.EqualTo (typeof (string)));

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithNoOwner.ToExpression ()));
      methodEmitter.AddStatement (new AssignStatement (propertyWithNoOwner, new ConstReference ("New").ToExpression ()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      PrivateInvoke.SetPublicStaticProperty (GetBuiltType(), "Property", "Old");
      Assert.That (InvokeMethod(), Is.EqualTo ("Old"));
      Assert.That (PrivateInvoke.GetPublicStaticProperty (GetBuiltType (), "Property"), Is.EqualTo ("New"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The property .*.Property cannot be loaded, it has no getter.", MatchType = MessageMatch.Regex)]
    public void LoadPropertyWithoutGetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      var methodEmitter = GetUnsavedMethodEmitter (false, typeof (string), new Type[0]);

      LocalReference oldValueLocal = methodEmitter.DeclareLocal (typeof (string));
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (oldValueLocal, propertyWithSelfOwner.ToExpression ()));
      methodEmitter.AddStatement (new ReturnStatement (oldValueLocal));

      GetUnsavedBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "The property .*.Property cannot be stored, it has no setter.", MatchType = MessageMatch.Regex)]
    public void SavePropertyWithoutSetterThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      var methodEmitter = GetUnsavedMethodEmitter (false, typeof (string), new Type[0]);

      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (propertyWithSelfOwner, NullExpression.Instance));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      GetUnsavedBuiltType ();
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "A property's address cannot be loaded.")]
    public void LoadPropertyAddressThrows ()
    {
      CustomPropertyEmitter propertyEmitter = UnsavedClassEmitter.CreateProperty ("Property", PropertyKind.Instance, typeof (string));

      var methodEmitter = GetUnsavedMethodEmitter (false, typeof (string), new Type[0]);

      LocalReference valueAddress = methodEmitter.DeclareLocal (typeof (string).MakeByRefType());
      PropertyReference propertyWithSelfOwner = new PropertyReference (propertyEmitter.PropertyBuilder);

      methodEmitter.AddStatement (new AssignStatement (valueAddress, propertyWithSelfOwner.ToAddressOfExpression()));
      methodEmitter.AddStatement (new ReturnStatement (NullExpression.Instance));

      GetUnsavedBuiltType ();
    }
  }
}
