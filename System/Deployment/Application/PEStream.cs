// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.PEStream
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Deployment.Application
{
  internal class PEStream : Stream
  {
    protected PEStream.StreamComponentList _streamComponents = new PEStream.StreamComponentList();
    protected ArrayList _dataDirectories = new ArrayList();
    protected ArrayList _sectionHeaders = new ArrayList();
    protected ArrayList _sections = new ArrayList();
    protected bool _canRead;
    protected bool _canSeek;
    protected FileStream _peFile;
    protected long _length;
    protected long _position;
    protected const ushort _id1ManifestId = 1;
    protected const ushort _id1ManifestLanguageId = 1033;
    internal const ushort IMAGE_DOS_SIGNATURE = 23117;
    internal const uint IMAGE_NT_SIGNATURE = 17744;
    internal const uint IMAGE_NT_OPTIONAL_HDR32_MAGIC = 267;
    internal const uint IMAGE_NT_OPTIONAL_HDR64_MAGIC = 523;
    internal const uint IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
    internal const uint IMAGE_FILE_DLL = 8192;
    protected const uint IMAGE_DIRECTORY_ENTRY_EXPORT = 0;
    protected const uint IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
    protected const uint IMAGE_DIRECTORY_ENTRY_RESOURCE = 2;
    protected const uint IMAGE_DIRECTORY_ENTRY_EXCEPTION = 3;
    protected const uint IMAGE_DIRECTORY_ENTRY_SECURITY = 4;
    protected const uint IMAGE_DIRECTORY_ENTRY_BASERELOC = 5;
    protected const uint IMAGE_DIRECTORY_ENTRY_DEBUG = 6;
    protected const uint IMAGE_DIRECTORY_ENTRY_ARCHITECTURE = 7;
    protected const uint IMAGE_DIRECTORY_ENTRY_GLOBALPTR = 8;
    protected const uint IMAGE_DIRECTORY_ENTRY_TLS = 9;
    protected const uint IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG = 10;
    protected const uint IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT = 11;
    protected const uint IMAGE_DIRECTORY_ENTRY_IAT = 12;
    protected const uint IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT = 13;
    protected const uint IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;
    protected const uint IMAGE_RESOURCE_NAME_IS_STRING = 2147483648;
    protected const uint IMAGE_RESOURCE_DATA_IS_DIRECTORY = 2147483648;
    protected PEStream.DosHeader _dosHeader;
    protected PEStream.DosStub _dosStub;
    protected PEStream.NtSignature _ntSignature;
    protected PEStream.FileHeader _fileHeader;
    protected PEStream.OptionalHeader _optionalHeader;
    protected PEStream.ResourceSection _resourceSection;
    protected bool _partialConstruct;
    protected const ushort ManifestDirId = 24;
    protected const int ErrorBadFormat = 11;

    public override bool CanRead
    {
      get
      {
        return this._canRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return this._canSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return this._length;
      }
    }

    public override long Position
    {
      get
      {
        return this._position;
      }
      set
      {
        this.Seek(value, SeekOrigin.Begin);
      }
    }

    public bool IsImageFileDll
    {
      get
      {
        return this._fileHeader.IsImageFileDll;
      }
    }

    public static ushort Id1ManifestId
    {
      get
      {
        return 1;
      }
    }

    public static ushort Id1ManifestLanguageId
    {
      get
      {
        return 1033;
      }
    }

    public PEStream(string filePath)
    {
      this.ConstructFromFile(filePath, true);
    }

    public PEStream(string filePath, bool partialConstruct)
    {
      this.ConstructFromFile(filePath, partialConstruct);
    }

    private void ConstructFromFile(string filePath, bool partialConstruct)
    {
      string fileName = Path.GetFileName(filePath);
      bool flag = false;
      try
      {
        this._peFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        this.ConstructPEImage(this._peFile, partialConstruct);
        flag = true;
      }
      catch (IOException ex)
      {
        throw new IOException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidPEImage"), new object[1]
        {
          (object) fileName
        }), (Exception) ex);
      }
      catch (Win32Exception ex)
      {
        throw new IOException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidPEImage"), new object[1]
        {
          (object) fileName
        }), (Exception) ex);
      }
      catch (NotSupportedException ex)
      {
        throw new IOException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.GetString("Ex_InvalidPEImage"), new object[1]
        {
          (object) fileName
        }), (Exception) ex);
      }
      finally
      {
        if (!flag && this._peFile != null)
          this._peFile.Close();
      }
    }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      bool flag = false;
      int count1 = count;
      long sourceOffset = 0;
      int bufferOffset = offset;
      foreach (PEStream.PEComponent streamComponent in (ArrayList) this._streamComponents)
      {
        if (!flag && this._position <= streamComponent.Address + streamComponent.Size - 1L)
        {
          sourceOffset = this._position - streamComponent.Address;
          if (sourceOffset < 0L)
            throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEImage"));
          flag = true;
        }
        if (flag)
        {
          int num = streamComponent.Read(buffer, bufferOffset, sourceOffset, count1);
          bufferOffset += num;
          this._position = this._position + (long) num;
          count1 -= num;
          sourceOffset = 0L;
        }
        if (count1 <= 0)
          break;
      }
      return count - count1;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (origin == SeekOrigin.Begin)
        this._position = offset;
      else if (origin == SeekOrigin.Current)
        this._position = this._position + offset;
      else if (origin == SeekOrigin.End)
        this._position = this._length + offset;
      if (this._position < 0L)
        this._position = 0L;
      if (this._position > this._length)
        this._position = this._length;
      return this._position;
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!disposing || this._peFile == null)
          return;
        this._peFile.Close();
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    public void ZeroOutOptionalHeaderCheckSum()
    {
      this._optionalHeader.CheckSum = 0U;
    }

    public void ZeroOutManifestResource(ushort manifestId, ushort languageId)
    {
      PEStream.ResourceComponent resourceComponent = this.RetrieveResource(new object[3]
      {
        (object) (ushort) 24,
        (object) manifestId,
        (object) languageId
      });
      if (resourceComponent == null || !(resourceComponent is PEStream.ResourceData))
        return;
      ((PEStream.ResourceData) resourceComponent).ZeroData();
    }

    public byte[] GetManifestResource(ushort manifestId, ushort languageId)
    {
      PEStream.ResourceComponent resourceComponent = this.RetrieveResource(new object[3]
      {
        (object) (ushort) 24,
        (object) manifestId,
        (object) languageId
      });
      if (resourceComponent != null && resourceComponent is PEStream.ResourceData)
        return ((PEStream.ResourceData) resourceComponent).Data;
      return (byte[]) null;
    }

    public byte[] GetDefaultId1ManifestResource()
    {
      PEStream.ResourceData manifestResource = this.GetId1ManifestResource();
      if (manifestResource != null)
        return manifestResource.Data;
      return (byte[]) null;
    }

    public void ZeroOutDefaultId1ManifestResource()
    {
      PEStream.ResourceData manifestResource = this.GetId1ManifestResource();
      if (manifestResource == null)
        return;
      manifestResource.ZeroData();
    }

    protected PEStream.ResourceData GetId1ManifestResource()
    {
      PEStream.ResourceComponent resourceComponent1 = this.RetrieveResource(new object[2]
      {
        (object) (ushort) 24,
        (object) PEStream.Id1ManifestId
      });
      if (resourceComponent1 != null && resourceComponent1 is PEStream.ResourceDirectory)
      {
        PEStream.ResourceDirectory resourceDirectory = (PEStream.ResourceDirectory) resourceComponent1;
        if (resourceDirectory.ResourceComponentCount > 1)
          throw new Win32Exception(11, Resources.GetString("Ex_MultipleId1Manifest"));
        if (resourceDirectory.ResourceComponentCount == 1)
        {
          PEStream.ResourceComponent resourceComponent2 = resourceDirectory.GetResourceComponent(0);
          if (resourceComponent2 != null && resourceComponent2 is PEStream.ResourceData)
            return (PEStream.ResourceData) resourceComponent2;
        }
      }
      return (PEStream.ResourceData) null;
    }

    protected PEStream.ResourceComponent RetrieveResource(object[] keys)
    {
      if (this._resourceSection == null)
        return (PEStream.ResourceComponent) null;
      PEStream.ResourceDirectory resourceDirectory = this._resourceSection.RootResourceDirectory;
      if (resourceDirectory == null)
        return (PEStream.ResourceComponent) null;
      return this.RetrieveResource(resourceDirectory, keys, 0U);
    }

    protected PEStream.ResourceComponent RetrieveResource(PEStream.ResourceDirectory resourcesDirectory, object[] keys, uint keyIndex)
    {
      PEStream.ResourceComponent resourceComponent = resourcesDirectory[keys[(int) keyIndex]];
      if ((long) keyIndex == (long) (keys.Length - 1))
        return resourceComponent;
      if (resourceComponent is PEStream.ResourceDirectory)
        return this.RetrieveResource((PEStream.ResourceDirectory) resourceComponent, keys, keyIndex + 1U);
      return (PEStream.ResourceComponent) null;
    }

    protected void ConstructPEImage(FileStream file, bool partialConstruct)
    {
      this._partialConstruct = partialConstruct;
      this._dosHeader = new PEStream.DosHeader(file);
      long size = (long) this._dosHeader.NtHeaderPosition - (this._dosHeader.Address + this._dosHeader.Size);
      if (size < 0L)
        throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEFormat"));
      this._dosStub = new PEStream.DosStub(file, this._dosHeader.Address + this._dosHeader.Size, size);
      this._ntSignature = new PEStream.NtSignature(file, (long) this._dosHeader.NtHeaderPosition);
      this._fileHeader = new PEStream.FileHeader(file, this._ntSignature.Address + this._ntSignature.Size);
      this._optionalHeader = new PEStream.OptionalHeader(file, this._fileHeader.Address + this._fileHeader.Size);
      long address1 = this._optionalHeader.Address + this._optionalHeader.Size;
      for (int index = 0; (long) index < (long) this._optionalHeader.NumberOfRvaAndSizes; ++index)
      {
        PEStream.DataDirectory dataDirectory = new PEStream.DataDirectory(file, address1);
        address1 += dataDirectory.Size;
        this._dataDirectories.Add((object) dataDirectory);
      }
      if ((long) this._fileHeader.SizeOfOptionalHeader < this._optionalHeader.Size + (long) this._optionalHeader.NumberOfRvaAndSizes * (long) Marshal.SizeOf(typeof (PEStream.IMAGE_DATA_DIRECTORY)))
        throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEFormat"));
      bool flag = false;
      uint num = 0;
      if (this._optionalHeader.NumberOfRvaAndSizes > 2U)
      {
        num = ((PEStream.DataDirectory) this._dataDirectories[2]).VirtualAddress;
        flag = true;
      }
      long address2 = this._optionalHeader.Address + (long) this._fileHeader.SizeOfOptionalHeader;
      for (int index = 0; index < (int) this._fileHeader.NumberOfSections; ++index)
      {
        PEStream.SectionHeader sectionHeader = new PEStream.SectionHeader(file, address2);
        PEStream.Section section = !flag || (int) sectionHeader.VirtualAddress != (int) num ? new PEStream.Section(file, sectionHeader) : (PEStream.Section) (this._resourceSection = new PEStream.ResourceSection(file, sectionHeader, partialConstruct));
        sectionHeader.Section = section;
        this._sectionHeaders.Add((object) sectionHeader);
        this._sections.Add((object) section);
        address2 += sectionHeader.Size;
      }
      this.ConstructStream();
      ArrayList arrayList = new ArrayList();
      long address3 = 0;
      foreach (PEStream.PEComponent streamComponent in (ArrayList) this._streamComponents)
      {
        if (streamComponent.Address < address3)
          throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEFormat"));
        if (streamComponent.Address > address3)
        {
          PEStream.PEComponent peComponent = new PEStream.PEComponent(file, address3, streamComponent.Address - address3);
          arrayList.Add((object) peComponent);
        }
        address3 = streamComponent.Address + streamComponent.Size;
      }
      if (address3 < file.Length)
      {
        PEStream.PEComponent peComponent = new PEStream.PEComponent(file, address3, file.Length - address3);
        arrayList.Add((object) peComponent);
      }
      this._streamComponents.AddRange((ICollection) arrayList);
      this._streamComponents.Sort((IComparer) new PEStream.PEComponentComparer());
      this._canRead = true;
      this._canSeek = true;
      this._length = file.Length;
      this._position = 0L;
    }

    protected void ConstructStream()
    {
      this._streamComponents.Clear();
      this._streamComponents.Add((PEStream.PEComponent) this._dosHeader);
      this._streamComponents.Add((PEStream.PEComponent) this._dosStub);
      this._streamComponents.Add((PEStream.PEComponent) this._ntSignature);
      this._streamComponents.Add((PEStream.PEComponent) this._fileHeader);
      this._streamComponents.Add((PEStream.PEComponent) this._optionalHeader);
      foreach (PEStream.PEComponent dataDirectory in this._dataDirectories)
        this._streamComponents.Add(dataDirectory);
      foreach (PEStream.PEComponent sectionHeader in this._sectionHeaders)
        this._streamComponents.Add(sectionHeader);
      foreach (PEStream.Section section in this._sections)
        section.AddComponentsToStream(this._streamComponents);
      this._streamComponents.Sort((IComparer) new PEStream.PEComponentComparer());
    }

    protected class StreamComponentList : ArrayList
    {
      public int Add(PEStream.PEComponent peComponent)
      {
        if (peComponent.Size > 0L)
          return this.Add((object) peComponent);
        return -1;
      }
    }

    protected class PEComponentComparer : IComparer
    {
      public int Compare(object a, object b)
      {
        PEStream.PEComponent peComponent1 = (PEStream.PEComponent) a;
        PEStream.PEComponent peComponent2 = (PEStream.PEComponent) b;
        if (peComponent1.Address > peComponent2.Address)
          return 1;
        return peComponent1.Address < peComponent2.Address ? -1 : 0;
      }
    }

    protected struct IMAGE_DOS_HEADER
    {
      public ushort e_magic;
      public ushort e_cblp;
      public ushort e_cp;
      public ushort e_crlc;
      public ushort e_cparhdr;
      public ushort e_minalloc;
      public ushort e_maxalloc;
      public ushort e_ss;
      public ushort e_sp;
      public ushort e_csum;
      public ushort e_ip;
      public ushort e_cs;
      public ushort e_lfarlc;
      public ushort e_ovno;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public ushort[] e_res;
      public ushort e_oemid;
      public ushort e_oeminfo;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      public ushort[] e_res2;
      public uint e_lfanew;
    }

    protected struct IMAGE_FILE_HEADER
    {
      public ushort Machine;
      public ushort NumberOfSections;
      public uint TimeDateStamp;
      public uint PointerToSymbolTable;
      public uint NumberOfSymbols;
      public ushort SizeOfOptionalHeader;
      public ushort Characteristics;
    }

    protected struct IMAGE_OPTIONAL_HEADER32
    {
      public ushort Magic;
      public byte MajorLinkerVersion;
      public byte MinorLinkerVersion;
      public uint SizeOfCode;
      public uint SizeOfInitializedData;
      public uint SizeOfUninitializedData;
      public uint AddressOfEntryPoint;
      public uint BaseOfCode;
      public uint BaseOfData;
      public uint ImageBase;
      public uint SectionAlignment;
      public uint FileAlignment;
      public ushort MajorOperatingSystemVersion;
      public ushort MinorOperatingSystemVersion;
      public ushort MajorImageVersion;
      public ushort MinorImageVersion;
      public ushort MajorSubsystemVersion;
      public ushort MinorSubsystemVersion;
      public uint Win32VersionValue;
      public uint SizeOfImage;
      public uint SizeOfHeaders;
      public uint CheckSum;
      public ushort Subsystem;
      public ushort DllCharacteristics;
      public uint SizeOfStackReserve;
      public uint SizeOfStackCommit;
      public uint SizeOfHeapReserve;
      public uint SizeOfHeapCommit;
      public uint LoaderFlags;
      public uint NumberOfRvaAndSizes;
    }

    [Serializable]
    protected struct IMAGE_OPTIONAL_HEADER64
    {
      internal ushort Magic;
      internal byte MajorLinkerVersion;
      internal byte MinorLinkerVersion;
      internal uint SizeOfCode;
      internal uint SizeOfInitializedData;
      internal uint SizeOfUninitializedData;
      internal uint AddressOfEntryPoint;
      internal uint BaseOfCode;
      internal ulong ImageBase;
      internal uint SectionAlignment;
      internal uint FileAlignment;
      internal ushort MajorOperatingSystemVersion;
      internal ushort MinorOperatingSystemVersion;
      internal ushort MajorImageVersion;
      internal ushort MinorImageVersion;
      internal ushort MajorSubsystemVersion;
      internal ushort MinorSubsystemVersion;
      internal uint Win32VersionValue;
      internal uint SizeOfImage;
      internal uint SizeOfHeaders;
      internal uint CheckSum;
      internal ushort Subsystem;
      internal ushort DllCharacteristics;
      internal ulong SizeOfStackReserve;
      internal ulong SizeOfStackCommit;
      internal ulong SizeOfHeapReserve;
      internal ulong SizeOfHeapCommit;
      internal uint LoaderFlags;
      internal uint NumberOfRvaAndSizes;
    }

    [Serializable]
    protected struct IMAGE_DATA_DIRECTORY
    {
      public uint VirtualAddress;
      public uint Size;
    }

    [Serializable]
    protected struct IMAGE_SECTION_HEADER
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public byte[] Name;
      public uint VirtualSize;
      public uint VirtualAddress;
      public uint SizeOfRawData;
      public uint PointerToRawData;
      public uint PointerToRelocations;
      public uint PointerToLinenumbers;
      public ushort NumberOfRelocations;
      public ushort NumberOfLinenumbers;
      public uint Characteristics;
    }

    [Serializable]
    protected struct IMAGE_RESOURCE_DIRECTORY
    {
      public uint Characteristics;
      public uint TimeDateStamp;
      public ushort MajorVersion;
      public ushort MinorVersion;
      public ushort NumberOfNamedEntries;
      public ushort NumberOfIdEntries;
    }

    [Serializable]
    protected struct IMAGE_RESOURCE_DATA_ENTRY
    {
      public uint OffsetToData;
      public uint Size;
      public uint CodePage;
      public uint Reserved;
    }

    [Serializable]
    protected struct IMAGE_RESOURCE_DIRECTORY_ENTRY
    {
      public uint Name;
      public uint OffsetToData;
    }

    protected class PEComponent
    {
      protected long _address;
      protected long _size;
      protected object _data;

      public long Address
      {
        get
        {
          return this._address;
        }
      }

      public long Size
      {
        get
        {
          return this._size;
        }
      }

      public PEComponent()
      {
        this._address = 0L;
        this._size = 0L;
        this._data = (object) null;
      }

      public PEComponent(FileStream file, long address, long size)
      {
        this._address = address;
        this._size = size;
        this._data = (object) new PEStream.DiskDataBlock(file, address, size);
      }

      public virtual int Read(byte[] buffer, int bufferOffset, long sourceOffset, int count)
      {
        int num1;
        if (this._data is PEStream.DataComponent)
        {
          PEStream.DataComponent data = (PEStream.DataComponent) this._data;
          long num2 = Math.Min((long) count, this._size - sourceOffset);
          if (num2 < 0L)
            throw new ArgumentException(Resources.GetString("Ex_InvalidCopyRequest"));
          num1 = data.Read(buffer, bufferOffset, sourceOffset, (int) num2);
        }
        else
        {
          byte[] byteArray = PEStream.PEComponent.ToByteArray(this._data);
          long num2 = Math.Min((long) count, (long) byteArray.Length - sourceOffset);
          if (num2 < 0L)
            throw new ArgumentException(Resources.GetString("Ex_InvalidCopyRequest"));
          Array.Copy((Array) byteArray, (int) sourceOffset, (Array) buffer, bufferOffset, (int) num2);
          num1 = (int) num2;
        }
        return num1;
      }

      protected static byte[] ToByteArray(object data)
      {
        int cb = Marshal.SizeOf(data);
        IntPtr num = Marshal.AllocCoTaskMem(cb);
        Marshal.StructureToPtr(data, num, false);
        byte[] destination = new byte[cb];
        Marshal.Copy(num, destination, 0, destination.Length);
        Marshal.FreeCoTaskMem(num);
        return destination;
      }

      protected static object ReadData(FileStream file, long position, Type dataType)
      {
        int length = Marshal.SizeOf(dataType);
        byte[] numArray = new byte[length];
        if (file.Seek(position, SeekOrigin.Begin) != position)
          throw new IOException(Resources.GetString("Ex_NotEnoughDataInFile"));
        if (file.Read(numArray, 0, numArray.Length) < length)
          throw new IOException(Resources.GetString("Ex_NotEnoughDataInFile"));
        IntPtr num = Marshal.AllocCoTaskMem(length);
        Marshal.Copy(numArray, 0, num, length);
        object structure = Marshal.PtrToStructure(num, dataType);
        Marshal.FreeCoTaskMem(num);
        return structure;
      }

      protected long CalculateSize(object data)
      {
        return (long) Marshal.SizeOf(data);
      }
    }

    protected class DosHeader : PEStream.PEComponent
    {
      protected PEStream.IMAGE_DOS_HEADER _dosHeader;

      public uint NtHeaderPosition
      {
        get
        {
          return this._dosHeader.e_lfanew;
        }
      }

      public DosHeader(FileStream file)
      {
        file.Seek(0L, SeekOrigin.Begin);
        this._dosHeader = (PEStream.IMAGE_DOS_HEADER) PEStream.PEComponent.ReadData(file, 0L, this._dosHeader.GetType());
        if ((int) this._dosHeader.e_magic != 23117)
          throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEImage"));
        this._data = (object) this._dosHeader;
        this._address = 0L;
        this._size = this.CalculateSize((object) this._dosHeader);
      }
    }

    protected class DosStub : PEStream.PEComponent
    {
      public DosStub(FileStream file, long startAddress, long size)
      {
        this._address = startAddress;
        this._size = size;
        this._data = (object) new PEStream.DiskDataBlock(file, this._address, this._size);
      }
    }

    protected class NtSignature : PEStream.PEComponent
    {
      public NtSignature(FileStream file, long address)
      {
        uint num1 = 0;
        uint num2 = (uint) PEStream.PEComponent.ReadData(file, address, num1.GetType());
        if ((int) num2 != 17744)
          throw new Win32Exception(11, Resources.GetString("Ex_InvalidPEFormat"));
        this._address = address;
        this._size = this.CalculateSize((object) num2);
        this._data = (object) num2;
      }
    }

    protected class FileHeader : PEStream.PEComponent
    {
      protected PEStream.IMAGE_FILE_HEADER _fileHeader;

      public ushort SizeOfOptionalHeader
      {
        get
        {
          return this._fileHeader.SizeOfOptionalHeader;
        }
      }

      public ushort NumberOfSections
      {
        get
        {
          return this._fileHeader.NumberOfSections;
        }
      }

      public bool IsImageFileDll
      {
        get
        {
          return ((uint) this._fileHeader.Characteristics & 8192U) > 0U;
        }
      }

      public FileHeader(FileStream file, long address)
      {
        this._fileHeader = (PEStream.IMAGE_FILE_HEADER) PEStream.PEComponent.ReadData(file, address, this._fileHeader.GetType());
        this._address = address;
        this._size = this.CalculateSize((object) this._fileHeader);
        this._data = (object) this._fileHeader;
      }
    }

    protected class OptionalHeader : PEStream.PEComponent
    {
      protected PEStream.IMAGE_OPTIONAL_HEADER32 _optionalHeader32;
      protected PEStream.IMAGE_OPTIONAL_HEADER64 _optionalHeader64;
      protected bool _is64Bit;

      public uint CheckSum
      {
        set
        {
          if (this._is64Bit)
          {
            this._optionalHeader64.CheckSum = value;
            this._data = (object) this._optionalHeader64;
          }
          else
          {
            this._optionalHeader32.CheckSum = value;
            this._data = (object) this._optionalHeader32;
          }
        }
      }

      public uint NumberOfRvaAndSizes
      {
        get
        {
          if (this._is64Bit)
            return this._optionalHeader64.NumberOfRvaAndSizes;
          return this._optionalHeader32.NumberOfRvaAndSizes;
        }
      }

      public OptionalHeader(FileStream file, long address)
      {
        this._optionalHeader32 = (PEStream.IMAGE_OPTIONAL_HEADER32) PEStream.PEComponent.ReadData(file, address, this._optionalHeader32.GetType());
        if ((int) this._optionalHeader32.Magic == 523)
        {
          this._is64Bit = true;
          this._optionalHeader64 = (PEStream.IMAGE_OPTIONAL_HEADER64) PEStream.PEComponent.ReadData(file, address, this._optionalHeader64.GetType());
          this._size = this.CalculateSize((object) this._optionalHeader64);
          this._data = (object) this._optionalHeader64;
        }
        else
        {
          if ((int) this._optionalHeader32.Magic != 267)
            throw new NotSupportedException(Resources.GetString("Ex_PEImageTypeNotSupported"));
          this._is64Bit = false;
          this._size = this.CalculateSize((object) this._optionalHeader32);
          this._data = (object) this._optionalHeader32;
        }
        this._address = address;
      }
    }

    protected class DataDirectory : PEStream.PEComponent
    {
      private PEStream.IMAGE_DATA_DIRECTORY _dataDirectory;

      public uint VirtualAddress
      {
        get
        {
          return this._dataDirectory.VirtualAddress;
        }
      }

      public DataDirectory(FileStream file, long address)
      {
        this._dataDirectory = (PEStream.IMAGE_DATA_DIRECTORY) PEStream.PEComponent.ReadData(file, address, this._dataDirectory.GetType());
        this._address = address;
        this._size = this.CalculateSize((object) this._dataDirectory);
        this._data = (object) this._dataDirectory;
      }
    }

    protected class SectionHeader : PEStream.PEComponent
    {
      protected PEStream.IMAGE_SECTION_HEADER _imageSectionHeader;
      protected PEStream.Section _section;

      public PEStream.Section Section
      {
        set
        {
          this._section = value;
        }
      }

      public uint VirtualAddress
      {
        get
        {
          return this._imageSectionHeader.VirtualAddress;
        }
      }

      public uint PointerToRawData
      {
        get
        {
          return this._imageSectionHeader.PointerToRawData;
        }
      }

      public uint SizeOfRawData
      {
        get
        {
          return this._imageSectionHeader.SizeOfRawData;
        }
      }

      public SectionHeader(FileStream file, long address)
      {
        this._imageSectionHeader = (PEStream.IMAGE_SECTION_HEADER) PEStream.PEComponent.ReadData(file, address, this._imageSectionHeader.GetType());
        this._address = address;
        this._size = this.CalculateSize((object) this._imageSectionHeader);
        this._data = (object) this._imageSectionHeader;
      }
    }

    protected class Section : PEStream.PEComponent
    {
      public PEStream.SectionHeader _sectionHeader;

      public Section(FileStream file, PEStream.SectionHeader sectionHeader)
      {
        this._address = (long) sectionHeader.PointerToRawData;
        this._size = (long) sectionHeader.SizeOfRawData;
        this._data = (object) new PEStream.DiskDataBlock(file, this._address, this._size);
        this._sectionHeader = sectionHeader;
      }

      public virtual void AddComponentsToStream(PEStream.StreamComponentList stream)
      {
        stream.Add((PEStream.PEComponent) this);
      }
    }

    protected class ResourceComponent : PEStream.PEComponent
    {
      public virtual void AddComponentsToStream(PEStream.StreamComponentList stream)
      {
        stream.Add((PEStream.PEComponent) this);
      }
    }

    protected class ResourceDirectory : PEStream.ResourceComponent
    {
      protected Hashtable _resourceDirectoryItems = new Hashtable();
      protected ArrayList _resourceDirectoryEntries = new ArrayList();
      protected PEStream.IMAGE_RESOURCE_DIRECTORY _imageResourceDirectory;

      public PEStream.ResourceComponent this[object key]
      {
        get
        {
          if (this._resourceDirectoryItems.Contains(key))
            return (PEStream.ResourceComponent) this._resourceDirectoryItems[key];
          return (PEStream.ResourceComponent) null;
        }
      }

      public int ResourceComponentCount
      {
        get
        {
          return this._resourceDirectoryItems.Count;
        }
      }

      public ResourceDirectory(PEStream.ResourceSection resourceSection, FileStream file, long rootResourceAddress, long resourceAddress, long addressDelta, bool partialConstruct)
      {
        this._imageResourceDirectory = (PEStream.IMAGE_RESOURCE_DIRECTORY) PEStream.PEComponent.ReadData(file, resourceAddress, this._imageResourceDirectory.GetType());
        this._address = resourceAddress;
        this._size = this.CalculateSize((object) this._imageResourceDirectory);
        this._data = (object) this._imageResourceDirectory;
        long address = this._address + this._size;
        for (int index = 0; index < (int) this._imageResourceDirectory.NumberOfIdEntries; ++index)
        {
          PEStream.ResourceDirectoryEntry resourceDirectoryEntry = new PEStream.ResourceDirectoryEntry(file, address);
          this._resourceDirectoryEntries.Add((object) resourceDirectoryEntry);
          address += resourceDirectoryEntry.Size;
        }
        for (int index = 0; index < (int) this._imageResourceDirectory.NumberOfNamedEntries; ++index)
        {
          PEStream.ResourceDirectoryEntry resourceDirectoryEntry = new PEStream.ResourceDirectoryEntry(file, address);
          this._resourceDirectoryEntries.Add((object) resourceDirectoryEntry);
          address += resourceDirectoryEntry.Size;
        }
        foreach (PEStream.ResourceDirectoryEntry resourceDirectoryEntry in this._resourceDirectoryEntries)
        {
          bool flag = false;
          object key;
          if (resourceDirectoryEntry.NameIsString)
          {
            key = (object) resourceSection.CreateResourceDirectoryString(file, rootResourceAddress + resourceDirectoryEntry.NameOffset).NameString;
          }
          else
          {
            key = (object) resourceDirectoryEntry.Id;
            if (rootResourceAddress == resourceAddress && (int) resourceDirectoryEntry.Id == 24)
              flag = true;
          }
          resourceDirectoryEntry.Key = key;
          object obj = (object) null;
          if (resourceDirectoryEntry.IsDirectory)
          {
            if (!partialConstruct || partialConstruct & flag)
              obj = (object) new PEStream.ResourceDirectory(resourceSection, file, rootResourceAddress, rootResourceAddress + resourceDirectoryEntry.OffsetToData, addressDelta, false);
          }
          else
            obj = (object) new PEStream.ResourceData(file, rootResourceAddress, rootResourceAddress + resourceDirectoryEntry.OffsetToData, addressDelta);
          if (obj != null)
            this._resourceDirectoryItems.Add(key, obj);
        }
      }

      public override void AddComponentsToStream(PEStream.StreamComponentList stream)
      {
        stream.Add((PEStream.PEComponent) this);
        foreach (PEStream.ResourceComponent resourceDirectoryEntry in this._resourceDirectoryEntries)
          resourceDirectoryEntry.AddComponentsToStream(stream);
        foreach (PEStream.ResourceComponent resourceComponent in (IEnumerable) this._resourceDirectoryItems.Values)
          resourceComponent.AddComponentsToStream(stream);
      }

      public PEStream.ResourceComponent GetResourceComponent(int index)
      {
        return this[((PEStream.ResourceDirectoryEntry) this._resourceDirectoryEntries[index]).Key];
      }
    }

    protected class ResourceDirectoryEntry : PEStream.ResourceComponent
    {
      protected PEStream.IMAGE_RESOURCE_DIRECTORY_ENTRY _imageResourceDirectoryEntry;
      protected object _key;

      public long NameOffset
      {
        get
        {
          return (long) (this._imageResourceDirectoryEntry.Name & (uint) int.MaxValue);
        }
      }

      public bool NameIsString
      {
        get
        {
          return (this._imageResourceDirectoryEntry.Name & 2147483648U) > 0U;
        }
      }

      public ushort Id
      {
        get
        {
          return (ushort) (this._imageResourceDirectoryEntry.Name & (uint) ushort.MaxValue);
        }
      }

      public long OffsetToData
      {
        get
        {
          return (long) (this._imageResourceDirectoryEntry.OffsetToData & (uint) int.MaxValue);
        }
      }

      public bool IsDirectory
      {
        get
        {
          return (this._imageResourceDirectoryEntry.OffsetToData & 2147483648U) > 0U;
        }
      }

      public object Key
      {
        get
        {
          return this._key;
        }
        set
        {
          this._key = value;
        }
      }

      public ResourceDirectoryEntry(FileStream file, long address)
      {
        this._imageResourceDirectoryEntry = (PEStream.IMAGE_RESOURCE_DIRECTORY_ENTRY) PEStream.PEComponent.ReadData(file, address, this._imageResourceDirectoryEntry.GetType());
        this._address = address;
        this._size = this.CalculateSize((object) this._imageResourceDirectoryEntry);
        this._data = (object) this._imageResourceDirectoryEntry;
      }
    }

    protected class ResourceDirectoryString : PEStream.ResourceComponent
    {
      protected ushort _length;
      protected byte[] _nameStringBuffer;
      protected string _nameString;

      public string NameString
      {
        get
        {
          return this._nameString;
        }
      }

      public ResourceDirectoryString(FileStream file, long offset)
      {
        this._length = (ushort) PEStream.PEComponent.ReadData(file, offset, this._length.GetType());
        if ((int) this._length > 0)
        {
          long length = (long) ((int) this._length * Marshal.SizeOf(typeof (ushort)));
          this._nameStringBuffer = new byte[length];
          long offset1 = offset + this.CalculateSize((object) this._length);
          if (file.Seek(offset1, SeekOrigin.Begin) != offset1)
            throw new IOException(Resources.GetString("Ex_NotEnoughDataInFile"));
          if ((long) file.Read(this._nameStringBuffer, 0, this._nameStringBuffer.Length) < length)
            throw new IOException(Resources.GetString("Ex_NotEnoughDataInFile"));
          this._nameString = Encoding.Unicode.GetString(this._nameStringBuffer);
          this._address = offset;
          this._size = length + this.CalculateSize((object) this._length);
        }
        else
        {
          this._nameStringBuffer = (byte[]) null;
          this._nameString = (string) null;
          this._address = offset;
          this._size = this.CalculateSize((object) this._length);
        }
        this._data = (object) new PEStream.DiskDataBlock(file, this._address, this._size);
      }
    }

    protected class ResourceData : PEStream.ResourceComponent
    {
      protected PEStream.IMAGE_RESOURCE_DATA_ENTRY _resourceDataEntry;
      protected PEStream.ResourceRawData _resourceRawData;

      public byte[] Data
      {
        get
        {
          return this._resourceRawData.Data;
        }
      }

      public ResourceData(FileStream file, long rootResourceAddress, long address, long addressDelta)
      {
        this._resourceDataEntry = (PEStream.IMAGE_RESOURCE_DATA_ENTRY) PEStream.PEComponent.ReadData(file, address, this._resourceDataEntry.GetType());
        this._resourceRawData = new PEStream.ResourceRawData(file, (long) this._resourceDataEntry.OffsetToData - addressDelta, (long) this._resourceDataEntry.Size);
        this._address = address;
        this._size = this.CalculateSize((object) this._resourceDataEntry);
        this._data = (object) this._resourceDataEntry;
      }

      public override void AddComponentsToStream(PEStream.StreamComponentList stream)
      {
        stream.Add((PEStream.PEComponent) this);
        stream.Add((PEStream.PEComponent) this._resourceRawData);
      }

      public void ZeroData()
      {
        this._resourceRawData.ZeroData();
      }
    }

    protected class ResourceRawData : PEStream.ResourceComponent
    {
      public byte[] Data
      {
        get
        {
          byte[] buffer = new byte[this._size];
          if (!(this._data is PEStream.DataComponent))
            throw new NotSupportedException();
          ((PEStream.DataComponent) this._data).Read(buffer, 0, 0L, buffer.Length);
          return buffer;
        }
      }

      public ResourceRawData(FileStream file, long address, long size)
      {
        this._address = address;
        this._size = size;
        this._data = (object) new PEStream.DiskDataBlock(file, address, size);
      }

      public void ZeroData()
      {
        this._data = (object) new PEStream.BlankDataBlock(this._size);
      }
    }

    protected class ResourceSection : PEStream.Section
    {
      protected ArrayList _resourceDirectoryStrings = new ArrayList();
      protected PEStream.ResourceDirectory _resourceDirectory;

      public PEStream.ResourceDirectory RootResourceDirectory
      {
        get
        {
          return this._resourceDirectory;
        }
      }

      public ResourceSection(FileStream file, PEStream.SectionHeader sectionHeader, bool partialConstruct)
        : base(file, sectionHeader)
      {
        this._resourceDirectory = new PEStream.ResourceDirectory(this, file, (long) sectionHeader.PointerToRawData, (long) sectionHeader.PointerToRawData, (long) sectionHeader.VirtualAddress - (long) sectionHeader.PointerToRawData, partialConstruct);
        this._address = 0L;
        this._size = 0L;
        this._data = (object) null;
      }

      public PEStream.ResourceDirectoryString CreateResourceDirectoryString(FileStream file, long offset)
      {
        foreach (PEStream.ResourceDirectoryString resourceDirectoryString in this._resourceDirectoryStrings)
        {
          if (resourceDirectoryString.Address == offset)
            return resourceDirectoryString;
        }
        PEStream.ResourceDirectoryString resourceDirectoryString1 = new PEStream.ResourceDirectoryString(file, offset);
        this._resourceDirectoryStrings.Add((object) resourceDirectoryString1);
        return resourceDirectoryString1;
      }

      public override void AddComponentsToStream(PEStream.StreamComponentList stream)
      {
        this._resourceDirectory.AddComponentsToStream(stream);
        foreach (PEStream.ResourceComponent resourceDirectoryString in this._resourceDirectoryStrings)
          resourceDirectoryString.AddComponentsToStream(stream);
      }
    }

    protected abstract class DataComponent
    {
      public abstract int Read(byte[] buffer, int bufferOffset, long sourceOffset, int count);
    }

    protected class DiskDataBlock : PEStream.DataComponent
    {
      public long _address;
      public long _size;
      public FileStream _file;

      public DiskDataBlock(FileStream file, long address, long size)
      {
        this._address = address;
        this._size = size;
        this._file = file;
      }

      public override int Read(byte[] buffer, int bufferOffset, long sourceOffset, int count)
      {
        int count1 = (int) Math.Min((long) count, this._size - sourceOffset);
        if (count1 < 0)
          throw new ArgumentException(Resources.GetString("Ex_InvalidCopyRequest"));
        this._file.Seek(this._address + sourceOffset, SeekOrigin.Begin);
        return this._file.Read(buffer, bufferOffset, count1);
      }
    }

    protected class BlankDataBlock : PEStream.DataComponent
    {
      public long _size;

      public BlankDataBlock(long size)
      {
        this._size = size;
      }

      public override int Read(byte[] buffer, int bufferOffset, long sourceOffset, int count)
      {
        int num = (int) Math.Min((long) count, this._size - sourceOffset);
        if (num < 0)
          throw new ArgumentException(Resources.GetString("Ex_InvalidCopyRequest"));
        for (int index = 0; index < num; ++index)
          buffer[bufferOffset + index] = (byte) 0;
        return num;
      }
    }
  }
}
