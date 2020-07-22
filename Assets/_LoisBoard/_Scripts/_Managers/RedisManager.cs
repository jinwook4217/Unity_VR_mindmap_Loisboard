using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using socket.io;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class RedisManager : MonoBehaviour, IPageProvider
{
    Socket socket;
    public string ip;
    public string port;
    public static RedisManager Instance;
    public GameObject nodePrefab;
    public bool isHost;
    public bool isGuset;
    public bool isJoined;
    static string DEFAULT_ENCRYPT_KEY = "danij9112";
    private int lastMapId = -1;
    public bool isLoby = true;
    public bool isNotStart;
    public bool isLoaded;
    public bool isConnected;
    public bool isNoticed;

    public GameObject EnterArrow;

    // public GameObject tempEnterArrow;

    [Tooltip("the base space prefab.")]
    public GameObject exampleSpacePrefab;

    [Tooltip("the base space transpaent material.")]
    public Material[] themeMaterials = new Material[12];

    [Tooltip("The spacing between pages.")]
    public float spacing = 2f;
    public List<GameObject> _savedSpaces = new List<GameObject>();

    private Material[] _themeMaterials;

    public float GetSpacing()
    {
        return spacing;
    }

    public int GetNumberOfPages()
    {
        return _savedSpaces.Count;
    }

    public Transform ProvidePage(int index)
    {
        GameObject spaceObj = GameObject.Instantiate(_savedSpaces[index]);
        Transform space = spaceObj.transform;
        return space;
    }

    public void RemovePage(int index, Transform space)
    {
        GameObject.Destroy(space.gameObject);
    }

    List<ServerData> serverDatas = new List<ServerData>();

    public SceneLoader OwnSceneLoader
    {
        get
        {
            if (cashedSceneLoader != null)
            {
                return cashedSceneLoader;
            }
            else
            {
                cashedSceneLoader = GameObject.FindObjectOfType<SceneLoader>();
            }

            return cashedSceneLoader;
        }
    }
    private SceneLoader cashedSceneLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        isLoaded = true;
    }

    void Start()
    {

        var serverUrl = "http://" + ip + ":" + port;
        socket = Socket.Connect(serverUrl);

        socket.On(SystemEvents.connect, () =>
        {
            // print("connected");
            isConnected = true;
            socket.Emit("login", SystemInfo.deviceUniqueIdentifier);
        });
        socket.On("loadReturn", (string data) =>
        {
            dataSet ndsDataSet = JsonUtility.FromJson<dataSet>(data);
            Load(ndsDataSet.list);
        });

        socket.On("returnMapIds", (string data) =>
            {
                mapIds temp = JsonUtility.FromJson<mapIds>(data);
                List<int> ids = temp.Ids;
                lastMapId = ids.Count == 0 ? 0 : ids[ids.Count - 1];
                // print("count=" + ids.Count);
                for (int i = 0; i < ids.Count; i++)
                {
                    socket.Emit("searchMapData", SystemInfo.deviceUniqueIdentifier + "_" + ids[i]);
                    if (i == ids.Count - 1)
                    {
                        OwnSceneLoader.mapId = ids[i] + 1;
                    }
                }
            });
        socket.On("returnMapDatas", (string data) =>
        {
            ServerData mapDatas = JsonUtility.FromJson<ServerData>(data);
            GameObject temp = GameObject.Instantiate(exampleSpacePrefab);
            temp.transform.position = Vector3.back * 1000f;
            temp.name = mapDatas.date;

            Debug.Log(mapDatas.themeId);

            //  테마아이디 임시방편
            temp.GetComponentInChildren<JumpToPage>().themeIndex = mapDatas.themeId % 12;
            temp.GetComponentInChildren<JumpToPage>().mapId = mapDatas.mapId;
            temp.GetComponentInChildren<Text>().text = DateTime.Parse(mapDatas.date).ToString("yyyy.MM.dd") + "\n" + mapDatas.title;
            temp.GetComponentInChildren<Renderer>().material = CreateSpaceMaterial(mapDatas);
            // _savedSpaces.Add(temp);
            _savedSpaces.Insert(0, temp);

            if (mapDatas.mapId == lastMapId)
            {
                GameObject.FindObjectOfType<SavedPagesScroll>().enabled = true;

                if (EnterArrow == null)
                {
                    EnterArrow = GameObject.Find("EnterHere");
                }
                // tempEnterArrow =  Instantiate(EnterArrow) as GameObject;
                EnterArrow.transform.localScale = Vector3.one * 0.003f;
                iTween.ScaleTo(EnterArrow, iTween.Hash(
                    "scale", Vector3.one * 0.003f * 1.2f,
                    "easetype", iTween.EaseType.easeInBack,
                    "looptype", iTween.LoopType.pingPong,
                    "time", 1.5f
                ));
            }


        });
        socket.On("stateNow", (string data) =>
        {
            nodedatasWrap temp = JsonUtility.FromJson<nodedatasWrap>(data);
            List<nodeData> ndsNow = Instance.NodeDataSerialize();
            List<nodeData> ndsNew = temp.nodedatas;
            // print(ndsNow.Count + "//" + ndsNew.Count);

            for (int i = 0; i < ndsNow.Count; i++)
            {
                for (int j = 0; j < ndsNew.Count; j++)
                {
                    if (ndsNow[i].key.Equals(ndsNew[j].key))
                    {
                        if (!ndsNow[i].text.Equals(ndsNew[j].text))
                        {
                            searchNode(ndsNow[i].key).GetComponentInChildren<Text>().text = ndsNew[j].text;
                        }
                        ndsNow.RemoveAt(i);
                        ndsNew.RemoveAt(j);
                        i--;
                        j--;
                        break;
                    }
                }
            }
            if (ndsNow.Count != 0)
            {
                for (int i = 0; i < ndsNow.Count; i++)
                {
                    RemoveOne(ndsNow[i].key);
                }
            }
            if (ndsNew.Count != 0)
            {
                for (int i = 0; i < ndsNew.Count; i++)
                {
                    GameObject newNode = CreateNode(ndsNew[i]);
                }
            }
            Invoke("FindParentAll", 0.2f);
        });

    }

    // Update is called once per frame
    void Update()
    {
        //  if (Input.GetKey("escape"))
        //     Application.Quit();
        //Debug.Log(Application.internetReachability);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!isNoticed) StartCoroutine(TutorialManager.Instance.NetworkWarning());
            isNoticed = true;
        }
        else isNoticed = false;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (isNotStart && !isLoaded)
            {
                socket.Emit("reload", SystemInfo.deviceUniqueIdentifier);
                _savedSpaces = new List<GameObject>();
                isLoaded = true;
            }
            return;
        }
        if (OwnSceneLoader.willLoad)
        {
            LoadCall();
            OwnSceneLoader.willLoad = false;
        }
        if (OwnSceneLoader.isTempData)
        {
            if (PlayerPrefs.GetInt("isExistTempData") == 1)
            {
                DataManager.Instance.LoadSavedData();
                OwnSceneLoader.isTempData = false;
                PlayerPrefs.DeleteAll();
                return;
            }
            OwnSceneLoader.isTempData = false;
        }
        if (isHost)
        {
            if (!isJoined)
            {
                // print("I'm host!!");
                socket.Emit("createRoom", "host");
                isJoined = true;
            }
            else
            {
                List<nodeData> nds = Instance.NodeDataSerialize();
                keyList ndsKeyList = new keyList(OwnSceneLoader.mapId + OwnSceneLoader.deviceId, nds);
                socket.EmitJson("broadcast", JsonUtility.ToJson(ndsKeyList));
            }

        }
        else if (isGuset)
        {
            if (!isJoined)
            {
                // print("I'm guest!!");
                socket.Emit("createRoom", "guest");
                isJoined = true;
            }

        }
    }
    public GameObject searchNode(string key)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].GetComponent<Node>().nodeKey.Equals(key))
            {
                return nodes[i];
            }
        }
        return null;
    }
    public void LoadCall()
    {
        socket.Emit("load", SystemInfo.deviceUniqueIdentifier + "_" + OwnSceneLoader.mapId);
    }
    public void Load(List<nodeData> nodeDatas)
    {

        //Remove(false);
        string motherNodekey = NodeManager.Instance.motherNode.GetComponent<Node>().nodeKey;
        for (int i = 0; i < nodeDatas.Count; i++)
        {
            if (!nodeDatas[i].key.Equals(motherNodekey))
            {
                GameObject newNode = CreateNode(nodeDatas[i]);
                //FindParent(newNode);
            }
            else
            {
                NodeManager.Instance.motherNode.GetComponentInChildren<Text>().text = nodeDatas[i].text;
            }

        }
        Invoke("FindParentAll", 0.2f);
    }
    public void Save()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable || !isConnected)
        {
            DataManager.Instance.Save();
            return;
        }

        Debug.Log(ThemeManager.MainTheme.id);
        
        List<nodeData> nds = NodeDataSerialize();
        dataSet ndsDataSet = new dataSet(OwnSceneLoader.mapId,
          NodeManager.Instance.motherNode.GetComponentInChildren<Text>().text, DateTime.Now,
          ThemeManager.MainTheme.id, nds);

        // Debug.Log("save main theme id : " + ThemeManager.MainTheme.id);
        socket.EmitJson("save", JsonUtility.ToJson(ndsDataSet));
    }

    public List<nodeData> NodeDataSerialize()
    {
        List<nodeData> nodeDatas;
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        nodeDatas = new List<nodeData>();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeData nd = new nodeData();
            NodeToData(nd, nodes[i].GetComponent<Node>());
            nodeDatas.Add(nd);
        }
        return nodeDatas;
    }
    [System.Serializable]
    public class nodeData
    {
        public string key;
        public string parentKey;
        public float x;
        public float y;
        public float z;
        public int posNum;
        public float angle;
        public string text;
        public int gth;
    }
    public class keyList
    {
        public string key;
        public List<nodeData> list;

        public keyList(string key, List<nodeData> list)
        {
            this.key = key;
            this.list = list;
        }
    }

    public class serverDatasWrap
    {
        public List<ServerData> serverDataList;
    }

    public class ServerData
    {
        public int mapId;
        public string title;
        public string date;
        public int themeId;
        public ServerData(int mi, string title, string date, int themeId)
        {
            this.mapId = mi;
            this.date = date;
            this.title = title;
            this.themeId = themeId;
        }
    }
    [System.Serializable]
    public class nodedatasWrap
    {
        public List<nodeData> nodedatas;
    }

    [System.Serializable]
    public class dataSet
    {
        public string deviceId = SystemInfo.deviceUniqueIdentifier;
        public string title;
        public int mapId;
        public string date;
        public int themeId;
        public List<nodeData> list;
        public dataSet(int mapId, string title, DateTime date, int themeId, List<nodeData> list)
        {
            this.mapId = mapId;
            this.title = title;
            this.date = date.ToString();
            this.themeId = themeId;
            this.list = list;
        }
    }
    public class mapIds
    {
        public List<int> Ids;
    }

    public void NodeToData(nodeData nd, Node nodeInfo)
    {
        nd.key = nodeInfo.nodeKey;
        nd.parentKey = nodeInfo.parentNodeKey;
        nd.x = nodeInfo.transform.position.x;
        nd.y = nodeInfo.transform.position.y;
        nd.z = nodeInfo.transform.position.z;
        nd.posNum = nodeInfo.posNum;
        nd.angle = nodeInfo.angle;
        nd.text = nodeInfo.GetComponentInChildren<Text>().text;
        nd.gth = nodeInfo.gth;
    }
    public void Remove(bool willSave)
    {
        Node motherNodeInfo = NodeManager.Instance.motherNode.GetComponent<Node>();
        motherNodeInfo.willDestroy = true;
        motherNodeInfo.isFirstDeleteNode = true;
        //SaveState(work.CLEAR);
    }
    public GameObject CreateNode(nodeData nd)
    {
        GameObject newNode = Instantiate(nodePrefab);
        Node nodeInfo = newNode.GetComponent<Node>();
        DataToNode(nd, nodeInfo);
        return newNode;
    }
    public void DataToNode(nodeData nd, Node nodeInfo)
    {
        nodeInfo.nodeKey = nd.key;
        nodeInfo.parentNodeKey = nd.parentKey;
        nodeInfo.transform.position = new Vector3(nd.x, nd.y, nd.z);
        nodeInfo.posNum = nd.posNum;
        nodeInfo.angle = nd.angle;
        nodeInfo.GetComponentInChildren<Text>().text = nd.text;
        nodeInfo.gth = nd.gth;
    }
    public void FindParentAll()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; i++)
        {
            FindParent(nodes[i]);

        }
    }
    public void FindParent(GameObject node)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        Node nodeInfo = node.GetComponent<Node>();
        if (nodeInfo.parentNodeKey != null)
        {
            for (int j = 0; j < nodes.Length; j++)
            {
                if (nodeInfo.parentNodeKey.Equals(nodes[j].GetComponent<Node>().nodeKey))
                {
                    Node parentNodeInfo = nodes[j].GetComponent<Node>();
                    nodeInfo.parentNode = nodes[j];
                    nodeInfo.shouldDraw = true;
                    parentNodeInfo.hasChild = true;
                    parentNodeInfo.children[nodeInfo.posNum] = node;
                    break;
                }
            }
        }
    }
    public void RemoveOne(String key)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].GetComponent<Node>().nodeKey.Equals(key))
            {
                nodes[i].GetComponent<Node>().willDestroy = true;
                nodes[i].GetComponent<Node>().isFirstDeleteNode = true;
            }
        }
    }

    void SetString(string key, string value, string encryptKey)
    {
        //Hide '_key' string.
        MD5 md5Hash = MD5.Create();
        byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
        string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
        byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

        //Encrypt '_value' into a byte array
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);

        //Encrypt '_value' with 3DES
        TripleDES des = new TripleDESCryptoServiceProvider();
        des.Key = secret;
        des.Mode = CipherMode.ECB;
        ICryptoTransform xform = des.CreateEncryptor();
        byte[] encrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

        //Convert encrypted array into a readable string.
        string encryptedString = Convert.ToBase64String(encrypted);

        //Set the (key, encrypted value) pair in regular PlayerPrefs.
        PlayerPrefs.SetString(hashKey, encryptedString);
    }

    string GetString(string key, string encryptKey)
    {
        //Hide '_key' string.
        MD5 md5Hash = MD5.Create();
        byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
        string hashKey = System.Text.Encoding.UTF8.GetString(hashData);
        byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(encryptKey));

        // Retrieve encrypted '_value' and Base64 decode it.
        string _value = PlayerPrefs.GetString(hashKey);
        byte[] bytes = Convert.FromBase64String(_value);

        //Decrypt '_value' with 3DES
        TripleDES des = new TripleDESCryptoServiceProvider();
        des.Key = secret;
        des.Mode = CipherMode.ECB;
        ICryptoTransform xform = des.CreateDecryptor();
        byte[] decrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);

        // decrypte_value as a proper string
        string decryptedString = System.Text.Encoding.UTF8.GetString(decrypted);
        return decryptedString;
    }
    private Material CreateSpaceMaterial(ServerData serverData)
    {
        //  임시방편
        var material = new Material(themeMaterials[serverData.themeId % 12]);
        material.name = serverData.date;
        return material;
    }
    public void deleteMap(int mapIdForDelete)
    {
        deviceIdAndMapId DAMI = new deviceIdAndMapId(mapIdForDelete);

        socket.EmitJson("deleteMap", JsonUtility.ToJson(DAMI));
    }

    [System.Serializable]
    public class deviceIdAndMapId
    {
        public string deviceId;
        public int mapId;
        public deviceIdAndMapId(int mapId)
        {
            this.deviceId = SystemInfo.deviceUniqueIdentifier;
            this.mapId = mapId;
        }
    }
}