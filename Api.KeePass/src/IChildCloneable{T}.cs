namespace NerdyMishka.Api.KeePass
{
    public interface IChildCloneable<T>
    {
        T Clone(IKeePassPackage package, IKeePassGroup parent);
    }
}