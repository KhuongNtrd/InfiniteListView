using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using InfiniteListView.Core.Models;

namespace InfiniteListView.Core.ViewModels
{
    public class MainViewModel : PagingViewModelBase
    {
        public ObservableCollection<Movie> Collection { get; } = new ObservableCollection<Movie>();

        protected override async Task LoadAsync()
        {
            LoadingInfinite = true;

            if (CurrentPage == 1)
                IsRefreshing = true;

            var page = CurrentPage++;

            var response = await FetchAsync(page);

            if (response != null)
            {
                foreach (var item in response.Results)
                    Collection.Add(item);

                HasMoreItems = response.TotalPages > page;
            }
            else
            {
                HasMoreItems = false;
            }

            ItemsCount = Collection.Count;

            LoadingInfinite = false;
            if (IsRefreshing)
                IsRefreshing = false;
        }

        protected override void Refresh()
        {
            CurrentPage = 1;
            Collection.Clear();
            HasMoreItems = true;
            base.Refresh();
        }

        public async Task<PagingResult<Movie>> FetchAsync(int page)
        {
            var api = "https://api.themoviedb.org/3/discover/movie" +
                      "?sort_by=popularity.desc" +
                      $"&page={page}" +
                      "&api_key=c64e3049b5c72f028a704da43f151b4c";

            using (var httpClient = new HttpClient())
            {
                var content = await httpClient.GetStringAsync(api);

                var result = PagingResult<Movie>.FromJson(content);

                return result;
            }
        }

        private bool Loaded { get; set; }

        public void Init()
        {
            if(Loaded)
                return;

            Loaded = true;

            LoadingCommand?.Execute();
        }
    }
}