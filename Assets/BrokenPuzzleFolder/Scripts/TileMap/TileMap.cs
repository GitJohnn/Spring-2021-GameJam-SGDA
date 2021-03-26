using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap
{
    public event EventHandler OnLoaded; 
    
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

    public void SetTileMapVisual(TileMapVisual tilemapVisual)
    {
        tilemapVisual.SetGrid(this, grid);
    }

    /*
     * Save - Load 
    */
    public class SaveObject
    {
        public TileMapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public void Save()
    {
        List<TileMapObject.SaveObject> tilemapObjectSaveObjectList = new List<TileMapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach(TileMapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            TileMapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public class TileMapObject
    {
        public enum TileMapSprite
        {
            None,
            Path,
            Grass,
            Dirt
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

        [System.Serializable]
        public class SaveObject
        {
            public TileMapSprite tilemapSprite;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = tilemapSprite,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSprite = saveObject.tilemapSprite;
        }

    }

}
