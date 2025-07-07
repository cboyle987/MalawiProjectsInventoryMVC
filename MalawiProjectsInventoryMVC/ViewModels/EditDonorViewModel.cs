using System.ComponentModel.DataAnnotations;

namespace MalawiProjectsInventoryMVC.ViewModels;

public class EditDonorViewModel
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
}