// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.InvalidDeploymentException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;

namespace System.Deployment.Application
{
  /// <summary>Indicates that ClickOnce could not read either the deployment or application manifests.</summary>
  [Serializable]
  public class InvalidDeploymentException : DeploymentException
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.InvalidDeploymentException" /> class. </summary>
    public InvalidDeploymentException()
      : this(Resources.GetString("Ex_InvalidDeploymentException"))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.InvalidDeploymentException" /> class with the specified message that describes the exception. </summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
    public InvalidDeploymentException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.InvalidDeploymentException" /> class. </summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
    public InvalidDeploymentException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal InvalidDeploymentException(ExceptionTypes exceptionType, string message)
      : base(exceptionType, message)
    {
    }

    internal InvalidDeploymentException(ExceptionTypes exceptionType, string message, Exception innerException)
      : base(exceptionType, message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.InvalidDeploymentException" /> class. </summary>
    /// <param name="serializationInfo">The object that holds the serialized object data. </param>
    /// <param name="streamingContext">The contextual information about the source or destination. </param>
    protected InvalidDeploymentException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
