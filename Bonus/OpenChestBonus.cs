namespace Arkanoid
{
    //Otwiera skrzynię. Gracz trafiając piłką do otwartej skrzyni wygrywa grę.
    class OpenChestBonus : Bonus
    {
        public OpenChestBonus(GameController gc, Position blockCenter, Paddle paddle) : base(gc, blockCenter, paddle)
        {
            
        }

        public override void Work()
        {
            gc.OpenChestBonus();
        }
    }
}
