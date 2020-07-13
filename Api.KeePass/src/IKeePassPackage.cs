using System;
using NerdyMishka.Api.KeePass.Package;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassPackage : IDisposable
    {
        CompositeKey CompositeKey { get; }

        HeaderInfo HeaderInfo { get; }

        IPackageMetaInfo MetaInfo { get; }

        IKeePassDocument Document { get; }

        CustomDataDictionary CustomData { get; }

        MemoryProtectedBytesMap BinaryMap { get; }

        /*
        void AttachFile(IKeePassEntry entry, string path);

        void AttachBinary(IKeePassEntry entry, string key, byte[] data);
        //void AttachBinary(IKeePassEntry entry, string key, ProtectedMemoryBinary protectedMemoryBinary);

        IKeePassGroup CreateGroup(
            string path,
            bool force = false);

        IKeePassGroup CreateGroup(
           string path,
           IKeePassGroup group,
           bool force = false);

        IKeePassEntry CreateEntry(
            string path,
            byte[] password = null,
            string username = null,
            string uri = null,
            string notes = null,
            IEnumerable<string> tags = null,
            bool force = false);

        IKeePassEntry CreateEntry(
           string path,
           IKeePassEntry entry,
           bool force = false);

        IKeePassEntry FindEntry(
           string path,
           bool caseInsensitive = true);

        IKeePassEntry[] FindEntriesMatchesByField(
           string name,
           string value);

        IKeePassEntry[] FindEntriesMatchesByField(
           string name,
           string value,
           bool caseSensitve,
           StringValueComparison stringValueComparison);

        IKeePassEntry[] FindEntriesByTitle(
            string title);

        IKeePassEntry[] FindEntriesByTitle(
            string title,
            bool caseSensitve,
            StringValueComparison stringValueComparison);

        IKeePassGroup FindGroup(
            string path,
            bool caseInsensitive = true);

        void Merge(IKeePassPackage package);

        IKeePassPackage SetKey(MasterKey key);

        IKeePassPackage Open(MasterKey key, Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Open(Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Save(MasterKey key, Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Save(Stream stream, IKeePassPackageSerializer serializer);
        */
    }
}