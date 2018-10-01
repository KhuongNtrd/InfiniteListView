using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace InfiniteListView.Core.Controls
{
    public class InfiniteListView: ListView
    {
        public static BindableProperty CollectionProperty =
               BindableProperty.Create(nameof(Collection), typeof(ICollection), typeof(InfiniteListView));

        public static readonly BindableProperty HasMoreItemsProperty =
            BindableProperty.Create("HasMoreItems", typeof(bool), typeof(InfiniteListView), false);

        public static readonly BindableProperty IsLoadingInfiniteProperty =
            BindableProperty.Create("IsLoadingInfinite", typeof(bool), typeof(InfiniteListView), false,
                BindingMode.TwoWay);

        public static readonly BindableProperty IsLoadingInfiniteEnabledProperty =
            BindableProperty.Create("IsLoadingInfiniteEnabled", typeof(bool), typeof(InfiniteListView), false);

        public static readonly BindableProperty LoadingCommandProperty =
            BindableProperty.Create("LoadingCommand", typeof(ICommand), typeof(InfiniteListView));

        public InfiniteListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            PropertyChanged += OnPropertyChanged;
            ItemAppearing += ViewItemAppearing;
            ItemTemplate = new CustomTemplateSelector(GetItemTemplate);
            if (Device.RuntimePlatform == Device.Android)
                Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ListView.SetIsFastScrollEnabled(this, true);
        }

        public ICollection Collection
        {
            get => (IList)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }


        public bool HasMoreItems
        {
            get => (bool)GetValue(HasMoreItemsProperty);
            set => SetValue(HasMoreItemsProperty, value);
        }


        public bool IsLoadingInfinite
        {
            get => (bool)GetValue(IsLoadingInfiniteProperty);
            set => SetValue(IsLoadingInfiniteProperty, value);
        }


        public bool IsLoadingInfiniteEnabled
        {
            get => (bool)GetValue(IsLoadingInfiniteEnabledProperty);
            set => SetValue(IsLoadingInfiniteEnabledProperty, value);
        }


        public ICommand LoadingCommand
        {
            get => (ICommand)GetValue(LoadingCommandProperty);
            set => SetValue(LoadingCommandProperty, value);
        }


        public DataTemplate ListItemTemplate { get; set; }

        public DataTemplate LoadingItemTemplate { get; set; }

        private DataTemplate GetItemTemplate(object item, BindableObject bindableObject)
        {
            if (item is LoadingModel)
                return LoadingItemTemplate;
            return ListItemTemplate;
        }

        private void ReloadContainerList(NotifyCollectionChangedAction? changedAction = null)
        {
            ItemAppearing -= ViewItemAppearing;
            ItemsSource = GetContainerList(changedAction);
            ItemAppearing += ViewItemAppearing;
        }

        private void ViewItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is LoadingModel)
                if (IsLoadingInfiniteEnabled && !IsRefreshing && HasMoreItems && !IsLoadingInfinite)
                {
                    IsLoadingInfinite = true;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        ScrollTo(e.Item, ScrollToPosition.End, false);
                    }

                    if (LoadingCommand.CanExecute(null))
                        LoadingCommand.Execute(null);
                }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == HasMoreItemsProperty.PropertyName)
                if (HasMoreItems == false)
                {
                    if (ItemsSource is IList list && list.Count > 0)
                    {
                        var item = list[list.Count - 1];
                        if (item is LoadingModel)
                            list.Remove(item);
                    }
                }
                else
                {
                    if (HasMoreItems)
                    {
                        if (ItemsSource is IList list)
                        {
                            if (list.Count > 0)
                            {
                                var item = list[list.Count - 1];
                                if (!(item is LoadingModel))
                                    list.Add(new LoadingModel());
                            }
                        }
                    }
                }
            if (e.PropertyName == CollectionProperty.PropertyName)
            {
                ReloadContainerList();

                if (Collection is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged -= CollectionChanged;
                    collection.CollectionChanged += CollectionChanged;
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var i = 0;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (ItemsSource is IList source)
                            {
                                foreach (var item in e.NewItems)
                                {
                                    var index = e.NewStartingIndex + i++;
                                    source.Insert(index, item);
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (ItemsSource is IList source)
                            {
                                foreach (var item in e.OldItems)
                                {
                                    source.Remove(item);
                                }
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        ReloadContainerList(e.Action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }


        private ObservableCollection<object> GetContainerList(NotifyCollectionChangedAction? changedAction)
        {
            if (Collection == null)
                return new ObservableCollection<object>();

            var tempList = new List<object>();

            if (Collection != null && Collection.Count == 0)
            {
                if (IsLoadingInfiniteEnabled && HasMoreItems && !IsRefreshing && changedAction != NotifyCollectionChangedAction.Reset)
                    tempList.Add(new LoadingModel());
            }
            else
            {
                foreach (var item in Collection)
                    tempList.Add(item);

                if (IsLoadingInfiniteEnabled && HasMoreItems && !IsRefreshing)
                    tempList.Add(new LoadingModel());
            }

            return new ObservableCollection<object>(tempList);
        }


        private class LoadingModel
        {
        }

        private class CustomTemplateSelector : DataTemplateSelector
        {
            private readonly Func<object, BindableObject, DataTemplate> _getDataTemplate;

            public CustomTemplateSelector(Func<object, BindableObject, DataTemplate> getDataTemplate)
            {
                _getDataTemplate = getDataTemplate;
            }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            {
                return _getDataTemplate(item, container);
            }
        }
    }
}
