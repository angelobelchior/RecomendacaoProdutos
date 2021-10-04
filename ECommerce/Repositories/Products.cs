using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ECommerce.Models;

namespace ECommerce.Repositories
{
    public class Products
    {
        #region Caminho dos Arquivos
        private const string _productsList = "/Users/angelobelchior/desktop/VSSummit2021/products.csv";
        private const string _producsRecomendationsPath = "/Users/angelobelchior/desktop/VSSummit2021/producs_recomendations.csv";
        #endregion Caminho dos Arquivos
        
        private static List<Product> _allProducts = new List<Product>();
        private static List<Recomendacao> _recomendations = new List<Recomendacao>();

        static Products()
        {
            if (!File.Exists(_productsList))
                throw new FileNotFoundException(_productsList);
            
            var allLines = File.ReadAllLines(_productsList);
            foreach (var line in allLines.Skip(1))
            {
                var parts = line.Split(';');
                var product = new Models.Product(int.Parse(parts[0]), parts[1], parts[2],decimal.Parse(parts[3]));
                _allProducts.Add(product);
            }

            if (!File.Exists(_producsRecomendationsPath))
                return;
            
            allLines = File.ReadAllLines(_producsRecomendationsPath);
            foreach (var line in allLines.Skip(1))
            {
                var parts = line.Split(';');
                _recomendations.Add(new Recomendacao
                {
                    Id = int.Parse(parts[0]),
                    RecomendacaoId = int.Parse(parts[1]),
                    Score = float.Parse(parts[2])
                });
            }            
        }
        
        public IEnumerable<Product> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
                return List();

            var random = new Random();
            return _allProducts.Where(p => p.Name.ToLower().Contains(term.ToLower()));
        }
        
        public IEnumerable<Product> List(int max = 5)
        {
            var random = new Random();
            return _allProducts.OrderBy(p => random.Next())
                               .Take(max);
        }

        public Product GetById(int id)
        {
            return _allProducts.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Product> GetRecomendations(int id, float scoreMin = 0.7f, int max = 3)
            => _recomendations
                .Where(r => r.Id == id && r.Score >= scoreMin)
                .OrderByDescending(r => r.Score)
                .Take(max)
                .Select(r =>
                {
                    var produto = GetById(r.RecomendacaoId);
                    produto.Score = r.Score;
                    return produto;
                });
    }
}