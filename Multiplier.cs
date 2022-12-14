using Battle.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Superball
{
    [RequireComponent(typeof(Attack))]
    [RequireComponent(typeof(PachinkoBall))]
    public class Multiplier : MonoBehaviour
    {
        public PachinkoBall Pachinko;
        public Attack Attack;
        public BattleController BattleController;
        private int _level;
        private float _multiplier;

        private int _hitAmount;
        private int _currentHits;

        public void Awake()
        {
            Pachinko = GetComponent<PachinkoBall>();
            Attack = GetComponent<Attack>();
            _level = Attack.Level;
            _multiplier = SuperballOrb.GetMultiplier(_level);
            _hitAmount = SuperballOrb.GetPegHitAmount(_level);
            _currentHits = 0;

            if (!Pachinko.IsDummy)
            {
                BattleController = Resources.FindObjectsOfTypeAll<BattleController>().FirstOrDefault();
            }
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

                _currentHits++;

                if(_currentHits == _hitAmount)
                {
                    BattleController.AddDamageMultiplier(_multiplier);
                }
            }
        }
    }
}
