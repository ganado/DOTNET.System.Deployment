// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.Manifest.DeploymentUpdate
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application.Manifest
{
  internal class DeploymentUpdate
  {
    private readonly bool _beforeApplicationStartup;
    private readonly bool _maximumAgeSpecified;
    private readonly TimeSpan _maximumAgeAllowed;
    private readonly uint _maximumAgeCount;
    private readonly timeUnitType _maximumAgeUnit;

    public bool BeforeApplicationStartup
    {
      get
      {
        return this._beforeApplicationStartup;
      }
    }

    public bool MaximumAgeSpecified
    {
      get
      {
        return this._maximumAgeSpecified;
      }
    }

    public TimeSpan MaximumAgeAllowed
    {
      get
      {
        return this._maximumAgeAllowed;
      }
    }

    public DeploymentUpdate(DeploymentMetadataEntry entry)
    {
      this._beforeApplicationStartup = (entry.DeploymentFlags & 4U) > 0U;
      this._maximumAgeAllowed = DeploymentUpdate.GetTimeSpanFromItem(entry.MaximumAge, entry.MaximumAge_Unit, out this._maximumAgeCount, out this._maximumAgeUnit, out this._maximumAgeSpecified);
    }

    private static TimeSpan GetTimeSpanFromItem(ushort time, byte elapsedunit, out uint count, out timeUnitType unit, out bool specified)
    {
      specified = true;
      TimeSpan timeSpan;
      switch (elapsedunit)
      {
        case 1:
          timeSpan = TimeSpan.FromHours((double) time);
          count = (uint) time;
          unit = timeUnitType.hours;
          break;
        case 2:
          timeSpan = TimeSpan.FromDays((double) time);
          count = (uint) time;
          unit = timeUnitType.days;
          break;
        case 3:
          timeSpan = TimeSpan.FromDays((double) ((int) time * 7));
          count = (uint) time;
          unit = timeUnitType.weeks;
          break;
        default:
          specified = false;
          timeSpan = TimeSpan.Zero;
          count = 0U;
          unit = timeUnitType.days;
          break;
      }
      return timeSpan;
    }
  }
}
