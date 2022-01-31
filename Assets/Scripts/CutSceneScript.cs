using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutSceneScript : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _video;

    private void Start()
    {
        
    }

    public void OnClickStart()
    {
        for (int i = transform.GetChild(3).GetChild(2).GetChild(0).childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(3).GetChild(2).GetChild(0).GetChild(i).gameObject);
        }

        if (!Directory.Exists(Application.dataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
        }
        string[] allFiles = Directory.GetFiles(Application.dataPath + "/Saves", "*.json");

        for (int i = 0; i < allFiles.Length; i++)
        {
            var button = Instantiate(Resources.Load<Button>("SavedConfButton"), transform.GetChild(3).GetChild(2).GetChild(0));

            button.transform.GetChild(0).GetComponent<Text>().text = allFiles[i].Split('\\').Last().Split('.')[0];
            button.onClick.AddListener(() =>
            {
                string json = string.Empty;
                using (FileStream fileStream = new FileStream($"{Application.dataPath}/Saves/{button.transform.GetChild(0).GetComponent<Text>().text}.json", FileMode.Open))
                {
                    using (StreamReader sw = new StreamReader(fileStream))
                    {
                        json = sw.ReadToEnd();
                    }
                }
                ModelsScript.savedCategory = JsonConvert.DeserializeObject<SaveCategory[]>(json);
                transform.GetChild(5).GetComponent<AudioSource>().Stop();
                transform.GetChild(4).gameObject.SetActive(true);

                StartCoroutine(LoadMainScene());
            });
        }
    }

    public void OnClickEdit()
    {
        SceneManager.LoadScene(2);
    }

    private IEnumerator LoadMainScene()
    {
        _video.Play();
        yield return new WaitForSeconds(1);

        AsyncOperation load = SceneManager.LoadSceneAsync(1);
        load.allowSceneActivation = false;
     
        while (_video.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);
        load.allowSceneActivation = true;
    }
}
