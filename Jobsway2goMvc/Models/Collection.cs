using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobsway2goMvc.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}

