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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Serialization;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixedTypeAttributeTest
  {
    [Test]
    public void FromAttributeApplication ()
    {
      var attribute = ((ConcreteMixedTypeAttribute[]) 
          typeof (LoadableConcreteMixedTypeForBaseType1).GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Single();
      var classContext = attribute.GetClassContext ();

      var expectedContext = new ClassContext (
          typeof (BaseType1),
          new[]
          {
              new MixinContext (
                  MixinKind.Used,
                  typeof (BT1Mixin1),
                  MemberVisibility.Private,
                  Enumerable.Empty<Type>(),
                  new MixinContextOrigin ("some kind", typeof (object).Assembly, "some location")
              )
          },
          Enumerable.Empty<Type> ());
      Assert.That (classContext, Is.EqualTo (expectedContext));
      Assert.That (classContext.Mixins.Single().Origin, Is.EqualTo (expectedContext.Mixins.Single().Origin));
    }

    [Test]
    public void FromClassContextSimple ()
    {
      var simpleContext = ClassContextObjectMother.Create(typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (simpleContext);

      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (simpleContext));
    }

    [Test]
    public void FromClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddComposedInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .AddMixin (typeof (double)).WithDependency (typeof (int))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);

      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
    }

    [Test]
    public void FromClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddComposedInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      var deserializer = new AttributeClassContextDeserializer (attribute.ClassContextData);
      Assert.That (ClassContext.Deserialize (deserializer), Is.EqualTo (context));
    }

    [Test]
    public void GetClassContextSimple ()
    {
      var simpleContext = ClassContextObjectMother.Create(typeof (object), typeof (string));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (simpleContext);
      ClassContext regeneratedContext = attribute.GetClassContext();

      Assert.That (simpleContext, Is.EqualTo (regeneratedContext));
      Assert.That (simpleContext, Is.Not.SameAs (regeneratedContext));
    }

    [Test]
    public void GetClassContextComplex ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (double))
          .AddComposedInterface (typeof (uint))
          .AddMixin (typeof (string)).WithDependency (typeof (bool))
          .BuildClassContext();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.That (context, Is.EqualTo (regeneratedContext));
      Assert.That (context, Is.Not.SameAs (regeneratedContext));
    }

    [Test]
    public void GetClassContext_MixinKinds ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddComposedInterface (typeof (uint))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending)
          .AddMixin (typeof (double)).OfKind (MixinKind.Used)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();
      Assert.That (regeneratedContext.Mixins[typeof (string)].MixinKind, Is.EqualTo (MixinKind.Extending));
      Assert.That (regeneratedContext.Mixins[typeof (double)].MixinKind, Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void GetClassContext_Dependencies ()
    {
      ClassContext context = new ClassContextBuilder (typeof (int))
          .AddMixin (typeof (object)).OfKind (MixinKind.Extending).WithDependencies (typeof (double), typeof (bool))
          .AddMixin (typeof (string)).OfKind (MixinKind.Extending).WithDependencies (typeof (bool))
          .AddMixin (typeof (int)).OfKind (MixinKind.Extending)
          .BuildClassContext ();

      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);
      ClassContext regeneratedContext = attribute.GetClassContext ();

      Assert.That (regeneratedContext.Mixins.Count, Is.EqualTo (3));

      Assert.That (regeneratedContext.Mixins[typeof (object)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (double), typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (string)].ExplicitDependencies, Is.EqualTo (new object[] { typeof (bool) }));
      Assert.That (regeneratedContext.Mixins[typeof (int)].ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void AttributeWithGenericType ()
    {
      ClassContext context = ClassContextObjectMother.Create(typeof (List<>)).SpecializeWithTypeArguments (new[] {typeof (int)});
      Assert.That (context.Type, Is.EqualTo (typeof (List<int>)));
      ConcreteMixedTypeAttribute attribute = CreateAttribute (context);

      ClassContext regeneratedContext = attribute.GetClassContext ();
      Assert.That (regeneratedContext.Type, Is.EqualTo (typeof (List<int>)));
    }

    [Test]
    public void Roundtrip_WithPublicVisibility_IntegrationTest ()
    {
      var classContext = new ClassContext (
          typeof (BaseType1), 
          new[] { MixinContextObjectMother.Create() },
          Enumerable.Empty<Type>());
      var attribute = CreateAttribute (classContext);
      var classContext2 = attribute.GetClassContext ();

      Assert.That (classContext2, Is.EqualTo (classContext));
    }

    private ConcreteMixedTypeAttribute CreateAttribute (ClassContext context)
    {
      return ConcreteMixedTypeAttribute.FromClassContext (context, context.Mixins.Select (m => m.MixinType).ToArray());
    }
  }
}
