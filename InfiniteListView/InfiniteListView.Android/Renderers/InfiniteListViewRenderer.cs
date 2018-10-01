using Android.Content;
using Android.Graphics.Drawables;
using InfiniteListView.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(InfiniteListView.Core.Controls.InfiniteListView), typeof(InfiniteListViewRenderer))]
namespace InfiniteListView.Droid.Renderers
{
    public class InfiniteListViewRenderer : ListViewRenderer
    {
        public InfiniteListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;
        }
    }
}