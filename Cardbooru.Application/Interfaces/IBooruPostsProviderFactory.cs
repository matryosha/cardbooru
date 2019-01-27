namespace Cardbooru.Application.Interfaces
{
    public interface IBooruPostsProviderFactory
    {
        BooruPostsProvider Create();
        BooruPostsProvider CreateFrom(BooruPostsProvider srcProvider);
    }
}