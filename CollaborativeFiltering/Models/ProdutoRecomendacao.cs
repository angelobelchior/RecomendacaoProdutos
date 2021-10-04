using System.Collections.Generic;
using System.Text;

namespace CollaborativeBased.Models
{
    public class ProdutoRecomendacao
    {
        public static string ToCsv(List<ProdutoRecomendacao> products)
        {
            var sb = new StringBuilder();
            sb.AppendLine("ProdutoId;ProdutoRelacionadoId;Score");

            foreach (var product in products)
                sb.AppendLine(product.ToLine());

            return sb.ToString();
        }
        
        public uint ProdutoId { get; set; }
        
        public uint ProdutoRelacionadoId { get; set; }
        
        public float Score { get; set; }

        public string ToLine()
            => $"{ProdutoId};{ProdutoRelacionadoId};{Score}";
    }
}