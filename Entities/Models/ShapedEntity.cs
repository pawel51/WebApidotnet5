using System;
using System.Dynamic;

namespace Entities.Models
{
    public class ShapedEntity
    {
        public ShapedEntity()
        {
            Entity = new ExpandoObject();
        }
        public Guid Id { get; set; }
        public ExpandoObject Entity { get; set; }
    }
}
