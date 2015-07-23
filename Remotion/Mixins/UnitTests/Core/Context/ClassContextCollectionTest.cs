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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class ClassContextCollectionTest
  {
    private ClassContext _ccObjectWithMixin;
    private ClassContext _ccString;
    private ClassContextCollection _collectionWithObjectAndString;

    private ClassContext _ccListOfT;
    private ClassContext _ccListOfString;

    private ClassContextCollection _emptyCollection;

    [SetUp]
    public void SetUp ()
    {
      _ccObjectWithMixin = ClassContextObjectMother.Create(typeof (object), typeof (NullMixin2));
      _ccString = ClassContextObjectMother.Create(typeof (string));
      _collectionWithObjectAndString = new ClassContextCollection (_ccObjectWithMixin, _ccString);

      _ccListOfT = ClassContextObjectMother.Create(typeof (List<>));
      _ccListOfString = ClassContextObjectMother.Create(typeof (List<string>));

      _emptyCollection = new ClassContextCollection();
    }

    [Test]
    public void Initialization_ParamsArray ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString);
      Assert.That (collection, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void Initialization_Enumerable ()
    {
      var collection = new ClassContextCollection (((IEnumerable<ClassContext>) new[] { _ccObjectWithMixin, _ccString }));
      Assert.That (collection, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void GetExact ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.That (collection.GetExact (typeof (object)), Is.SameAs (_ccObjectWithMixin));
      Assert.That (collection.GetExact (typeof (string)), Is.SameAs (_ccString));
      Assert.That (collection.GetExact (typeof (int)), Is.Null);
      Assert.That (collection.GetExact (typeof (List<>)), Is.SameAs (_ccListOfT));
      Assert.That (collection.GetExact (typeof (List<int>)), Is.Null);
      Assert.That (collection.GetExact (typeof (List<string>)), Is.SameAs (_ccListOfString));
    }

    [Test]
    public void GetWithInheritance_Simple ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.That (collection.GetWithInheritance (typeof (object)), Is.SameAs (_ccObjectWithMixin));
      Assert.That (collection.GetWithInheritance (typeof (string)), Is.SameAs (_ccString));
      Assert.That (collection.GetWithInheritance (typeof (List<>)), Is.SameAs (_ccListOfT));
      Assert.That (collection.GetWithInheritance (typeof (List<string>)), Is.SameAs (_ccListOfString));
    }

    [Test]
    public void GetWithInheritance_Null ()
    {
      Assert.That (_emptyCollection.GetWithInheritance (typeof (int)), Is.Null);
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromBaseType ()
    {
      ClassContext inherited1 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.That (inherited1, Is.Not.Null);
      Assert.That (inherited1.Type, Is.EqualTo (typeof (ClassContextCollectionTest)));
      Assert.That (inherited1.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);

      ClassContext inherited2 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));
      Assert.That (inherited2, Is.EqualTo (inherited1));

      ClassContext inherited3 = _collectionWithObjectAndString.GetWithInheritance (typeof (int));
      Assert.That (inherited3, Is.Not.Null);
      Assert.That (inherited3.Type, Is.EqualTo (typeof (int)));
      Assert.That (inherited3.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterface ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      var collection = new ClassContextCollection (classContext);

      ClassContext inherited = collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.That (inherited, Is.Not.Null);
      Assert.That (inherited.Type, Is.EqualTo (typeof (ClassWithMixedInterface)));
      Assert.That (inherited.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (inherited.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromInterfaceAndBase ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof (IMixedInterface), typeof (NullMixin), typeof (NullMixin2));
      var classContext2 = ClassContextObjectMother.Create(typeof (object), typeof (NullMixin3));

      var collection = new ClassContextCollection (classContext1, classContext2);

      ClassContext inherited = collection.GetWithInheritance (typeof (ClassWithMixedInterface));
      Assert.That (inherited, Is.Not.Null);
      Assert.That (inherited.Type, Is.EqualTo (typeof (ClassWithMixedInterface)));
      Assert.That (inherited.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (inherited.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (inherited.Mixins.ContainsKey (typeof (NullMixin3)), Is.True);
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinition ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof (List<>), typeof (NullMixin3));
      var classContext2 = ClassContextObjectMother.Create(typeof (List<string>), typeof (NullMixin4));

      var collection = new ClassContextCollection (classContext1, classContext2);

      ClassContext inherited4 = collection.GetWithInheritance (typeof (List<int>));
      Assert.That (inherited4, Is.Not.Null);
      Assert.That (inherited4.Type, Is.EqualTo (typeof (List<int>)));
      Assert.That (inherited4.Mixins.ContainsKey (typeof (NullMixin3)), Is.True);
      Assert.That (inherited4.Mixins.ContainsKey (typeof (NullMixin4)), Is.False);
    }

    [Test]
    public void GetWithInheritance_Inheritance_FromGenericTypeDefinitionAndBase ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof (List<>), typeof (NullMixin3));
      var classContext2 = ClassContextObjectMother.Create(typeof (List<string>), typeof (NullMixin4));
      var classContext3 = ClassContextObjectMother.Create(typeof (object), typeof (NullMixin2));

      var collection = new ClassContextCollection (classContext1, classContext2, classContext3);

      ClassContext inherited4 = collection.GetWithInheritance (typeof (List<int>));
      Assert.That (inherited4, Is.Not.Null);
      Assert.That (inherited4.Type, Is.EqualTo (typeof (List<int>)));
      Assert.That (inherited4.Mixins.ContainsKey (typeof (NullMixin2)), Is.True);
      Assert.That (inherited4.Mixins.ContainsKey (typeof (NullMixin3)), Is.True);
    }

    [Test]
    public void GetWithInheritance_Cached ()
    {
      Assert.That (_collectionWithObjectAndString.ContainsExact (typeof (ClassContextInheritanceTest)), Is.False);
      
      var inheritedContext1 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));
      var inheritedContext2 = _collectionWithObjectAndString.GetWithInheritance (typeof (ClassContextCollectionTest));

      Assert.That (inheritedContext1, Is.SameAs (inheritedContext2));
    }

    [Test]
    public void ContainsExact ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.That (collection.ContainsExact (typeof (object)), Is.True);
      Assert.That (collection.ContainsExact (typeof (string)), Is.True);
      Assert.That (collection.ContainsExact (typeof (int)), Is.False);
      Assert.That (collection.ContainsExact (typeof (List<>)), Is.True);
      Assert.That (collection.ContainsExact (typeof (List<int>)), Is.False);
      Assert.That (collection.ContainsExact (typeof (List<string>)), Is.True);
    }

    [Test]
    public void ContainsWithInheritance ()
    {
      var collection = new ClassContextCollection (_ccObjectWithMixin, _ccString, _ccListOfT, _ccListOfString);

      Assert.That (collection.ContainsWithInheritance (typeof (object)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (string)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (ClassContextCollectionTest)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (int)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (List<>)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (List<int>)), Is.True);
      Assert.That (collection.ContainsWithInheritance (typeof (List<string>)), Is.True);

      Assert.That (_emptyCollection.ContainsWithInheritance (typeof (int)), Is.False);
    }

    [Test]
    public void GetEnumerator ()
    {
      var enumerable = (IEnumerable<ClassContext>) _collectionWithObjectAndString;
      Assert.That (enumerable.ToArray(), Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString }));
    }

    [Test]
    public void NonGeneric_GetEnumerator ()
    {
      var enumerable = (IEnumerable) _collectionWithObjectAndString;
      var enumerator = enumerable.GetEnumerator ();
      Assert.That (enumerator.MoveNext(), Is.True);
    }

    [Test]
    public void CopyTo ()
    {
      var classContexts = new ClassContext[5];
      _collectionWithObjectAndString.CopyTo (classContexts, 2);
      Assert.That (classContexts, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString, null, null, null }));
      Assert.That (classContexts[0], Is.Null);
      Assert.That (classContexts[1], Is.Null);
      Assert.That (classContexts[4], Is.Null);
    }

    [Test]
    public void NonGeneric_CopyTo ()
    {
      var classContexts = new object[5];
      ((ICollection) _collectionWithObjectAndString).CopyTo (classContexts, 1);
      Assert.That (classContexts, Is.EquivalentTo (new[] { _ccObjectWithMixin, _ccString, null, null, null }));
    }

    [Test]
    public void SyncRoot ()
    {
      Assert.That (((ICollection) _collectionWithObjectAndString).SyncRoot, Is.Not.Null);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (((ICollection) _collectionWithObjectAndString).IsSynchronized, Is.False);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (((ICollection<ClassContext>) _collectionWithObjectAndString).IsReadOnly, Is.True);
    }

    [Test]
    public void Contains ()
    {
      var cc3 = ClassContextObjectMother.Create(typeof (int));
      var cc4 = ClassContextObjectMother.Create(typeof (object), typeof (NullMixin));

      Assert.That (_collectionWithObjectAndString.Contains (_ccObjectWithMixin), Is.True);
      Assert.That (_collectionWithObjectAndString.Contains (_ccString), Is.True);
      Assert.That (_collectionWithObjectAndString.Contains (cc3), Is.False);
      Assert.That (_collectionWithObjectAndString.Contains (cc4), Is.False);
    }
  }
}
