namespace Arkanoid
{
    //Zamyka skrzynię. Gracz trafiając piłką do otwartej skrzyni wygrywa grę.
    class CloseChestBonus : Bonus
    {
        public CloseChestBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {

        }

        public override void Work()
        {
            gc.CloseChestBonus();
        }
    }
}
