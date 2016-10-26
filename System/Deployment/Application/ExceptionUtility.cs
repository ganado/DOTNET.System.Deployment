// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.ExceptionUtility
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  internal sealed class ExceptionUtility
  {
    public static bool IsHardException(Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException("exception");
      Win32Exception win32Exception = exception as Win32Exception;
      if (win32Exception != null)
        exception = Marshal.GetExceptionForHR(win32Exception.ErrorCode);
      return exception is DivideByZeroException || exception is OutOfMemoryException || (exception is StackOverflowException || exception is AccessViolationException);
    }
  }
}
