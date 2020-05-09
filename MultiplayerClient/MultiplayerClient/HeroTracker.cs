using UnityEngine;

namespace MultiplayerClient
{
    public class HeroTracker : MonoBehaviour
    {
        private Vector3 _storedPosition = Vector3.zero;
        private Vector3 _storedScale = Vector3.zero;
        private void FixedUpdate()
        {
            Vector3 heroPos = transform.position;
            if (heroPos != _storedPosition)
            {
                ClientSend.PlayerPosition(heroPos);
                _storedPosition = heroPos;
            }

            Vector3 heroScale = transform.localScale;
            if (heroScale != _storedScale)
            {
                ClientSend.PlayerScale(heroScale);
                _storedScale = heroScale;
            }
        }

        private void Log(object message) => Modding.Logger.Log("[Hero Tracker] " + message);
    }
}