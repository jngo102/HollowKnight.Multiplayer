using UnityEngine;

namespace MultiplayerClient
{
    public class Fireball : MonoBehaviour
    {
        public float xDir;
        public int playerId;
        
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
            if (GameManager.Instance.pvpEnabled)
            {
                gameObject.AddComponent<DamageHero>();
            }
            if (GameManager.Players[playerId].equippedCharm_19)
            {
                transform.localScale = new Vector3(xDir * 1.8f * 1.3f, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(xDir * 1.8f, transform.localScale.y, transform.localScale.z);
            }
            Destroy(gameObject, 2);
        }
        
        private void Log(object message) => Modding.Logger.Log("[Fireball] " + message);
    }
}