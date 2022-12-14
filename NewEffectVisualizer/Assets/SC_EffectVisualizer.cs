using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SC_EffectVisualizer : MonoBehaviour
{
    [SerializeField] private string assetPath = "";
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private TextMeshProUGUI stateText = null;
    [SerializeField,Min(0.00001f)] private float cameraMoveSpeed = 0.1f;
    [SerializeField,Min(0.00001f)] private float objectMoveSpeed = 1.0f;
    [SerializeField] private string videoFileName = "effectVideo";

    private List<GameObject> effectList = new List<GameObject>();
    private GameObject mainCamera = null;
    
    [Serializable]
    public enum RecordType
    {
        picture,
        movie,
        inGame,
        Max
    }
    [SerializeField] private RecordType type = RecordType.movie;

    private void Awake()
    {
        mainCamera = Camera.main.gameObject;

        text.text = "now loading...";

        stateText.text = "";
    }

    private async void Start()
    {
        Debug.Log("ロード開始1 = " + assetPath);

        if (assetPath.ToCharArray()[assetPath.Length - 1].CompareTo('/') != 0)
        {
            Debug.Log("パス = " + assetPath);
            assetPath += "/";
            Debug.Log("足したよ = " + assetPath);
        }
        
        string[] fs = System.IO.Directory.GetFiles(assetPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        int i = 0;
        foreach (string _name in fs)
        {
            // Debug.Log("Load File 1 = " + _name);
            string _newName = _name.Replace("\\", "/");
            // Debug.Log("Load File 2 = " + _newName);
            try
            {
                GameObject temp = await Addressables.InstantiateAsync(_newName).Task;
                temp.SetActive(false);
                effectList.Add(temp);
            }
            catch (Exception e)
            {
                Debug.LogWarning("<color=orange>" + _newName + " オブジェクトがロードできませんでした" + "</color>" + e);
                throw;
            }

            i++;
        }
        
        Debug.Log("ロード終了");
    
        if (effectList.Count <= 0)
        {
            Debug.LogError("指定のフォルダ内に.prefabファイルがありませんでした");
        }
    
        if (type == RecordType.inGame)
        {
            await InGameUpdate();
        }
        else if (type == RecordType.movie)
        {
            await VideoShooting();
            // await Recorder();
            // Debug.Log("<color=orange>動画は未実装です</color>");
        }
        else if (type == RecordType.picture)
        {
            await TakePicture();
        }
        
        effectList.Clear();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
    }
    
    async UniTask InGameUpdate()
    {
        int currentIndex = 0;
        GameObject currentObject = effectList[currentIndex];
        currentObject.SetActive(true);
        currentObject.transform.position = Vector3.zero;
        text.text = currentObject.name;
        
        await UniTask.WaitWhile(() =>
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("Pキーが押されたのでゲームシーンでの確認モードを終了します");
                return false;
            }

            if (Input.GetKey(KeyCode.LeftShift))//エフェクト移動
            {
                stateText.text = "OBJ MOVE";
                if (Input.GetKey(KeyCode.A))
                {
                    currentObject.transform.position += Vector3.left.normalized * (objectMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    currentObject.transform.position += Vector3.right.normalized * (objectMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    currentObject.transform.position += Vector3.forward.normalized * (objectMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    currentObject.transform.position += Vector3.back.normalized * (objectMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    currentObject.transform.position += Vector3.up.normalized * (objectMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    currentObject.transform.position += Vector3.down.normalized * (objectMoveSpeed * Time.deltaTime);
                }
            }
            else//カメラ操作
            {
                stateText.text = "CAM MOVE";
                if (Input.GetKey(KeyCode.A))
                {
                    mainCamera.transform.position += Vector3.left.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    mainCamera.transform.position += Vector3.right.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    mainCamera.transform.position += Vector3.forward.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    mainCamera.transform.position += Vector3.back.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    mainCamera.transform.position += Vector3.up.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    mainCamera.transform.position += Vector3.down.normalized * (cameraMoveSpeed * Time.deltaTime);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentObject.SetActive(false);
                currentIndex++;
                if (currentIndex > effectList.Count)
                {
                    currentIndex = 0;
                }
                currentObject = effectList[currentIndex];
                currentObject.SetActive(true);
                currentObject.transform.position = Vector3.zero;
                text.text = currentObject.name;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("最初左 = " + currentIndex);
                currentObject.SetActive(false);
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = effectList.Count - 1;
                }
                Debug.Log("のちの左 = " + currentIndex);
                currentObject = effectList[currentIndex];
                currentObject.SetActive(true);
                currentObject.transform.position = Vector3.zero;
                text.text = currentObject.name;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentObject.TryGetComponent<ParticleSystem>(out var _eff))
                {
                    if (_eff.isPlaying)
                    {
                        _eff.Stop();

                    }
                    Debug.Log("エフェクト再生");
                    _eff.Play();
                }
            }
            
            return true;
        });
    }

    /// <summary>
    /// 画像を取る
    /// </summary>
    async UniTask TakePicture()
    {
        //保存先
        string folderName = "Screenshots";
        string path = Application.dataPath + "/" + folderName + "/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string date = DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        string fileName = "";
        
        foreach (GameObject _obj in effectList)
        {
            if (_obj.TryGetComponent<ParticleSystem>(out var _eff))
            {
                float currentTime = 0.0f;

                //有効化
                _obj.SetActive(true);
                _obj.transform.position = Vector3.zero;

                await UniTask.WaitWhile(() =>
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= _eff.main.duration * 0.5f)
                    {
                        string objectName = _obj.name.Replace("(Clone)", "");
                        fileName = path + date + objectName + ".png";
                        //todo:待機？
                        return false;
                    }
                    return true;
                });
                
                //写真取るよ
                Debug.Log("取るよー");
                ScreenCapture.CaptureScreenshot(fileName);
                await WaitGetTextutre(fileName);
                Debug.Log("取り終わったよー");
                _obj.SetActive(false);
            }
        }
    }

    async UniTask WaitGetTextutre(string _name)
    {
        await UniTask.WaitUntil(() => File.Exists(_name));
    }

    async UniTask VideoShooting()
    {
        foreach (GameObject _obj in effectList)
        {
            if (_obj.TryGetComponent<ParticleSystem>(out var _eff))
            {
                float currentTime = 0.0f;

                //有効化
                _obj.SetActive(true);
                _obj.transform.position = new Vector3(-3.0f, 0, 0);
                float moveDef = (6.0f / _eff.main.duration);
                
                await UniTask.WaitWhile(() =>
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= _eff.main.duration)
                    {
                        return false;
                    }

                    _obj.transform.position += Vector3.right.normalized * moveDef * Time.deltaTime;
                    return true;
                });
                
                _obj.SetActive(false);
            }
        }
    }

    
    /// <summary>
    /// 動画を取る
    /// </summary>
    async UniTask Recorder()
    {
        Debug.Log("録画設定開始");
        
        var setting = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        setting.FrameRate = 30.0f;
        setting.SetRecordModeToManual();
        
        var movieRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movieRecorderSettings.ImageInputSettings = new GameViewInputSettings()
            { OutputWidth = 1920, OutputHeight = 1080, };
        movieRecorderSettings.AudioInputSettings.PreserveAudio = false;
        movieRecorderSettings.OutputFile = videoFileName;
        movieRecorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        movieRecorderSettings.Enabled = true;
        setting.AddRecorderSettings(movieRecorderSettings);
        
        var recorderController = new RecorderController(setting);
        recorderController.StartRecording();
        
        Debug.Log("録画開始 = "  + recorderController.IsRecording());

        foreach (GameObject _obj in effectList)
        {
            if (_obj.TryGetComponent<ParticleSystem>(out var _eff))
            {
                //有効化
                _obj.SetActive(true);
                _obj.transform.position = Vector3.zero;

                // UniTask.Delay((int)(1000 * _eff.main.duration));

                //撮影
                // Debug.Log("録画待機");
                await UniTask.Delay((int)(1000 * _eff.main.duration));
                // setting.SetRecordModeToTimeInterval(0.0f, _eff.main.duration);
                // Debug.Log("録画一個終了");
                
                //無効化
                _obj.SetActive(false);
            }
        }
        
        recorderController.StopRecording();

        Debug.Log("録画終了");
    }
}
