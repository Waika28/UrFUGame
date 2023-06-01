namespace UrFUGame;

public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

public struct ParticleState
{
    public Vector2 Velocity;
    public ParticleType Type;
    public float LengthMultiplier;

    private static Random rand = new Random();

    public ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
    {
        Velocity = velocity;
        Type = type;
        LengthMultiplier = lengthMultiplier;
    }

    public static ParticleState GetRandom(float minVel, float maxVel)
    {
        var state = new ParticleState();
        state.Velocity = rand.NextVector2(minVel, maxVel);
        state.Type = ParticleType.None;
        state.LengthMultiplier = 1;

        return state;
    }

    public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
    {
        var vel = particle.State.Velocity;
        float speed = vel.Length();

        Vector2.Add(ref particle.Position, ref vel, out particle.Position);

        float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
        alpha *= alpha;

        particle.Tint.A = (byte)(255 * alpha);

        if (particle.State.Type == ParticleType.Bullet)
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
        else
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

        particle.Orientation = vel.ToAngle();

        var pos = particle.Position;
        int width = (int)GameRoot.ScreenSize.X;
        int height = (int)GameRoot.ScreenSize.Y;

        if (pos.X < 0)
            vel.X = Math.Abs(vel.X);
        else if (pos.X > width)
            vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0)
            vel.Y = Math.Abs(vel.Y);
        else if (pos.Y > height)
            vel.Y = -Math.Abs(vel.Y);

        if (particle.State.Type != ParticleType.IgnoreGravity)
        {
            foreach (var blackHole in EntityManager.BlackHoles)
            {
                var dPos = blackHole.GetModel().Position - pos;
                float distance = dPos.Length();
                var n = dPos / distance;
                vel += 10000 * n / (distance * distance + 10000);

                if (distance < 400)
                    vel += 45 * new Vector2(n.Y, -n.X) / (distance + 100);
            }
        }

        if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
            vel = Vector2.Zero;
        else if (particle.State.Type == ParticleType.Enemy)
            vel *= 0.94f;
        else
            vel *= 0.96f + Math.Abs(pos.X) % 0.04f; 

        particle.State.Velocity = vel;
    }
}
