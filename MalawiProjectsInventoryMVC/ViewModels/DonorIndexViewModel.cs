using MalawiProjectsInventoryMVC.Entities;

namespace MalawiProjectsInventoryMVC.ViewModels;

public class DonorIndexViewModel
{
    public List<Donor> Donors { get; set; }
    public bool IsAdmin { get; set; }
}