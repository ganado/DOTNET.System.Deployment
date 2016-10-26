// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.LockedFile
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Deployment.Application
{
  internal static class LockedFile
  {
    [ThreadStatic]
    private static Hashtable _threadReaderLocks;
    [ThreadStatic]
    private static Hashtable _threadWriterLocks;

    private static Hashtable ThreadReaderLocks
    {
      get
      {
        if (LockedFile._threadReaderLocks == null)
          LockedFile._threadReaderLocks = new Hashtable();
        return LockedFile._threadReaderLocks;
      }
    }

    private static Hashtable ThreadWriterLocks
    {
      get
      {
        if (LockedFile._threadWriterLocks == null)
          LockedFile._threadWriterLocks = new Hashtable();
        return LockedFile._threadWriterLocks;
      }
    }

    public static IDisposable AcquireLock(string path, TimeSpan timeout, bool writer)
    {
      LockedFile.LockedFileHandle lockedFileHandle = LockedFile.LockHeldByThread(path, writer);
      if (lockedFileHandle != null)
        return (IDisposable) lockedFileHandle;
      DateTime dateTime = DateTime.UtcNow + timeout;
      FileAccess access;
      NativeMethods.GenericAccess genericAccess;
      NativeMethods.ShareMode shareMode;
      if (writer)
      {
        access = FileAccess.Write;
        genericAccess = NativeMethods.GenericAccess.GENERIC_WRITE;
        shareMode = NativeMethods.ShareMode.FILE_SHARE_NONE;
      }
      else
      {
        access = FileAccess.Read;
        genericAccess = NativeMethods.GenericAccess.GENERIC_READ;
        shareMode = PlatformSpecific.OnWin9x ? NativeMethods.ShareMode.FILE_SHARE_READ : NativeMethods.ShareMode.FILE_SHARE_READ | NativeMethods.ShareMode.FILE_SHARE_DELETE;
      }
      SafeFileHandle file;
      while (true)
      {
        file = NativeMethods.CreateFile(path, (uint) genericAccess, (uint) shareMode, IntPtr.Zero, 4U, 67108864U, IntPtr.Zero);
        int lastWin32Error = Marshal.GetLastWin32Error();
        if (file.IsInvalid)
        {
          if (lastWin32Error != 32 && lastWin32Error != 5)
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
          if (!(DateTime.UtcNow > dateTime))
            Thread.Sleep(1);
          else
            goto label_10;
        }
        else
          break;
      }
      return (IDisposable) new LockedFile.LockedFileHandle(file, path, access);
label_10:
      throw new DeploymentException(ExceptionTypes.LockTimeout, Resources.GetString("Ex_LockTimeoutException"));
    }

    private static LockedFile.LockedFileHandle LockHeldByThread(string path, bool writer)
    {
      if ((LockedFile.LockedFileHandle) LockedFile.ThreadWriterLocks[(object) path] != null)
        return new LockedFile.LockedFileHandle();
      if ((LockedFile.LockedFileHandle) LockedFile.ThreadReaderLocks[(object) path] == null)
        return (LockedFile.LockedFileHandle) null;
      if (!writer)
        return new LockedFile.LockedFileHandle();
      throw new NotImplementedException();
    }

    private class LockedFileHandle : IDisposable
    {
      private SafeFileHandle _handle;
      private string _path;
      private FileAccess _access;
      private bool _disposed;

      public LockedFileHandle()
      {
      }

      public LockedFileHandle(SafeFileHandle handle, string path, FileAccess access)
      {
        if (handle == null)
          throw new ArgumentNullException("handle");
        this._handle = handle;
        this._path = path;
        this._access = access;
        (this._access == FileAccess.Read ? LockedFile.ThreadReaderLocks : LockedFile.ThreadWriterLocks).Add((object) this._path, (object) this);
      }

      public void Dispose()
      {
        if (this._disposed)
          return;
        if (this._handle != null)
        {
          (this._access == FileAccess.Read ? LockedFile.ThreadReaderLocks : LockedFile.ThreadWriterLocks).Remove((object) this._path);
          this._handle.Dispose();
        }
        GC.SuppressFinalize((object) this);
        this._disposed = true;
      }
    }
  }
}
