namespace Arkanoid
{
    //Odwraca działanie klawiszy lewo-prawo. Jeśli jest aktywny, paletka ma barwę czerwoną.
    class SwapArrowKeysBonus : Bonus
    {
        public SwapArrowKeysBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {

        }

        public override void Work()
        {
            gc.SwapArrowKeysBonus();
        }
    }
}
