// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.CmiAuthenticodeTimestamperInfo
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Security.Cryptography.X509Certificates;

namespace System.Deployment.Internal.CodeSigning
{
  internal class CmiAuthenticodeTimestamperInfo
  {
    private int m_error;
    private X509Chain m_timestamperChain;
    private DateTime m_timestampTime;
    private uint m_algHash;

    internal int ErrorCode
    {
      get
      {
        return this.m_error;
      }
    }

    internal uint HashAlgId
    {
      get
      {
        return this.m_algHash;
      }
    }

    internal DateTime TimestampTime
    {
      get
      {
        return this.m_timestampTime;
      }
    }

    internal X509Chain TimestamperChain
    {
      get
      {
        return this.m_timestamperChain;
      }
    }

    private CmiAuthenticodeTimestamperInfo()
    {
    }

    internal CmiAuthenticodeTimestamperInfo(Win32.AXL_TIMESTAMPER_INFO timestamperInfo)
    {
      this.m_error = (int) timestamperInfo.dwError;
      this.m_algHash = timestamperInfo.algHash;
      this.m_timestampTime = DateTime.FromFileTime((long) (uint) timestamperInfo.ftTimestamp.dwHighDateTime << 32 | (long) (uint) timestamperInfo.ftTimestamp.dwLowDateTime);
      if (!(timestamperInfo.pChainContext != IntPtr.Zero))
        return;
      this.m_timestamperChain = new X509Chain(timestamperInfo.pChainContext);
    }
  }
}
