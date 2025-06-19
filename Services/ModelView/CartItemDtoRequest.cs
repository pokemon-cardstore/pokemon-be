namespace Services.ModelView
{
    public class CartItemDtoRequest
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
