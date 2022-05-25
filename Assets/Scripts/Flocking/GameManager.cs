namespace Flocking
{
    public static class GameManager
    {
        public static int WhoWon { get; set; } = -1; // -1 for anyone, 0 for humans, 1 for zombies

        public static int ZombiesScore { get; set; } = 0;
        public static int HumansScore { get; set; } = 0;
    }
}