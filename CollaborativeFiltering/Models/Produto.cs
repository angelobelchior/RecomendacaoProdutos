using Microsoft.ML.Data;

namespace CollaborativeBased.Models
{
    public class Produto
    {
        // OS números inteiros devem ser uint
        
        [KeyType(count : 100)]
        public uint ProdutoId { get; set; }
        
        [KeyType(count : 100)]
        public uint ProdutoRelacionadoId { get; set; }
    }
}