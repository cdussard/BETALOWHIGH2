using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class sceneManager : MonoBehaviour
{
    public static sceneManager instance;
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            LoadGoNoGo();
        }

                if (Input.GetKeyDown(KeyCode.C))
        {
            LoadNeurofeedback();
        }*/
    }

    void Awake()
    {

        if (instance!= null && instance!= this)
        {
            Destroy(gameObject);
            return;
            
        }

        else 
        {
            instance = this;
        }
    }

    public void LoadGoNoGo()
    {

        //SceneManager.LoadScene("NoGo_openvibeTCP.unity",LoadSceneMode.Single);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1,LoadSceneMode.Additive);
        Debug.Log("loaded GONOGO scene");
        
    }

    public void LoadNeurofeedback ()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1,LoadSceneMode.Additive);
            Debug.Log("loaded NF SCENE");
            

        }

    
    
}
