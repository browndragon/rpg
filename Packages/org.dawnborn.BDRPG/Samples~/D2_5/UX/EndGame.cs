using BDUtil;
using BDUtil.Math;
using BDUtil.Pubsub;
using BDUtil.Raw;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BDRPG
{
    public class EndGame : MonoBehaviour
    {
        public int RequireFlags = 4;
        public ObjsSet Flags;
        public Image FlagPrefab;
        public string VictoryScene = "Samples/D2_5/Victory";
        public Color FadeColor = Color.white;
        public Timer ending = .5f;
        LayoutGroup LayoutGroup;
        Texture2D pixel;

        readonly Disposes.All unsubscribe = new();
        void Awake() => pixel = new(1, 1);
        void OnEnable()
        {
            LayoutGroup = GetComponentInChildren<LayoutGroup>();
            unsubscribe.Add(Flags.Subscribe(OnFlagChange));
        }
        void OnDisable() => unsubscribe.Dispose();
        void OnDestroy() => Destroy(pixel);
        void OnFlagChange(Observable.Update update)
        {
            if (!gameObject.scene.isLoaded) return;
            int i = 0;
            foreach (GameObject sceneFlag in Flags.Collection)
            {
                Image hudFlag;
                if (i >= LayoutGroup.transform.childCount) hudFlag = Instantiate(FlagPrefab, LayoutGroup.transform);
                else hudFlag = LayoutGroup.transform.GetChild(i).GetComponent<Image>();
                hudFlag.sprite = sceneFlag.GetComponent<SpriteRenderer>().sprite;
                i++;
            }
            while (LayoutGroup.transform.childCount > Flags.Collection.Count)
            {
                var exFlag = LayoutGroup.transform.GetChild(LayoutGroup.transform.childCount - 1);
                exFlag.SetParent(null);
                Destroy(exFlag.gameObject);
            }
            if (!ending && Flags.Count >= RequireFlags) ending = ending.Restart();
        }
        void OnGUI()
        {
            if (!ending.IsStarted) return;
            else if (ending.FullRatio > 1f)
            {
                AudioListener.volume = 0f;
                pixel.SetPixel(0, 0, FadeColor);
                pixel.Apply();
                GUI.DrawTexture(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), pixel);
                Coroutines.Schedule(() => AudioListener.volume = 1f);
                SceneManager.LoadScene(VictoryScene);
                return;
            }
            AudioListener.volume = Mathf.Lerp(1f, 0f, ending);
            pixel.SetPixel(0, 0, FadeColor.WithA(ending));
            pixel.Apply();
            GUI.DrawTexture(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), pixel);
        }
    }
}
