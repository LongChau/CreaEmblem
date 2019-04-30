﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileDataStruct
{
    public TileBase tile;
    public ZoneData data;
}

[DefaultExecutionOrder(-100)]
public class MapManager : MonoBehaviour
{
    static MapManager instance = null;

    [SerializeField] Tilemap path;
    [SerializeField] TileBase pathTile;

    [SerializeField] List<TileDataStruct> tileData = new List<TileDataStruct>();

    GridInformation gridInfo = null;
    Grid grid = null;

    // List containing all heroes currently on the map
    List<Hero> heroes = new List<Hero>();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Tilemap tileMap = GetComponentInChildren<Tilemap>();
        gridInfo = GetComponent<GridInformation>();
        grid = GetComponent<Grid>();

        if (tileMap != null)
        {
            for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
            {
                for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
                {
                    Vector3Int localPlace = (new Vector3Int(n, p, 0));
                    if (tileMap.HasTile(localPlace))
                    {
                        ZoneData data = tileData.Find(x => x.tile == tileMap.GetTile(localPlace)).data;

                        gridInfo.SetPositionProperty(localPlace, "data", data as Object);
                        
                    }
                    else
                    {
                        //No tile at "place"
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }






    /*  ===========================================================
     *              STATIC METHODS ACCESSIBLE ANYWHERE
     *  ===========================================================
     */

    public static ZoneData GetZoneAtPosition(Vector3Int _position)
    {
        return instance.gridInfo.GetPositionProperty<ZoneData>(_position, "data", null);
    }

    public static ZoneData GetZoneUnderMouse()
    {
        return GetZoneAtPosition(GetTileUnderMouse());
    }


    public static Vector3Int GetTileAtPosition(Vector3 _position)
    {
        return instance.grid.WorldToCell(_position);
    }

    public static Vector3Int GetTileUnderMouse()
    {
        return GetTileAtPosition(Utils.GetMouseWorldPosition());
    }

    public static Vector3 GetTileCenter(Vector3Int _tile)
    {
        return instance.grid.GetCellCenterWorld(_tile);
    }


    public static void RegisterHero(Hero _hero)
    {
        if(!instance.heroes.Contains(_hero))
        {
            instance.heroes.Add(_hero);
        }
    }

    public static void UnregisterHero(Hero _hero)
    {
        if (instance.heroes.Contains(_hero))
        {
            instance.heroes.Remove(_hero);
        }
    }

    public static Hero GetHeroAtTile(Vector3Int _tile)
    {
        return instance.heroes.Find(x => x.gridPosition == _tile);
    }


    public static void ResetPath()
    {
        instance.path.ClearAllTiles();
    }

    public static void SetPathTo(Vector3Int _tile)
    {
        instance.path.SetTile(_tile, instance.pathTile);
    }
}
