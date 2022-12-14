using UnityEngine;

namespace Superball
{
    [RequireComponent(typeof(PachinkoBall))]
    class Speed : MonoBehaviour, IUpdateableBySimulation
    {
        public PachinkoBall Pachinko;
        public Rigidbody2D Rigid;

        public int HitAmount;
        public float HitVelocity;
        public float InitialVelocity;

        public void Awake()
        {
            Pachinko = GetComponent<PachinkoBall>();
            HitAmount = 0;
            InitialVelocity = Plugin.InitialVelocity.Value;
            HitVelocity = Plugin.VelocityGained.Value;
        }

        public void Start()
        {
            Rigid = Pachinko._rigid;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (Pachinko.IsDummy || Pachinko.CurrentState != PachinkoBall.FireballState.FIRING) return;
            if (collision.collider.CompareTag("Peg") || collision.collider.CompareTag("Bomb"))
            {
                Peg peg = collision.collider.GetComponent<Peg>();
                if (peg is LongPeg longPeg && longPeg.hit)
                {
                    return;
                }

                HitAmount++;
            }
        }

        public void FixedUpdate()
        {
            Vector2 velocity = Rigid.velocity.normalized;
            float speed = InitialVelocity + (HitVelocity * HitAmount);

            Rigid.velocity = velocity * speed;
        }

        public void DoUpdate(PhysicsScene2D physicsScene2D)
        {
            FixedUpdate();
        }

        public void Disable()
        {
        }
    }
}
