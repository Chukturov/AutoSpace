using System.ComponentModel.DataAnnotations;

namespace TaskStore.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string WorkingTime { get; set; }
        //[DataType(DataType.Time)]
        //public DateTime OpeningHours { get; set; }
        //[DataType(DataType.Time)]
        //public DateTime CloseningHours { get; set; }
        public List<Product>? Products { get; set; }
    }
}
