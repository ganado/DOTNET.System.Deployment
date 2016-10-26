// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DownloadFileGroupCompletedEventHandler
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

namespace System.Deployment.Application
{
  /// <summary>Represents the method that will handle the <see cref="E:System.Deployment.Application.ApplicationDeployment.DownloadFileGroupCompleted" /> event of an <see cref="T:System.Deployment.Application.ApplicationDeployment" />. </summary>
  /// <param name="sender">The source of the event. </param>
  /// <param name="e">A <see cref="T:System.Deployment.Application.DownloadFileGroupCompletedEventArgs" /> that contains the event data.</param>
  public delegate void DownloadFileGroupCompletedEventHandler(object sender, DownloadFileGroupCompletedEventArgs e);
}
