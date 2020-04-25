using System;
using ModCommon.Util;
using UnityEngine;

namespace MultiplayerClient
{
    public class PlayerController : MonoBehaviour
    {
        private GameObject _hero;

        private void Awake()
        {
            _hero = HeroController.instance.gameObject;

            var mFilter = gameObject.GetComponent<MeshFilter>();
            
            Mesh mesh = mFilter.mesh;
            Mesh heroMesh = _hero.GetComponent<MeshFilter>().sharedMesh;
            
            mesh.vertices = heroMesh.vertices;
            mesh.normals = heroMesh.normals;
            mesh.uv = heroMesh.uv;
            mesh.triangles = heroMesh.triangles;
            mesh.tangents = heroMesh.tangents;
            
            var mRend = gameObject.GetComponent<MeshRenderer>();
            mRend.material = new Material(_hero.GetComponent<MeshRenderer>().material);
            
            var nb = gameObject.GetComponent<NonBouncer>();
            nb.active = false;

            /*var rb = gameObject.GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0.0f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.isKinematic = false;*/

            var anim = gameObject.GetComponent<tk2dSpriteAnimator>();
            anim.Library = _hero.GetComponent<tk2dSpriteAnimator>().Library;
        }

        private string _storedClip = "";
        private Vector3 _storedPosition = Vector3.zero;
        private Vector3 _storedScale = Vector3.zero;
        private void FixedUpdate()
        {
            Vector3 heroPos = _hero.transform.position;
            if (heroPos != _storedPosition)
            {
                ClientSend.PlayerPosition(heroPos);
                _storedPosition = heroPos;
            }

            Vector3 heroScale = _hero.transform.localScale;
            if (heroScale != _storedScale)
            {
                ClientSend.PlayerScale(heroScale);
                _storedScale = heroScale;
            }

            /*var anim = _hero.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationFrame frame = anim.CurrentClip.frames[0];
            tk2dSpriteAnimationClip currentClip = anim.CurrentClip;
            string clipName = currentClip.name;
            if (currentFrame == 0 && currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once ||
                currentFrame == 0 && currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop && clipName != _storedClip ||
                currentFrame == 0 && currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.LoopSection && clipName != _storedClip)
            {
                _storedClip = clipName;
                Log("Sending Animation: " + clipName);
                ClientSend.PlayerAnimation(clipName);
            }*/
        }

        private void Log(object message) => Modding.Logger.Log("[Player Controller] " + message);
    }
}