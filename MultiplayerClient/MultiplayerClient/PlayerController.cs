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

            BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
            var heroCollider = _hero.GetComponent<BoxCollider2D>();

            collider.isTrigger = true;
            collider.offset = heroCollider.offset;
            collider.size = heroCollider.size;
            collider.enabled = true;

            Bounds bounds = collider.bounds;
            Bounds heroBounds = heroCollider.bounds;
            bounds.min = heroBounds.min;
            bounds.max = heroBounds.max;

            gameObject.GetComponent<DamageHero>().enabled = false;
            
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

            var anim = gameObject.GetComponent<tk2dSpriteAnimator>();
            anim.Library = _hero.GetComponent<tk2dSpriteAnimator>().Library;
        }

        private void Log(object message) => Modding.Logger.Log("[Player Controller] " + message);
    }
}