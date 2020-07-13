namespace NerdyMishka.Api.KeePass
{
    public interface IKeePassChild
    {
        IKeePassPackage Package { get; set; }

        IKeePassGroup Parent { get; set; }
    }
}