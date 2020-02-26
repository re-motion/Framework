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
using System.Runtime.Serialization;

namespace Remotion.Web.Development.WebTesting.Accessibility.AxeJsonResultDtos
{
  /// <summary>
  /// Represents information about the browser used during the AXE analysis.
  /// </summary>
  [DataContract]
  public class AxeTestEnvironment
  {
    /// <summary>Webdriver that was used.</summary>
    [DataMember (Name = "userAgent")]
    public string UserAgent { get; set; }

    /// <summary>Width of the window.</summary>
    [DataMember (Name = "windowWidth")]
    public int WindowWidth { get; set; }

    /// <summary>Height of the window.</summary>
    [DataMember (Name = "windowHeight")]
    public int WindowHeight { get; set; }

    /// <summary>Orientation angle of the screen.</summary>
    [DataMember (Name = "orientationAngle")]
    public int OrientationAngle { get; set; }

    /// <summary>Orientation type of the screen.</summary>
    [DataMember (Name = "orientationType")]
    public string OrientationType { get; set; }
  }
}