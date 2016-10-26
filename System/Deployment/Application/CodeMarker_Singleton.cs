// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.CodeMarker_Singleton
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Internal.Performance;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class CodeMarker_Singleton
  {
    private static CodeMarkers codemarkers = (CodeMarkers) null;
    private static object syncObject = new object();

    public static CodeMarkers Instance
    {
      get
      {
        if (CodeMarker_Singleton.codemarkers == null)
        {
          lock (CodeMarker_Singleton.syncObject)
          {
            if (CodeMarker_Singleton.codemarkers == null)
            {
              CodeMarkers local_2 = CodeMarkers.Instance;
              local_2.InitPerformanceDll(63, "Software\\Microsoft\\VisualStudio\\8.0");
              Thread.MemoryBarrier();
              CodeMarker_Singleton.codemarkers = local_2;
            }
          }
        }
        return CodeMarker_Singleton.codemarkers;
      }
    }
  }
}
