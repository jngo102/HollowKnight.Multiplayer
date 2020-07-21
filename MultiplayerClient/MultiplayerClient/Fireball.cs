using UnityEngine;

namespace MultiplayerClient
{
    public class Fireball : MonoBehaviour
    {
        public float xDir;
        public byte playerId;
        
        private const float FireballSpeed = 45;
            
        private tk2dSpriteAnimator _anim;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _anim = GetComponent<tk2dSpriteAnimator>();
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            _anim.PlayFromFrame(0);
            _rb.velocity = Vector2.right * FireballSpeed * xDir;
            if (SessionManager.Instance.PvPEnabled)
            {
                gameObject.AddComponent<DamageHero>();
            }
            Vector3 scale = transform.localScale;
            if (SessionManager.Instance.Players[playerId].equippedCharm_19)
            {
                transform.localScale = new Vector3(1.8f * xDir, 1.4f * scale.y, scale.z);
            }
            else 
            {
                transform.localScale = new Vector3(1.8f * xDir, scale.y, scale.z);
            }
            Destroy(gameObject, 2);
        }
        
        private void Log(object message) => Modding.Logger.Log("[Fireball] " + message);
    }
}