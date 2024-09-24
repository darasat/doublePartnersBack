namespace doublePartnersBack.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string UserId { get; set; } // Puedes usar un tipo adecuado para tu UserId
    }
}
