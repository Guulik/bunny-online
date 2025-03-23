using System;

namespace Dolls.Health
{
    public class TakeScoreEventArgs : EventArgs
    {
        public TakeScoreEventArgs(Player taker, int score)
        {
            Taker = taker;
            Score = score;
        }

        public Player Taker { get; }
        public int Score { get; }
    }
   
}