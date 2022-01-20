using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutSceneScript : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _video;

    public void OnClickStart()
    {
        StartCoroutine(LoadMainScene());
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
