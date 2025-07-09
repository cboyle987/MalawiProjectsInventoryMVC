using MalawiProjectsInventoryMVC.Entities;

namespace MalawiProjectsInventoryMVC.ViewModels;

public class DonorDetailsViewModel
{
    public Donor Donor { get; set; }
    public bool IsAdmin { get; set; }
}