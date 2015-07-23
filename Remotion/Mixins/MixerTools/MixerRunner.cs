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
using System.IO;
using System.Linq;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.Tools;
using Remotion.Tools.Console;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTools
{
  [Serializable]
  public class MixerRunner : AppDomainRunnerBase
  {
    public static AppDomainSetup CreateAppDomainSetup (MixerParameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      var setup = new AppDomainSetup
                  {
                      ApplicationName = "Mixer",
                      ApplicationBase = parameters.BaseDirectory
                  };

      if (!string.IsNullOrEmpty (parameters.ConfigFile))
      {
        setup.ConfigurationFile = parameters.ConfigFile;
        if (!File.Exists (setup.ConfigurationFile))
        {
          throw new FileNotFoundException (
              string.Format (
                  "The configuration file supplied by the 'config' parameter was not found.\r\nFile: {0}",
                  setup.ConfigurationFile),
              setup.ConfigurationFile);
        }
      }
      return setup;
    }

    private readonly MixerParameters _parameters;

    public MixerRunner (MixerParameters parameters)
        : base (CreateAppDomainSetup (ArgumentUtility.CheckNotNull ("parameters", parameters)))
    {
      _parameters = parameters;
    }

    protected override void CrossAppDomainCallbackHandler ()
    {
      ConfigureLogging();

      Mixer mixer = CreateMixer();

      try
      {
        mixer.PrepareOutputDirectory ();
        mixer.Execute (MixinConfiguration.ActiveConfiguration);
      }
      catch (Exception ex)
      {
        using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
        {
          Console.WriteLine (ex.Message);
        }
      }
    }

    private void ConfigureLogging ()
    {
      if (_parameters.Verbose)
      {
        LogManager.InitializeConsole();
      }
      else
      {
        var mixerLoggers = from t in AssemblyTypeCache.GetTypes (typeof (Mixer).Assembly)
                           where t.Namespace == typeof (Mixer).Namespace
                           select LogManager.GetLogger (t);
        var logThresholds = from l in mixerLoggers
                            select new LogThreshold (l, LogLevel.Info);
        LogManager.InitializeConsole (LogLevel.Warn, logThresholds.ToArray ());
      }
    }

    private Mixer CreateMixer ()
    {
      var mixer = Mixer.Create (_parameters.AssemblyName, _parameters.AssemblyOutputDirectory, _parameters.DegreeOfParallelism);
      
      mixer.ValidationErrorOccurred += Mixer_ValidationErrorOccurred;
      mixer.ErrorOccurred += Mixer_ErrorOccurred;
      return mixer;
    }

    private void Mixer_ValidationErrorOccurred (object sender, ValidationErrorEventArgs e)
    {
      using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
      {
        Console.WriteLine (e.ValidationException.Message);
      }
    }

    void Mixer_ErrorOccurred (object sender, ErrorEventArgs e)
    {
      using (ConsoleUtility.EnterColorScope (ConsoleColor.Red, null))
      {
        Console.WriteLine (e.Exception.ToString ());
      }
    }
  }
}
