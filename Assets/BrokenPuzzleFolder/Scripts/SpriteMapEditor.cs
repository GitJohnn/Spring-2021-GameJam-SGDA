using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using CodeMonkey;

public class SpriteMapEditor : MonoBehaviour
{    
    [SerializeField] SpriteTileMapVisual spriteTilemapVisual;

    public int width = 10;
    public int heigh = 10;
    public int cellSize = 10;
    public string saveFileName;

    [SerializeField] Image noneSprite;
    [SerializeField] Image pathSprite;
    [SerializeField] Image grassSprite;
    [SerializeField] Image dirtSprite;
    [SerializeField] Image roadSprite;
    [SerializeField] Image woodSprite;
    [SerializeField] Image obstacleSprite;

    private SpriteTileMap spriteTilemap;
    private SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite tilemapSprite;
    
    private void Start()
    {
        spriteTilemap = new SpriteTileMap(width, heigh, cellSize, Vector3.zero);

        spriteTilemap.SetTileMapVisual(spriteTilemapVisual);
        SetSprites();
        //spriteTilemap.Load(saveFileName);

        //GameHandler_GridCombatSystem.Instance.gridPathfinding.RaycastWalkable();
        //GameHandler_GridCombatSystem.Instance.gridPathfinding.PrintMap((Vector3 vec, Vector3 size, Color color) => World_Sprite.Create(vec, size, color));
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            SpriteTileMap.SpriteTileMapObject tileObj = spriteTilemap.GetTileMapObject(position);
            if (tilemapSprite != SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Obstacle)
            {
                spriteTilemap.SetTileMapSprite(position, tilemapSprite);
            }
            else if (tileObj.GetObstacleObject() == null)
            {
                tileObj.InitializeObstacle();
            }
            else if (tileObj.GetObstacleObject() != null)
            {
                tileObj.ObstacleActivation(true);
            }
            //Debug.Log(spriteTilemap.GetTileMapObject(position).GetSpriteObject().name + " sprite has been set to " + tilemapSprite.ToString());
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            SpriteTileMap.SpriteTileMapObject tileObj = spriteTilemap.GetTileMapObject(position);
            if (tileObj.GetObstacleObject() != null)
            {
                tileObj.ObstacleActivation(false);
            }
        }

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.None;
        //    CMDebug.TextPopupMouse(tilemapSprite.ToString());
        //}
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Path;
        //    CMDebug.TextPopupMouse(tilemapSprite.ToString());
        //}
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Grass;
        //    CMDebug.TextPopupMouse(tilemapSprite.ToString());
        //}
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Dirt;
        //    CMDebug.TextPopupMouse(tilemapSprite.ToString());
        //}


        if (Input.GetKeyDown(KeyCode.P))
        {
            spriteTilemap.Save(saveFileName);
            CMDebug.TextPopupMouse("Saved!");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            spriteTilemap.Load(saveFileName);
            CMDebug.TextPopupMouse("Loaded!");
        }
    }

    public void SetSprites()
    {
        noneSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.None);
        pathSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Path);
        grassSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Grass);
        dirtSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Dirt);
        roadSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Road);
        woodSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Wood);
        obstacleSprite.sprite = spriteTilemapVisual.GetSprite(SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Obstacle);
    }

    public void SwitchTilemapSprite(string tileName)
    {
        switch (tileName)
        {
            case "None":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.None;
                break;
            case "Path":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Path;
                break;
            case "Grass":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Grass;
                break;
            case "Dirt":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Dirt;
                break;
            case "Road":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Road;
                break;
            case "Wood":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Wood;
                break;
            case "Obstacle":
                tilemapSprite = SpriteTileMap.SpriteTileMapObject.GroundTileMapSprite.Obstacle;
                break;
        }
        CMDebug.TextPopupMouse(tilemapSprite.ToString());
    }
}
