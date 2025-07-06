namespace MalawiProjectsInventoryMVC.Entities;

public class Donation
{
    public DateTime DonationDate { get; set; }
    public List<DonatedItem> DonatedItems { get; set; } = new();
}