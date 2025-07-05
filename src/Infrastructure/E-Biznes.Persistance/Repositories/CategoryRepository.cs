using AzBinaTeam.Persistance.Repositories;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Domain.Entities;
using E_Biznes.Persistance.Contexts;

namespace E_Biznes.Persistance.Repositories;

public class CategoryRepository : Repository<Category>,ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }
}
