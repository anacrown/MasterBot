using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.CodenjoyBotInstance
{
    public class CodenjoyBotInstanceCollection : ObservableCollection<CodenjoyBotInstance>
    {
        public CodenjoyBotInstanceCollection()
        {
        }

        public CodenjoyBotInstanceCollection(IEnumerable<CodenjoyBotInstance> collection) : base(collection)
        {
            foreach (var botInstance in collection)
            {
                botInstance.Started += BotInstanceOnStarted;
                botInstance.Stopped += BotInstanceOnStopped;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.OldItems != null)
            {
                foreach (var eOldItem in e.OldItems)
                {
                    if (eOldItem is CodenjoyBotInstance botInstance)
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
                    if (eNewItem is CodenjoyBotInstance botInstance)
                    {
                        botInstance.Started += BotInstanceOnStarted;
                        botInstance.Stopped += BotInstanceOnStopped;
                    }
                }
            }
        }

        private void BotInstanceOnStarted(object sender, IDataProvider e) => OnStarted(sender as CodenjoyBotInstance);

        private void BotInstanceOnStopped(object sender, IDataProvider e) => OnStopped(sender as CodenjoyBotInstance);

        public event EventHandler<CodenjoyBotInstance> Started;
        public event EventHandler<CodenjoyBotInstance> Stopped;

        protected virtual void OnStarted(CodenjoyBotInstance e) => Started?.Invoke(this, e);

        protected virtual void OnStopped(CodenjoyBotInstance e) => Stopped?.Invoke(this, e);
    }
}