// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Logger
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32;
using System.Collections;
using System.Deployment.Application.Manifest;
using System.Deployment.Internal.Isolation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace System.Deployment.Application
{
  internal class Logger
  {
    protected static Hashtable _loggerCollection = new Hashtable();
    protected static Hashtable _threadLogIdTable = new Hashtable();
    protected static object _logAccessLock = new object();
    protected static bool _detailedLoggingEnabled = PolicyKeys.ProduceDetailedExecutionSectionInLog();
    protected Logger.SourceSection _sources = new Logger.SourceSection();
    protected Logger.IdentitySection _identities = new Logger.IdentitySection();
    protected Logger.SummarySection _summary = new Logger.SummarySection();
    protected Logger.ErrorSection _errors = new Logger.ErrorSection();
    protected Logger.WarningSection _warnings = new Logger.WarningSection();
    protected Logger.PhaseSection _phases = new Logger.PhaseSection();
    protected Logger.ExecutionFlowSection _executionFlow = new Logger.ExecutionFlowSection();
    protected Logger.TransactionSection _transactions = new Logger.TransactionSection();
    protected Logger.LogIdentity _logIdentity = new Logger.LogIdentity();
    protected string _logFilePath;
    protected string _urlName;
    protected Logger.LogFileLocation _logFileLocation;
    protected static object _logFileEncoding;
    protected static object _header;

    protected Logger.TransactionSection Transactions
    {
      get
      {
        return this._transactions;
      }
    }

    protected Logger.ErrorSection Errors
    {
      get
      {
        return this._errors;
      }
    }

    protected Logger.WarningSection Warnings
    {
      get
      {
        return this._warnings;
      }
    }

    protected Logger.PhaseSection Phases
    {
      get
      {
        return this._phases;
      }
    }

    protected Logger.ExecutionFlowSection ExecutionFlow
    {
      get
      {
        return this._executionFlow;
      }
    }

    protected Logger.SourceSection Sources
    {
      get
      {
        return this._sources;
      }
    }

    protected Logger.IdentitySection Identities
    {
      get
      {
        return this._identities;
      }
    }

    protected Logger.SummarySection Summary
    {
      get
      {
        return this._summary;
      }
    }

    protected Logger.LogIdentity Identity
    {
      get
      {
        return this._logIdentity;
      }
    }

    protected string LogFilePath
    {
      get
      {
        if (this._logFilePath == null)
        {
          this._logFilePath = Logger.GetRegitsryBasedLogFilePath();
          if (this._logFilePath == null)
          {
            this._logFilePath = this.GetWinInetBasedLogFilePath();
            if (this._logFilePath != null)
              this._logFileLocation = Logger.LogFileLocation.WinInetCache;
          }
          else
            this._logFileLocation = Logger.LogFileLocation.RegistryBased;
        }
        return this._logFilePath;
      }
    }

    protected static Encoding LogFileEncoding
    {
      get
      {
        if (Logger._logFileEncoding == null)
        {
          Encoding encoding = !PlatformSpecific.OnWin9x ? Encoding.Unicode : Encoding.Default;
          Interlocked.CompareExchange(ref Logger._logFileEncoding, (object) encoding, (object) null);
        }
        return (Encoding) Logger._logFileEncoding;
      }
    }

    protected static Logger.HeaderSection Header
    {
      get
      {
        if (Logger._header == null)
        {
          object obj = (object) new Logger.HeaderSection();
          Interlocked.CompareExchange(ref Logger._header, obj, (object) null);
        }
        return (Logger.HeaderSection) Logger._header;
      }
    }

    protected Logger()
    {
    }

    protected static string GetRegitsryBasedLogFilePath()
    {
      string str = (string) null;
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes\\Software\\Microsoft\\Windows\\CurrentVersion\\Deployment"))
        {
          if (registryKey != null)
            str = registryKey.GetValue("LogFilePath") as string;
        }
      }
      catch (ArgumentException ex)
      {
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (SecurityException ex)
      {
      }
      return str;
    }

    protected string GetWinInetBasedLogFilePath()
    {
      try
      {
        string str = "System_Deployment_Log_";
        if (this.Identities.DeploymentIdentity != null)
          str += this.Identities.DeploymentIdentity.KeyForm;
        string urlName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", new object[2]
        {
          (object) str,
          (object) this.Identity.ToString()
        });
        StringBuilder fileName = new StringBuilder(261);
        if (!NativeMethods.CreateUrlCacheEntry(urlName, 0, "log", fileName, 0))
          return (string) null;
        this._urlName = urlName;
        return fileName.ToString();
      }
      catch (COMException ex)
      {
        return (string) null;
      }
      catch (SEHException ex)
      {
        return (string) null;
      }
      catch (FormatException ex)
      {
        return (string) null;
      }
    }

    protected FileStream CreateLogFileStream()
    {
      FileStream fileStream = (FileStream) null;
      string logFilePath = this.LogFilePath;
      if (logFilePath == null)
        return (FileStream) null;
      for (uint index = 0; index < 1000U; ++index)
      {
        try
        {
          fileStream = this._logFileLocation != Logger.LogFileLocation.RegistryBased ? new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.None) : new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
          break;
        }
        catch (IOException ex)
        {
          if ((int) index == 1000)
            throw;
        }
        Thread.Sleep(20);
      }
      return fileStream;
    }

    protected bool FlushLogs()
    {
      try
      {
        FileStream logFileStream = this.CreateLogFileStream();
        if (logFileStream == null)
          return false;
        StreamWriter streamWriter = new StreamWriter((Stream) logFileStream, Logger.LogFileEncoding);
        streamWriter.WriteLine((object) Logger.Header);
        streamWriter.Write((object) this.Sources);
        streamWriter.Write((object) this.Identities);
        streamWriter.Write((object) this.Summary);
        streamWriter.WriteLine(this.Errors.ErrorSummary);
        streamWriter.WriteLine(this.Transactions.FailureSummary);
        streamWriter.WriteLine((object) this.Warnings);
        streamWriter.WriteLine((object) this.Phases);
        streamWriter.WriteLine((object) this.Errors);
        streamWriter.WriteLine((object) this.Transactions);
        if (Logger._detailedLoggingEnabled)
          streamWriter.WriteLine((object) this.ExecutionFlow);
        streamWriter.Close();
        logFileStream.Close();
      }
      catch (IOException ex)
      {
        return false;
      }
      catch (SecurityException ex)
      {
        return false;
      }
      catch (UnauthorizedAccessException ex)
      {
        return false;
      }
      return true;
    }

    protected void EndLogOperation()
    {
      if (!this.FlushLogs() || this._logFileLocation != Logger.LogFileLocation.WinInetCache)
        return;
      NativeMethods.CommitUrlCacheEntry(this._urlName, this._logFilePath, 0L, 0L, 4U, (string) null, 0, (string) null, (string) null);
    }

    protected static uint GetCurrentLogThreadId()
    {
      return NativeMethods.GetCurrentThreadId();
    }

    protected static Logger GetCurrentThreadLogger()
    {
      Logger logger = (Logger) null;
      uint currentLogThreadId = Logger.GetCurrentLogThreadId();
      lock (Logger._logAccessLock)
      {
        if (Logger._threadLogIdTable.Contains((object) currentLogThreadId))
        {
          Logger.LogIdentity local_4 = (Logger.LogIdentity) Logger._threadLogIdTable[(object) currentLogThreadId];
          if (Logger._loggerCollection.Contains((object) local_4.ToString()))
            logger = (Logger) Logger._loggerCollection[(object) local_4.ToString()];
        }
      }
      return logger;
    }

    protected static Logger GetLogger(Logger.LogIdentity logIdentity)
    {
      Logger logger = (Logger) null;
      lock (Logger._logAccessLock)
      {
        if (Logger._loggerCollection.Contains((object) logIdentity.ToString()))
          logger = (Logger) Logger._loggerCollection[(object) logIdentity.ToString()];
      }
      return logger;
    }

    protected static void AddLogger(Logger logger)
    {
      lock (Logger._logAccessLock)
      {
        if (Logger._loggerCollection.Contains((object) logger.Identity.ToString()))
          return;
        Logger._loggerCollection.Add((object) logger.Identity.ToString(), (object) logger);
      }
    }

    protected static void AddCurrentThreadLogger(Logger logger)
    {
      lock (Logger._logAccessLock)
      {
        if (Logger._threadLogIdTable.Contains((object) logger.Identity.ThreadId))
          Logger._threadLogIdTable.Remove((object) logger.Identity.ThreadId);
        Logger._threadLogIdTable.Add((object) logger.Identity.ThreadId, (object) logger.Identity);
        if (Logger._loggerCollection.Contains((object) logger.Identity.ToString()))
          return;
        Logger._loggerCollection.Add((object) logger.Identity.ToString(), (object) logger);
      }
    }

    protected static void RemoveLogger(Logger.LogIdentity logIdentity)
    {
      lock (Logger._logAccessLock)
      {
        if (!Logger._loggerCollection.Contains((object) logIdentity.ToString()))
          return;
        Logger._loggerCollection.Remove((object) logIdentity.ToString());
      }
    }

    protected static void RemoveCurrentThreadLogger()
    {
      lock (Logger._logAccessLock)
      {
        uint local_2 = Logger.GetCurrentLogThreadId();
        if (!Logger._threadLogIdTable.Contains((object) local_2))
          return;
        Logger.LogIdentity local_3 = (Logger.LogIdentity) Logger._threadLogIdTable[(object) local_2];
        Logger._threadLogIdTable.Remove((object) local_2);
        if (!Logger._loggerCollection.Contains((object) local_3.ToString()))
          return;
        Logger._loggerCollection.Remove((object) local_3.ToString());
      }
    }

    internal static Logger.LogIdentity StartCurrentThreadLogging()
    {
      Logger.EndCurrentThreadLogging();
      Logger logger = new Logger();
      Logger.AddCurrentThreadLogger(logger);
      return logger.Identity;
    }

    internal static void EndCurrentThreadLogging()
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.EndLogOperation();
      Logger.RemoveCurrentThreadLogger();
    }

    internal static Logger.LogIdentity StartLogging()
    {
      Logger logger = new Logger();
      Logger.AddLogger(logger);
      return logger.Identity;
    }

    internal static void EndLogging(Logger.LogIdentity logIdentity)
    {
      try
      {
        Logger logger = Logger.GetLogger(logIdentity);
        if (logger != null)
        {
          lock (logger)
            logger.EndLogOperation();
        }
        Logger.RemoveLogger(logIdentity);
      }
      catch (Exception ex)
      {
        if (!ExceptionUtility.IsHardException(ex))
          return;
        throw;
      }
    }

    internal static void SetSubscriptionUrl(Uri subscriptionUri)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      currentThreadLogger.SetSubscriptionUri(subscriptionUri);
    }

    internal static void SetSubscriptionUrl(Logger.LogIdentity log, Uri subscriptionUri)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      logger.SetSubscriptionUri(subscriptionUri);
    }

    private void SetSubscriptionUri(Uri subscriptionUri)
    {
      lock (this)
        this.Sources.SubscriptionUri = subscriptionUri;
    }

    internal static void SetSubscriptionServerInformation(ServerInformation serverInformation)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Sources.SubscriptionServerInformation = serverInformation;
    }

    internal static void SetSubscriptionUrl(string subscrioptionUrl)
    {
      try
      {
        Logger.SetSubscriptionUrl(new Uri(subscrioptionUrl));
      }
      catch (UriFormatException ex)
      {
      }
    }

    internal static void SetDeploymentProviderUrl(Uri deploymentProviderUri)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Sources.DeploymentProviderUri = deploymentProviderUri;
    }

    internal static void SetDeploymentProviderServerInformation(ServerInformation serverInformation)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Sources.DeploymentProviderServerInformation = serverInformation;
    }

    internal static void SetApplicationUrl(Uri applicationUri)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Sources.ApplicationUri = applicationUri;
    }

    internal static void SetApplicationUrl(Logger.LogIdentity log, Uri applicationUri)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
        logger.Sources.ApplicationUri = applicationUri;
    }

    internal static void SetApplicationServerInformation(ServerInformation serverInformation)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Sources.ApplicationServerInformation = serverInformation;
    }

    internal static void SetTextualSubscriptionIdentity(string textualIdentity)
    {
      try
      {
        Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
        if (currentThreadLogger == null)
          return;
        currentThreadLogger.SetTextualSubscriptionIdentity(new DefinitionIdentity(textualIdentity));
      }
      catch (COMException ex)
      {
      }
      catch (SEHException ex)
      {
      }
    }

    internal static void SetTextualSubscriptionIdentity(Logger.LogIdentity log, string textualIdentity)
    {
      try
      {
        Logger logger = Logger.GetLogger(log);
        if (logger == null)
          return;
        logger.SetTextualSubscriptionIdentity(new DefinitionIdentity(textualIdentity));
      }
      catch (COMException ex)
      {
      }
      catch (SEHException ex)
      {
      }
    }

    internal void SetTextualSubscriptionIdentity(DefinitionIdentity definitionIdentity)
    {
      lock (this)
        this.Identities.DeploymentIdentity = definitionIdentity;
    }

    internal static void SetDeploymentManifest(AssemblyManifest deploymentManifest)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
      {
        if (deploymentManifest.Identity != null)
          currentThreadLogger.Identities.DeploymentIdentity = deploymentManifest.Identity;
        currentThreadLogger.Summary.DeploymentManifest = deploymentManifest;
      }
    }

    internal static void SetDeploymentManifest(Logger.LogIdentity log, AssemblyManifest deploymentManifest)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
      {
        if (deploymentManifest.Identity != null)
          logger.Identities.DeploymentIdentity = deploymentManifest.Identity;
        logger.Summary.DeploymentManifest = deploymentManifest;
      }
    }

    internal static void SetApplicationManifest(AssemblyManifest applicationManifest)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
      {
        if (applicationManifest.Identity != null)
          currentThreadLogger.Identities.ApplicationIdentity = applicationManifest.Identity;
        currentThreadLogger.Summary.ApplicationManifest = applicationManifest;
      }
    }

    internal static void SetApplicationManifest(Logger.LogIdentity log, AssemblyManifest applicationManifest)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
      {
        if (applicationManifest.Identity != null)
          logger.Identities.ApplicationIdentity = applicationManifest.Identity;
        logger.Summary.ApplicationManifest = applicationManifest;
      }
    }

    internal static void AddErrorInformation(string message, Exception exception, DateTime time)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Errors.AddError(message, exception, time);
    }

    internal static void AddErrorInformation(Logger.LogIdentity log, string message, Exception exception, DateTime time)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
        logger.Errors.AddError(message, exception, time);
    }

    internal static void AddWarningInformation(string message, DateTime time)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Warnings.AddWarning(message, time);
    }

    internal static void AddPhaseInformation(string message, DateTime time)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Phases.AddPhaseInformation(message, time);
    }

    internal static void AddMethodCall(string message, DateTime time)
    {
      if (!Logger._detailedLoggingEnabled)
        return;
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.ExecutionFlow.AddMethodCall(message, time);
    }

    internal static void AddMethodCall(Logger.LogIdentity log, string message, DateTime time)
    {
      if (!Logger._detailedLoggingEnabled)
        return;
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
        logger.ExecutionFlow.AddMethodCall(message, time);
    }

    internal static void AddInternalState(string message, DateTime time)
    {
      if (!Logger._detailedLoggingEnabled)
        return;
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.ExecutionFlow.AddInternalState(message, time);
    }

    internal static void AddInternalState(Logger.LogIdentity log, string message, DateTime time)
    {
      if (!Logger._detailedLoggingEnabled)
        return;
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return;
      lock (logger)
        logger.ExecutionFlow.AddInternalState(message, time);
    }

    internal static void AddTransactionInformation(StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time)
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return;
      lock (currentThreadLogger)
        currentThreadLogger.Transactions.AddTransactionInformation(storeOperations, rgDispositions, rgResults, time);
    }

    internal static void AddErrorInformation(string message, Exception exception)
    {
      Logger.AddErrorInformation(message, exception, DateTime.Now);
    }

    internal static void AddErrorInformation(Logger.LogIdentity log, string message, Exception exception)
    {
      Logger.AddErrorInformation(log, message, exception, DateTime.Now);
    }

    internal static void AddErrorInformation(Exception exception, string messageFormat, params object[] args)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat(messageFormat, args);
        Logger.AddErrorInformation(stringBuilder.ToString(), exception, DateTime.Now);
      }
      catch (FormatException ex)
      {
      }
    }

    internal static void AddWarningInformation(string message)
    {
      Logger.AddWarningInformation(message, DateTime.Now);
    }

    internal static void AddPhaseInformation(string message)
    {
      Logger.AddPhaseInformation(message, DateTime.Now);
    }

    internal static void AddMethodCall(string message)
    {
      Logger.AddMethodCall(message, DateTime.Now);
    }

    internal static void AddMethodCall(Logger.LogIdentity log, string message)
    {
      Logger.AddMethodCall(log, message, DateTime.Now);
    }

    internal static void AddMethodCall(string messageFormat, params object[] args)
    {
      if (!Logger._detailedLoggingEnabled)
        return;
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, messageFormat, args);
        Logger.AddMethodCall(stringBuilder.ToString(), DateTime.Now);
      }
      catch (FormatException ex)
      {
      }
    }

    internal static void AddInternalState(Logger.LogIdentity log, string message)
    {
      Logger.AddInternalState(log, message, DateTime.Now);
    }

    internal static void AddInternalState(string message)
    {
      Logger.AddInternalState(message, DateTime.Now);
    }

    internal static void AddPhaseInformation(string messageFormat, params object[] args)
    {
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat(messageFormat, args);
        Logger.AddPhaseInformation(stringBuilder.ToString(), DateTime.Now);
      }
      catch (FormatException ex)
      {
      }
    }

    internal static void AddTransactionInformation(StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults)
    {
      Logger.AddTransactionInformation(storeOperations, rgDispositions, rgResults, DateTime.Now);
    }

    internal static string GetLogFilePath()
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger != null)
        return Logger.GetLogFilePath(currentThreadLogger);
      return (string) null;
    }

    internal static string GetLogFilePath(Logger.LogIdentity log)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger != null)
        return Logger.GetLogFilePath(logger);
      return (string) null;
    }

    internal static string GetLogFilePath(Logger logger)
    {
      if (logger == null)
        return (string) null;
      lock (logger)
        return logger.LogFilePath;
    }

    internal static bool FlushCurrentThreadLogs()
    {
      Logger currentThreadLogger = Logger.GetCurrentThreadLogger();
      if (currentThreadLogger == null)
        return false;
      lock (currentThreadLogger)
        return currentThreadLogger.FlushLogs();
    }

    internal static bool FlushLog(Logger.LogIdentity log)
    {
      Logger logger = Logger.GetLogger(log);
      if (logger == null)
        return false;
      lock (logger)
        return logger.FlushLogs();
    }

    internal static string Serialize(WebRequest httpreq)
    {
      if (httpreq == null)
        return "";
      IWebProxy proxy = httpreq.Proxy;
      StringBuilder stringBuilder = new StringBuilder();
      if (proxy != null)
      {
        stringBuilder.Append(" Proxy.IsByPassed=" + proxy.IsBypassed(httpreq.RequestUri).ToString());
        stringBuilder.Append(", ProxyUri=" + (object) proxy.GetProxy(httpreq.RequestUri));
      }
      else
        stringBuilder.Append(" No proxy set.");
      return stringBuilder.ToString();
    }

    internal static string Serialize(WebResponse response)
    {
      if (response == null)
        return "";
      return "" + "ResponseUri=" + (object) response.ResponseUri;
    }

    internal static string Serialize(TrustManagerContext tmc)
    {
      if (tmc == null)
        return "";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("IgnorePersistedDecision=" + tmc.IgnorePersistedDecision.ToString());
      stringBuilder.Append(", NoPrompt=" + tmc.NoPrompt.ToString());
      stringBuilder.Append(", Persist=" + tmc.Persist.ToString());
      stringBuilder.Append(", PreviousApplicationIdentity = " + (object) tmc.PreviousApplicationIdentity);
      return stringBuilder.ToString();
    }

    protected class LogInformation
    {
      protected string _message = "";
      protected DateTime _time = DateTime.Now;

      public string Message
      {
        get
        {
          return this._message;
        }
      }

      public DateTime Time
      {
        get
        {
          return this._time;
        }
      }

      public LogInformation()
      {
      }

      public LogInformation(string message, DateTime time)
      {
        if (message != null)
          this._message = message;
        this._time = time;
      }
    }

    protected class ErrorInformation : Logger.LogInformation
    {
      protected Exception _exception;

      public string Summary
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorSummary"), new object[1]
          {
            (object) this._message
          });
          for (Exception exception = this._exception; exception != null; exception = exception.InnerException)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorSummaryBullets"), new object[1]
            {
              (object) exception.Message
            });
          return stringBuilder.ToString();
        }
      }

      public ErrorInformation(string message, Exception exception, DateTime time)
        : base(message, time)
      {
        this._exception = exception;
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (Exception exception = this._exception; exception != null; exception = exception.InnerException)
        {
          string str = (string) null;
          if (exception.StackTrace != null)
            str = exception.StackTrace.Replace("   ", "\t\t\t");
          if (exception == this._exception)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorOutermostException"), (object) this.Time.ToString((IFormatProvider) DateTimeFormatInfo.CurrentInfo), (object) Logger.ErrorInformation.GetExceptionType(exception), (object) exception.Message, (object) exception.Source, (object) str);
          else
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IndividualErrorInnerException"), (object) Logger.ErrorInformation.GetExceptionType(exception), (object) exception.Message, (object) exception.Source, (object) str);
        }
        return stringBuilder.ToString();
      }

      private static string GetExceptionType(Exception exception)
      {
        DeploymentException deploymentException = exception as DeploymentException;
        if (deploymentException == null)
          return exception.GetType().ToString();
        if (deploymentException.SubType != ExceptionTypes.Unknown)
          return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ExceptionType"), new object[2]
          {
            (object) deploymentException.GetType().ToString(),
            (object) deploymentException.SubType.ToString()
          });
        return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ExceptionTypeUnknown"), new object[1]
        {
          (object) deploymentException.GetType().ToString()
        });
      }
    }

    protected class TransactionInformation : Logger.LogInformation
    {
      protected ArrayList _operations = new ArrayList();
      protected bool _failed;

      public bool Failed
      {
        get
        {
          return this._failed;
        }
      }

      public string FailureSummary
      {
        get
        {
          if (!this.Failed)
            return Resources.GetString("LogFile_TransactionFailureSummaryNoFailure");
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionFailureSummaryItem"), new object[1]
          {
            (object) this.Time.ToString((IFormatProvider) DateTimeFormatInfo.CurrentInfo)
          });
          foreach (Logger.TransactionInformation.TransactionOperation operation in this._operations)
          {
            if (operation.Failed)
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionFailureSummaryBullets"), new object[1]
              {
                (object) operation.FailureMessage
              });
          }
          return stringBuilder.ToString();
        }
      }

      public TransactionInformation(StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time)
        : base((string) null, time)
      {
        int num = Math.Min(Math.Min(storeOperations.Length, rgDispositions.Length), rgResults.Length);
        for (int index = 0; index < num; ++index)
        {
          Logger.TransactionInformation.TransactionOperation transactionOperation = new Logger.TransactionInformation.TransactionOperation(storeOperations[index], rgDispositions[index], rgResults[index]);
          this._operations.Add((object) transactionOperation);
          if (transactionOperation.Failed)
            this._failed = true;
        }
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionItem"), new object[1]
        {
          (object) this.Time.ToString((IFormatProvider) DateTimeFormatInfo.CurrentInfo)
        });
        foreach (Logger.TransactionInformation.TransactionOperation operation in this._operations)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionBullets"), new object[1]
          {
            (object) operation
          });
        return stringBuilder.ToString();
      }

      public class TransactionOperation
      {
        protected string _message = "";
        protected string _failureMessage = "";
        protected bool _failed;

        public bool Failed
        {
          get
          {
            return this._failed;
          }
        }

        public string FailureMessage
        {
          get
          {
            return this._failureMessage;
          }
        }

        public TransactionOperation(StoreTransactionOperation operation, uint disposition, int hresult)
        {
          this.AnalyzeTransactionOperation(operation, disposition, hresult);
        }

        public override string ToString()
        {
          return this._message;
        }

        protected void AnalyzeTransactionOperation(StoreTransactionOperation operation, uint dispositionValue, int hresult)
        {
          try
          {
            if (operation.Operation == StoreTransactionOperationType.StageComponent)
            {
              StoreOperationStageComponent structure = (StoreOperationStageComponent) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationStageComponent));
              StoreOperationStageComponent.Disposition disposition = (StoreOperationStageComponent.Disposition) dispositionValue;
              string str = disposition.ToString();
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponent"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) Path.GetFileName(structure.ManifestPath));
              if (disposition != StoreOperationStageComponent.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFailure"), new object[1]
              {
                (object) Path.GetFileName(structure.ManifestPath)
              });
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.PinDeployment)
            {
              StoreOperationPinDeployment structure = (StoreOperationPinDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationPinDeployment));
              StoreOperationPinDeployment.Disposition disposition = (StoreOperationPinDeployment.Disposition) dispositionValue;
              string str = disposition.ToString();
              DefinitionAppId definitionAppId = new DefinitionAppId(structure.Application);
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionPinDeployment"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) definitionAppId.ToString());
              if (disposition != StoreOperationPinDeployment.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionPinDeploymentFailure"), new object[1]
              {
                (object) definitionAppId.ToString()
              });
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.UnpinDeployment)
            {
              StoreOperationUnpinDeployment structure = (StoreOperationUnpinDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationUnpinDeployment));
              StoreOperationUnpinDeployment.Disposition disposition = (StoreOperationUnpinDeployment.Disposition) dispositionValue;
              string str = disposition.ToString();
              DefinitionAppId definitionAppId = new DefinitionAppId(structure.Application);
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnPinDeployment"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) definitionAppId.ToString());
              if (disposition != StoreOperationUnpinDeployment.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnPinDeploymentFailure"), new object[1]
              {
                (object) definitionAppId.ToString()
              });
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.InstallDeployment)
            {
              StoreOperationInstallDeployment structure = (StoreOperationInstallDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationInstallDeployment));
              StoreOperationInstallDeployment.Disposition disposition = (StoreOperationInstallDeployment.Disposition) dispositionValue;
              string str = disposition.ToString();
              DefinitionAppId definitionAppId = new DefinitionAppId(structure.Application);
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionInstallDeployment"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) definitionAppId.ToString());
              if (disposition != StoreOperationInstallDeployment.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionInstallDeploymentFailure"), new object[1]
              {
                (object) definitionAppId.ToString()
              });
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.UninstallDeployment)
            {
              StoreOperationUninstallDeployment structure = (StoreOperationUninstallDeployment) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationUninstallDeployment));
              StoreOperationUninstallDeployment.Disposition disposition = (StoreOperationUninstallDeployment.Disposition) dispositionValue;
              string str = disposition.ToString();
              DefinitionAppId definitionAppId = new DefinitionAppId(structure.Application);
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUninstallDeployment"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) definitionAppId.ToString());
              if (disposition != StoreOperationUninstallDeployment.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUninstallDeploymentFailure"), new object[1]
              {
                (object) definitionAppId.ToString()
              });
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.SetDeploymentMetadata)
            {
              StoreOperationSetDeploymentMetadata structure = (StoreOperationSetDeploymentMetadata) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationSetDeploymentMetadata));
              StoreOperationSetDeploymentMetadata.Disposition disposition = (StoreOperationSetDeploymentMetadata.Disposition) dispositionValue;
              string str = disposition.ToString();
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionSetDeploymentMetadata"), new object[3]
              {
                (object) structure.GetType().ToString(),
                (object) str,
                (object) hresult
              });
              if (disposition != StoreOperationSetDeploymentMetadata.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionSetDeploymentMetadataFailure"), new object[0]);
              this._failed = true;
            }
            else if (operation.Operation == StoreTransactionOperationType.StageComponentFile)
            {
              StoreOperationStageComponentFile structure = (StoreOperationStageComponentFile) Marshal.PtrToStructure(operation.Data.DataPtr, typeof (StoreOperationStageComponentFile));
              StoreOperationStageComponentFile.Disposition disposition = (StoreOperationStageComponentFile.Disposition) dispositionValue;
              string str = disposition.ToString();
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFile"), (object) structure.GetType().ToString(), (object) str, (object) hresult, (object) structure.ComponentRelativePath);
              if (disposition != StoreOperationStageComponentFile.Disposition.Failed)
                return;
              this._failureMessage = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionStageComponentFileFailure"), new object[1]
              {
                (object) structure.ComponentRelativePath
              });
              this._failed = true;
            }
            else
              this._message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_TransactionUnknownOperation"), new object[3]
              {
                (object) operation.Operation.GetType().ToString(),
                (object) (uint) operation.Operation,
                (object) hresult
              });
          }
          catch (FormatException ex)
          {
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
    }

    protected class HeaderSection : Logger.LogInformation
    {
      public HeaderSection()
      {
        this._message = Logger.HeaderSection.GenerateLogHeaderText();
      }

      public override string ToString()
      {
        return this._message;
      }

      protected static string GetModulePathInSystemFolder(string moduleName)
      {
        try
        {
          return Path.Combine(Environment.SystemDirectory, moduleName);
        }
        catch (ArgumentException ex)
        {
          return (string) null;
        }
      }

      protected static string GetModulePathInClrFolder(string moduleName)
      {
        try
        {
          return Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), moduleName);
        }
        catch (ArgumentException ex)
        {
          return (string) null;
        }
      }

      protected static string GetModulePath(string moduleName)
      {
        return NativeMethods.GetLoadedModulePath(moduleName) ?? Logger.HeaderSection.GetModulePathInClrFolder(moduleName) ?? Logger.HeaderSection.GetModulePathInSystemFolder(moduleName);
      }

      protected static string GetExecutingAssemblyPath()
      {
        return Assembly.GetExecutingAssembly().Location;
      }

      protected static FileVersionInfo GetVersionInfo(string modulePath)
      {
        FileVersionInfo fileVersionInfo = (FileVersionInfo) null;
        if (modulePath != null)
        {
          if (System.IO.File.Exists(modulePath))
          {
            try
            {
              fileVersionInfo = FileVersionInfo.GetVersionInfo(modulePath);
            }
            catch (FileNotFoundException ex)
            {
            }
          }
        }
        return fileVersionInfo;
      }

      protected static string GenerateLogHeaderText()
      {
        string executingAssemblyPath = Logger.HeaderSection.GetExecutingAssemblyPath();
        string modulePathInClrFolder1 = Logger.HeaderSection.GetModulePathInClrFolder("clr.dll");
        string modulePathInClrFolder2 = Logger.HeaderSection.GetModulePathInClrFolder("dfdll.dll");
        string modulePath = Logger.HeaderSection.GetModulePath("dfshim.dll");
        FileVersionInfo fileVersionInfo = Logger.HeaderSection.GetVersionInfo(executingAssemblyPath) ?? Logger.HeaderSection.GetVersionInfo(Logger.HeaderSection.GetModulePathInClrFolder("system.deployment.dll"));
        FileVersionInfo versionInfo1 = Logger.HeaderSection.GetVersionInfo(modulePathInClrFolder1);
        FileVersionInfo versionInfo2 = Logger.HeaderSection.GetVersionInfo(modulePathInClrFolder2);
        FileVersionInfo versionInfo3 = Logger.HeaderSection.GetVersionInfo(modulePath);
        StringBuilder stringBuilder = new StringBuilder();
        try
        {
          stringBuilder.Append(Resources.GetString("LogFile_Header"));
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderOSVersion"), new object[2]
          {
            (object) Environment.OSVersion.Platform.ToString(),
            (object) Environment.OSVersion.Version.ToString()
          });
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderCLRVersion"), new object[1]
          {
            (object) Environment.Version.ToString()
          });
          if (fileVersionInfo != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderSystemDeploymentVersion"), new object[1]
            {
              (object) fileVersionInfo.FileVersion
            });
          if (versionInfo1 != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderClrDllVersion"), new object[1]
            {
              (object) versionInfo1.FileVersion
            });
          if (versionInfo2 != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderDfdllVersion"), new object[1]
            {
              (object) versionInfo2.FileVersion
            });
          if (versionInfo3 != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_HeaderDfshimVersion"), new object[1]
            {
              (object) versionInfo3.FileVersion
            });
        }
        catch (ArgumentException ex)
        {
        }
        catch (FormatException ex)
        {
        }
        return stringBuilder.ToString();
      }
    }

    protected class SourceSection : Logger.LogInformation
    {
      protected Uri _subscriptonUri;
      protected Uri _deploymentProviderUri;
      protected Uri _applicationUri;
      protected ServerInformation _subscriptionServerInformation;
      protected ServerInformation _deploymentProviderServerInformation;
      protected ServerInformation _applicationServerInformation;

      public Uri SubscriptionUri
      {
        set
        {
          this._subscriptonUri = value;
        }
      }

      public Uri DeploymentProviderUri
      {
        set
        {
          this._deploymentProviderUri = value;
        }
      }

      public Uri ApplicationUri
      {
        set
        {
          this._applicationUri = value;
        }
      }

      public ServerInformation SubscriptionServerInformation
      {
        set
        {
          this._subscriptionServerInformation = value;
        }
      }

      public ServerInformation DeploymentProviderServerInformation
      {
        set
        {
          this._deploymentProviderServerInformation = value;
        }
      }

      public ServerInformation ApplicationServerInformation
      {
        set
        {
          this._applicationServerInformation = value;
        }
      }

      public override string ToString()
      {
        StringBuilder destination = new StringBuilder();
        if (this._subscriptonUri != (Uri) null || this._deploymentProviderUri != (Uri) null || this._applicationUri != (Uri) null)
        {
          destination.Append(Resources.GetString("LogFile_Source"));
          if (this._subscriptonUri != (Uri) null)
            destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceDeploymentUrl"), new object[1]
            {
              (object) this._subscriptonUri.AbsoluteUri
            });
          if (this._subscriptionServerInformation != null)
            Logger.SourceSection.AppendServerInformation(destination, this._subscriptionServerInformation);
          if (this._deploymentProviderUri != (Uri) null)
            destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceDeploymentProviderUrl"), new object[1]
            {
              (object) this._deploymentProviderUri.AbsoluteUri
            });
          if (this._deploymentProviderServerInformation != null)
            Logger.SourceSection.AppendServerInformation(destination, this._deploymentProviderServerInformation);
          if (this._applicationUri != (Uri) null)
            destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_SourceApplicationUrl"), new object[1]
            {
              (object) this._applicationUri.AbsoluteUri
            });
          if (this._applicationServerInformation != null)
            Logger.SourceSection.AppendServerInformation(destination, this._applicationServerInformation);
          destination.Append(Environment.NewLine);
        }
        return destination.ToString();
      }

      private static void AppendServerInformation(StringBuilder destination, ServerInformation serverInformation)
      {
        if (!string.IsNullOrEmpty(serverInformation.Server))
          destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationServer"), new object[1]
          {
            (object) serverInformation.Server
          });
        if (!string.IsNullOrEmpty(serverInformation.PoweredBy))
          destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationPoweredBy"), new object[1]
          {
            (object) serverInformation.PoweredBy
          });
        if (string.IsNullOrEmpty(serverInformation.AspNetVersion))
          return;
        destination.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_ServerInformationAspNetVersion"), new object[1]
        {
          (object) serverInformation.AspNetVersion
        });
      }
    }

    protected class IdentitySection : Logger.LogInformation
    {
      protected DefinitionIdentity _deploymentIdentity;
      protected DefinitionIdentity _applicationIdentity;

      public DefinitionIdentity DeploymentIdentity
      {
        get
        {
          return this._deploymentIdentity;
        }
        set
        {
          this._deploymentIdentity = value;
        }
      }

      public DefinitionIdentity ApplicationIdentity
      {
        set
        {
          this._applicationIdentity = value;
        }
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (this._deploymentIdentity != null || this._applicationIdentity != null)
        {
          stringBuilder.Append(Resources.GetString("LogFile_Identity"));
          if (this._deploymentIdentity != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IdentityDeploymentIdentity"), new object[1]
            {
              (object) this._deploymentIdentity.ToString()
            });
          if (this._applicationIdentity != null)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_IdentityApplicationIdentity"), new object[1]
            {
              (object) this._applicationIdentity.ToString()
            });
          stringBuilder.Append(Environment.NewLine);
        }
        return stringBuilder.ToString();
      }
    }

    protected class SummarySection : Logger.LogInformation
    {
      protected AssemblyManifest _deploymentManifest;
      protected AssemblyManifest _applicationManifest;

      public AssemblyManifest DeploymentManifest
      {
        set
        {
          this._deploymentManifest = value;
        }
      }

      public AssemblyManifest ApplicationManifest
      {
        set
        {
          this._applicationManifest = value;
        }
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (this._deploymentManifest != null)
        {
          stringBuilder.Append(Resources.GetString("LogFile_Summary"));
          if (this._deploymentManifest.Deployment.Install)
            stringBuilder.Append(Resources.GetString("LogFile_SummaryInstallableApp"));
          else
            stringBuilder.Append(Resources.GetString("LogFile_SummaryOnlineOnlyApp"));
          if (this._deploymentManifest.Deployment.TrustURLParameters)
            stringBuilder.Append(Resources.GetString("LogFile_SummaryTrustUrlParameterSet"));
          if (this._applicationManifest != null && this._applicationManifest.EntryPoints[0].HostInBrowser)
            stringBuilder.Append(Resources.GetString("LogFile_SummaryBrowserHostedApp"));
          stringBuilder.Append(Environment.NewLine);
        }
        return stringBuilder.ToString();
      }
    }

    protected class ErrorSection : Logger.LogInformation
    {
      protected ArrayList _errors = new ArrayList();

      public string ErrorSummary
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(Resources.GetString("LogFile_ErrorSummary"));
          if (this._errors.Count > 0)
          {
            stringBuilder.Append(Resources.GetString("LogFile_ErrorSummaryStatusError"));
            foreach (Logger.ErrorInformation error in this._errors)
              stringBuilder.Append(error.Summary);
          }
          else
            stringBuilder.Append(Resources.GetString("LogFile_ErrorSummaryStatusNoError"));
          return stringBuilder.ToString();
        }
      }

      public void AddError(string message, Exception exception, DateTime time)
      {
        this._errors.Add((object) new Logger.ErrorInformation(message, exception, time));
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Resources.GetString("LogFile_Error"));
        if (this._errors.Count > 0)
        {
          stringBuilder.Append(Resources.GetString("LogFile_ErrorStatusError"));
          foreach (Logger.ErrorInformation error in this._errors)
            stringBuilder.Append(error.ToString());
        }
        else
          stringBuilder.Append(Resources.GetString("LogFile_ErrorStatusNoError"));
        return stringBuilder.ToString();
      }
    }

    protected class WarningSection : Logger.LogInformation
    {
      protected ArrayList _warnings = new ArrayList();

      public void AddWarning(string message, DateTime time)
      {
        this._warnings.Add((object) new Logger.LogInformation(message, time));
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Resources.GetString("LogFile_Warning"));
        if (this._warnings.Count > 0)
        {
          foreach (Logger.LogInformation warning in this._warnings)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_WarningStatusIndivualWarning"), new object[1]
            {
              (object) warning.Message
            });
        }
        else
          stringBuilder.Append(Resources.GetString("LogFile_WarningStatusNoWarning"));
        return stringBuilder.ToString();
      }
    }

    protected class PhaseSection : Logger.LogInformation
    {
      protected ArrayList _phaseInformations = new ArrayList();

      public void AddPhaseInformation(string phaseMessage, DateTime time)
      {
        this._phaseInformations.Add((object) new Logger.LogInformation(phaseMessage, time));
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Resources.GetString("LogFile_PhaseInformation"));
        if (this._phaseInformations.Count > 0)
        {
          foreach (Logger.LogInformation phaseInformation in this._phaseInformations)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("LogFile_PhaseInformationStatusIndivualPhaseInformation"), new object[2]
            {
              (object) phaseInformation.Time.ToString((IFormatProvider) DateTimeFormatInfo.CurrentInfo),
              (object) phaseInformation.Message
            });
        }
        else
          stringBuilder.Append(Resources.GetString("LogFile_PhaseInformationStatusNoPhaseInformation"));
        return stringBuilder.ToString();
      }
    }

    protected class ExecutionFlowSection : Logger.LogInformation
    {
      private static DateTimeFormatInfo DTFI = CultureInfo.InvariantCulture.DateTimeFormat;
      protected ArrayList _executionFlow = new ArrayList();

      public void AddMethodCall(string phaseMessage, DateTime time)
      {
        phaseMessage = "Method Call : " + phaseMessage;
        this._executionFlow.Add((object) new Logger.LogInformation(phaseMessage, time));
      }

      public void AddInternalState(string phaseMessage, DateTime time)
      {
        this._executionFlow.Add((object) new Logger.LogInformation(phaseMessage, time));
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("DETAILED EXECUTION FLOW\r\n");
        if (this._executionFlow.Count > 0)
        {
          foreach (Logger.LogInformation logInformation in this._executionFlow)
            stringBuilder.AppendFormat("[{0}] : {1}\r\n", (object) logInformation.Time.ToString(Logger.ExecutionFlowSection.DTFI.LongTimePattern, (IFormatProvider) CultureInfo.InvariantCulture), (object) logInformation.Message);
        }
        else
          stringBuilder.Append("No detailed execution log found.");
        return stringBuilder.ToString();
      }
    }

    protected class TransactionSection : Logger.LogInformation
    {
      protected ArrayList _transactionInformations = new ArrayList();
      protected ArrayList _failedTransactionInformations = new ArrayList();

      public string FailureSummary
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(Resources.GetString("LogFile_TransactionFailureSummary"));
          if (this._failedTransactionInformations.Count > 0)
          {
            foreach (Logger.TransactionInformation transactionInformation in this._failedTransactionInformations)
              stringBuilder.Append(transactionInformation.FailureSummary);
          }
          else
            stringBuilder.Append(Resources.GetString("LogFile_TransactionFailureSummaryNoError"));
          return stringBuilder.ToString();
        }
      }

      public void AddTransactionInformation(StoreTransactionOperation[] storeOperations, uint[] rgDispositions, int[] rgResults, DateTime time)
      {
        Logger.TransactionInformation transactionInformation = new Logger.TransactionInformation(storeOperations, rgDispositions, rgResults, time);
        this._transactionInformations.Add((object) transactionInformation);
        if (!transactionInformation.Failed)
          return;
        this._failedTransactionInformations.Add((object) transactionInformation);
      }

      public override string ToString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Resources.GetString("LogFile_Transaction"));
        if (this._transactionInformations.Count > 0)
        {
          foreach (Logger.TransactionInformation transactionInformation in this._transactionInformations)
            stringBuilder.Append(transactionInformation.ToString());
        }
        else
          stringBuilder.Append(Resources.GetString("LogFile_TransactionNoTransaction"));
        return stringBuilder.ToString();
      }
    }

    public class LogIdentity
    {
      protected readonly long _ticks = DateTime.Now.Ticks;
      protected readonly uint _threadId = NativeMethods.GetCurrentThreadId();
      protected string _logIdentityStringForm;

      public uint ThreadId
      {
        get
        {
          return this._threadId;
        }
      }

      public override string ToString()
      {
        if (this._logIdentityStringForm == null)
          this._logIdentityStringForm = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:x8}{1:x16}", new object[2]
          {
            (object) this._threadId,
            (object) this._ticks
          });
        return this._logIdentityStringForm;
      }
    }

    protected enum LogFileLocation
    {
      NoLogFile,
      RegistryBased,
      WinInetCache,
    }
  }
}
