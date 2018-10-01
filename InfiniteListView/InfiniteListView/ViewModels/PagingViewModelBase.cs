using System.Threading.Tasks;
using InfiniteListView.Core.Mvvm;
using InfiniteListView.Core.Mvvm.Commands;

namespace InfiniteListView.Core.ViewModels
{
    public abstract class PagingViewModelBase : BindableBase
    {
        private bool _hasMoreItems;
        private bool _isBusy;
        private bool _isRefreshing;
        private bool _loadingInfinite;
        private bool _isEmpty;

        protected int ItemsCount { get; set; }

        protected int CurrentPage { get; set; } = 1;

        protected PagingViewModelBase()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LoadingInfinite))
                {
                    if (LoadingInfinite)
                        IsEmpty = false;
                    else
                        IsEmpty = ItemsCount == 0;
                }
            };
        }


        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        public bool LoadingInfinite
        {
            get => _loadingInfinite;
            set => SetProperty(ref _loadingInfinite, value);
        }


        public DelegateCommand RefreshCommand => new DelegateCommand(Refresh);

        public DelegateCommand LoadingCommand => new DelegateCommand(() => LoadAsync());

        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        protected virtual async void Refresh()
        {
            CurrentPage = 1;
            ItemsCount = 0;
            HasMoreItems = false;
            IsRefreshing = false;
            IsBusy = true;
            await LoadAsync();
            IsBusy = false;
        }

        protected abstract Task LoadAsync();
    }
}