using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CollaborativeBased.Models;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

namespace CollaborativeBased
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Variáveis de Apoio
            const string _productsList = "/Users/angelobelchior/desktop/VSSummit2021/products.csv";
            const string producsCopurchasedPath = "/Users/angelobelchior/desktop/VSSummit2021/producs_copurchased.csv";
            const string producsRecomendationsPath = "/Users/angelobelchior/desktop/VSSummit2021/producs_recomendations.csv";
            #endregion
            
            var context = new MLContext();

            #region Carregando os dados de producsCopurchasedPath
            Console.WriteLine($"Carregando os dados de {producsCopurchasedPath}");
            var data = context.Data.LoadFromTextFile(path:producsCopurchasedPath,
                columns: new[]
                {
                    new TextLoader.Column("Label", DataKind.Single, 0),
                    new TextLoader.Column(name:nameof(Produto.ProdutoId), dataKind:DataKind.UInt32, source: new [] { new TextLoader.Range(0) }, keyCount: new KeyCount(100)), 
                    new TextLoader.Column(name:nameof(Produto.ProdutoRelacionadoId), dataKind:DataKind.UInt32, source: new [] { new TextLoader.Range(1) }, keyCount: new KeyCount(100))
                },
                hasHeader: true,
                separatorChar: ';');
            #endregion Carregando os dados de producsCopurchasedPath

            #region Criando a configuração do MatrixFactorizationTrainer
            Console.WriteLine("Criando a configuração do MatrixFactorizationTrainer");
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(Produto.ProdutoId),
                MatrixRowIndexColumnName = nameof(Produto.ProdutoRelacionadoId),
                LabelColumnName= "Label",
                NumberOfIterations = 50,
                ApproximationRank = 100,                
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
            };
            #endregion Criando a configuração do MatrixFactorizationTrainer
            
            #region Efetuando o treinamento
            Console.WriteLine($"Efetuando o treinamento");
            var estimator = context.Recommendation().Trainers.MatrixFactorization(options);
            #endregion Efetuando o treinamento
            
            #region Efetuando a predição: Produto 20 (Notebook), Produto Relacionado 19 (HD Externo)
            Console.WriteLine("Efetuando a predição: Produto 20 (Notebook), Produto Relacionado 19 (HD Externo)");
            var model = estimator.Fit(data);
            var engine = context.Model.CreatePredictionEngine<Models.Produto, Models.Prediction>(model);
            var prediction = engine.Predict(
                new Produto
                {
                    ProdutoId = 20, //Notebook
                    ProdutoRelacionadoId = 19 //HD Externo
                });
            Console.WriteLine($"Prediction Score: {Math.Round(prediction.Score, 2)}");
            #endregion Efetuando a predição: Produto 20 (Notebook), Produto Relacionado 19 (HD Externo)
            
            #region Salvando o Modelo Gerado para ser usado no Site
            //Console.WriteLine($"Salvando o Modelo Gerado em {producsCopurchasedModelPath} para ser usado no Site");
            //context.Model.Save(model, data.Schema, producsCopurchasedModelPath);
            #endregion Salvando o Modelo Gerado para ser usado no Site
            
            #region Gerando lista com probabilidades de um determinado produto ser recomendado
            
            Console.WriteLine($"Gerando lista com probabilidades de um determinado produto ser recomendado");

            Console.WriteLine($"Carregando a lista de Produtos");
            var allLines = File.ReadAllLines(_productsList);
            var allProducts = new List<(uint id, string name)>();
            foreach (var line in allLines.Skip(1))
            {
                var parts = line.Split(';');
                allProducts.Add((uint.Parse(parts[0]), parts[1]));
            }

            var recomendationsList = new List<ProdutoRecomendacao>();
            foreach (var product in allProducts)
            {
                foreach (var recomendation in allProducts)
                {
                    if(product.id == recomendation.id) continue;
                    
                    prediction = engine.Predict(
                        new Produto
                        {
                            ProdutoId = product.id,
                            ProdutoRelacionadoId = recomendation.id
                        });
                    Console.WriteLine($"A probabilidade do produto {recomendation.name} ser recomendao ao comprar o produto {product.name} é de {Math.Round(prediction.Score, 1)}");

                    if (prediction.Score > 0)
                        recomendationsList.Add(new ProdutoRecomendacao
                        {
                            ProdutoId = product.id,
                            ProdutoRelacionadoId = recomendation.id,
                            Score = prediction.Score
                        });
                }
            }

            if(File.Exists(producsRecomendationsPath))
                File.Delete(producsRecomendationsPath);

            var content = ProdutoRecomendacao.ToCsv(recomendationsList);
            File.WriteAllText(producsRecomendationsPath, content);

            #endregion Gerando lista com probabilidades de um determinado produto ser recomendado
        }
    }
}
