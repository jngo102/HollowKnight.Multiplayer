using Modding;

namespace MultiplayerServer
{
    public class MultiplayerServer : Mod
    {
        public override string GetVersion()
        {
            return "0.0.1";
        } 

        public override void Initialize()
        {
            GameManager.instance.gameObject.AddComponent<MPServer>();
        }
    }
}