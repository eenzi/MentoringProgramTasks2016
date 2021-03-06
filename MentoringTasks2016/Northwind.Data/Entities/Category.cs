using System.Collections.Generic;
using LinqToDB.Mapping;

namespace Northwind.Data.Entities
{
    [Table(Schema = "dbo", Name = "Categories")]
    public class Category
    {
        [PrimaryKey, Identity]
        public int CategoryID { get; set; } // int
        [Column, NotNull]
        public string CategoryName { get; set; } // nvarchar(15)
        [Column, Nullable]
        public string Description { get; set; } // ntext
        [Column, Nullable]
        public byte[] Picture { get; set; } // image

        #region Associations

        /// <summary>
        /// FK_Products_Categories_BackReference
        /// </summary>
        [Association(ThisKey = "CategoryID", OtherKey = "CategoryID", CanBeNull = true, IsBackReference = true)]
        public IEnumerable<Product> Products { get; set; }

        #endregion
    }
}