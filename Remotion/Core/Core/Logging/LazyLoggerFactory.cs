using System;
using Microsoft.Extensions.Logging;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using IMicrosoftLogger = Microsoft.Extensions.Logging.ILogger;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Remotion.Logging;

public static class LazyLoggerFactory
{
  public static LazyLogger CreateLogger<T> ()
  {
    return CreateLogger(typeof(T));
  }

  public static LazyLogger CreateLogger (Type type)
  {
    ArgumentUtility.CheckNotNull(nameof(type), type);

    return new LazyLogger(new Lazy<IMicrosoftLogger>(() => SafeServiceLocator.Current.GetInstance<ILoggerFactory>().CreateLogger(type)));
  }
}

public class LazyLogger : IMicrosoftLogger
{
  private readonly Lazy<IMicrosoftLogger> _lazyLogger;

  public LazyLogger (Lazy<IMicrosoftLogger> lazyLogger)
  {
    _lazyLogger = lazyLogger;
  }

  public void Log<TState> (MicrosoftLogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    _lazyLogger.Value.Log(logLevel, eventId, state, exception, formatter);
  }

  public bool IsEnabled (MicrosoftLogLevel logLevel)
  {
    return _lazyLogger.Value.IsEnabled(logLevel);
  }

  public IDisposable? BeginScope<TState> (TState state)
      where TState : notnull
  {
    return _lazyLogger.Value.BeginScope(state);
  }
}
