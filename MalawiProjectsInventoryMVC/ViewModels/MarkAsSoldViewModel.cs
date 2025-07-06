namespace MalawiProjectsInventoryMVC.ViewModels;

public class MarkAsSoldViewModel
{
    public string ItemId { get; set; }
    public string DonorId { get; set; }
    public string Description { get; set; }
    public DateTime SoldDate { get; set; }
    public double SoldPrice { get; set; }
}