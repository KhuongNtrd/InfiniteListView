using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfiniteListView.Core.ViewModels;
using Xamarin.Forms;

namespace InfiniteListView.Core
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new MainViewModel();            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.Init();
        }

        public MainViewModel ViewModel { get; set; }
    }
}
