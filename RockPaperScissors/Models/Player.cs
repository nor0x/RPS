using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri AvatarUri 
        {
            get
            {
                return new Uri(AvatarPath);
            }
        }
        public string AvatarPath {get;set;}
    }
}
