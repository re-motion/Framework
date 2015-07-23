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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe.Dlr.Ast;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class CodeGenerationClassContextSerializerTest : CodeGenerationBaseTest
  {
    [Test]
    public void IntegrationTest()
    {
      var referenceMixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (BT1Mixin1));
      var referenceMixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (BT1Mixin2));
      var referenceClassContext = new ClassContext (typeof (BaseType1), new[] { referenceMixinContext1, referenceMixinContext2 }, new[] { typeof (DateTime) });

      var serializer = new CodeGenerationClassContextSerializer ();
      referenceClassContext.Serialize (serializer);
      var expression = serializer.GetConstructorInvocationExpression();

      var compiledExpression = Expression.Lambda<Func<ClassContext>> (expression).Compile ();
      var result = compiledExpression ();
      Assert.That (result, Is.EqualTo (referenceClassContext));
    }
  }
}
