using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public GameObject nodePrefab;
    static string DEFAULT_ENCRYPT_KEY = "danij9112";
    public List<nodeDatasWithWork> state;
    public int stateFlag;
    public int maxStateCount;
    public int checker;
    public enum work
    {
        REMOVE,
        CLEAR,
        CREATE,
        LOAD
    }
    // Use this for initialization

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        state = new List<nodeDatasWithWork>();
    }

    void Update()
    {
        checker = state.Count;
    }
    //현재 상태를 기록하는 함수
    //1. 현재 노드들의 상태를 기록한다, 인덱스를 기록한다.
    public void SaveState(work did)
    {
        nodeDatasWithWork ndww = new nodeDatasWithWork();
        ndww.nodeDatas = NodeDataSerialize();
        ndww.work = did;
        if (stateFlag < state.Count - 1)
        {
            state.RemoveRange(stateFlag + 1, state.Count - stateFlag - 1);
            state.Add(ndww);
        }
        else if (state.Count >= maxStateCount)
        {
            state.Add(ndww);
            state.RemoveAt(0);
        }
        else
        {
            state.Add(ndww);
        }
        stateFlag = state.Count - 1;
    }

    public void Redo()
    {
        //state= (List<List<nodeData>>)LoadFromPrefs("state");
        if (stateFlag == state.Count - 1)
        {
            print("can't redo");
            return;
        }
        switch (state[stateFlag + 1].work)
        {
            case work.REMOVE:
                {
                    List<nodeData> nodeDatas = CompareNode(state[stateFlag].nodeDatas, state[stateFlag + 1].nodeDatas);
                    nodeData firstNodeData = nodeDatas[0];
                    //첫번째 리무브 노드 찾기
                    for (int i = 1; i < nodeDatas.Count; i++)
                    {
                        if (firstNodeData.key.Length > nodeDatas[i].key.Length)
                        {
                            firstNodeData = nodeDatas[i];
                        }
                    }
                    RemoveOne(firstNodeData.key, false);
                }
                break;
            case work.CLEAR:
                {
                    Remove(false);
                }
                break;
            case work.CREATE:
                {
                    List<nodeData> nodeDatas = CompareNode(state[stateFlag].nodeDatas, state[stateFlag + 1].nodeDatas);
                    for (int i = 0; i < nodeDatas.Count; i++)
                    {
                        GameObject newNode = CreateNode(nodeDatas[i]);
                        FindParent(newNode);
                    }
                }
                break;
            case work.LOAD:
                {
                    Load(state[stateFlag + 1].nodeDatas);
                }
                break;
        }
        stateFlag++;
    }
    public void Undo()
    {
        //state= (List<List<nodeData>>)LoadFromPrefs("state");
        if (stateFlag == 0)
        {
            print("can't undo");
            return;
        }
        switch (state[stateFlag].work)
        {

            case work.REMOVE:
                {
                    List<nodeData> nodeDatas = CompareNode(state[stateFlag].nodeDatas, state[stateFlag - 1].nodeDatas);

                    for (int i = 0; i < nodeDatas.Count; i++)
                    {
                        GameObject newNode = CreateNode(nodeDatas[i]);
                    }
                    Invoke("FindParentAll", 0.2f);
                }
                break;
            case work.CLEAR:
                {
                    Load(state[stateFlag - 1].nodeDatas);
                }
                break;
            case work.CREATE:
                {
                    List<nodeData> nodeDatas = CompareNode(state[stateFlag].nodeDatas, state[stateFlag - 1].nodeDatas);
                    nodeData firstNodeData = nodeDatas[0];
                    //첫번째 리무브 노드 찾기
                    for (int i = 1; i < nodeDatas.Count; i++)
                    {
                        if (firstNodeData.key.Length > nodeDatas[i].key.Length)
                        {
                            firstNodeData = nodeDatas[i];
                        }
                    }
                    RemoveOne(firstNodeData.key, false);
                }
                break;
            case work.LOAD:
                {
                    Load(state[stateFlag - 1].nodeDatas);
                }
                break;
        }
        stateFlag--;

    }

    public void Save()
    {
        List<nodeData> nds = NodeDataSerialize();
        // nodeDatasWithWork ndww = new nodeDatasWithWork();
        // ndww.work = work.Save;
        // ndww.nodeDatas = nds;
        SaveAsPrefs("temp", nds);
        PlayerPrefs.SetInt("isExistTempData",1);
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

    public void Load(List<nodeData> nodeDatas)
    {

        if (nodeDatas == null)
        {
            return;
        }
        Remove(false);
        string motherNodekey = NodeManager.Instance.motherNode.GetComponent<Node>().nodeKey;
        for (int i = 0; i < nodeDatas.Count; i++)
        {
            if (!nodeDatas[i].key.Equals(motherNodekey))
            {
                CreateNode(nodeDatas[i]);
            }

        }
        Invoke("FindParentAll", 0.2f);
    }

    public void LoadSavedData()
    {
        Load((List<nodeData>)LoadFromPrefs("temp"));
        SaveState(work.LOAD);
    }

    public void Remove(bool willSave)
    {
        Node motherNodeInfo = NodeManager.Instance.motherNode.GetComponent<Node>();
        motherNodeInfo.willDestroy = true;
        motherNodeInfo.isFirstDeleteNode = true;
        if (willSave)
        {
            StartCoroutine(saveStateAfterTime(work.CLEAR));
        }
        //SaveState(work.CLEAR);
    }

    public void RemoveOne(String key, bool shouldSave)
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
        if (shouldSave)
        {
            StartCoroutine(saveStateAfterDelete(work.REMOVE, key));
        }
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
    }
    public class nodeDatasWithWork
    {
        public List<nodeData> nodeDatas;
        public work work;
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
    }
    public void DataToNode(nodeData nd, Node nodeInfo)
    {
        nodeInfo.nodeKey = nd.key;
        nodeInfo.parentNodeKey = nd.parentKey;
        nodeInfo.transform.position = new Vector3(nd.x, nd.y, nd.z);
        nodeInfo.posNum = nd.posNum;
        nodeInfo.angle = nd.angle;
        nodeInfo.GetComponentInChildren<Text>().text = nd.text;
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

    void SaveAsPrefs(string key, object targetToSave)
    {
        var binaryFormatter = new BinaryFormatter();
        var memoryStream = new MemoryStream();

        // score를 바이트 배열로 변환해서 저장합니다.
        binaryFormatter.Serialize(memoryStream, targetToSave);

        // 그것을 다시 한번 문자열 값으로 변환해서 
        // 'HighScore'라는 스트링 키값으로 PlayerPrefs에 저장합니다.
        //PlayerPrefs.SetString("nodeDatas", Convert.ToBase64String(memoryStream.GetBuffer()));
        SetString(key, Convert.ToBase64String(memoryStream.GetBuffer()), DEFAULT_ENCRYPT_KEY);
    }
    object LoadFromPrefs(string key)
    {
        object targetToLoad = null;
        // 'HighScore' 스트링 키값으로 데이터를 가져옵니다.
        //var data = PlayerPrefs.GetString("nodeDatas");
        var data = GetString(key, DEFAULT_ENCRYPT_KEY);

        if (!string.IsNullOrEmpty(data))
        {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(Convert.FromBase64String(data));

            // 가져온 데이터를 바이트 배열로 변환하고
            // 사용하기 위해 다시 리스트로 캐스팅해줍니다.
            targetToLoad = (object)binaryFormatter.Deserialize(memoryStream);

        }
        return targetToLoad;
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

    public List<nodeData> CompareNode(List<nodeData> Data1, List<nodeData> Data2)
    {
        List<string> key1 = new List<string>();
        List<string> key2 = new List<string>();
        for (int i = 0; i < Data1.Count; i++)
        {
            key1.Add(Data1[i].key);
        }
        for (int i = 0; i < Data2.Count; i++)
        {
            key2.Add(Data2[i].key);
        }
        for (int i = 0; i < key1.Count; i++)
        {
            for (int j = 0; j < key2.Count; j++)
            {
                if (key1[i].Equals(key2[j]))
                {
                    key1.RemoveAt(i);
                    key2.RemoveAt(j);

                    i--;
                    break;
                }
            }
        }
        List<nodeData> diffNodes = new List<nodeData>();
        if (key1.Count != 0)
        {
            for (int i = 0; i < key1.Count; i++)
            {
                for (int j = 0; j < Data1.Count; j++)
                {
                    if (key1[i].Equals(Data1[j].key))
                    {
                        diffNodes.Add(Data1[j]);
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < key2.Count; i++)
            {
                for (int j = 0; j < Data2.Count; j++)
                {
                    if (key2[i].Equals(Data2[j].key))
                    {
                        diffNodes.Add(Data2[j]);
                        break;
                    }
                }
            }
        }

        return diffNodes;
    }
    public GameObject CreateNode(nodeData nd)
    {
        GameObject newNode = Instantiate(nodePrefab);
        Node nodeInfo = newNode.GetComponent<Node>();
        DataToNode(nd, nodeInfo);
        return newNode;
    }

    IEnumerator saveStateAfterTime(work did)
    {
        while (GameObject.FindGameObjectsWithTag("Node").Length > 1)
        {
            yield return null;
        }
        SaveState(did);
    }
    IEnumerator saveStateAfterDelete(work did, String nodeKey)
    {
        bool canSave = false;
        while (!canSave)
        {
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
            canSave = true;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodeKey.Equals(nodes[i].GetComponent<Node>().nodeKey))
                {
                    canSave = false;
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        SaveState(did);
    }
    public void ChageState()
    {
        state[stateFlag].nodeDatas = NodeDataSerialize();
    }

}