﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct BundleFile {
    public string name;
    public string fullPath;
}

public static class AssetBundleManager {
    // Store all loaded bundles
    static Dictionary<BundleFile, AssetBundle> loadedBundles = new Dictionary<BundleFile, AssetBundle>();

    static public List<BundleFile> ListBundles(string _path)
    {
        List<BundleFile> bundleList = new List<BundleFile>();
        string fullPath = Path.Combine(Application.streamingAssetsPath, _path);
        DirectoryInfo directory = new DirectoryInfo(fullPath);
        if(directory.Exists)
        {
            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Name.LastIndexOf('.') == -1) // no point in name
                {
                    bundleList.Add(new BundleFile{name=file.Name, fullPath=Path.Combine(fullPath, file.Name)});
                }
            }
        }
        return bundleList;
    }

    static public void LoadAllBundlesFrom(string _path)
    {
        List<BundleFile> infos = ListBundles(_path);
        foreach(BundleFile info in infos)
        {
            LoadBundle(info);
        }
    }

	static public AssetBundle LoadBundle(string _bundleName)
	{
        return LoadBundle(new BundleFile { name = _bundleName, fullPath = Path.Combine(Application.streamingAssetsPath, _bundleName) });
    }

    static public AssetBundle LoadBundle(BundleFile _bundle)
    {
        if (!loadedBundles.ContainsKey(_bundle))
        {
            loadedBundles[_bundle] = AssetBundle.LoadFromFile(_bundle.fullPath);
        }

        return loadedBundles[_bundle];
    }

    static public void UnloadBundle(BundleFile _bundle)
    {
        if (loadedBundles.ContainsKey(_bundle))
        {
            loadedBundles[_bundle].Unload(true);
            loadedBundles.Remove(_bundle);
        }
    }

    static public void UnloadAllBundles()
    {
        foreach (AssetBundle bundle in loadedBundles.Values)
        {
            bundle.Unload(true);
        }
        loadedBundles.Clear();
    }

    // Load all assets from a bundle and return only thos of type T
    static public List<T> LoadAllAssets<T>(AssetBundle _bundle) where T : Object
    {
        List<T> matchingAssets = new List<T>();
        Object[] assets = _bundle.LoadAllAssets();

        foreach (Object asset in assets)
        {
            if (asset.GetType() == typeof(T))
            {
                matchingAssets.Add(asset as T);
            }
        }
        return matchingAssets;
    }
}
