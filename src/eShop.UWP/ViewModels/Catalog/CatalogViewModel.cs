﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.Practices.ServiceLocation;

using eShop.UWP.Models;
using eShop.UWP.Helpers;
using eShop.Providers;

namespace eShop.UWP.ViewModels
{
    public class CatalogViewModel : CommonViewModel
    {
        public CatalogViewModel()
        {
        }

        public ICatalogProvider DataProvider => ServiceLocator.Current.GetInstance<ICatalogProvider>();

        public CatalogState State { get; private set; }

        public ItemsGridViewModel GridViewModel { get; set; }
        public ItemsListViewModel ListViewModel { get; set; }

        private IList<CatalogTypeModel> _catalogTypes;
        public IList<CatalogTypeModel> CatalogTypes
        {
            get => _catalogTypes;
            set => Set(ref _catalogTypes, value);
        }

        private IList<CatalogBrandModel> _catalogBrands;
        public IList<CatalogBrandModel> CatalogBrands
        {
            get => _catalogBrands;
            set => Set(ref _catalogBrands, value);
        }

        private int _filterTypeId = 0;
        public int FilterTypeId
        {
            get => _filterTypeId;
            set { Set(ref _filterTypeId, value); RefreshItems(); }
        }

        private int _filterBrandId = 0;
        public int FilterBrandId
        {
            get => _filterBrandId;
            set { Set(ref _filterBrandId, value); RefreshItems(); }
        }

        private bool _isGridChecked = false;
        public bool IsGridChecked
        {
            get => _isGridChecked;
            set { Set(ref _isGridChecked, value); ViewSelectionChanged(); }
        }

        private bool _isListChecked = false;
        public bool IsListChecked
        {
            get => _isListChecked;
            set { Set(ref _isListChecked, value); ViewSelectionChanged(); }
        }

        public async Task LoadAsync(CatalogState state)
        {
            State = state;

            GridViewModel.Items = null;
            ListViewModel.Items = null;

            GridViewModel.State = state;
            ListViewModel.State = state;

            _cancelRefresh = true;
            await LoadFiltersAsync();
            await RefreshItemsAsync();
            _cancelRefresh = false;

            IsGridChecked = State.IsGridChecked;
            IsListChecked = State.IsListChecked;
            GridViewModel.Mode = GridCommandBarMode.Idle;

            GridViewModel.UpdateCommandBar();

            HeaderText = String.IsNullOrWhiteSpace(State.Query) ? "Catalog" : $"Catalog results for \"{State.Query}\"";
        }

        private async Task LoadFiltersAsync()
        {
            FilterTypeId = 0;
            FilterBrandId = 0;

            var catalogTypes = await DataProvider.GetCatalogTypesAsync();
            var catalogBrands = await DataProvider.GetCatalogBrandsAsync();

            ListViewModel.CatalogTypes = catalogTypes;
            ListViewModel.CatalogBrands = catalogBrands;

            catalogTypes = catalogTypes.ToList();
            catalogTypes.Insert(0, new CatalogTypeModel(new Data.CatalogType { Id = -1, Type = Constants.CatalogAllViewKey.GetLocalized() }));
            CatalogTypes = catalogTypes;

            catalogBrands = catalogBrands.ToList();
            catalogBrands.Insert(0, new CatalogBrandModel(new Data.CatalogBrand { Id = -1, Brand = Constants.CatalogAllViewKey.GetLocalized() }));
            CatalogBrands = catalogBrands;

            FilterTypeId = State.FilterTypeId;
            FilterBrandId = State.FilterBrandId;
        }

        public async Task UnloadAsync()
        {
            State.FilterTypeId = FilterTypeId;
            State.FilterBrandId = FilterBrandId;
            State.IsGridChecked = IsGridChecked;
            State.IsListChecked = IsListChecked;

            if (GridViewModel.Items != null)
            {
                foreach (var item in GridViewModel.Items.Where(r => r.HasChanges))
                {
                    item.Commit();
                    await DataProvider.SaveItemAsync(item);
                }
            }
        }

        private bool _cancelRefresh = false;

        private async void RefreshItems()
        {
            if (!_cancelRefresh)
            {
                await RefreshItemsAsync();
            }
        }
        private async Task RefreshItemsAsync()
        {
            var items = await DataProvider.GetItemsAsync(FilterTypeId, FilterBrandId, State.Query);
            var collectionItems = new ObservableCollection<CatalogItemModel>(items);
            GridViewModel.Items = collectionItems;
            ListViewModel.Items = collectionItems;
        }

        private void ViewSelectionChanged()
        {
            GridViewModel.IsCommandBarOpen = false;
            ListViewModel.IsCommandBarOpen = false;

            GridViewModel.IsActive = _isGridChecked && !_isListChecked;
            ListViewModel.IsActive = _isListChecked && !_isGridChecked;

            if (GridViewModel.IsActive)
            {
                GridViewModel.UpdateExternalSelection();
            }

            if (ListViewModel.IsActive)
            {
                ListViewModel.UpdateExternalSelection();
            }
        }
    }
}
