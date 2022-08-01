using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Babelon.Infrastucture.Components
{
    public class CategoriesViewComponent :ViewComponent
    {
        private readonly DataContext _Context;
        public CategoriesViewComponent(DataContext Context)
        {
            _Context = Context;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _Context.Categories.ToListAsync());
    }
}
