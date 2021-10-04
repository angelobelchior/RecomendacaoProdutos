using System.Collections.Generic;
using System.Linq;
using ECommerce.Models;
using ECommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ECommerce.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Products _products;

        [BindProperty] public string Term { get; set; }

        public IndexModel(ILogger<IndexModel> logger, Products products)
        {
            _logger = logger;
            _products = products;
        }

        public IEnumerable<Product> Products { get; private set; }

        public void OnGet()
        {
            Products = _products.List();
        }

        public void OnPost()
        {
            Products = _products.Search(Term);
        }
    }
}