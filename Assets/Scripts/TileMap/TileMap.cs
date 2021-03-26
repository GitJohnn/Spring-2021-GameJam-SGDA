using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap
{
    private Grid<TileMapObject> grid;

    public TileMap(int width, int height, float cellsize, Vector3 originPosition)
    {
        grid = new Grid<TileMapObject>(width, height, cellsize, originPosition, (Grid<TileMapObject> g, int x, int y) => new TileMapObject(g, x, y));
    }


    public void SetTileMapSprite(Vector3 worldposition, TileMapObject.TileMapSprite tilemapSprite)
    {
        TileMapObject tilemapObject = grid.GetGridObject(worldposition);
        if(tilemapObject != null)
        {
            tilemapObject.SetTileMapSprite(tilemapSprite);
        }
    }

    public class TileMapObject
    {
        public enum TileMapSprite
        {
            None,
            Ground
        }

        private Grid<TileMapObject> grid;
        private int x;
        private int y;
        private TileMapSprite tilemapSprite;

        public TileMapObject(Grid<TileMapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTileMapSprite(TileMapSprite tilemapSprite)
        {
            this.tilemapSprite = tilemapSprite;
            if(grid != null)
            {
                grid.TriggerGridObjectChanged(x, y);
            }
            else
            {
                Debug.Log("No grid has been set");
            }
        }

        public TileMapSprite GetTileMapSprite()
        {
            return tilemapSprite;
        }

        public override string ToString()
        {
            return tilemapSprite.ToString();
        }
    }

}
