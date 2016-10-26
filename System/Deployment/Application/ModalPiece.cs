// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ModalPiece
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Threading;

namespace System.Deployment.Application
{
  internal class ModalPiece : FormPiece
  {
    protected ManualResetEvent _modalEvent;
    protected UserInterfaceModalResult _modalResult;

    public UserInterfaceModalResult ModalResult
    {
      get
      {
        return this._modalResult;
      }
    }

    public override bool OnClosing()
    {
      bool flag = base.OnClosing();
      this._modalEvent.Set();
      return flag;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this._modalEvent.Set();
    }
  }
}
