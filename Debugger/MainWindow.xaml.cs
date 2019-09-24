using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using BotBase;

namespace Debugger
{
    public partial class MainWindow
    {
        //        private SettingsCollection _unVisibleSettings;

        public static readonly DependencyProperty BotInstancesProperty = DependencyProperty.Register(
            "BotInstances", typeof(BotInstanceCollection), typeof(MainWindow), new PropertyMetadata(default(BotInstanceCollection)));

        public BotInstanceCollection BotInstances
        {
            get => (BotInstanceCollection)GetValue(BotInstancesProperty);
            set => SetValue(BotInstancesProperty, value);
        }

        public static readonly DependencyProperty SelectedBotInstanceProperty = DependencyProperty.Register(
            "SelectedBotInstance", typeof(BotInstance), typeof(MainWindow), new PropertyMetadata(default(BotInstance)));

        public BotInstance SelectedBotInstance
        {
            get => (BotInstance) GetValue(SelectedBotInstanceProperty);
            set => SetValue(SelectedBotInstanceProperty, value);
        }

        public MainWindow()
        {
            AppData.Set();

            InitializeComponent();

            var dllFileNames = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .Where(t => Path.GetExtension(t) == ".dll" || 
                            Path.GetExtension(t) == ".exe")
                .ToArray();

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (var dllFile in dllFileNames)
            {
                var an = AssemblyName.GetAssemblyName(dllFile);
                var assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            foreach (var assembly in assemblies)
            {
                try
                {
                    var resourceFile = $"/{assembly.GetName().Name};component/ResourceDictionary.xaml";

                    var myResourceDictionary = Application.LoadComponent(new Uri(resourceFile, UriKind.RelativeOrAbsolute)) as ResourceDictionary;

                    Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);

                    Console.WriteLine($"LOADED {resourceFile}");
                }
                catch (System.IO.IOException e)
                {

                }
                catch (Exception e)
                {
                    throw;
                }

            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var fs = new FileStream(Path.Combine(FileSystemConfigurator.AppDataDir, "Settings.bin"), FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    var settings = formatter.Deserialize(fs);

                    BotInstances = new BotInstanceCollection((settings as BotInstanceSettings[])?.Select(t => new BotInstance(t)));
                }
            }
            catch (Exception exception)
            {
                BotInstances = new BotInstanceCollection();
                Console.WriteLine(exception);
                //throw;
            }
        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            foreach (var codenjoyBotInstance in BotInstances)
                codenjoyBotInstance?.Stop();

            SaveSettings();
        }

        private void SaveSettings()
        {
            using (var fs = new FileStream(Path.Combine(FileSystemConfigurator.AppDataDir, "Settings.bin"), FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, BotInstances.Select(t => t.Settings).ToArray());
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBotInstance?.Start();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedBotInstance?.Stop();
        }

        private void BattleBotInstanceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.Count > 0 ? e.AddedItems[0] as BotInstance : null;
            var oldValue = e.RemovedItems.Count > 0 ? e.RemovedItems[0] as BotInstance : null;

            SelectedBotInstance = newValue;
        }

        private void BotInstanceList_OnRemoveInstance(object sender, BotInstance botInstance)
        {
            //            if (botInstance.IsStarted)
            //                botInstance.Stop();
            //
            //            using (var db = new BotDbContext())
            //            {
            //                var settings = db.LaunchSettingsModels.Find(botInstance.SettingsId);
            //                if (settings == null)
            //                    throw new Exception("Settings not found");
            //
            //                UnVisibleSettings.Add(new SettingsViewModel(settings));
            //
            //                settings.Visibility = false;
            //
            //                db.SaveChanges();
            //            }
        }
        private void BotInstancesOnStarted(object sender, BotInstance botInstance)
        {
            //            var settings = UnVisibleSettings.FirstOrDefault(t => t.Id == botInstance.SettingsId);
            //            if (settings != null)
            //                Dispatcher.Invoke(() => UnVisibleSettings.Remove(settings));

            Dispatcher.Invoke(() => TabControl.SelectedIndex = 1);
        }
    }
}
