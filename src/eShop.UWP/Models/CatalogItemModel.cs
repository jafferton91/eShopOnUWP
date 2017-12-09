﻿using System;

using GalaSoft.MvvmLight;

using eShop.Data;
using eShop.Providers;

namespace eShop.UWP.Models
{
    public class CatalogItemModel : ObservableObject
    {
        public CatalogItemModel(int id = 0) : this(new CatalogItem { Id = id })
        {
        }
        public CatalogItemModel(CatalogItem source)
        {
            Source = source;
            CopyValues(Source);
        }

        public CatalogItem Source { get; private set; }

        public int Id => Source.Id;

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private double _price;
        public double Price
        {
            get => _price;
            set { Set(ref _price, value); RaisePropertyChanged("PriceDesc"); }
        }

        public string PriceDesc => $"${Price.ToString("0.00")}";

        public string PriceString
        {
            get => Price.ToString("0.00");
            set => Price = ParseDecimal(value);
        }

        private double ParseDecimal(string value)
        {
            double d = 0;
            Double.TryParse(value, out d);
            return d;
        }

        public string PictureFileName { get; set; }
        public byte[] Picture { get; set; }
        public string PictureContentType { get; set; }

        private string _pictureUri;
        public string PictureUri
        {
            get => _pictureUri;
            set => Set(ref _pictureUri, value);
        }

        private CatalogTypeModel _catalogType;
        public CatalogTypeModel CatalogType
        {
            get => _catalogType;
            set => Set(ref _catalogType, value);
        }

        private CatalogBrandModel _catalogBrand;
        public CatalogBrandModel CatalogBrand
        {
            get => _catalogBrand;
            set => Set(ref _catalogBrand, value);
        }

        private bool _isDisabled;
        public bool IsDisabled
        {
            get => _isDisabled;
            set => Set(ref _isDisabled, value);
        }

        // Management Properties

        public bool IsSelected { get; set; }

        private bool _isNew = false;
        public bool IsNew
        {
            get => _isNew;
            set => Set(ref _isNew, value);
        }

        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get => _isDeleted;
            set => Set(ref _isDeleted, value);
        }

        public bool HasChanges => Source.Name != Name ||
                                  Source.Description != Description ||
                                  Source.Price != Price ||
                                  Source.PictureUri != PictureUri ||
                                  Source.CatalogTypeId != CatalogType.Id ||
                                  Source.CatalogBrandId != CatalogBrand.Id ||
                                  Source.IsDisabled != IsDisabled;

        public void Undo()
        {
            CopyValues(Source);

            IsDeleted = false;
            RaisePropertyChanged(nameof(HasChanges));
        }

        private void CopyValues(CatalogItem source)
        {
            Name = source.Name;
            Description = source.Description;
            Price = source.Price;
            PictureUri = source.PictureUri;
            CatalogType = CatalogProvider.GetCatalogType(source.CatalogTypeId) ?? new CatalogTypeModel();
            CatalogBrand = CatalogProvider.GetCatalogBrand(source.CatalogBrandId) ?? new CatalogBrandModel();
            IsDisabled = source.IsDisabled;
        }

        public void Commit()
        {
            Source.Name = Name;
            Source.Description = Description;
            Source.Price = Price;
            Source.PictureUri = PictureUri;
            Source.CatalogTypeId = CatalogType.Id;
            Source.CatalogBrandId = CatalogBrand.Id;
            Source.IsDisabled = IsDisabled;

            RaisePropertyChanged(nameof(HasChanges));
        }

        public override bool Equals(object obj)
        {
            var model = obj as CatalogItemModel;
            if (model != null)
            {
                return model.Id == Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
