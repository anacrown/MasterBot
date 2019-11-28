using System.Collections.ObjectModel;

namespace DataBaseDataProviderView
{
    public class SettingsCollection : ObservableCollection<SettingsViewModel>
    {
//        public SettingsCollection(IEnumerable<SettingsViewModel> collection) : base(collection)
//        {
//            foreach (var viewModel in collection)
//            {
//                viewModel.DoCommandEvent += ViewModelOnDoCommandEvent;
//            }
//        }
//
//        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
//        {
//            base.OnCollectionChanged(e);
//
//            if (e.OldItems != null)
//            {
//                foreach (var eOldItem in e.OldItems)
//                {
//                    if (eOldItem is SettingsViewModel viewModel)
//                    {
//                        viewModel.DoCommandEvent -= ViewModelOnDoCommandEvent;
//                    }
//                }
//            }
//
//            if (e.NewItems != null)
//            {
//                foreach (var eNewItem in e.NewItems)
//                {
//                    if (eNewItem is SettingsViewModel viewModel)
//                    {
//                        viewModel.DoCommandEvent += ViewModelOnDoCommandEvent;
//                    }
//                }
//            }
//        }
//
//        public event EventHandler<SettingsViewModel> DoCommandEvent;
//        private void ViewModelOnDoCommandEvent(object sender, SettingsViewModel e) => DoCommandEvent?.Invoke(this, e);
    }
}