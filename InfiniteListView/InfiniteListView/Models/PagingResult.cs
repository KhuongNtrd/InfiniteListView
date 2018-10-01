// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var pagingResult = PagingResult.FromJson(jsonString);

using System.Collections.Generic;
using Newtonsoft.Json;

namespace InfiniteListView.Core.Models
{
    public partial class PagingResult<T>
    {
        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("total_results")]
        public long TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public long TotalPages { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }

    public partial class PagingResult<T>
    {
        public static PagingResult<T> FromJson(string json) => JsonConvert.DeserializeObject<PagingResult<T>>(json);
    }
}
