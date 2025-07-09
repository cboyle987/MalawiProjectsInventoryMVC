using MalawiProjectsInventoryMVC.Context;
using MalawiProjectsInventoryMVC.Entities;
using Microsoft.EntityFrameworkCore;

namespace MalawiProjectsInventoryMVC.Services;

public interface IDonorService
{
    public Task<Donor> GetDonor(string donorId);
    public Task<List<Donor>> GetAllDonors();
    public Task<Donor> EditDonor(string donorId, string name, string address);
    public Task DeleteDonor(string donorId);
    public Task<Donor> AddDonor(string name, string address);
    public Task AddDonation(string donorId, Donation donation);
    public Task AddItem(string donorId, int donationIndex, DonatedItem item);
    public Task UpdateItem(string donorId,  DonatedItem item);

    public Task<DonatedItem?> GetDonatedItem(string donorId, string itemId);
}


public class DonorService: IDonorService
{
    private readonly DatabaseContext _context;

    public DonorService(DatabaseContext context)
    {
        _context = context;
    }


    public async Task<Donor> GetDonor(string donorId)
    {
        var donor =  await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }

        return donor;
    }

    public async Task<List<Donor>> GetAllDonors()
    {
        return await _context.Donors.ToListAsync();
    }

    public async Task<Donor> EditDonor(string donorId, string name, string address)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }
        
        donor.Name = name;
        donor.Address = address;
        await _context.SaveChangesAsync();
        return donor;
    }

    public async Task DeleteDonor(string donorId)
    {
        var donor  = await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }
        
        _context.Donors.Remove(donor);
        await _context.SaveChangesAsync();
    }

    public async Task<Donor> AddDonor(string name, string address)
    {
        var donor = new Donor()
        {
            Id = Guid.NewGuid().ToString(),
            Donations = new List<Donation>(),
            Name = name,
            Address = address
        };
        await _context.Donors.AddAsync(donor);
        await _context.SaveChangesAsync();
        return donor;
    }

    public async Task AddDonation(string donorId, Donation donation)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }
        donation.DonatedItems = new List<DonatedItem>();
        donor.Donations.Add(donation);
        _context.Donors.Update(donor);
        await _context.SaveChangesAsync();
    }

    public async Task AddItem(string donorId, int donationIndex, DonatedItem item)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null || donationIndex >= donor.Donations.Count)
        {
            throw new Exception("Donor Not Found");
        }

        item.Id = Guid.NewGuid().ToString();
        donor.Donations[donationIndex].DonatedItems.Add(item);

        _context.Update(donor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItem(string donorId, DonatedItem updatedItem)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }

        foreach (var donation in donor.Donations)
        {
            var item = donation.DonatedItems.FirstOrDefault(i => i.Id == updatedItem.Id);
            if (item != null)
            {
                item.SoldDate = updatedItem.SoldDate;
                item.SoldPrice = updatedItem.SoldPrice;
                break;
            }
        }
        _context.Update(donor);
        await _context.SaveChangesAsync();
    }

    public async Task<DonatedItem?> GetDonatedItem(string donorId, string itemId)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null)
        {
            throw new Exception("Donor Not Found");
        }

        foreach (var donation in donor.Donations)
        {
            var item = donation.DonatedItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                return item;
            }
        }

        return null;
    }
}