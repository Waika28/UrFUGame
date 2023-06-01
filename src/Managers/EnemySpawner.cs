namespace UrFUGame;

static class EnemySpawner
{
    static Random rand = new Random();
    static float baseInverseSpawnChance = 50;
    static float inverseSpawnChance = baseInverseSpawnChance;
    static float inverseBlackHoleChance = 300;

    public static void Update()
    {
        if (!PlayerShipController.Instance.Model.IsDead && EntityManager.Count < 200)
        {
            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(EnemyController.CreateSeekerController(GetSpawnPosition()));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(EnemyController.CreateWandererController(GetSpawnPosition()));

            if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                EntityManager.Add(new BlackHoleController(GetSpawnPosition()));
        }

        if (inverseSpawnChance > 30)
            inverseSpawnChance -= 0.005f;
    }

    private static Vector2 GetSpawnPosition()
    {
        Vector2 pos;
        do
        {
            pos = new Vector2(rand.Next((int)GameRoot.ScreenSize.X), rand.Next((int)GameRoot.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, PlayerShipController.Instance.Model.Position) < 250 * 250);

        return pos;
    }

    public static void Reset()
    {
        inverseSpawnChance = baseInverseSpawnChance;
    }
}

