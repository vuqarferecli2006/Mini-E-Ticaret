namespace E_Biznes.Domain.Enum;

public enum OrderStatus
{
    Pending,       // Sifariş verildi, amma hələ təsdiqlənməyib
    Confirmed,     // Sifariş təsdiqləndi
    Shipped,       // Məhsul göndərildi
    Delivered,     // Məhsul çatdırıldı
    Cancelled      // Sifariş ləğv edildi
}
