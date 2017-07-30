namespace Arkanoid
{
    //Spowalnia ruch piłki.
    class SlowDownBonus : Bonus
    {
        public SlowDownBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {
        }

        public override void Work()
        {
            gc.SlowDownBallBonus();
        }
    }
}
