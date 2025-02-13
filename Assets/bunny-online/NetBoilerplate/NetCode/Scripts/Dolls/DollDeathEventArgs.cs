using System;

public class DollDeathEventArgs : EventArgs
{
    public DollDeathEventArgs(Player killer, Player victim)
    {
        Killer = killer;
        Victim = victim;
    }

    public Player Killer { get; }
    public Player Victim { get; }
}