using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    #region Setup

    [SerializeField] GameObject joinChatButton;
    ChatClient chatClient;
    bool isConnected;
    [SerializeField] string username;
    [SerializeField] string nativeLanguage; //

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
    }

    public void LanguageOnValueChange(string valueIn) //
    {
        nativeLanguage = valueIn;
    }

    public void ChatConnectOnClick()
    {
        isConnected = true;
        chatClient = new ChatClient(this);
        //chatClient.ChatRegion = "US";
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
        Debug.Log("Connecting");
    }

    #endregion Setup

    #region General

    [SerializeField] GameObject chatPanel;
    string privateReceiver = "";
    string currentChat;
    [SerializeField] InputField chatField;
    [SerializeField] Text chatDisplay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            chatClient.Service();
        }

        if (chatField.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }
    }

    #endregion General

    #region PublicChat

    public void SubmitPublicChatOnClick()
    {
        if (privateReceiver == "")
        {
            chatClient.PublishMessage("RegionChannel", currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    #endregion PublicChat

    #region PrivateChat

    public void ReceiverOnValueChange(string valueIn)
    {
        // API call here - translation
        privateReceiver = valueIn;
    }

    public void SubmitPrivateChatOnClick()
    {
        if (privateReceiver != "")
        {
            chatClient.SendPrivateMessage(privateReceiver, currentChat);
            chatField.text = "";
            currentChat = "";
        }
    }

    #endregion PrivateChat

    #region Callbacks

    

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        if(state == ChatState.Uninitialized)
        {
            isConnected = false;
            joinChatButton.SetActive(true);
            chatPanel.SetActive(false);
        }
    }

    public void OnConnected()
    {
        Debug.Log("Connected");
        joinChatButton.SetActive(false);
        chatClient.Subscribe(new string[] { "RegionChannel" });
    }

    public void OnDisconnected()
    {
        isConnected = false;
        joinChatButton.SetActive(true);
        chatPanel.SetActive(false);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        string msgs = "";
        for (int i = 0; i < senders.Length; i++)
        {
            msgs = string.Format("{0}: {1}", senders[i], messages[i]);
            // if(nativeLanguage == "french" || nativeLanguage == "French")
            // {
            //     StartCoroutine(TranslateMessageEnToFr(msgs)); // English to French
            // }
            // if(nativeLanguage == "english" || nativeLanguage == "English")
            // {
            //     StartCoroutine(TranslateMessageFrToEn(msgs)); // French to English
            // }
            // if(nativeLanguage == "Hindi" || nativeLanguage == "hindi")
            // {
            //     StartCoroutine(TranslateMessageEnToHn(msgs));
            // }
            
            StartCoroutine(TranslateMessageToAll(msgs));
            // chatDisplay.text += "\n" + msgs;

            // Debug.Log(msgs);
        }
    }
    IEnumerator TranslateMessageToAll(string message) // English to French
    {
        // Send POST request to API with the message to be translated
        WWWForm form = new WWWForm();
        form.AddField("input", message);
        form.AddField("lang", nativeLanguage);
        UnityWebRequest www = UnityWebRequest.Post("https://flask-api-yvuq.onrender.com/api/data", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Get the translated message from the response
            string translatedMessage = JSON.Parse(www.downloadHandler.text)["data"];

            // Display the translated message in the chatDisplay text field
            chatDisplay.text += "\n" + translatedMessage;

            Debug.Log(translatedMessage);
        }
        else
        {
            Debug.LogError("Translation request failed with error: " + www.error);
        }
        www.Dispose();
    }
    IEnumerator TranslateMessageFrToEn(string message)  //French to English
    {
        // Send POST request to API with the message to be translated
        WWWForm form = new WWWForm();
        form.AddField("input", message);
        UnityWebRequest www = UnityWebRequest.Post("https://flask-api-fr-en.onrender.com/api/data", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Get the translated message from the response
            string translatedMessage = JSON.Parse(www.downloadHandler.text)["data"];

            // Display the translated message in the chatDisplay text field
            chatDisplay.text += "\n" + translatedMessage;

            Debug.Log(translatedMessage);
        }
        else
        {
            Debug.LogError("Translation request failed with error: " + www.error);
        }

        www.Dispose();
    }

    IEnumerator TranslateMessageEnToHn(string message) // English to French
    {
        // Send POST request to API with the message to be translated
        WWWForm form = new WWWForm();
        form.AddField("input", message);
        UnityWebRequest www = UnityWebRequest.Post("https://flask-api-en-hn.onrender.com/api/data", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Get the translated message from the response
            string translatedMessage = JSON.Parse(www.downloadHandler.text)["data"];

            // Display the translated message in the chatDisplay text field
            chatDisplay.text += "\n" + translatedMessage;

            Debug.Log(translatedMessage);
        }
        else
        {
            Debug.LogError("Translation request failed with error: " + www.error);
        }
        www.Dispose();
    }


    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        string msgs = "";

        msgs = string.Format("(Private) {0}: {1}", sender, message);
        // if(nativeLanguage == "french" || nativeLanguage == "French")
        // {
        //     StartCoroutine(TranslateMessageEnToFr(msgs)); // English to French
        // }
        // else if(nativeLanguage == "english" || nativeLanguage == "English")
        // {
        //     StartCoroutine(TranslateMessageFrToEn(msgs)); // French to English
        // }
        StartCoroutine(TranslateMessageToAll(msgs));

        // chatDisplay.text += "\n " + msgs;

        // Debug.Log(msgs);
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    #endregion Callbacks
}