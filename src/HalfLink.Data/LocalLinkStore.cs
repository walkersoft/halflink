using HalfLink.Core.Entities;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text.Json;

namespace HalfLink.Data
{
    public class LocalLinkStore(IJSRuntime js)
    {
        private readonly IJSRuntime js = js;
        private const string STORAGE_KEY = "localLinks";
        private List<Link>? links;

        public event Func<Task>? OnLinksChanged;

        private async Task EnsureStoreLoaded()
        {
            if (links is not null) return;
            var storedLinks = await js.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
            links = string.IsNullOrEmpty(storedLinks)
                ? []
                : JsonSerializer.Deserialize<List<Link>>(storedLinks);
        }

        private async Task UpdateStore()
        {
            await js.InvokeVoidAsync(
                "localStorage.setItem",
                STORAGE_KEY,
                JsonSerializer.Serialize(links)
            );
        }

        private async Task NotifyStoreUpdated()
        {
            if (OnLinksChanged is not null) await OnLinksChanged.Invoke();
        }

        public async Task<List<Link>> GetLocalLinks()
        {
            await EnsureStoreLoaded();
            Debug.Assert(links is not null);
            return links;
        }

        public async Task AddLink(Link link)
        {
            await EnsureStoreLoaded();
            Debug.Assert(links is not null);
            links.Add(link);
            await UpdateStore();
            await NotifyStoreUpdated();
        }

        public async Task RemoveLink(Link link)
        {
            await EnsureStoreLoaded();
            Debug.Assert(links is not null);
            links.Remove(link);
            await UpdateStore();
            await NotifyStoreUpdated();
        }
    }
}
