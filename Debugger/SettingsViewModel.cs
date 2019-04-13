using System;
using System.Windows.Input;
using CodenjoyBot.Entity;

namespace Debugger
{
    public class SettingsViewModel
    {
        public int Id { get; }
        public string Title { get; }
        
        public ICommand Command { get; }

        public SettingsViewModel(LaunchSettingsModel model)
        {
            Id = model.Id;
            Title = model.Title;
            
            Command = new Command(() => DoCommandEvent?.Invoke(null, this));
        }

        public event EventHandler<SettingsViewModel> DoCommandEvent;
    }
}