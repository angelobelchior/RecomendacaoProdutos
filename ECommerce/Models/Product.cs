namespace ECommerce.Models
{
    public class Product
    {
        public Product(int id, string name, string image, decimal price, float? score = null)
        {
            Id = id;
            Name = name;
            Image = image;
            Price = price;
            Score = score;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public float? Score { get; set; }
    }
}