// Decompiled with JetBrains decompiler
// Type: System.Deployment.Internal.CodeSigning.CmiAuthenticodeSignerInfo
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.Deployment.Internal.CodeSigning
{
  internal class CmiAuthenticodeSignerInfo
  {
    private int m_error;
    private X509Chain m_signerChain;
    private uint m_algHash;
    private string m_hash;
    private string m_description;
    private string m_descriptionUrl;
    private CmiAuthenticodeTimestamperInfo m_timestamperInfo;

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

    internal string Hash
    {
      get
      {
        return this.m_hash;
      }
    }

    internal string Description
    {
      get
      {
        return this.m_description;
      }
    }

    internal string DescriptionUrl
    {
      get
      {
        return this.m_descriptionUrl;
      }
    }

    internal CmiAuthenticodeTimestamperInfo TimestamperInfo
    {
      get
      {
        return this.m_timestamperInfo;
      }
    }

    internal X509Chain SignerChain
    {
      get
      {
        return this.m_signerChain;
      }
    }

    internal CmiAuthenticodeSignerInfo()
    {
    }

    internal CmiAuthenticodeSignerInfo(int errorCode)
    {
      this.m_error = errorCode;
    }

    internal CmiAuthenticodeSignerInfo(Win32.AXL_SIGNER_INFO signerInfo, Win32.AXL_TIMESTAMPER_INFO timestamperInfo)
    {
      this.m_error = (int) signerInfo.dwError;
      if (signerInfo.pChainContext != IntPtr.Zero)
        this.m_signerChain = new X509Chain(signerInfo.pChainContext);
      this.m_algHash = signerInfo.algHash;
      if (signerInfo.pwszHash != IntPtr.Zero)
        this.m_hash = Marshal.PtrToStringUni(signerInfo.pwszHash);
      if (signerInfo.pwszDescription != IntPtr.Zero)
        this.m_description = Marshal.PtrToStringUni(signerInfo.pwszDescription);
      if (signerInfo.pwszDescriptionUrl != IntPtr.Zero)
        this.m_descriptionUrl = Marshal.PtrToStringUni(signerInfo.pwszDescriptionUrl);
      if ((int) timestamperInfo.dwError == -2146762496)
        return;
      this.m_timestamperInfo = new CmiAuthenticodeTimestamperInfo(timestamperInfo);
    }
  }
}
