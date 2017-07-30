namespace Arkanoid
{
    //Zwiększa ilość żyć o 1.
    class IncreaseLivesBonus : Bonus
    {
        public IncreaseLivesBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {

        }


        public override void Work()
        {
            gc.IncreaseLivesBonus();
        }
    }
}
