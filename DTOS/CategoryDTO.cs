using Minimart_Api.Models;
using System.Collections;

namespace Minimart_Api.DTOS
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        //Navigation Property of indepent table
        public virtual ICollection<SubCategoryDTO> Subcategoryids { get; set; }

    }

  

}
