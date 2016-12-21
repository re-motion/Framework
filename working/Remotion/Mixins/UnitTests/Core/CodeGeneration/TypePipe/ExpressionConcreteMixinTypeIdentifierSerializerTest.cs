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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class ExpressionConcreteMixinTypeIdentifierSerializerTest
  {
    private MethodInfo _simpleMethod1;
    private MethodInfo _simpleMethod2;
    private MethodInfo _genericMethod;
    private MethodInfo _methodOnGenericClosedWithReferenceType;
    private MethodInfo _methodOnGenericClosedWithValueType;

    [SetUp]
    public /* override*/ void SetUp ()
    {
      // base.SetUp ();

      _simpleMethod1 = Assertion.IsNotNull (typeof (BT1Mixin1).GetMethod ("VirtualMethod"));
      _simpleMethod2 = Assertion.IsNotNull (typeof (BT1Mixin2).GetMethod ("VirtualMethod"));
      _genericMethod = Assertion.IsNotNull (typeof (BaseType7).GetMethod ("One"));
      
      _methodOnGenericClosedWithReferenceType = typeof (GenericClassWithAllKindsOfMembers<string>).GetMethod ("Method");
      _methodOnGenericClosedWithValueType = typeof (GenericClassWithAllKindsOfMembers<int>).GetMethod ("Method");
    }

    [Test]
    public void IntegrationTest ()
    {
      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _simpleMethod1 },
          new HashSet<MethodInfo> { _simpleMethod2 });

      var serializer = new ExpressionConcreteMixinTypeIdentifierSerializer();
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (serializer.CreateExpression());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    [Test]
    public void IntegrationTest_MethodsOnGenericType ()
    {
      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _methodOnGenericClosedWithReferenceType, _methodOnGenericClosedWithValueType },
          new HashSet<MethodInfo> { _methodOnGenericClosedWithReferenceType, _methodOnGenericClosedWithValueType });

      var serializer = new ExpressionConcreteMixinTypeIdentifierSerializer();
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (serializer.CreateExpression());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    [Test]
    public void IntegrationTest_GenericMethods ()
    {
      var referenceIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          new HashSet<MethodInfo> { _genericMethod },
          new HashSet<MethodInfo> { _genericMethod });

      var serializer = new ExpressionConcreteMixinTypeIdentifierSerializer();
      referenceIdentifier.Serialize (serializer);

      object result = BuildTypeAndInvokeMethod (serializer.CreateExpression());

      Assert.That (result, Is.EqualTo (referenceIdentifier));
    }

    private ConcreteMixinTypeIdentifier BuildTypeAndInvokeMethod (Expression expressionToReturn)
    {
      // This needs to generate code into a TypeBuilder as dynamic methods won't trigger the code gen bug with generic methods (see comment in 
      // ExpressionConcreteMixinTypeIdentifierSerializer.ConcreteMixinTypeIdentifier).
      var adHocCodeGenerator = new AdHocCodeGenerator();
      var lambda = Expression.Lambda<Func<ConcreteMixinTypeIdentifier>> (expressionToReturn);

      return adHocCodeGenerator.CreateMethodAndRun<ConcreteMixinTypeIdentifier> (action: lambda.CompileToMethod, saveOnError: true);
    }
  }
}