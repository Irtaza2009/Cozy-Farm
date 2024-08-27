using UnityEngine;

#if !UNITY_WEBGL
using Firebase;
using Firebase.Extensions;
#endif

public class FirebaseInitializer : MonoBehaviour
{
    void Start()
    {
        #if !UNITY_WEBGL
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase is ready to use.");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
        #endif
    }
}
