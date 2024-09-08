using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TCategory
    {
        public TCategory()
        {
            TProduct1s = new HashSet<TProduct1>();
            TSubcategoryids = new HashSet<TSubcategoryid>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<TProduct1> TProduct1s { get; set; }
        public virtual ICollection<TSubcategoryid> TSubcategoryids { get; set; }
    }
}
