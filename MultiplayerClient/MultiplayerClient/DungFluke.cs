using System.Collections;
using ModCommon;
using UnityEngine;

namespace MultiplayerClient
{
    public class DungFluke : MonoBehaviour
    {
        public AudioClip blowClip;
        
        private tk2dSpriteAnimator _anim;
        private AudioSource _audio;

        private void Awake()
        {
            _anim = GetComponent<tk2dSpriteAnimator>();
            _audio = GetComponent<AudioSource>();
        }
        
        private IEnumerator Start()
        {
            _anim.Play("Dung Air");
            _audio.Play();
            
            yield return new WaitForSeconds(1.0f);

            _anim.Play("Dung Antic");
            gameObject.FindGameObjectInChildren("Pt Antic").GetComponent<ParticleSystem>().Play();

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Explode());
        }
        
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.name != "HeroBox") return;

            StartCoroutine(Explode());
        }
        
        private IEnumerator Explode()
        {
            GameObject dungCloudObj = gameObject.FindGameObjectInChildren("Knight Dung Cloud");
            GameObject dungCloud = Instantiate(dungCloudObj, transform.position, Quaternion.identity);
            dungCloud.SetActive(true);
            dungCloud.layer = 22;
            Destroy(dungCloud.GetComponent<DamageEffectTicker>());
            dungCloud.LocateMyFSM("Control").SetState("Collider On");
            dungCloud.AddComponent<AudioSource>().PlayOneShot(blowClip);
            if (GameManager.Instance.PvPEnabled)
            {
                dungCloud.AddComponent<DamageHero>();
            }

            Destroy(gameObject);    

            yield return new WaitForSeconds(3.0f);
                
            Destroy(dungCloud);
        }

        private void Log(object message) => Modding.Logger.Log("[Dung Fluke] " + message);
    }
}