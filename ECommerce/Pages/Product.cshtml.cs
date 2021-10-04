using System.Collections.Generic;
using System.Linq;
using ECommerce.Models;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ECommerce.Pages
{
    public class ProductModel : PageModel
    {
        private readonly ILogger<ProductModel> _logger;
        private readonly Products _products;

        public ProductModel(ILogger<ProductModel> logger, Products products)
        {
            _logger = logger;
            _products = products;
        }

        public Product Product { get; private set; }
        public IEnumerable<Product> Recommendations { get; private set; }
        
        public void OnGet(int id)
        {
            Product = _products.GetById(id);
            Recommendations = _products.GetRecomendations(id);
        }
    }
}