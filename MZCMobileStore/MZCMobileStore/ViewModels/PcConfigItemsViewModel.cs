﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MZCMobileStore.Models;
using MZCMobileStore.Services;
using MZCMobileStore.Services.Interfaces;
using MZCMobileStore.ViewModels.Base;
using MZCMobileStore.Views;
using Xamarin.Forms;

namespace MZCMobileStore.ViewModels
{
    public class PcConfigItemsViewModel : BaseViewModel
    {
        private readonly IPcConfigurationRepository _pcConfigurationRepository;
        private PcConfiguration _selectedItem;

        public ObservableCollection<PcConfiguration> PcConfigurations { get; }
        public Command LoadConfigurationsCommand { get; }
        public Command<PcConfiguration> ConfigurationTapped { get; }

        public PcConfigItemsViewModel(IPcConfigurationRepository pcConfigurationRepository)
        {
            Title = "Конфигурации ПК";

            _pcConfigurationRepository = pcConfigurationRepository;
            PcConfigurations = new ObservableCollection<PcConfiguration>();

            LoadConfigurationsCommand = new Command(async () => await ExecuteLoadConfigurationsCommand());
            ConfigurationTapped = new Command<PcConfiguration>(OnConfigurationSelected);
        }

        private async Task ExecuteLoadConfigurationsCommand()
        {
            IsBusy = true;

            try
            {
                PcConfigurations.Clear();
                var items = await _pcConfigurationRepository.GeAllAsync();
                foreach (var item in items)
                {
                    PcConfigurations.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public PcConfiguration SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                OnConfigurationSelected(value);
            }
        }

        async void OnConfigurationSelected(PcConfiguration config)
        {
            if (config == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(PcConfigItemDetailViewModel.ItemId)}={config.Id}");
        }
    }
}