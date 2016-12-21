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

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  public interface IIndirectRequirementBase1
  {
    string BaseMethod1 ();
  }

  public interface IIndirectRequirement1 : IIndirectRequirementBase1
  {
    string Method1 ();
  }

  public interface IIndirectRequirementBase2
  {
  }

  public interface IIndirectRequirement2 : IIndirectRequirementBase2
  {
  }

  public interface IIndirectRequirementBase3
  {
    string Method3 ();
  }

  public interface IIndirectRequirement3 : IIndirectRequirementBase3
  {
  }

  // Target dependencies can contain unfulfilled dependencies, e.g. IIndirectRequirement2
  public interface IIndirectTargetAggregator : IIndirectRequirement1, IIndirectRequirement2, IIndirectRequirement3
  {
  }

  // Next dependencies cannot contain unfulfilled dependencies, e.g. IIndirectRequirement2
  public interface IIndirectBaseAggregator : IIndirectRequirement1, IIndirectRequirement3
  {
  }

  public class MixinWithIndirectRequirements : Mixin <IIndirectTargetAggregator, IIndirectBaseAggregator>
  {
    public string GetStuffViaThis()
    {
      return Target.Method1() + "-" + Target.BaseMethod1() + "-" + Target.Method3();
    }

    public string GetStuffViaBase()
    {
      return Next.Method1() + "-" + Next.BaseMethod1() + "-" + Next.Method3();
    }
  }

  [Uses (typeof (MixinWithIndirectRequirements))]
  public class ClassImplementingIndirectRequirements : IIndirectRequirement1, IIndirectRequirement3
  {
    public string Method1 ()
    {
      return "ClassImplementingIndirectRequirements.Method1";
    }

    public string BaseMethod1 ()
    {
      return "ClassImplementingIndirectRequirements.BaseMethod1";
    }

    public string Method3 ()
    {
      return "ClassImplementingIndirectRequirements.Method3";

    }
  }
}
