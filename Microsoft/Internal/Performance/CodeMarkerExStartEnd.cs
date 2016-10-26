// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerExStartEnd
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System;
using System.Text;

namespace Microsoft.Internal.Performance
{
  internal struct CodeMarkerExStartEnd : IDisposable
  {
    private int _end;
    private byte[] _aBuff;

    internal CodeMarkerExStartEnd(int begin, int end, byte[] aBuff)
    {
      CodeMarkers.Instance.CodeMarkerEx(begin, aBuff);
      this._end = end;
      this._aBuff = aBuff;
    }

    internal CodeMarkerExStartEnd(int begin, int end, Guid guidData)
    {
      this = new CodeMarkerExStartEnd(begin, end, guidData.ToByteArray());
    }

    internal CodeMarkerExStartEnd(int begin, int end, string stringData)
    {
      this = new CodeMarkerExStartEnd(begin, end, Encoding.Unicode.GetBytes(stringData));
    }

    internal CodeMarkerExStartEnd(int begin, int end, uint uintData)
    {
      this = new CodeMarkerExStartEnd(begin, end, BitConverter.GetBytes(uintData));
    }

    internal CodeMarkerExStartEnd(int begin, int end, ulong ulongData)
    {
      this = new CodeMarkerExStartEnd(begin, end, BitConverter.GetBytes(ulongData));
    }

    public void Dispose()
    {
      if (this._end == 0)
        return;
      CodeMarkers.Instance.CodeMarkerEx(this._end, this._aBuff);
      this._end = 0;
      this._aBuff = (byte[]) null;
    }
  }
}
