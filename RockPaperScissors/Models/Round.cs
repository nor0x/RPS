using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RockPaperScissors.Models
{
    public class Round
    {
        public Move Player1Move { get; set; }
        public Move Player2Move { get; set; }
        public Brush Player1Color { get; set; }
        public Brush Player2Color { get; set; }
    }
}
