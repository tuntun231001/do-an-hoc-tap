using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TECH.Areas.Admin.Models;
using TECH.Areas.Admin.Models.Search;
using TECH.Service;

namespace TECH.Controllers.Components

{
    //[ViewComponent(Name = "CategoryMenuComponent")]
    public class ProductNewComponent : ViewComponent
    {

        private readonly IProductsService _productsService;
        private readonly ICategoryService _categoryService;
        public ProductNewComponent(IProductsService productsService,
            ICategoryService categoryService)
        {
            _productsService = productsService;
            _categoryService = categoryService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productModelViewSearch = new ProductViewModelSearch();
            productModelViewSearch.status = 1;
            productModelViewSearch.PageIndex = 1;
            productModelViewSearch.PageSize = 10;
            var categoryModel = _productsService.GetAllPaging(productModelViewSearch);
            var model = new List<ProductModelView>();
            if (categoryModel.Results != null && categoryModel.Results.Count > 0)
            {
                categoryModel.Results = categoryModel.Results.Where(p=>p.ishidden != 1).OrderByDescending(p => p.id).ToList();
                foreach (var item in categoryModel.Results)
                {
                    if (item.category_id.HasValue && item.category_id.Value > 0)
                    {
                        var category = _categoryService.GetByid(item.category_id.Value);
                        if (category != null && !string.IsNullOrEmpty(category.name))
                        {
                            item.categorystr = category.name;
                        }
                        else
                        {
                            item.categorystr = "Chờ xử lý";
                        }
                    }
                    else
                    {
                        item.categorystr = "";
                    }                    
                    //item.trademark = !string.IsNullOrEmpty(item.trademark) ? item.trademark : "";
                    item.price_sell_str = item.price_sell.HasValue && item.price_sell.Value > 0 ? item.price_sell.Value.ToString("#,###") : "";
                    item.price_import_str = item.price_import.HasValue && item.price_import.Value > 0 ? item.price_import.Value.ToString("#,###") : "";
                    item.price_reduced_str = item.price_reduced.HasValue && item.price_reduced.Value > 0 ? item.price_reduced.Value.ToString("#,###") : "";
                    //item.total_product = 10;
                }

                model = categoryModel.Results.ToList();
                //if (model != null && model.Count > 0)
                //{
                //    var models = model.GroupBy(p => p.category_id.Value).ToList();
                //    if (models != null && models.Count > 0)
                //    {
                //        foreach (var item in models)
                //        {
                //            if (item.Key)
                //            {

                //            }
                //        }
                //    }
                   
                //}
            }
            return View(model);
        }
    }
}