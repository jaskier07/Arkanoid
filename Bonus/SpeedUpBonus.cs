namespace Arkanoid
{
    //Przyspiesza ruch piłki.
    class SpeedUpBonus : Bonus
    {
        public SpeedUpBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {
        }

        public override void Work()
        {
            gc.SpeedUpBallBonus();
        }
    }
}
