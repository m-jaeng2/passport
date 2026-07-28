using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmPassport.Runtime
{
    public sealed class GameplayFlowUiActions : MonoBehaviour
    {
        public void RestartScene()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.path);
        }

        public void ReturnToStart()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.path);
        }
    }
}
