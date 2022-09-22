using BDUtil;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BDRPG
{
    public class SceneChanger : MonoBehaviour
    {
        public string DefaultScene;
        public void OpenDefaultScene() => OpenCommaScene(DefaultScene);
        public void OpenCommaScene(string commaScene)
        {
            string[] scenes = commaScene?.Split(',');
            OpenScenes(scenes);
        }
        public void OpenScene(string scene)
        {
            if (scene.IsEmpty()) return;
            SceneManager.LoadScene(scene);
        }
        public void OpenScenes(params string[] scenes)
        {
            if (scenes.IsEmpty()) return;
            OpenScene(scenes[0]);
            for (int i = 1; i < scenes.Length; ++i)
            {
                if (scenes[i].IsEmpty()) continue;
                SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);
            }
        }
    }
}
