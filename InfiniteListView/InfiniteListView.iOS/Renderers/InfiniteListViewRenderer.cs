using InfiniteListView.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(InfiniteListView.Core.Controls.InfiniteListView), typeof(InfiniteListViewRenderer))]
namespace InfiniteListView.iOS.Renderers
{
    public class InfiniteListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.TableFooterView = new UIView();
        }
    }
}