namespace MobileClient
{
    /// <summary>
    /// Marker interface for SharedResource so we can inject IStringLocalizer<SharedResource>
    /// </summary>
    public interface ISharedResource
    {
    }

    /// <summary>
    /// Marker class for SharedResource so we can inject IStringLocalizer<SharedResource>
    /// </summary>
    public class SharedResource : ISharedResource
    {
    }
}