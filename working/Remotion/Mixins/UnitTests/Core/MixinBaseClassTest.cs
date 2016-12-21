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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class MixinBaseClassTest
  {
    public class MixinWithOnInitialize1 : Mixin<object>
    {
      public object ThisValue;

      public MixinWithOnInitialize1()
      {
        try
        {
          object t = Target;
          Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }
        Assert.That (ThisValue, Is.Null);
      }

      protected override void OnInitialized ()
      {
        Assert.That (Target, Is.Not.Null);
        ThisValue = Target;
        base.OnInitialized();
      }
    }

    [Test]
    public void ThisAccessInCtorAndInitialize()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithOnInitialize1));
      MixinWithOnInitialize1 mixin = Mixin.Get<MixinWithOnInitialize1> (bt1);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.ThisValue, Is.Not.Null);
    }

    private TTargetType CreateMixedObject<TTargetType> (params Type[] types)
    {
      using (MixinConfiguration.BuildNew ().ForClass<TTargetType> ().AddMixins (types).EnterScope ())
      {
        return ObjectFactory.Create<TTargetType> (ParamList.Empty);
      }
    }

    public class MixinWithOnInitialize2 : Mixin<object, IBaseType2>
    {
      public object ThisValue;
      public object BaseValue;

      public MixinWithOnInitialize2 ()
      {
        try
        {
          object t = Target;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        try
        {
          object t = Next;
          Assert.Fail ("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException)
        {
          // good
        }
        catch (Exception e)
        {
          Assert.Fail ("Expected InvalidOperationException, but was: " + e);
        }

        Assert.That (ThisValue, Is.Null);
        Assert.That (BaseValue, Is.Null);
      }

      protected override void OnInitialized ()
      {
        Assert.That (Target, Is.Not.Null);
        Assert.That (Next, Is.Not.Null);
        ThisValue = Target;
        BaseValue = Next;
        base.OnInitialized ();
      }
    }

    [Test]
    public void BaseAccessInCtorAndInitialize ()
    {
      BaseType2 bt2 = CreateMixedObject<BaseType2> (typeof (MixinWithOnInitialize2));
      MixinWithOnInitialize2 mixin = Mixin.Get<MixinWithOnInitialize2> (bt2);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.ThisValue, Is.Not.Null);
      Assert.That (mixin.BaseValue, Is.Not.Null);
    }
  }
}
