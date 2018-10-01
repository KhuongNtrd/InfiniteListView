using Newtonsoft.Json;

namespace InfiniteListView.Core.Models
{
    public static class Serialize
    {
        public static string ToJson<T>(this PagingResult<T> self) => JsonConvert.SerializeObject(self);
    }
}