using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System;
using System.IO;

public class main_controller : MonoBehaviour {

    // Audio sources
    string absolutePath = ".\\"; // relative path to where the app is running
    AudioSource src;
    List<AudioClip> clips = new List<AudioClip>();
    //int numClipsLastPlayed;
    //random number 
    float randomDelayLow, randomDelayHigh;


    // All sources
    private List<GameObject> sources;
    public GameObject source0;
    public GameObject source1;
    public GameObject source2;
    public GameObject source3;
    public GameObject source4;
    public GameObject source5;
    public GameObject source6;
    public GameObject source7;
    public GameObject source8;
    public GameObject source9;
    
    // Text box
    public Text dispText;

    // File IO related
    FileSystemWatcher watcher;
    string locSoundFile = "location_sound.txt";
    string configFile = "config.txt";
    bool useKeyboard;
    bool enableTextDisp;
    // Use this for initialization
    void Start () {
        parseConfigFile();

        absolutePath = Path.GetFullPath(absolutePath);
        // Prepare the audio files
        reloadSounds();

        // Set up the list of object
        sources = new List<GameObject>();
        sources.Add(source0);
        sources.Add(source1);
        sources.Add(source2);
        sources.Add(source3);
        sources.Add(source4);
        sources.Add(source5);
        sources.Add(source6);
        sources.Add(source7);
        sources.Add(source8);
        sources.Add(source9);

        // File watcher
        watcher = new FileSystemWatcher();
        watcher.Path = ".";
        // Watch for change of LastWrite
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        // Only watch one file
        watcher.Filter = locSoundFile;
        // Add event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.EnableRaisingEvents = true;

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
        //dispText.text += e.FullPath[0];
        if (!useKeyboard) readFileMoveAndPlay();
        

    }
    // Update is called once per frame
    void FixedUpdate () {
        if (useKeyboard && Input.GetKeyDown(KeyCode.Space))
        {
            readFileMoveAndPlay();
            if (enableTextDisp) dispText.text += clips[0].name;
        }
    }

    void readFileMoveAndPlay()
    {
        StreamReader reader = File.OpenText(locSoundFile);
        string line;
        int i = 0;
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');
            float x = float.Parse(items[0]);
            float y = float.Parse(items[1]);
            float z = float.Parse(items[2]);
            int c = int.Parse(items[3]);
            sources[i].transform.position = new Vector3(x, y, z);
            i++;
            if (enableTextDisp)dispText.text += "S"+c.ToString();
            AudioSource asource = sources[i].GetComponent<AudioSource>();
            asource.clip = clips[c];
            asource.PlayDelayed(Random.Range(randomDelayLow,randomDelayHigh));

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
                Debug.Log("Start loading "+f.FullName);
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
