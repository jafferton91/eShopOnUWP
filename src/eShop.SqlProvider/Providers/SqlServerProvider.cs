using System;
using System.Data;

namespace eShop.SqlProvider
{
    using Properties;
    using CatalogDSTableAdapters;

    public partial class SqlServerProvider
    {
        public SqlServerProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString
        {
            get => Settings.Default.ConnectionString;
            set => Settings.Default.ConnectionString = value;
        }

        // GET

        public DataTable GetCatalogTypes()
        {
            var dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogBrands()
        {
            var dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogItem(int id)
        {
            var dataAdapter = new CatalogItemTableAdapter();
            return dataAdapter.GetData(id);
        }

        public DataTable GetCatalogItems()
        {
            var dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.GetData();
        }

        public DataTable GetCatalogItemsFilter(int typeId = -1, int brandId = -1, string query = null)
        {
            if (!String.IsNullOrEmpty(query))
            {
                query = String.Format("%{0}%", query);
            }
            var dataAdapter = new CatalogItemsFilterTableAdapter();
            return dataAdapter.GetData(typeId, brandId, query);
        }

        public DataTable GetCatalogImage(int id)
        {
            var dataAdapter = new CatalogPictureTableAdapter();
            return dataAdapter.GetData(id);
        }

        // INSERT

        public int InsertCatalogType(int id, string name)
        {
            var dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Insert(id, name);
        }

        public int InsertCatalogBrand(int id, string name)
        {
            var dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Insert(id, name);
        }

        public int InsertCatalogItem(int id, string name, string description, string pictureName, double price, int typeId, int brandId, bool isDisabled)
        {
            var dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Insert(id, name, description, pictureName, price, typeId, brandId, isDisabled, DateTime.UtcNow);
        }

        public int InsertCatalogImage(int id, byte[] imageBytes)
        {
            // TODO: Check if exists
            var dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Insert(id, imageBytes);
        }

        // UPDATE

        public int UpdateCatalogType(int id, string type)
        {
            var dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Update(type, id);
        }
        public int UpdateCatalogType(DataRow dataRow)
        {
            var dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogBrand(int id, string brand)
        {
            var dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Update(brand, id);
        }
        public int UpdateCatalogBrand(DataRow dataRow)
        {
            var dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogItem(int id, string name, string description, string pictureName, double price, int typeId, int brandId, bool isDisabled)
        {
            var dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Update(name, description, pictureName, price, typeId, brandId, isDisabled, DateTime.UtcNow, id);
        }
        public int UpdateCatalogItem(DataRow dataRow)
        {
            var dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        public int UpdateCatalogImage(int id, byte[] imageBytes)
        {
            var dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Update(imageBytes, id);
        }
        public int UpdateCatalogImage(DataRow dataRow)
        {
            var dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Update(dataRow);
        }

        // DELETE

        public int DeleteCatalogType(int id)
        {
            var dataAdapter = new CatalogTypesTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogBrand(int id)
        {
            var dataAdapter = new CatalogBrandsTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogItem(int id)
        {
            var dataAdapter = new CatalogItemsTableAdapter();
            return dataAdapter.Delete(id);
        }

        public int DeleteCatalogImage(int id)
        {
            var dataAdapter = new CatalogPicturesTableAdapter();
            return dataAdapter.Delete(id);
        }
    }
}
