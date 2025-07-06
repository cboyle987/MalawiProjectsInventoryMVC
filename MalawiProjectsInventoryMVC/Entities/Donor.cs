namespace MalawiProjectsInventoryMVC.Entities;

public class Donor
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Donation> Donations { get; set;} = new();
}