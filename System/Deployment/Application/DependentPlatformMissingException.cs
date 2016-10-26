// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DependentPlatformMissingException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Deployment.Application
{
  /// <summary>The exception that is thrown when the platform dependency is not found during activation of the ClickOnce deployment and the deployment will not run.</summary>
  [Serializable]
  public class DependentPlatformMissingException : DeploymentException
  {
    private Uri _supportUrl;

    /// <summary>Gets a URI that indicates where support can be found for the problem encountered.</summary>
    /// <returns>A URI that indicates where support can be found for the problem encountered.</returns>
    public Uri SupportUrl
    {
      get
      {
        return this._supportUrl;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DependentPlatformMissingException" /> class. </summary>
    public DependentPlatformMissingException()
      : this(Resources.GetString("Ex_DependentPlatformMissingException"))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DependentPlatformMissingException" /> class with a specified message that describes the error. </summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
    public DependentPlatformMissingException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DependentPlatformMissingException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception. </summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
    public DependentPlatformMissingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DependentPlatformMissingException" /> class. </summary>
    /// <param name="serializationInfo">The object that holds the serialized object data. </param>
    /// <param name="streamingContext">The contextual information about the source or destination. </param>
    protected DependentPlatformMissingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this._supportUrl = (Uri) serializationInfo.GetValue("_supportUrl", typeof (Uri));
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DependentPlatformMissingException" /> class. </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="supportUrl">The URL to visit for product support information.</param>
    public DependentPlatformMissingException(string message, Uri supportUrl)
      : base(message)
    {
      this._supportUrl = supportUrl;
    }

    internal DependentPlatformMissingException(ExceptionTypes exceptionType, string message)
      : base(exceptionType, message)
    {
    }

    internal DependentPlatformMissingException(ExceptionTypes exceptionType, string message, Exception innerException)
      : base(exceptionType, message, innerException)
    {
    }

    /// <summary>Gets the object data.</summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_supportUrl", (object) this._supportUrl);
    }
  }
}
