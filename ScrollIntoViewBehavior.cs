using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace VMIClientePix
{
    public class ScrollIntoViewBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            ListView listView = AssociatedObject;
            ((INotifyCollectionChanged)listView.Items).CollectionChanged += OnListView_CollectionChanged;
        }

        protected override void OnDetaching()
        {
            ListView listView = AssociatedObject;
            ((INotifyCollectionChanged)listView.Items).CollectionChanged -= OnListView_CollectionChanged;
        }

        private void OnListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListView listView = AssociatedObject;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (listView.Items.Count > 0)
                    // scroll the new item into view   
                    listView.ScrollIntoView(listView.Items[listView.Items.Count - 1]);
            }
        }
    }
}
