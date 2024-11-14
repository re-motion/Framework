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
using Remotion.BuildScript.Test;
using Remotion.BuildScript.Test.Dimensions;

namespace Customizations;

public class EnforcedLocalMachineExecutionRuntimes : ExecutionRuntimes, IRequiresTestParameters
{
  public new static ExecutionRuntimes EnforcedLocalMachine ()
  {
    return ExecutionRuntimes.EnforcedLocalMachine;
  }

  public new static ExecutionRuntimes EnforcedLocalMachine (DockerExecutionRuntimes dockerExecutionRuntimes)
  {
    ArgumentNullException.ThrowIfNull(dockerExecutionRuntimes);

    return new EnforcedLocalMachineExecutionRuntimes(nameof(EnforcedLocalMachine), dockerExecutionRuntimes);
  }

  public DockerExecutionRuntimes DockerExecutionRuntime { get; }

  public EnforcedLocalMachineExecutionRuntimes (string value, DockerExecutionRuntimes dockerExecutionRuntime)
      : base(value)
  {
    ArgumentNullException.ThrowIfNull(dockerExecutionRuntime);

    DockerExecutionRuntime = dockerExecutionRuntime;
  }

  public void ConfigureTestParameters (TestParameterBuilder builder)
  {
    DockerExecutionRuntime.ConfigureTestParameters(builder);
  }

  public override string ToString () => $"{Value}({DockerExecutionRuntime})";
}
