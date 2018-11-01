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
using Remotion.Web.Configuration;

namespace Remotion.Development.Web.UnitTesting.Configuration
{
  public static class WebConfigurationFactory
  {
    public static WebConfiguration GetExecutionEngineWithDefaultWxeHandler ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.DefaultWxeHandler = "WxeHandler.ashx";
      return config;
    }

    public static WebConfiguration GetExecutionEngineUrlMapping ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.UrlMappingFile = @"Res\UrlMapping.xml";
      return config;
    }

    public static WebConfiguration GetExecutionEngineMappingWithNoFilename ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.ExecutionEngine.UrlMappingFile = null;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetDebugLoggingLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Logging;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelDoubleA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
      return config;
    }

    public static WebConfiguration GetDebugExceptionLevelUndefined ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Exception;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
      return config;
    }

    public static WebConfiguration GetLevelA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.A;
      return config;
    }

    public static WebConfiguration GetLevelDoubleA ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.DoubleA;
      return config;
    }

    public static WebConfiguration GetLevelUndefined ()
    {
      WebConfigurationMock config = new WebConfigurationMock ();
      config.Wcag.Debugging = WcagDebugMode.Disabled;
      config.Wcag.ConformanceLevel = WaiConformanceLevel.Undefined;
      return config;
    }
  }
}
