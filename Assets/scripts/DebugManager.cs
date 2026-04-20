public class DebugManager
{
    public static bool isDebugMode = true;
    public static DebugManager _instance;

    public static DebugManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DebugManager();
            }
            return _instance;
        }
    }


}