namespace MalawiProjectsInventoryMVC.Entities;

public class DonatedItem
{
    public string Id { get; set; }
    public string Description { get; set; }
    public DateTime? SoldDate { get; set; }
    public double SoldPrice { get; set; }
}