using RockPaperScissors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors.Logic
{
    public class Game
    {
        private static Game instance;
        private Game() { }
        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Game();
                }
                return instance;
            }
        }
 

        public void SetupGame()
        {

        }

        public void ResetGame()
        {

        }
    }
}
