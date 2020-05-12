using Modding;

namespace MultiplayerClient
{
    public class SaveSettings : ModSettings { }
    public class GlobalSettings : ModSettings
    {
        public string host { get => GetString("127.0.0.1"); set => SetString(value); }
        public int port { get => GetInt(26950); set => SetInt(value); }
        public string username { get => GetString("Newbie"); set => SetString(value); }
    }
}