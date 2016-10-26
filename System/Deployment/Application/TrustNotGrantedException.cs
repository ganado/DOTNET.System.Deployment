// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.TrustNotGrantedException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;

namespace System.Deployment.Application
{
  /// <summary>Indicates that the application does not have the appropriate level of trust to run on the local computer.</summary>
  [Serializable]
  public class TrustNotGrantedException : DeploymentException
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.TrustNotGrantedException" /> class with a system-supplied message that describes the error.</summary>
    public TrustNotGrantedException()
      : this(Resources.GetString("Ex_TrustNotGrantedException"))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.TrustNotGrantedException" /> class with a specified message that describes the error.</summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
    public TrustNotGrantedException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.TrustNotGrantedException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
    public TrustNotGrantedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal TrustNotGrantedException(ExceptionTypes exceptionType, string message)
      : base(exceptionType, message)
    {
    }

    internal TrustNotGrantedException(ExceptionTypes exceptionType, string message, Exception innerException)
      : base(exceptionType, message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.TrustNotGrantedException" /> class with serialized data.</summary>
    /// <param name="serializationInfo">The object that holds the serialized object data. </param>
    /// <param name="streamingContext">The contextual information about the source or destination. </param>
    protected TrustNotGrantedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
