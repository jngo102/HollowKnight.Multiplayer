using System;
using UnityEngine;

namespace MultiplayerServer
{
    public class Player : MonoBehaviour
    {
        public int id;
        public string username;
        public string animation;

        private void Awake()
        {
            Texture2D tex = new Texture2D(100, 100);
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2.0f, tex.height / 2.0f));

            /*GameObject hero = HeroController.instance.gameObject;
            
            var collider = gameObject.GetComponent<BoxCollider2D>();
            var heroCollider = hero.GetComponent<BoxCollider2D>();

            collider.size = heroCollider.size;
            collider.offset = heroCollider.offset;
            collider.enabled = false;

            Bounds bounds = collider.bounds;
            Bounds heroBounds = heroCollider.bounds;
            bounds.min = heroBounds.min;
            bounds.max = heroBounds.max;

            var mFilter = gameObject.GetComponent<MeshFilter>();
            
            Mesh mesh = mFilter.mesh;
            Mesh heroMesh = hero.GetComponent<MeshFilter>().sharedMesh;

            mesh.vertices = heroMesh.vertices;
            mesh.normals = heroMesh.normals;
            mesh.uv = heroMesh.uv;
            mesh.triangles = heroMesh.triangles;
            mesh.tangents = heroMesh.tangents;

            var mRend = gameObject.GetComponent<MeshRenderer>();
            mRend.material = new Material(hero.GetComponent<MeshRenderer>().material);

            var nb = gameObject.GetComponent<NonBouncer>();
            nb.active = false;

            var rb = gameObject.GetComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 0.0f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.isKinematic = false;

            var anim = gameObject.GetComponent<tk2dSpriteAnimator>();
            anim.Library = hero.GetComponent<tk2dSpriteAnimator>().Library;*/
        }
        
        
        public void Initialize(int id, string username)
        {
            this.id = id;
            this.username = username;
            animation = "Idle";
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;

            ServerSend.PlayerPosition(this);
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;

            ServerSend.PlayerScale(this);
        }
        
        public void SetAnimation(string anim)
        {
            animation = anim;

            Log("ServerSend.PlayerAnimation(this)");
            ServerSend.PlayerAnimation(this);
        }

        private void Log(object message) => Modding.Logger.Log("[Player] " + message);
    }
}