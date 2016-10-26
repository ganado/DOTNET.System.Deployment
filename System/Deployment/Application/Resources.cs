// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Resources
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace System.Deployment.Application
{
  internal static class Resources
  {
    private static object lockObject = new object();
    private static ResourceManager _resources = (ResourceManager) null;
    private static Assembly _assembly = (Assembly) null;

    public static string GetString(string s)
    {
      if (System.Deployment.Application.Resources._resources == null)
      {
        lock (System.Deployment.Application.Resources.lockObject)
        {
          if (System.Deployment.Application.Resources._resources == null)
          {
            System.Deployment.Application.Resources.InitializeReferenceToAssembly();
            System.Deployment.Application.Resources._resources = new ResourceManager("System.Deployment", System.Deployment.Application.Resources._assembly);
          }
        }
      }
      return System.Deployment.Application.Resources._resources.GetString(s);
    }

    public static Image GetImage(string imageName)
    {
      System.Deployment.Application.Resources.InitializeReferenceToAssembly();
      Stream stream = (Stream) null;
      try
      {
        stream = System.Deployment.Application.Resources._assembly.GetManifestResourceStream(imageName);
        return Image.FromStream(stream);
      }
      catch
      {
        if (stream != null)
          stream.Close();
        throw;
      }
    }

    public static Icon GetIcon(string iconName)
    {
      System.Deployment.Application.Resources.InitializeReferenceToAssembly();
      using (Stream manifestResourceStream = System.Deployment.Application.Resources._assembly.GetManifestResourceStream(iconName))
        return new Icon(manifestResourceStream);
    }

    private static void InitializeReferenceToAssembly()
    {
      if (!(System.Deployment.Application.Resources._assembly == (Assembly) null))
        return;
      lock (System.Deployment.Application.Resources.lockObject)
      {
        if (!(System.Deployment.Application.Resources._assembly == (Assembly) null))
          return;
        System.Deployment.Application.Resources._assembly = Assembly.GetExecutingAssembly();
      }
    }
  }
}
