using RockPaperScissors.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Models
{
    public class Move
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Emoji { get; set; }
        public List<int> Beats { get; set; }
    }
}