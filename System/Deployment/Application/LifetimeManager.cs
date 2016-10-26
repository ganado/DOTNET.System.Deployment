// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.LifetimeManager
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Threading;

namespace System.Deployment.Application
{
  internal static class LifetimeManager
  {
    private static ManualResetEvent _lifetimeEndedEvent = new ManualResetEvent(false);
    private static Timer _periodicTimer = new Timer(new TimerCallback(LifetimeManager.PeriodicTimerCallback), (object) null, 600000, 600000);
    private static object _TimerLock = new object();
    private static int _operationsInProgress;
    private static bool _lifetimeExtended;
    private static bool _lifetimeEnded;
    private static bool _immediate;

    public static void StartOperation()
    {
      lock (LifetimeManager._TimerLock)
      {
        LifetimeManager.CheckAlive();
        ++LifetimeManager._operationsInProgress;
      }
    }

    public static void EndOperation()
    {
      lock (LifetimeManager._TimerLock)
      {
        LifetimeManager.CheckAlive();
        --LifetimeManager._operationsInProgress;
        LifetimeManager._lifetimeExtended = true;
      }
    }

    public static void ExtendLifetime()
    {
      lock (LifetimeManager._TimerLock)
      {
        LifetimeManager.CheckAlive();
        LifetimeManager._lifetimeExtended = true;
      }
    }

    public static bool WaitForEnd()
    {
      LifetimeManager._lifetimeEndedEvent.WaitOne();
      return LifetimeManager._immediate;
    }

    public static void EndImmediately()
    {
      lock (LifetimeManager._TimerLock)
      {
        if (LifetimeManager._operationsInProgress != 0)
        {
          Logger.StartCurrentThreadLogging();
          Logger.AddPhaseInformation(Resources.GetString("Life_OperationsInProgress"), new object[1]
          {
            (object) LifetimeManager._operationsInProgress
          });
          Logger.EndCurrentThreadLogging();
        }
        LifetimeManager._lifetimeEndedEvent.Set();
        LifetimeManager._lifetimeEnded = true;
        LifetimeManager._immediate = true;
      }
    }

    private static void CheckAlive()
    {
      if (LifetimeManager._lifetimeEnded)
        throw new InvalidOperationException(Resources.GetString("Ex_LifetimeEnded"));
    }

    private static void PeriodicTimerCallback(object state)
    {
      lock (LifetimeManager._TimerLock)
      {
        if (LifetimeManager._operationsInProgress == 0 && !LifetimeManager._lifetimeExtended)
        {
          LifetimeManager._lifetimeEndedEvent.Set();
          LifetimeManager._lifetimeEnded = true;
          LifetimeManager._periodicTimer.Dispose();
        }
        else
          LifetimeManager._lifetimeExtended = false;
      }
    }
  }
}
