﻿// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.IMetaDataImport
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Deployment.Application
{
  [Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IMetaDataImport
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    void CloseEnum();

    void CountEnum(IntPtr iRef, ref uint ulCount);

    void ResetEnum();

    void EnumTypeDefs();

    void EnumInterfaceImpls();

    void EnumTypeRefs();

    void FindTypeDefByName();

    void GetScopeProps();

    void GetModuleFromScope();

    void GetTypeDefProps();

    void GetInterfaceImplProps();

    void GetTypeRefProps();

    void ResolveTypeRef();

    void EnumMembers();

    void EnumMembersWithName();

    void EnumMethods();

    void EnumMethodsWithName();

    void EnumFields();

    void EnumFieldsWithName();

    void EnumParams();

    void EnumMemberRefs();

    void EnumMethodImpls();

    void EnumPermissionSets();

    void FindMember();

    void FindMethod();

    void FindField();

    void FindMemberRef();

    void GetMethodProps();

    void GetMemberRefProps();

    void EnumProperties();

    void EnumEvents();

    void GetEventProps();

    void EnumMethodSemantics();

    void GetMethodSemantics();

    void GetClassLayout();

    void GetFieldMarshal();

    void GetRVA();

    void GetPermissionSetProps();

    void GetSigFromToken();

    void GetModuleRefProps();

    void EnumModuleRefs();

    void GetTypeSpecFromToken();

    void GetNameFromToken();

    void EnumUnresolvedMethods();

    void GetUserString();

    void GetPinvokeMap();

    void EnumSignatures();

    void EnumTypeSpecs();

    void EnumUserStrings();

    void GetParamForMethodIndex();

    void EnumCustomAttributes();

    void GetCustomAttributeProps();

    void FindTypeRef();

    void GetMemberProps();

    void GetFieldProps();

    void GetPropertyProps();

    void GetParamProps();

    void GetCustomAttributeByName();

    void IsValidToken();

    void GetNestedClassProps();

    void GetNativeCallConvFromSig();

    void IsGlobal();
  }
}
