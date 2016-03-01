using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System;
using System.IO;

public class main_controller : MonoBehaviour {

    // Audio sources
    string absolutePath = ".\\"; // relative path to where the app is running
    List<AudioClip> clips;
    //int numClipsLastPlayed;
    //random number 
    float randomDelayLow, randomDelayHigh;
    int NUM_OF_BALLS = 9;
    private List<GameObject> balls;
    private GameObject objprefab;
    
    // Text box
    public Text dispText;

    // File IO related
    FileSystemWatcher watcher;
    string locSoundFile;
    string configFile = "config.txt";
    bool useKeyboard;
    bool enableTextDisp;
    bool flagFileChanged;   // for file watcher
    // Use this for initialization
    void Start () {
        parseConfigFile();

        absolutePath = Path.GetFullPath(absolutePath);
        // Prepare the audio files
        reloadSounds();
        clips = new List<AudioClip>();
        
        objprefab = (GameObject) Resources.Load("Lightbulbprefab");
        balls = new List<GameObject>();
        for (int j = 0; j<NUM_OF_BALLS; j++) balls.Add(Instantiate(objprefab));
        // File watcher
        locSoundFile = "location_sound.txt"; // the file initially watch
        watcher = new FileSystemWatcher();
        watcher.Path = ".";
        // Watch for change of LastWrite
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        // watch all similar files
        watcher.Filter = "location_sound*.txt";
        // Add event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnChanged);
        watcher.EnableRaisingEvents = true;
        flagFileChanged = false;

    }

    private void parseConfigFile()
    {
        StreamReader reader = File.OpenText(configFile);
        string line;
        line = reader.ReadLine(); // skip first line
        line = reader.ReadLine(); // read second line
        string[] items = line.Split(',');
        useKeyboard = bool.Parse(items[0]);
        enableTextDisp = bool.Parse(items[1]);
        randomDelayLow = float.Parse(items[2]);
        randomDelayHigh = float.Parse(items[3]);
        reader.Close();

        }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        if (!useKeyboard) {
            flagFileChanged = true;
            locSoundFile = e.Name;
        }
    }
    // Update is called once per frame
    void FixedUpdate () {
        if (useKeyboard && Input.GetKeyDown(KeyCode.Space))
        {
            readFileMoveAndPlay(locSoundFile);
        }

        if (!useKeyboard && flagFileChanged) {
            readFileMoveAndPlay(locSoundFile);
            flagFileChanged = false;
        }
    }

    void readFileMoveAndPlay(string name)
    {
        //Debug.Log("Start loading " + name);
        StreamReader reader = File.OpenText(name);
        string line;
        int i = 0;
		
        while ((line = reader.ReadLine()) != null)
        {
            if (enableTextDisp) dispText.text += "l:" + line + ":l";
            string[] items = line.Split(',');
            float x = float.Parse(items[0]);
            float y = float.Parse(items[1]);
            float z = float.Parse(items[2]);
            int c = int.Parse(items[3]);

            balls[i].transform.position = new Vector3(x, y, z);
            if (enableTextDisp)dispText.text += "S"+c.ToString();
            AudioSource asource = balls[i].GetComponent<AudioSource>();
            asource.clip = clips[c];
            asource.PlayDelayed(Random.Range(randomDelayLow,randomDelayHigh));

            i++;

        }
        reader.Close();
    }

    
    
    //compatible file extensions
    string[] fileTypes = { "ogg", "wav" };
    FileInfo[] files;
    void reloadSounds()
    {
        DirectoryInfo info = new DirectoryInfo(absolutePath);
        files = info.GetFiles();
        
        //check if the file is valid and load it
        foreach (FileInfo f in files)
        {
            // debug: dispText.text += f.FullName;
            if (validFileType(f.FullName))
            {
                //Debug.Log("Start loading "+f.FullName);
                StartCoroutine(loadFile(f.FullName));
            }
        }
    }
    bool validFileType(string filename)
    {
        foreach (string ext in fileTypes)
        {
            if (filename.IndexOf(ext) > -1) return true;
        }
        return false;
    }
    IEnumerator loadFile(string path)
    {
        WWW www = new WWW("file://" + path);

        AudioClip myAudioClip = www.audioClip;
        while (!myAudioClip.isReadyToPlay)
            yield return www;

        AudioClip clip = www.GetAudioClip(false);
        string[] parts = path.Split('\\');
        clip.name = parts[parts.Length - 1];
        clips.Add(clip);
    }

}
