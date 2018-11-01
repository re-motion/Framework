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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.FluentBuilders
{
  [TestFixture]
  public class InheritanceResolvingClassContextBuilderTest
  {
    private ClassContext _parentContextWithoutBuilder;
    private ClassContextCollection _parentContexts;

    private ClassContextBuilder _classContextBuilderWithParent;
    private ClassContext _parentContextWithBuilder;
    private ClassContextBuilder _classContextBuilderWithIndirectParent;
    private ClassContextBuilder _classContextBuilderWithoutParent;

    private Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> _buildersWithParentContexts;
    
    private IMixinInheritancePolicy _inheritancePolicyMock;
    private ClassContext _inheritedContext;

    private InheritanceResolvingClassContextBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      _classContextBuilderWithParent = new ClassContextBuilder (typeof (NullTarget));
      _classContextBuilderWithParent.AddMixin (typeof (NullMixin2));
      _parentContextWithBuilder = ClassContextObjectMother.Create(typeof (NullTarget), typeof (NullMixin));

      _classContextBuilderWithIndirectParent = new ClassContextBuilder (typeof (DerivedNullTarget));
      
      _classContextBuilderWithoutParent = new ClassContextBuilder (typeof (BaseType4));
      _classContextBuilderWithParent.AddMixin (typeof (BT4Mixin1));

      _buildersWithParentContexts = new Dictionary<Type, Tuple<ClassContextBuilder, ClassContext>> ();
      _buildersWithParentContexts.Add (_classContextBuilderWithParent.TargetType, Tuple.Create (_classContextBuilderWithParent, _parentContextWithBuilder));
      _buildersWithParentContexts.Add (_classContextBuilderWithoutParent.TargetType, Tuple.Create (_classContextBuilderWithoutParent, (ClassContext) null));

      _parentContextWithoutBuilder = ClassContextObjectMother.Create(typeof (BaseType1));
      _parentContexts = new ClassContextCollection (_parentContextWithoutBuilder, _parentContextWithBuilder);

      _inheritancePolicyMock = MockRepository.GenerateMock<IMixinInheritancePolicy> ();
      _inheritedContext = ClassContextObjectMother.Create(typeof (object), typeof (NullMixin));

      var classContextBuilders = new[] { _classContextBuilderWithoutParent, _classContextBuilderWithIndirectParent, _classContextBuilderWithParent };
      _builder = new InheritanceResolvingClassContextBuilder (classContextBuilders, _parentContexts, _inheritancePolicyMock);
    }

    [Test]
    public void Build_WithCachedContext ()
    {
      var result = _builder.Build (typeof (BaseType1));
      Assert.That (result, Is.SameAs (_parentContextWithoutBuilder));
    }

    [Test]
    public void Build_WithoutCachedContext_UsesInheritancePolicy ()
    {
      var BuildMethod = typeof (InheritanceResolvingClassContextBuilder).GetMethod ("Build");
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (
              Arg.Is (typeof (BaseType2)), 
              Arg<Func<Type, ClassContext>>.Matches (func => func.Method.Equals (BuildMethod))))
          .Return (new ClassContext[0]);

      _inheritancePolicyMock.Replay ();

      _builder.Build (typeof (BaseType2));

      _inheritancePolicyMock.VerifyAllExpectations ();
    }

    [Test]
    public void Build_WithoutCachedContext_NoBuilder_ReturnsCombinedInherited ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT2Mixin1));
      var classContext2 = ClassContextObjectMother.Create(typeof (BaseType2), typeof (NullMixin));

      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new[] { classContext1, classContext2 } );
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (typeof (BaseType2));
      Assert.That (result, Is.EqualTo (ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT2Mixin1), typeof (NullMixin))));
    }

    [Test]
    public void Build_WithoutCachedContext_NoBuilder_WithoutInherited ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0]);
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (typeof (BaseType2));
      Assert.That (result, Is.EqualTo (ClassContextObjectMother.Create(typeof (BaseType2))));
    }

    [Test]
    public void Build_WithoutCachedContext_WithBuilder ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0]);
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (_classContextBuilderWithoutParent.TargetType);
      Assert.That (result, Is.EqualTo (_classContextBuilderWithoutParent.BuildClassContext()));
    }

    [Test]
    public void Build_WithoutCachedContext_WithBuilder_WithParent ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0]);
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (_classContextBuilderWithParent.TargetType);
      Assert.That (result, Is.EqualTo (_classContextBuilderWithParent.BuildClassContext (new[] { _parentContextWithBuilder })));
    }

    [Test]
    public void Build_WithoutCachedContext_WithBuilder_WithIndirectParent ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0]);
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (_classContextBuilderWithIndirectParent.TargetType);
      Assert.That (result, Is.EqualTo (_classContextBuilderWithIndirectParent.BuildClassContext (new[] { _parentContextWithBuilder })));
    }

    [Test]
    public void Build_WithoutCachedContext_WithBuilder_WithInherited ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new[] { _inheritedContext } );
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (_classContextBuilderWithoutParent.TargetType);
      Assert.That (result, Is.EqualTo (_classContextBuilderWithoutParent.BuildClassContext (new[] { _inheritedContext })));
    }

    [Test]
    public void Build_WithoutCachedContext_WithBuilder_WithInherited_AndParent ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new[] { _inheritedContext });
      _inheritancePolicyMock.Replay ();

      var result = _builder.Build (_classContextBuilderWithParent.TargetType);
      Assert.That (result, Is.EqualTo (_classContextBuilderWithParent.BuildClassContext (new[] { _inheritedContext, _parentContextWithBuilder })));
    }

    [Test]
    public void Build_WithoutCachedContext_CachesResult ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0]);
      _inheritancePolicyMock.Replay ();

      var result1 = _builder.Build (_classContextBuilderWithoutParent.TargetType);
      var result2 = _builder.Build (_classContextBuilderWithoutParent.TargetType);
      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void BuildAll ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0])
          .Repeat.Any();
      _inheritancePolicyMock.Replay ();

      var result = _builder.BuildAll ();

      var expectedContextWithParent = _classContextBuilderWithParent.BuildClassContext (new[] { _parentContextWithBuilder });
      var expectedContextWithIndirectParent = _classContextBuilderWithIndirectParent.BuildClassContext (new[] { _parentContextWithBuilder });
      var expectedContextWithoutParent = _classContextBuilderWithoutParent.BuildClassContext (new ClassContext[0]);
      Assert.That (result.ToArray (), Is.EquivalentTo (new[] { expectedContextWithParent, expectedContextWithIndirectParent, expectedContextWithoutParent }));
    }

    [Test]
    public void BuildAllAndCombineWithParentContexts ()
    {
      _inheritancePolicyMock
          .Expect (mock => mock.GetClassContextsToInheritFrom (Arg<Type>.Is.Anything, Arg<Func<Type, ClassContext>>.Is.Anything))
          .Return (new ClassContext[0])
          .Repeat.Any ();
      _inheritancePolicyMock.Replay ();

      var result = _builder.BuildAllAndCombineWithParentContexts ();

      var expectedContextWithParent = _classContextBuilderWithParent.BuildClassContext (new[] { _parentContextWithBuilder });
      var expectedContextWithIndirectParent = _classContextBuilderWithIndirectParent.BuildClassContext (new[] { _parentContextWithBuilder });
      var expectedContextWithoutParent = _classContextBuilderWithoutParent.BuildClassContext (new ClassContext[0]);
      Assert.That (result.ToArray (), Is.EquivalentTo (new[] { _parentContextWithoutBuilder, expectedContextWithParent, expectedContextWithIndirectParent, expectedContextWithoutParent }));
    }
  }
}
