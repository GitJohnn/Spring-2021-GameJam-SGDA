using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    public Transform gridTransform;

    private TileMap tilemap;
    
    private void Start()
    {
        tilemap = new TileMap(20, 10, 10f, gridTransform.position);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            tilemap.SetTileMapSprite(position, TileMap.TileMapObject.TileMapSprite.Ground);
        }
    }
}
