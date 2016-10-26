// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerStartEnd
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System;

namespace Microsoft.Internal.Performance
{
  internal struct CodeMarkerStartEnd : IDisposable
  {
    private int _end;

    internal CodeMarkerStartEnd(int begin, int end)
    {
      CodeMarkers.Instance.CodeMarker(begin);
      this._end = end;
    }

    public void Dispose()
    {
      if (this._end == 0)
        return;
      CodeMarkers.Instance.CodeMarker(this._end);
      this._end = 0;
    }
  }
}
