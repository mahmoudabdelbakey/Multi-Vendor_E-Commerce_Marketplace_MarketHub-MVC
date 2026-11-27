using MarketHub.BLL.Common;
using MarketHub.BLL.Models;
using MarKetHub.DAL.Enums;

namespace MarketHub.MVC.ViewModels;

/// <summary>TASK-29. The vendors list of the Super Admin: one page of vendors and the status filter that is selected.</summary>
public class VendorApplicationsViewModel
{
    public PagedResult<VendorDto> Result { get; init; } = new();
    public VendorStatus? Status { get; init; }
}
