using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTileMap : MonoBehaviour
{
    public event EventHandler OnLoaded;

    private Grid<SpriteTileMapObject> grid;

    public SpriteTileMap(int width, int height, float cellsize, Vector3 originPosition)
    {
        grid = new Grid<SpriteTileMapObject>(width, height, cellsize, originPosition, (Grid<SpriteTileMapObject> g, int x, int y) => new SpriteTileMapObject(g, x, y));
    }

    public void SetTileMapSprite(Vector3 worldposition, SpriteTileMapObject.GroundTileMapSprite tilemapSprite)
    {
        SpriteTileMapObject tilemapObject = grid.GetGridObject(worldposition);
        if (tilemapObject != null)
        {
            tilemapObject.SetTileMapSprite(tilemapSprite);
        }
    }

    public SpriteTileMapObject GetTileMapObject(Vector3 worldposition)
    {
        return grid.GetGridObject(worldposition);
    }

    public void SetTileMapVisual(SpriteTileMapVisual tilemapVisual)
    {
        tilemapVisual.SetGrid(this, grid);
    }

    public class SaveObject
    {
        public SpriteTileMapObject.SaveObject[] tilemapObjectSaveObjectArray;
    }

    public void Save(string fileName)
    {
        List<SpriteTileMapObject.SaveObject> tilemapObjectSaveObjectList = new List<SpriteTileMapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                SpriteTileMapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(fileName,saveObject, true);
    }

    public void Load(string fileName)
    {
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>(fileName);
        foreach (SpriteTileMapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            SpriteTileMapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);
            tilemapObject.Load(tilemapObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    [System.Serializable]
    public class SpriteTileMapObject
    {
        public enum GroundTileMapSprite
        {
            None,
            Path,
            Grass,
            Dirt,
            Road,
            Wood,
            Obstacle
        }

        private Grid<SpriteTileMapObject> grid;
        private int x;
        private int y;
        private GameObject groundSpriteObject;
        private GameObject obstacleSpriteObject;
        private SpriteRenderer groundSpriteRenderer;
        private SpriteRenderer obstacleSpriteRenderer;
        [SerializeField] private GroundTileMapSprite groundTilemapSprite;
        [SerializeField] private Sprite sprite;

        public SpriteTileMapObject(Grid<SpriteTileMapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            this.groundSpriteObject = new GameObject("Tile at " + x + "," + y, typeof(SpriteRenderer));// = Instantiate(new GameObject("Tile at " + x + "," + y), grid.GetWorldPosition(x, y), Quaternion.identity);
            this.groundSpriteObject.transform.position = grid.GetWorldPosition(x, y);
            this.groundSpriteObject.transform.localScale *= grid.GetCellSize();
            this.groundSpriteRenderer = groundSpriteObject.GetComponent<SpriteRenderer>();
            this.groundSpriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            //this.groundSpriteRenderer.la
            this.groundSpriteRenderer.sortingOrder = 0;
        }

        public void SetTileMapSprite(GroundTileMapSprite tilemapSprite)
        {
            this.groundTilemapSprite = tilemapSprite;
            if (grid != null)
            {
                grid.TriggerGridObjectChanged(x, y);
            }
            else
            {
                Debug.Log("No grid has been set");
            }
        }

        public void ObstacleActivation(bool isActive)
        {
            this.obstacleSpriteObject.SetActive(isActive);
        }

        public void InitializeObstacle()
        {
            this.obstacleSpriteObject = new GameObject("Obstacle at " + x + "," + y, typeof(SpriteRenderer));                
            this.obstacleSpriteObject.transform.parent = this.groundSpriteObject.transform;
            this.obstacleSpriteObject.transform.localPosition = Vector3.zero;
            this.obstacleSpriteObject.transform.localScale = Vector3.one;
            this.obstacleSpriteObject.AddComponent<BoxCollider2D>().size = Vector2.one;
            this.obstacleSpriteObject.GetComponent<BoxCollider2D>().offset = Vector2.one * 0.5f;
            this.obstacleSpriteRenderer = this.obstacleSpriteObject.GetComponent<SpriteRenderer>();
            this.obstacleSpriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            this.obstacleSpriteRenderer.sortingOrder = 1;
            if (grid != null)
            {
                grid.TriggerGridObjectChanged(x, y);
            }
            else
            {
                Debug.Log("No grid has been set");
            }
        }

        public void SetObstacleSprite(Sprite obstacleSprite)
        {
            this.obstacleSpriteRenderer.sprite = obstacleSprite;
        }

        public void SetGroundSprite(Sprite objSprite)
        {
            this.groundSpriteRenderer.sprite = objSprite;
        }

        public GameObject GetObstacleObject()
        {
            return obstacleSpriteObject;
        }

        public GameObject GetSpriteObject()
        {
            return groundSpriteObject;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public GroundTileMapSprite GetTileMapSprite()
        {
            return groundTilemapSprite;
        }

        public override string ToString()
        {
            return groundTilemapSprite.ToString();
        }

        [System.Serializable]
        public class SaveObject
        {
            public GroundTileMapSprite tilemapSprite;
            public bool hasObstacle;
            public int x;
            public int y;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = groundTilemapSprite,
                hasObstacle = obstacleSpriteObject,
                x = x,
                y = y
            };
        }

        public void Load(SaveObject saveObject)
        {
            groundTilemapSprite = saveObject.tilemapSprite;
            if (saveObject.hasObstacle)
            {
                if(obstacleSpriteObject == null)
                {
                    InitializeObstacle();
                }
                else
                {
                    ObstacleActivation(true);
                }
            }
        }

    }

}
