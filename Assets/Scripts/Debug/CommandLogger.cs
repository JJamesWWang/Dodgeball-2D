using UnityEngine;

// Used by clients to log their commands.
// Methods: LogCommand
public class CommandLogger : MonoBehaviour
{
    [SerializeField] private bool isLoggingOn;
    public static CommandLogger singleton { get; private set; }

    private void Awake()
    {
        if (singleton != null && singleton != this)
            Destroy(gameObject);
        else
            singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LogCommand(string message)
    {
        if (!isLoggingOn) { return; }
        Debug.Log($"CLIENT: COMMAND: {message}");
    }
}