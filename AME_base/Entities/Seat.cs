
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AME_base
{
    public interface ISeat
    {
        string FirstName  { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string AlternateEmail { get; set; }

    }

    public class Seat: ISeat
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 0)]
        public byte GroupID { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(Order = 1)]
        public byte SeatNum { get; set; }

        //[Required] could be blank
        public string FirstName { get; set; }
        //[Required] could be blank
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string AlternateEmail { get; set; }

        [Required]
        public virtual ICollection<Attempt> Attempts { get; set; }
        //[Required]
        //public ICollection<Mark> Marks { get; set; }
        //[Required]
        //public virtual ICollection<Response> Responses { get; set; } //removed. otherwise there is a seatnum_groupId column in each response!

        [ForeignKey("GroupID")]
        public virtual Group Group { get; set; }

        //NEEDED FOR EF
        public Seat()
        {
            Attempts = new Collection<Attempt>();
            //Responses = new Collection<Response>();
        }


        public Seat init(ISeat iseat,
            Group paramGroup,
            byte paramSeatNum)
        {
            FirstName = iseat.FirstName;
            LastName = iseat.LastName;
            Email = iseat.Email;
            AlternateEmail = iseat.AlternateEmail  ;

            SeatNum = paramSeatNum;
            Group = paramGroup;
            GroupID = paramGroup.GroupID;

            return this;
        }

        //DEBUG ONLY
        public Seat(string firstName, string lastName, string email,
            byte paramGroupId,
            byte paramSeatNum)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            //AlternateEmail = iseat.AlternateEmail;

            GroupID = paramGroupId;
            SeatNum = paramSeatNum;

            Attempts = new Collection<Attempt>();
            //Responses = new Collection<Response>();

        }


    }
}
