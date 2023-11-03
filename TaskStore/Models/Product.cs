using System.ComponentModel.DataAnnotations.Schema;

namespace TaskStore.Models
{
    public class Product
    {
        public int ProductId { get; set; } // Идентификатор товара
        public string Name { get; set; } // Название товара
        public string Description { get; set; } // Описание товара

        
        public int StoreId { get; set; } // Идентификатор магазина, в котором находится товар
        [ForeignKey("StoreId")]
        public Store? Store { get; set; } // Ссылка на магазин, в котором находится товар
    }
}
