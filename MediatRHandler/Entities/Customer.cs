using System.ComponentModel.DataAnnotations;

namespace MediatRHandler.Entities
{
    public class Customer
    {
        [Key]
        public Ulid Id { get; set; } /*= Ulid.NewUlid();*/
        //  public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }
    }
}
