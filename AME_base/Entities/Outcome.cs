
 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace AME_base
{
    public class Outcome
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Hash { get; set; }
        [Required]
        public string Description { get; set; }

        //[Required]
        //public virtual ICollection<Row> Rows { get; set; }

        //dont pass in higher data structures
        public Outcome(string paramDescription)
        {
            Hash = AME_base.Helpers.Hash(paramDescription);
            Description = paramDescription;
            //Rows = new Collection<Row>();
        }

        
        //used by EF
        private Outcome()
        {
            //Rows = new Collection<Row>();
        }

    }
}
