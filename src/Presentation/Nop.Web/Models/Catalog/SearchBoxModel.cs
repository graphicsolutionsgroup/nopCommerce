using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Catalog;

public partial record SearchBoxModel : BaseNopModel
{
    public int SearchCategoryId { get; set; }
    public List<SelectListItem> AvailableCategories { get; set; } = new();
}