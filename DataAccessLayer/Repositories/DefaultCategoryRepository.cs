using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;

namespace DataAccessLayer.Repositories;

public class DefaultCategoryRepository : IDefaultCategoryDal
{
    private readonly Context _context;

    public DefaultCategoryRepository(Context context) => _context = context;

    public void Delete(DefaultCategory category) { _context.Remove(category); _context.SaveChanges(); }
    public DefaultCategory GetByID(int id) => _context.DefaultCategories.Find(id)!;
    public List<DefaultCategory> GetListAll() => _context.DefaultCategories.OrderBy(category => category.Name).ToList();
    public void Insert(DefaultCategory category) { _context.Add(category); _context.SaveChanges(); }
    public void Update(DefaultCategory category) { _context.Update(category); _context.SaveChanges(); }
}
