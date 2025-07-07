using System.Runtime.InteropServices.JavaScript;
using MalawiProjectsInventoryMVC.Context;
using MalawiProjectsInventoryMVC.Entities;
using MalawiProjectsInventoryMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MalawiProjectsInventoryMVC.Controllers;

public class DonorController: Controller
{
    private readonly DatabaseContext _context;

    public DonorController(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var donors = await _context.Donors.ToListAsync();
        return View(donors);
    }

    public IActionResult Create()
    {
        var vm = new CreateDonorViewModel();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDonorViewModel createVm)
    {
        if (!ModelState.IsValid) return View(createVm);
        var donor = new Donor()
        {
            Id = Guid.NewGuid().ToString(),
            Donations = new List<Donation>(),
            Name = createVm.Name,
            Address = createVm.Address,
        };
        _context.Donors.Add(donor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = donor.Id });
    }

    public async Task<IActionResult> Edit(string id)
    {
        var donor = await _context.Donors.FindAsync(id);
        var vm = new EditDonorViewModel()
        {
            Id = donor.Id,
            Name = donor.Name,
            Address = donor.Address,
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditDonorViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var donor = await _context.Donors.FindAsync(vm.Id);
        donor.Name = vm.Name;
        donor.Address = vm.Address;
        _context.Donors.Update(donor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    public async Task<IActionResult> Delete(string id)
    {
        var donor = await _context.Donors.FindAsync(id);
        if (donor != null)
        {
            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var donor = await _context.Donors.FindAsync(id);
        return View(donor);
    }

    [HttpPost]
    public async Task<IActionResult> AddDonation(string donorId, Donation donation)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null) return NotFound();

        if (donation.DonationDate == DateTime.MinValue)
        {
            ModelState.AddModelError( nameof(Donation.DonationDate),"Donation date must be selected");
            return RedirectToAction(nameof(Details), new { id = donorId });
        }
        donation.DonatedItems ??= new List<DonatedItem>();
        donor.Donations.Add(donation);

        _context.Update(donor);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = donorId });
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(string donorId, int donationIndex, DonatedItem item)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null || donationIndex >= donor.Donations.Count) return NotFound();

        item.Id = Guid.NewGuid().ToString();
        donor.Donations[donationIndex].DonatedItems.Add(item);

        _context.Update(donor);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = donorId });
    }
    
    public async Task<IActionResult> MarkItemSold(string donorId, string itemId)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null) return NotFound();

        foreach (var donation in donor.Donations)
        {
            var item = donation.DonatedItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                var vm = new MarkAsSoldViewModel()
                {
                    Description = item.Description,
                    ItemId = item.Id,
                    DonorId = donor.Id,
                    SoldDate = DateTime.Today,
                    SoldPrice = 0.00
                };
                
                return View(vm);
            }
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> MarkItemSold(MarkAsSoldViewModel vm)
    {
        var donor = await _context.Donors.FindAsync(vm.DonorId);
        if (donor == null) return NotFound();

        foreach (var donation in donor.Donations)
        {
            var item = donation.DonatedItems.FirstOrDefault(i => i.Id == vm.ItemId);
            if (item != null)
            {
                item.SoldDate = vm.SoldDate;
                item.SoldPrice = vm.SoldPrice;
                break;
            }
        }

        _context.Update(donor);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id = vm.DonorId });
    }
    
    public async Task<IActionResult> PrintQRCodes(string donorId, int donationIndex)
    {
        var donor = await _context.Donors.FindAsync(donorId);
        if (donor == null || donationIndex >= donor.Donations.Count)
            return NotFound();

        var donation = donor.Donations[donationIndex];

        // Only show unsold items
        var unsoldItems = donation.DonatedItems
            .Where(i => !i.SoldDate.HasValue)
            .Select(i => new ItemQrCodeViewModel()
            {
                ItemId = i.Id,
                Description = i.Description,
                DonorId = donor.Id,
                DonorName = donor.Name,
                QrCodeUrl = Url.Action("MarkItemSold", "Donor", new { donorId, itemId = i.Id }, Request.Scheme)
            })
            .ToList();

        ViewBag.DonorName = donor.Name;
        ViewBag.DonationDate = donation.DonationDate;

        return View(unsoldItems);
    }
}