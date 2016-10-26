// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.TempDirectory
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.IO;
using System.Threading;

namespace System.Deployment.Application
{
  internal class TempDirectory : DisposableBase
  {
    private string _thePath;
    private const uint _directorySegmentCount = 2;

    public string Path
    {
      get
      {
        return this._thePath;
      }
    }

    public TempDirectory()
      : this(System.IO.Path.GetTempPath())
    {
    }

    public TempDirectory(string basePath)
    {
      do
      {
        this._thePath = System.IO.Path.Combine(basePath, PathHelper.GenerateRandomPath(2U));
      }
      while (Directory.Exists(this._thePath) || File.Exists(this._thePath));
      Directory.CreateDirectory(this._thePath);
    }

    protected override void DisposeUnmanagedResources()
    {
      string rootSegmentPath = PathHelper.GetRootSegmentPath(this._thePath, 2U);
      for (int index = 0; index < 2; ++index)
      {
        if (!Directory.Exists(rootSegmentPath))
          break;
        try
        {
          Directory.Delete(rootSegmentPath, true);
          break;
        }
        catch (IOException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        Thread.Sleep(10);
      }
    }
  }
}
