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
        StartCoroutine(PostItem());
    }

    public IEnumerator GetItems()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url + EndpointItems))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                string msg = $"Get Faild : {request.error}";
                Debug.LogError(msg);
                SetResultText(msg);
                yield break;
            }

            string json = request.downloadHandler.text;

            string wrapped = string.Format("{{\"items\":{0}}}", json);

            ItemList list = JsonUtility.FromJson<ItemList>(wrapped);

            if(TryMake_SB_FromItemList(list, "GET", ref builder))
            {
                SetResultText(builder.ToString());
            }
        }
    }

    public IEnumerator PostItem()
    {
        if (nameInput == null)
        {
            SetResultText("{Post} nameInput is Null.");
            yield break;
        }

        string name = nameInput.text;

        if (string.IsNullOrEmpty(name))
        {
            SetResultText("{Post} nameInput is Empty.");
            yield break;
        }

        if(name ==" " || name =="  "    //半角
            || name == "　" || name == "　　") //全角
        {
            SetResultText("[Post] nameInput.text is Space.");
            yield break;
        }

        string bodyJson = JsonUtility.ToJson(new CreateItemRequest(name));
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);

        using (UnityWebRequest req = new UnityWebRequest(url + EndpointItems, UnityWebRequest.kHttpVerbPOST))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if(req.result != UnityWebRequest.Result.Success)
            {
                string msg = $"Post Failed : {req.error}";
                Debug.LogError(msg);
                SetResultText(msg);
                yield break;
            }

            string json = req.downloadHandler.text;
            Item item = JsonUtility.FromJson<Item>(json);

            builder.Clear();
            builder.AppendLine("[Post] Lists");
            builder.AppendLine();

            if (TryGetItemData(item, builder))
            {
                SetResultText(builder.ToString());
            }
        }

        if(nameInput != null)
        {
            nameInput.text = string.Empty;
        }
    }

    private static bool TryMake_SB_FromItemList(ItemList list, string method, ref StringBuilder _builder)
    {
        if(_builder == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(method))
        {
            method = "Unknown";
        }

        _builder.Clear();
        _builder.AppendFormat("[{0}] Lists", method);
        _builder.AppendLine();
        _builder.AppendLine();

        if (list == null || list.items == null || list.items.Length <= 0)
        {
            _builder.AppendLine("(No Data)");
        }
        else
        {
            foreach (var item in list.items)
            {
                TryGetItemData(item, _builder);
            }
        }
        return true;
    }

    private static bool TryGetItemData(Item item, StringBuilder _builder)
    {
        if(item == null || _builder == null)
        {
            return false;
        }
        _builder.AppendFormat("ID:{0}, Name:{1}", item.id, item.name);
        _builder.AppendLine();
        return true;
    }

    void SetResultText(string text)
    {
        if(resultText != null)
        {
            resultText.SetText(text);
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
            getButton.onClick.RemoveAllListeners();
        }
        if (postButton != null)
        {
            postButton.onClick.RemoveAllListeners();
        }
    }
}
