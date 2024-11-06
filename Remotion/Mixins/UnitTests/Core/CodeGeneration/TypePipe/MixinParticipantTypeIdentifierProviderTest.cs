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
using Remotion.TypePipe.Dlr.Ast;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TypePipe
{
  [TestFixture]
  public class MixinParticipantTypeIdentifierProviderTest
  {
    private MixinParticipantTypeIdentifierProvider _provider;

    private ClassContext _classContext;

    [SetUp]
    public void SetUp ()
    {
      _provider = new MixinParticipantTypeIdentifierProvider();

      var mixinContext = new MixinContext(
          MixinKind.Extending,
          typeof(int),
          MemberVisibility.Private,
          new Type[0],
          new MixinContextOrigin("some kind", GetType().Assembly, "some location"));
      _classContext = ClassContextObjectMother.Create(typeof(string), mixinContext);
    }

    [Test]
    public void GetID_ForMixedClass_ReturnsClassContext ()
    {
      using (new MixinConfiguration(new ClassContextCollection(_classContext)).EnterScope())
      {
        var result = _provider.GetID(typeof(string));

        Assert.That(result, Is.SameAs(_classContext));
      }
    }

    [Test]
    public void GetID_ForNonMixedClass_ReturnsNull ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        var result = _provider.GetID(typeof(string));

        Assert.That(result, Is.Null);
      }
    }

    [Test]
    public void GetExpression_ReturnsExpressionConstructingClassContext ()
    {
      var result = _provider.GetExpression(_classContext);

      Assert.That(result.NodeType, Is.EqualTo(ExpressionType.Convert));
      var inner = ((UnaryExpression)result).Operand;

      // Must _not_ be a ConstantExpression containing the ClassContext, but instead a NewExpression constructing the ClassContext from scratch.
      Assert.That(inner.NodeType, Is.EqualTo(ExpressionType.New));

      var compiledResult = Expression.Lambda<Func<object>>(result).Compile();
      var evaluatedResult = compiledResult();

      Assert.That(evaluatedResult, Is.EqualTo(_classContext));
    }
  }
}
