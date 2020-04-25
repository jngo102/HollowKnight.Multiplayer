using ModCommon;
using UnityEngine;

namespace MultiplayerClient
{
    internal partial class MPClient
    {
        public GameObject attacks;
        public GameObject slash;
        public GameObject altSlash;
        public GameObject downSlash;
        public GameObject upSlash;
        public GameObject wallSlash;
        public GameObject cycloneSlash;
        public GameObject greatSlash;
        public GameObject dashSlash;
        public GameObject sharpShadow;
        private void GetKnightAttacks()
        {
            attacks = Instantiate(_hero.FindGameObjectInChildren("Attacks"), _playerPrefab.transform);
            attacks.name = "Attacks";
            foreach (Transform attackTransform in attacks.transform)
            {
                GameObject attack = attackTransform.gameObject;
                attack.layer = 22;
            }

            slash = attacks.FindGameObjectInChildren("Slash");
            //Destroy(slash.LocateMyFSM("damages_enemy"));
            GameObject clashTink = slash.FindGameObjectInChildren("Clash Tink");
            clashTink.layer = 22;
            GameObject slashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], slash.transform);
            slashCollider.layer = 22;
            slashCollider.GetComponent<PolygonCollider2D>().points = clashTink.GetComponent<PolygonCollider2D>().points;

            altSlash = attacks.FindGameObjectInChildren("AltSlash");
            Destroy(altSlash.LocateMyFSM("damages_enemy"));
            Destroy(altSlash.FindGameObjectInChildren("Clash Tink"));
            GameObject altSlashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], altSlash.transform);
            altSlashCollider.layer = 22;
            altSlashCollider.GetComponent<PolygonCollider2D>().points = altSlash.GetComponent<PolygonCollider2D>().points;;
            
            downSlash = attacks.FindGameObjectInChildren("DownSlash");
            Destroy(downSlash.LocateMyFSM("damages_enemy"));
            Destroy(downSlash.FindGameObjectInChildren("Clash Tink"));
            GameObject downSlashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], downSlash.transform);
            downSlashCollider.layer = 22;
            downSlashCollider.GetComponent<PolygonCollider2D>().points = downSlash.GetComponent<PolygonCollider2D>().points;;
            
            upSlash = attacks.FindGameObjectInChildren("UpSlash");
            Destroy(upSlash.LocateMyFSM("damages_enemy"));
            Destroy(upSlash.FindGameObjectInChildren("Clash Tink"));
            GameObject upSlashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], upSlash.transform);
            upSlashCollider.layer = 22;
            upSlashCollider.GetComponent<PolygonCollider2D>().points = upSlash.GetComponent<PolygonCollider2D>().points;;
            
            wallSlash = attacks.FindGameObjectInChildren("WallSlash");
            Destroy(wallSlash.LocateMyFSM("damages_enemy"));
            Destroy(wallSlash.FindGameObjectInChildren("Clash Tink"));
            GameObject wallSlashCollider = Instantiate(MultiplayerClient.GameObjects["Slash"], wallSlash.transform);
            wallSlashCollider.layer = 22;
            wallSlashCollider.GetComponent<PolygonCollider2D>().points = wallSlash.GetComponent<PolygonCollider2D>().points;;
            
            cycloneSlash = attacks.FindGameObjectInChildren("Cyclone Slash");
            GameObject hits = cycloneSlash.FindGameObjectInChildren("Hits");
            GameObject hitL = hits.FindGameObjectInChildren("Hit L");
            hitL.layer = 22;
            Destroy(hitL.LocateMyFSM("damages_enemy"));
            GameObject hitR = hits.FindGameObjectInChildren("Hit R");
            hitR.layer = 22;
            Destroy(hitR.LocateMyFSM("damages_enemy"));
            
            greatSlash = attacks.FindGameObjectInChildren("Great Slash");
            dashSlash = attacks.FindGameObjectInChildren("Dash Slash");
            
            sharpShadow = attacks.FindGameObjectInChildren("Sharp Shadow");
        }
    }
}