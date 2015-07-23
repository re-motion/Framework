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
using Remotion.Utilities;

namespace Remotion.Extensions.UnitTests.Utilities
{

[TestFixture]
public class WorkContextTest
{
  enum ThrowLocation { main_inside, main_outside, sub1_inside, sub1_outside, sub2_inside, sub2_outside, sub2_1_inside}

  // use inner catch

  [Test]
  public void TestCatchMainInside ()
  {
    Assert.That (PerformTest (ThrowLocation.main_inside, true), Is.EqualTo ("? main"));
  }

  [Test]
  public void TestCatchMainOutside ()
  {
    Assert.That (PerformTest (ThrowLocation.main_outside, true), Is.EqualTo (""));
  }

  [Test]
  public void TestCatchSub1Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub1_inside, true), Is.EqualTo ("main\n" + 
                                                                            "? sub1"));
  }

  [Test]
  public void TestCatchSub1Outside()
  {
    Assert.That (PerformTest (ThrowLocation.sub1_outside, true), Is.EqualTo ("main"));
  }

  [Test]
  public void TestCatchSub2Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_inside, true), Is.EqualTo ("main\n" + 
                                                                            "? sub2"));
  }

  [Test]
  public void TestCatchSub2Outside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_outside, true), Is.EqualTo ("main"));
  }

  [Test]
  public void TestCatchSub2_1Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_1_inside, true), Is.EqualTo ("main\n" + 
                                                                              "? sub2\n" + 
                                                                              "? sub2.1"));
  }

  // do not use inner catch

  [Test]
  public void TestNoCatchMainInside ()
  {
    Assert.That (PerformTest (ThrowLocation.main_inside, false), Is.EqualTo ("? main"));
  }

  [Test]
  public void TestNoCatchMainOutside ()
  {
    Assert.That (PerformTest (ThrowLocation.main_outside, false), Is.EqualTo (""));
  }

  [Test]
  public void TestNoCatchSub1Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub1_inside, false), Is.EqualTo ("? main\n" + 
                                                                             "? sub1"));
  }

  [Test]
  public void TestNoCatchSub1Outside()
  {
    Assert.That (PerformTest (ThrowLocation.sub1_outside, false), Is.EqualTo ("? main"));
  }

  [Test]
  public void TestNoCatchSub2Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_inside, false), Is.EqualTo ("? main\n" + 
                                                                             "? sub2"));
  }

  [Test]
  public void TestNoCatchSub2Outside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_outside, false), Is.EqualTo ("? main"));
  }

  [Test]
  public void TestNoCatchSub2_1Inside()
  {
    Assert.That (PerformTest (ThrowLocation.sub2_1_inside, false), Is.EqualTo ("? main\n" + 
                                                                               "? sub2\n" + 
                                                                               "? sub2.1"));
  }

  private string PerformTest (ThrowLocation location, bool catchInInnerHandler)
  {
    try
    {
      using (WorkContext ctxMain = WorkContext.EnterNew ("main"))
      {
        try
        {
          using (WorkContext ctxSub1 = WorkContext.EnterNew ("sub1"))
          {
            if (location == ThrowLocation.sub1_inside) throw new Exception (location.ToString());
            ctxSub1.Done();
          }
          if (location == ThrowLocation.sub1_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assert.That (e.Message, Is.EqualTo (location.ToString()));
          return WorkContext.Stack.ToString();
        }
        try
        {
          using (WorkContext ctxSub2 = WorkContext.EnterNew ("sub2"))
          {
            using (WorkContext ctxSub2_1 = WorkContext.EnterNew ("sub2.1"))
            {
              if (location == ThrowLocation.sub2_1_inside) throw new Exception (location.ToString());
              ctxSub2_1.Done();
            }
            if (location == ThrowLocation.sub2_inside) throw new Exception (location.ToString());
            ctxSub2.Done();
          }
          if (location == ThrowLocation.sub2_outside) throw new Exception (location.ToString());
        }
        catch (Exception e)
        {
          if (!catchInInnerHandler)
            throw;
          Assert.That (e.Message, Is.EqualTo (location.ToString()));
          return WorkContext.Stack.ToString();
        }
        if (location == ThrowLocation.main_inside) throw new Exception (location.ToString());
        ctxMain.Done();
      }
      if (location == ThrowLocation.main_outside) throw new Exception (location.ToString());
      return null;
    }
    catch (Exception e)
    {
      Assert.That (e.Message, Is.EqualTo (location.ToString()));
      return WorkContext.Stack.ToString();
    }
  }
}

}
