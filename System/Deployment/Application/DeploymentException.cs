// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.DeploymentException
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Deployment.Application
{
  /// <summary>Defines a base class for all deployment-related exceptions.</summary>
  [Serializable]
  public class DeploymentException : SystemException
  {
    private ExceptionTypes _type;

    internal ExceptionTypes SubType
    {
      get
      {
        return this._type;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DeploymentException" /> class. </summary>
    public DeploymentException()
      : this(Resources.GetString("Ex_DeploymentException"))
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DeploymentException" /> class. </summary>
    /// <param name="message">Represents text as a series of Unicode characters.</param>
    public DeploymentException(string message)
      : base(message)
    {
      this._type = ExceptionTypes.Unknown;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DeploymentException" /> class. </summary>
    /// <param name="message">Represents text as a series of Unicode characters.</param>
    /// <param name="innerException">Represents errors that occur during application execution.</param>
    public DeploymentException(string message, Exception innerException)
      : base(message, innerException)
    {
      this._type = ExceptionTypes.Unknown;
    }

    internal DeploymentException(ExceptionTypes exceptionType, string message)
      : base(message)
    {
      this._type = exceptionType;
    }

    internal DeploymentException(ExceptionTypes exceptionType, string message, Exception innerException)
      : base(message, innerException)
    {
      this._type = exceptionType;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Deployment.Application.DeploymentException" /> class. </summary>
    /// <param name="serializationInfo">Stores all the data needed to serialize or deserialize an object. This class cannot be inherited.</param>
    /// <param name="streamingContext">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
    protected DeploymentException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
      this._type = (ExceptionTypes) serializationInfo.GetValue("_type", typeof (ExceptionTypes));
    }

    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_type", (object) this._type);
    }
  }
}
