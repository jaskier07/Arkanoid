namespace Arkanoid
{
    //Zmniejsza ilość żyć o 1, ale nigdy nie spowoduje zmniejszenia liczby żyć do 0.
    class DecreaseLivesBonus : Bonus
    {
        public DecreaseLivesBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {

        }

        public override void Work()
        {
            gc.DecreaseLivesBonus();
        }
    }
}
