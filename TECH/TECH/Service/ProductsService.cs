
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TECH.Areas.Admin.Models;
using TECH.Areas.Admin.Models.Search;
using TECH.Data.DatabaseEntity;
using TECH.General;
using TECH.Reponsitory;
using TECH.Utilities;

namespace TECH.Service
{
    public interface IProductsService
    {
        PagedResult<ProductModelView> GetAllPaging(ProductViewModelSearch ProductModelViewSearch);
        ProductModelView GetByid(int id);
        int Add(ProductModelView view);
        bool Update(ProductModelView view);
        //void UpdateQuantitySell(int productId, decimal quantitySell);
        bool Deleted(int id);
        void Save();
        bool IsProductNameExist(string name);
        bool UpdateStatus(int id, int status);
        void UpdateImage(int id, string image);
        void UpdateQuantitySell(int id, int sell);
        int GetCount();
        int GetCountForCategory(int cateogryId);
        List<ProductModelView> GetProductReLated(int categoryId, int productId);
        List<ProductModelView> GetProductLike(int categoryId);
        List<ProductModelView> ProductSearch(string textSearch);
       
    }

    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private IUnitOfWork _unitOfWork;
        public ProductsService(IProductsRepository productsRepository,
            IUnitOfWork unitOfWork)
        {
            _productsRepository = productsRepository;
            _unitOfWork = unitOfWork;
        }

        

        public bool IsProductNameExist(string name)
        {
            var data = _productsRepository.FindAll().Any(p=>p.name == name);
            return data;
        }
        public void UpdateQuantitySell(int id, int sell)
        {
            if (id > 0)
            {
                var dataServer = _productsRepository.FindById(id);
                if (dataServer != null)
                {
                    dataServer.quantitysell = sell;
                    _productsRepository.Update(dataServer);
                    Save();
                }
            }
        }
        public ProductModelView GetByid(int id)
        {
            var data = _productsRepository.FindAll(p => p.id == id).FirstOrDefault();
            if (data != null)
            {                
                var model = new ProductModelView()
                {
                    id = data.id,
                    name = data.name,
                    category_id = data.category_id,
                    price_sell = data.price_sell.HasValue ? data.price_sell.Value:0,
                    price_import = data.price_import.HasValue ? data.price_import.Value :0,
                    colorId = data.color,                   
                    price_reduced = data.price_reduced.HasValue ? data.price_reduced.Value:0,
                    status = data.status,
                    description = data.description,
                    quantity = data.quantity,
                    quantitysell = data.quantitysell,
                    images =data.images,
                    specifications = data.specifications
                };
                return model;
            }
            return null;
        }
        public int Add(ProductModelView view)
        {
            try
            {
                if (view != null)
                {
                    var products = new Products
                    {
                        name = view.name,
                        //avatar = view.avatar,
                        category_id = view.category_id,
                        //slug = Regex.Replace(view.name.ToLower(), @"\s+", "-"),
                        //color = view.color,
                        color = view.colorId,
                        price_sell = view.price_sell,
                        price_import = view.price_import,
                        price_reduced = view.price_reduced,
                        status = view.status,
                        quantity = view.quantity,
                        ishidden = 0,                       
                        description = view.description,
                        specifications = view.specifications
                    };
                    _productsRepository.Add(products);
                    Save();
                    return products.id;                    
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;

        }
        public int GetCount()
        {
            int count = 0;
            count = _productsRepository.FindAll().Count();
            return count;
        }

        public int GetCountForCategory(int cateogryId)
        {
            int count = 0;
            count = _productsRepository.FindAll(p=>p.category_id == cateogryId).Count();
            return count;
        }
        public void Save()
        {
            _unitOfWork.Commit();
        }
        public bool Update(ProductModelView view)
        {
            try
            {
                var dataServer = _productsRepository.FindById(view.id);
                if (dataServer != null)
                {
                    dataServer.category_id = view.category_id;
                    dataServer.name = view.name;
                    dataServer.color = view.colorId;
                    dataServer.quantity = view.quantity;
                    dataServer.price_sell = view.price_sell.HasValue ? view.price_sell.Value : 0;
                    dataServer.price_import = view.price_import.HasValue ? view.price_import.Value : 0;
                    dataServer.price_reduced = view.price_reduced.HasValue ? view.price_reduced.Value : 0;
                    dataServer.status = view.status;
                    dataServer.description = view.description;
                    //quantitysell = p.quantitysell,
                    dataServer.specifications = view.specifications;
                    _productsRepository.Update(dataServer);                                        
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

      public  bool UpdateStatus(int id, int status)
        {
            try
            {
                var dataServer = _productsRepository.FindById(id);
                if (dataServer != null)
                {
                    dataServer.ishidden = status;
                    _productsRepository.Update(dataServer);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
        public void UpdateImage(int id, string image)
        {
            try
            {
                var dataServer = _productsRepository.FindById(id);
                if (dataServer != null)
                {
                    dataServer.images = image;
                    _productsRepository.Update(dataServer);                    
                }
            }
            catch (Exception ex) { 

            }
        }

        public bool Deleted(int id)
        {
            try
            {
                var dataServer = _productsRepository.FindById(id);
                if (dataServer != null)
                {
                    _productsRepository.Remove(dataServer);
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return false;
        }
        public List<ProductModelView> GetProductReLated(int categoryId, int productId)
        {
            var query = _productsRepository.FindAll(p=>p.ishidden == 0).Where(p=>p.category_id == categoryId && p.id != productId);
            var data = query.Select(p => new ProductModelView()
            {
                id = p.id,
                category_id = p.category_id,
                name = p.name,
                status = p.status,
                description = p.description
            }).Take(4).ToList();
            return data;
        }

        public List<ProductModelView> GetProductLike(int categoryId)
        {
            var query = _productsRepository.FindAll(p => p.ishidden == 0).Where(p => p.category_id != categoryId);
            var data = query.Select(p => new ProductModelView()
            {
                id = p.id,
                category_id = p.category_id,
                name = p.name,
                status = p.status,
                description = p.description,
            }).ToList();
            return data;
        }

        public PagedResult<ProductModelView> GetAllPaging(ProductViewModelSearch ProductModelViewSearch)
        {
            try
            {
                var query = _productsRepository.FindAll(p=>p.category_id.HasValue && p.category_id.Value > 0);

                if (ProductModelViewSearch.categoryId.HasValue && ProductModelViewSearch.categoryId.Value > 0)
                {
                    query = query.Where(c => c.category_id == ProductModelViewSearch.categoryId.Value);
                }
                
                if (!string.IsNullOrEmpty(ProductModelViewSearch.name))
                {
                    query = query.Where(c => c.name.ToLower().Trim().Contains(ProductModelViewSearch.name.ToLower().Trim()));
                }

                int totalRow = query.Count();
                query = query.OrderByDescending(p => p.id);
                // giá tăng dần
                if (ProductModelViewSearch.order == 2)
                {
                    query = query.OrderBy(p => p.price_sell);
                }else if (ProductModelViewSearch.order == 3)
                {
                    query = query.OrderByDescending(p => p.price_sell);
                }   
                query = query.Skip((ProductModelViewSearch.PageIndex - 1) * ProductModelViewSearch.PageSize).Take(ProductModelViewSearch.PageSize);
                var data = query.Select(p => new ProductModelView()
                {
                    id = p.id,
                    category_id = p.category_id,
                    name = p.name,
                    status = p.status,
                    colorId = p.color,
                    quantity = p.quantity,
                    quantitysell = p.quantitysell,
                    colorStr = p.color.HasValue && p.color.Value > 0 ? Common.GetColor(p.color.Value) : "",
                    price_sell = p.price_sell,
                    price_import = p.price_import,
                    price_reduced = p.price_reduced,
                    description = p.description,
                    specifications = p.specifications,
                    ishidden = p.ishidden,
                    images = p.images,
                    statusstr = p.status.HasValue && p.status.Value > 0? Common.GetStatusStr(p.status.Value):""
                }).ToList();             
              
                var pagingData = new PagedResult<ProductModelView>
                {
                    Results = data,
                    CurrentPage = ProductModelViewSearch.PageIndex,
                    PageSize = ProductModelViewSearch.PageSize,
                    RowCount = totalRow,
                };
                return pagingData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }       

        public List<ProductModelView> ProductSearch(string textSearch)
        {
            if (!string.IsNullOrEmpty(textSearch))
            {
                var query = _productsRepository.FindAll().Where(p=>p.name.ToLower().Contains(textSearch.ToLower().Trim())).Select(p => new ProductModelView()
                {
                    id = p.id,
                    category_id = p.category_id,
                    name = p.name,
                    status = p.status,
                    description = p.description
                }).ToList();
                return query;
            }
            return null;
        }
    }
}
