// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DisposableBase
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  internal abstract class DisposableBase : IDisposable
  {
    private bool _disposed;

    ~DisposableBase()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        if (disposing)
          this.DisposeManagedResources();
        this.DisposeUnmanagedResources();
      }
      this._disposed = true;
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeUnmanagedResources()
    {
    }
  }
}
