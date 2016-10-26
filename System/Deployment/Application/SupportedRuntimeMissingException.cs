// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.SupportedRuntimeMissingException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Deployment.Application
{
  /// <summary>This exception is thrown when a runtime version that is compatible with this application cannot be found.</summary>
  [Serializable]
  public class SupportedRuntimeMissingException : DependentPlatformMissingException
  {
    private string _supportedRuntimeVersion;

    /// <summary>Gets the version of the runtime where this application can install and run.</summary>
    /// <returns>A string that contains the runtime version number.</returns>
    public string SupportedRuntimeVersion
    {
      get
      {
        return this._supportedRuntimeVersion;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.SupportedRuntimeMissingException" /> class with a system-supplied message that describes the error.</summary>
    public SupportedRuntimeMissingException()
      : this(Resources.GetString("Ex_SupportedRuntimeMissingException"))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.SupportedRuntimeMissingException" /> class with a specified message that describes the error.</summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
    public SupportedRuntimeMissingException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.SupportedRuntimeMissingException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture. </param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not null, the current exception is raised in a catch block that handles the inner exception. </param>
    public SupportedRuntimeMissingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.SupportedRuntimeMissingException" /> class with serialized data.</summary>
    /// <param name="serializationInfo">The object that holds the serialized object data. </param>
    /// <param name="streamingContext">The contextual information about the source or destination. </param>
    protected SupportedRuntimeMissingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this._supportedRuntimeVersion = (string) serializationInfo.GetValue("_supportedRuntimeVersion", typeof (string));
    }

    internal SupportedRuntimeMissingException(string message, Uri supportUrl, string supportedRuntimeVersion)
      : base(message, supportUrl)
    {
      this._supportedRuntimeVersion = supportedRuntimeVersion;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.SupportedRuntimeMissingException" /> class with serialized data.</summary>
    /// <param name="info">The object that holds the serialized object data. </param>
    /// <param name="context">The contextual information about the source or destination. </param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_supportedRuntimeVersion", (object) this._supportedRuntimeVersion);
    }
  }
}
