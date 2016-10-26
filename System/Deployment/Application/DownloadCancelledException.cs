// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadCancelledException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;

namespace System.Deployment.Application
{
  [Serializable]
  internal class DownloadCancelledException : DeploymentDownloadException
  {
    public DownloadCancelledException()
      : this(Resources.GetString("Ex_DownloadCancelledException"))
    {
    }

    public DownloadCancelledException(string message)
      : base(message)
    {
    }

    public DownloadCancelledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected DownloadCancelledException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
