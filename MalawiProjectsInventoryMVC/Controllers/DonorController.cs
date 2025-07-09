using System.Runtime.InteropServices.JavaScript;
using MalawiProjectsInventoryMVC.Context;
using MalawiProjectsInventoryMVC.Entities;
using MalawiProjectsInventoryMVC.Services;
using MalawiProjectsInventoryMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MalawiProjectsInventoryMVC.Controllers;

public class DonorController: Controller
{
    private readonly IDonorService _donorService;
    private readonly IUserService _userService;

    public DonorController(DatabaseContext context, IUserService userService, IDonorService donorService)
    {
        _userService = userService;
        _donorService = donorService;
    }

    public async Task<IActionResult> Index()
    {
        var isAdmin = _userService.IsAdmin();
        var donors = await _donorService.GetAllDonors();
        var vm = new DonorIndexViewModel()
        {
            Donors = donors,
            IsAdmin = isAdmin
        };
        return View(vm);
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
        var donor = await _donorService.AddDonor(createVm.Name, createVm.Address);
        return RedirectToAction(nameof(Details), new { id = donor.Id });
    }

    public async Task<IActionResult> Edit(string id)
    {
        var donor = await _donorService.GetDonor(id);
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
        var donor = await _donorService.EditDonor(vm.Id, vm.Name, vm.Address);
        return RedirectToAction(nameof(Details), new { id = donor.Id });
    }

    public async Task<IActionResult> Delete(string id)
    {
        await _donorService.DeleteDonor(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var donor = await _donorService.GetDonor(id);
        return View(donor);
    }

    [HttpPost]
    public async Task<IActionResult> AddDonation(string donorId, Donation donation)
    {
        if (donation.DonationDate == DateTime.MinValue)
        {
            ModelState.AddModelError( nameof(Donation.DonationDate),"Donation date must be selected");
            return RedirectToAction(nameof(Details), new { id = donorId });
        }

        await _donorService.AddDonation(donorId, donation);
        return RedirectToAction("Details", new { id = donorId });
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(string donorId, int donationIndex, DonatedItem item)
    {
        await _donorService.AddItem(donorId, donationIndex, item);
        return RedirectToAction("Details", new { id = donorId });
    }
    
    public async Task<IActionResult> MarkItemSold(string donorId, string itemId)
    {
        var item = await _donorService.GetDonatedItem(donorId, itemId);
        if (item == null)
        {
            return NotFound();
        }

        var vm = new MarkAsSoldViewModel()
        {
            Description = item.Description,
            DonorId = donorId,
            ItemId = itemId,
            SoldDate = DateTime.Today,
            SoldPrice = 0.00
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> MarkItemSold(MarkAsSoldViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var updatedItem = new DonatedItem()
        {
            Id = vm.ItemId,
            Description = vm.Description,
            SoldDate = vm.SoldDate,
            SoldPrice = vm.SoldPrice
        };
        await _donorService.UpdateItem(vm.DonorId, updatedItem);
        return RedirectToAction("Details", new { id = vm.DonorId });
    }
    
    public async Task<IActionResult> PrintQRCodes(string donorId, int donationIndex)
    {
        var donor = await _donorService.GetDonor(donorId);

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