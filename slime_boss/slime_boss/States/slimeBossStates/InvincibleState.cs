using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/my/myBosses/slime_boss/states/slimeBossStates/Invincible Enemy State")]
    public class InvincibleState : EnemyState
    {
        protected override void OnEnter(Enemy enemy) { }

        protected override void OnExit(Enemy enemy) { }

        protected override void OnStep(Enemy enemy) { }

        public override void OnContact(Enemy enemy, Collider other) { }
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}