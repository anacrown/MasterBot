using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using BotBase.Interfaces;

namespace BotBase
{
    public class BotInstanceCollection : ObservableCollection<BotInstance>
    {
        public BotInstanceCollection(IEnumerable<BotInstance> collection) : base(collection)
        {
            foreach (var botInstance in collection)
            {
                botInstance.Started += BotInstanceOnStarted;
                botInstance.Stopped += BotInstanceOnStopped;
            }
        }

        public BotInstanceCollection()
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.OldItems != null)
            {
                foreach (var eOldItem in e.OldItems)
                {
                    if (eOldItem is BotInstance botInstance)
                    {
                        botInstance.Started -= BotInstanceOnStarted;
                        botInstance.Stopped -= BotInstanceOnStopped;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var eNewItem in e.NewItems)
                {
                    if (eNewItem is BotInstance botInstance)
                    {
                        botInstance.Started += BotInstanceOnStarted;
                        botInstance.Stopped += BotInstanceOnStopped;
                    }
                }
            }
        }

        private void BotInstanceOnStarted(object sender, IDataProvider e) => OnStarted(sender as BotInstance);

        private void BotInstanceOnStopped(object sender, IDataProvider e) => OnStopped(sender as BotInstance);

        public event EventHandler<BotInstance> Started;
        public event EventHandler<BotInstance> Stopped;

        protected virtual void OnStarted(BotInstance e) => Started?.Invoke(this, e);

        protected virtual void OnStopped(BotInstance e) => Stopped?.Invoke(this, e);
    }
}