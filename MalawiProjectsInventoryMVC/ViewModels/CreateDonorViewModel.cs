using System.ComponentModel.DataAnnotations;

namespace MalawiProjectsInventoryMVC.ViewModels;

public class CreateDonorViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
}