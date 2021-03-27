using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Testing : MonoBehaviour
{
    [SerializeField] TileMapVisual tilemapVisual;
    [SerializeField] Transform gridTransform;

    public int width = 10;
    public int heigh = 10;
    public int cellSize = 10;

    private TileMap tilemap;
    private TileMap.TileMapObject.TileMapSprite tilemapSprite;
    
    private void Start()
    {
        tilemap = new TileMap(width, heigh, cellSize, gridTransform.position);

        tilemap.SetTileMapVisual(tilemapVisual);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            tilemap.SetTileMapSprite(position, tilemapSprite);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            tilemapSprite = TileMap.TileMapObject.TileMapSprite.None;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            tilemapSprite = TileMap.TileMapObject.TileMapSprite.Path;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            tilemapSprite = TileMap.TileMapObject.TileMapSprite.Grass;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            tilemapSprite = TileMap.TileMapObject.TileMapSprite.Dirt;
            CMDebug.TextPopupMouse(tilemapSprite.ToString());
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            tilemap.Save();
            CMDebug.TextPopupMouse("Saved!");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            tilemap.Load();
            CMDebug.TextPopupMouse("Loaded!");
        }
    }
}
