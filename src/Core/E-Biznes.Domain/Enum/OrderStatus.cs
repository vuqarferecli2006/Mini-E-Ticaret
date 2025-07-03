namespace E_Biznes.Domain.Enum;

public enum OrderStatus
{
    Pending = 0,       // Sifariş verildi, amma hələ təsdiqlənməyib
    Confirmed = 1,     // Sifariş təsdiqləndi
    Shipped = 2,       // Məhsul göndərildi
    Delivered = 3,     // Məhsul çatdırıldı
    Cancelled = 4      // Sifariş ləğv edildi
}
