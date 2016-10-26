// Decompiled with JetBrains decompiler
// Type: System.Deployment.Application.HashCollection
// Assembly: System.Deployment, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3325BD7A-5C2D-4917-8EF5-AD0E0DDAE2E8
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Deployment.dll

using System.Collections;
using System.Deployment.Internal.Isolation.Manifest;

namespace System.Deployment.Application
{
  internal class HashCollection : IEnumerable
  {
    protected ArrayList _hashes = new ArrayList();

    public int Count
    {
      get
      {
        return this._hashes.Count;
      }
    }

    public void AddHash(byte[] digestValue, CMS_HASH_DIGESTMETHOD digestMethod, CMS_HASH_TRANSFORM transform)
    {
      this._hashes.Add((object) new Hash(digestValue, digestMethod, transform));
    }

    public HashCollection.HashEnumerator GetEnumerator()
    {
      return new HashCollection.HashEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    public class HashEnumerator : IEnumerator
    {
      private int _index;
      private HashCollection _hashCollection;

      public Hash Current
      {
        get
        {
          return (Hash) this._hashCollection._hashes[this._index];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return this._hashCollection._hashes[this._index];
        }
      }

      public HashEnumerator(HashCollection hashCollection)
      {
        this._hashCollection = hashCollection;
        this._index = -1;
      }

      public void Reset()
      {
        this._index = -1;
      }

      public bool MoveNext()
      {
        this._index = this._index + 1;
        return this._index < this._hashCollection._hashes.Count;
      }
    }
  }
}
