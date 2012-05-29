using System;

namespace BookInfo
{
    public abstract class Entity
    {
        public Entity() { }

        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
