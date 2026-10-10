using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ApiClient : MonoBehaviour
{
    const string EndpointItems = "items";

    [Header("サーバー設定")]
    [SerializeField] string url = "http://localhost:8080/";

    [Header("UI")]
    [SerializeField] TMP_InputField nameInput;


    [SerializeField,Tooltip("Getするボタン")] Button getButton;

    [SerializeField,Tooltip("Postするボタン")] Button postButton;

    [SerializeField, Tooltip("通信結果表示テキスト")] TMP_Text resultText;


    StringBuilder builder = new StringBuilder();

    void OnClickGet()
    {
        StartCoroutine(GetItems());
    }

    void OnClickPost()
    {

    }

    public IEnumerator GetItems()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url + EndpointItems))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string msg = $"Get 失敗 : {request.error}";
                Debug.LogError(msg);
                SetResultText(msg);
                yield break;
            }

            string json = request.downloadHandler.text;

            string wrapped = "{\"items\":" + json + "}";
            wrapped = string.Format("{{\"items\":{0}}}", json);

            ItemList list = JsonUtility.FromJson<ItemList>(wrapped);

            builder.Clear();
            builder.AppendLine("[Get] 一覧");
            builder.AppendLine();

            if(list == null || list.items == null || list.items.Length <= 0)
            {
                builder.AppendLine("(データ無し)");
            }
            else
            {
                foreach (var item in list.items)
                {
                    builder.AppendFormat("ID:{0}, Name:{1}", item.id, item.name);
                    builder.AppendLine();
                }
            }
        }
        
    }

    void SetResultText(string text)
    {
        if(resultText != null)
        {
            resultText.text = text;
        }
    }

    private void Awake()
    {
        if(getButton != null)
        {
            getButton.onClick.AddListener(OnClickGet);
        }
        if(postButton != null)
        {
            postButton.onClick.AddListener(OnClickPost);
        }
    }

    private void OnDestroy()
    {
        if (getButton != null)
        {
            getButton.onClick.AddListener(OnClickGet);
        }
        if (postButton != null)
        {
            postButton.onClick.AddListener(OnClickPost);
        }
    }
}
