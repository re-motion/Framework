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
using Remotion.Tools.Console;
using Remotion.Utilities;
using Remotion.Tools;

namespace Remotion.Mixins.MixerTools
{
  public class MixerRunner : CustomAppContextRunnerBase
  {
    private readonly MixerParameters _parameters;

    public MixerRunner (MixerParameters parameters)
        : base(ArgumentUtility.CheckNotNull("parameters", parameters).BaseDirectory, parameters.ConfigFile)
    {
      _parameters = parameters;
    }

    protected override void RunImplementation ()
    {
      ConfigureLogging();

      Mixer mixer = CreateMixer();

      try
      {
        mixer.PrepareOutputDirectory();
        mixer.Execute(MixinConfiguration.ActiveConfiguration);
      }
      catch (Exception ex)
      {
        using (ConsoleUtility.EnterColorScope(ConsoleColor.Red, null))
        {
          Console.WriteLine(ex.Message);
        }
      }
    }

    private void ConfigureLogging ()
    {
      //TODO: RM-9195
      // if (_parameters.Verbose)
      // {
      //   Configure console logging 
      //   log4net.Config.BaseConfigurator.Configure();
      // }
      // else
      // {
      //   var mixerLoggers = from t in AssemblyTypeCache.GetTypes(typeof(Mixer).Assembly)
      //       where t.Namespace == typeof(Mixer).GetNamespaceChecked()
      //       select LogManager.GetLogger(t);
      //   var logThresholds = from l in mixerLoggers
      //       select new LogThreshold(l, LogLevel.Info);
      //   LogManager.InitializeConsole(LogLevel.Warn, logThresholds.ToArray());
      //   log4net.Config.BaseConfigurator.Configure();
      // }
    }

    private Mixer CreateMixer ()
    {
      var mixer = Mixer.Create(_parameters.AssemblyName, _parameters.AssemblyOutputDirectory, _parameters.DegreeOfParallelism);

      mixer.ValidationErrorOccurred += Mixer_ValidationErrorOccurred;
      mixer.ErrorOccurred += Mixer_ErrorOccurred;
      return mixer;
    }

    private void Mixer_ValidationErrorOccurred (object sender, ValidationErrorEventArgs e)
    {
      using (ConsoleUtility.EnterColorScope(ConsoleColor.Red, null))
      {
        Console.WriteLine(e.ValidationException.Message);
      }
    }

    void Mixer_ErrorOccurred (object sender, ErrorEventArgs e)
    {
      using (ConsoleUtility.EnterColorScope(ConsoleColor.Red, null))
      {
        Console.WriteLine(e.Exception.ToString());
      }
    }
  }
}
