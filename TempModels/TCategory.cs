using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TCategory
    {
        public TCategory()
        {
            TProducts = new HashSet<TProduct>();
            TSubcategoryids = new HashSet<TSubcategoryid>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<TProduct> TProducts { get; set; }
        public virtual ICollection<TSubcategoryid> TSubcategoryids { get; set; }
    }
}
