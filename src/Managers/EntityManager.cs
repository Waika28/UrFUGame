using System.Linq;

namespace UrFUGame;

static class EntityManager
{
    static List<BaseController> entities = new();
    static List<EnemyController> enemies = new();
    static List<BulletController> bullets = new();
    static List<BlackHoleController> blackHoles = new();

    public static IEnumerable<BlackHoleController> BlackHoles { get { return blackHoles; } }

    static bool isUpdating;
    static List<BaseController> addedEntities = new();

    public static int Count { get { return entities.Count; } }
    public static int BlackHoleCount { get { return blackHoles.Count; } }

    public static void Add(BaseController entity)
    {
        if (!isUpdating)
            AddEntity(entity);
        else
            addedEntities.Add(entity);
    }

    private static void AddEntity(BaseController entity)
    {
        entities.Add(entity);
        if (entity is BulletController)
            bullets.Add(entity as BulletController);
        else if (entity is EnemyController)
            enemies.Add(entity as EnemyController);
        else if (entity is BlackHoleController)
            blackHoles.Add(entity as BlackHoleController);
    }

    public static void Update()
    {
        isUpdating = true;
        HandleCollisions();

        foreach (var entity in entities)
            entity.Update();

        isUpdating = false;

        foreach (var entity in addedEntities)
            AddEntity(entity);

        addedEntities.Clear();

        entities = entities.Where(x => !x.GetModel().IsExpired).ToList();
        bullets = bullets.Where(x => !x.GetModel().IsExpired).ToList();
        enemies = enemies.Where(x => !x.GetModel().IsExpired).ToList();
        blackHoles = blackHoles.Where(x => !x.GetModel().IsExpired).ToList();
    }

    static void HandleCollisions()
    {
        for (int i = 0; i < enemies.Count; i++)
            for (int j = i + 1; j < enemies.Count; j++)
            {
                if (IsColliding(enemies[i], enemies[j]))
                {
                    enemies[i].HandleCollision(enemies[j]);
                    enemies[j].HandleCollision(enemies[i]);
                }
            }

        for (int i = 0; i < enemies.Count; i++)
            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(enemies[i], bullets[j]))
                {
                    enemies[i].WasShot();
                    bullets[j].Model.IsExpired = true;
                }
            }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].Model.IsActive && IsColliding(PlayerShipController.Instance as BaseController, enemies[i]))
            {
                KillPlayer();
                break;
            }
        }

        for (int i = 0; i < blackHoles.Count; i++)
        {
            for (int j = 0; j < enemies.Count; j++)
                if (enemies[j].Model.IsActive && IsColliding(blackHoles[i], enemies[j]))
                    enemies[j].WasShot();

            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(blackHoles[i], bullets[j]))
                {
                    bullets[j].Model.IsExpired = true;
                    blackHoles[i].WasShot();
                }
            }

            if (IsColliding(PlayerShipController.Instance, blackHoles[i]))
            {
                KillPlayer();
                break;
            }
        }
    }

    private static void KillPlayer()
    {
        PlayerShipController.Instance.Kill();
        enemies.ForEach(x => x.WasShot());
        blackHoles.ForEach(x => x.Kill());
        EnemySpawner.Reset();
    }

    private static bool IsColliding(BaseController aController, BaseController bController)
    {
        var a = aController.GetModel();
        var b = bController.GetModel();
        float radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
    }

    public static IEnumerable<BaseController> GetNearbyEntities(Vector2 position, float radius)
    {
        return entities.Where(x => Vector2.DistanceSquared(position, x.GetModel().Position) < radius * radius);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in entities)
            entity.Draw(spriteBatch);
    }
}
