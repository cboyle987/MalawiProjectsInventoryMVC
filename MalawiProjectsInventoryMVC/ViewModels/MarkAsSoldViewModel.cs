using System.ComponentModel.DataAnnotations;

namespace MalawiProjectsInventoryMVC.ViewModels;

public class MarkAsSoldViewModel
{
    public string ItemId { get; set; }
    public string DonorId { get; set; }
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Sold date is required")]
    [DataType(DataType.Date)]
    public DateTime SoldDate { get; set; }
    [Required(ErrorMessage = "Sold price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Sold price must be greater than 0")]
    public double SoldPrice { get; set; }
}