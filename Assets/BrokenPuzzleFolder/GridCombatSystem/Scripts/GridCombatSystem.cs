using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using GridPathfindingSystem;

public class GridCombatSystem : MonoBehaviour {

    [SerializeField] private UnitGridCombat[] unitGridCombatArray;

    private State state;
    private UnitGridCombat unitGridCombat;
    private List<UnitGridCombat> playerTeamList;
    private List<UnitGridCombat> enemyTeamList;
    private int playerTeamActiveUnitIndex;
    private int enemyTeamActiveUnitIndex;
    private bool canMoveThisTurn;
    private bool canAttackThisTurn;

    private enum State
    {
        Normal,
        Waiting
    }

    private void Awake()
    {
        state = State.Normal;
    }

    private void Start()
    {
        playerTeamList = new List<UnitGridCombat>();
        enemyTeamList = new List<UnitGridCombat>();
        playerTeamActiveUnitIndex = -1;
        enemyTeamActiveUnitIndex = -1;

        // Set all UnitGridCombat on their GridPosition
        foreach (UnitGridCombat unitGridCombat in unitGridCombatArray)
        {
            GameHandler_GridCombatSystem.Instance.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);
             
            if (unitGridCombat.GetTeam() == UnitGridCombat.Team.Player)
            {
                playerTeamList.Add(unitGridCombat);
            }
            else
            {
                enemyTeamList.Add(unitGridCombat);
            }
        }

        SelectNextActiveUnit();
        UpdateValidMovePositions();
    }

    private void SelectNextActiveUnit()
    {
        if (unitGridCombat == null || unitGridCombat.GetTeam() == UnitGridCombat.Team.Enemy)
        {
            unitGridCombat = GetNextActiveUnit(UnitGridCombat.Team.Player);
        }
        else
        {
            unitGridCombat = GetNextActiveUnit(UnitGridCombat.Team.Enemy);
        }
        //turn on new active enemy light
        unitGridCombat.IsLightActive(true);
        //GameHandler_GridCombatSystem.Instance.SetCameraFollowPosition(unitGridCombat.GetPosition());
        canMoveThisTurn = true;
        canAttackThisTurn = true;
        UpdateValidMovePositions();
    }

    private UnitGridCombat GetNextActiveUnit(UnitGridCombat.Team team)
    {
        if (team == UnitGridCombat.Team.Player)
        {
            playerTeamActiveUnitIndex = (playerTeamActiveUnitIndex + 1) % playerTeamList.Count;
            if (playerTeamList[playerTeamActiveUnitIndex] == null || playerTeamList[playerTeamActiveUnitIndex].IsDead())
            {
                // Unit is Dead, get next one
                return GetNextActiveUnit(team);
            }
            else
            {
                return playerTeamList[playerTeamActiveUnitIndex];
            }
        }
        else
        {
            enemyTeamActiveUnitIndex = (enemyTeamActiveUnitIndex + 1) % enemyTeamList.Count;
            if (enemyTeamList[enemyTeamActiveUnitIndex] == null || enemyTeamList[enemyTeamActiveUnitIndex].IsDead())
            {
                // Unit is Dead, get next one
                return GetNextActiveUnit(team);
            }
            else
            {
                return enemyTeamList[enemyTeamActiveUnitIndex];
            }
        }
    }

    private void UpdateValidMovePositions()
    {
        Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
        GridPathfinding gridPathfinding = GameHandler_GridCombatSystem.Instance.gridPathfinding;

        // Get Unit Grid Position X, Y
        grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);

        // Set entire Tilemap to Invisible
        GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetAllTilemapSprite(
            MovementTilemap.TilemapObject.TilemapSprite.None
        );

        // Reset Entire Grid ValidMovePositions
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                grid.GetGridObject(x, y).SetIsValidMovePosition(false);
            }
        }

        int maxMoveDistance = unitGridCombat.GetMaxMoveDistance();
        for (int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++)
        {
            for (int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++)
            {
                //check if the x and y are inside the gridpathfinding
                if(x >= 0  && y >= 0 && x < gridPathfinding.GetMapWidth() && y < gridPathfinding.GetMapHeight())
                {
                    if (grid.GetGridObject(x, y).GetUnitGridCombat() == null)
                    {
                        if (gridPathfinding.IsWalkable(x, y))
                        {
                            // Position is Walkable
                            if (gridPathfinding.HasPath(unitX, unitY, x, y))
                            {
                                // There is a Path
                                if (gridPathfinding.GetPath(unitX, unitY, x, y).Count <= maxMoveDistance)
                                {
                                    // Path within Move Distance

                                    // Set Tilemap Tile to Move
                                    GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetTilemapSprite(
                                        x, y, MovementTilemap.TilemapObject.TilemapSprite.Move
                                    );

                                    grid.GetGridObject(x, y).SetIsValidMovePosition(true);
                                }
                                else
                                {
                                    // Path outside Move Distance!
                                }
                            }
                            else
                            {
                                // No valid Path
                            }
                        }
                        else
                        {
                            // Position is not Walkable
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 position = UtilsClass.GetMouseWorldPosition();
                    Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
                    GridObject gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());

                    // Check if clicking on a unit position
                    if (gridObject.GetUnitGridCombat() != null)
                    {
                        // Clicked on top of a Unit
                        if (unitGridCombat.IsEnemy(gridObject.GetUnitGridCombat()))
                        {
                            // Clicked on an Enemy of the current unit
                            if (unitGridCombat.CanAttackUnit(gridObject.GetUnitGridCombat()))
                            {
                                // Can Attack Enemy
                                if (canAttackThisTurn)
                                {
                                    canAttackThisTurn = false;
                                    // Attack Enemy
                                    state = State.Waiting;
                                    Debug.Log("Attacking");
                                    unitGridCombat.AttackUnit(gridObject.GetUnitGridCombat(), () =>
                                    {
                                        state = State.Normal;
                                        TestTurnOver();
                                    });
                                }
                            }
                            else
                            {
                                // Cannot attack enemy
                                CodeMonkey.CMDebug.TextPopupMouse("Cannot attack!");
                            }
                            break;
                        }
                        else
                        {
                            // Not an enemy
                        }
                    }
                    else
                    {
                        // No unit here
                    }

                    if (grid.IsInGrid(position))
                    {
                        if (gridObject.GetIsValidMovePosition())
                        {
                            if (canMoveThisTurn)
                            {
                                canMoveThisTurn = false;

                                state = State.Waiting;

                                // Set entire Tilemap to Invisible
                                GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetAllTilemapSprite(
                                    MovementTilemap.TilemapObject.TilemapSprite.None
                                );

                                // Remove Unit from current Grid Object
                                grid.GetGridObject(unitGridCombat.GetPosition()).ClearUnitGridCombat();
                                // Set Unit on target Grid Object
                                gridObject.SetUnitGridCombat(unitGridCombat);
                                //Move towards mouse click position
                                unitGridCombat.MoveTo(position, () =>
                                {
                                    state = State.Normal;
                                    UpdateValidMovePositions();
                                    TestTurnOver();
                                });
                            }
                            break;

                        }
                        else
                        {
                            //
                            CMDebug.TextPopupMouse("Not a valid move position");
                        }
                    }
                    else
                    {
                        //Not inside the grid
                        CMDebug.TextPopupMouse("Not a valid move position");
                    }
                }

                //force turn over
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ForceTurnOver();
                }

                break;
            case State.Waiting:
                //force turn over
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ForceTurnOver();
                }
                break;
        }        

        //switch (state)
        //{
        //    case State.Normal:

        //        if (Input.GetMouseButtonDown(0))
        //        {
        //            Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
        //            GridObject gridObject = grid.GetGridObject(UtilsClass.GetMouseWorldPosition());

        //            // Check if clicking on a unit position
        //            if (gridObject.GetUnitGridCombat() != null)
        //            {
        //                // Clicked on top of a Unit
        //                if (unitGridCombat.IsEnemy(gridObject.GetUnitGridCombat()))
        //                {
        //                    // Clicked on an Enemy of the current unit
        //                    if (unitGridCombat.CanAttackUnit(gridObject.GetUnitGridCombat()))
        //                    {
        //                        // Can Attack Enemy
        //                        if (canAttackThisTurn)
        //                        {
        //                            canAttackThisTurn = false;
        //                            // Attack Enemy
        //                            state = State.Waiting;
        //                            unitGridCombat.AttackUnit(gridObject.GetUnitGridCombat(), () => {
        //                                state = State.Normal;
        //                                TestTurnOver();
        //                            });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // Cannot attack enemy
        //                        CodeMonkey.CMDebug.TextPopupMouse("Cannot attack!");
        //                    }
        //                    break;
        //                }
        //                else
        //                {
        //                    // Not an enemy
        //                }
        //            }
        //            else
        //            {
        //                // No unit here
        //            }

        //            if (gridObject.GetIsValidMovePosition())
        //            {
        //                // Valid Move Position

        //                if (canMoveThisTurn)
        //                {
        //                    canMoveThisTurn = false;

        //                    state = State.Waiting;

        //                    // Set entire Tilemap to Invisible
        //                    GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetAllTilemapSprite(
        //                        MovementTilemap.TilemapObject.TilemapSprite.None
        //                    );

        //                    // Remove Unit from current Grid Object
        //                    grid.GetGridObject(unitGridCombat.GetPosition()).ClearUnitGridCombat();
        //                    // Set Unit on target Grid Object
        //                    gridObject.SetUnitGridCombat(unitGridCombat);

        //                    unitGridCombat.MoveTo(UtilsClass.GetMouseWorldPosition(), () => {
        //                        state = State.Normal;
        //                        //UpdateValidMovePositions();
        //                        TestTurnOver();
        //                    });
        //                }
        //            }
        //        }

        //        if (Input.GetKeyDown(KeyCode.Space))
        //        {
        //            ForceTurnOver();
        //        }
        //        break;
        //    case State.Waiting:
        //        break;
        //}
    }

    private void TestTurnOver()
    {
        if (!canMoveThisTurn && !canAttackThisTurn)
        {
            // Cannot move or attack, turn over
            ForceTurnOver();
        }
    }

    private void ForceTurnOver()
    {
        unitGridCombat.IsLightActive(false);
        SelectNextActiveUnit();
        UpdateValidMovePositions();
    }



    public class GridObject {

        private Grid<GridObject> grid;
        private int x;
        private int y;
        private bool isValidMovePosition;
        private UnitGridCombat unitGridCombat;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetIsValidMovePosition(bool set)
        {
            isValidMovePosition = set;
        }

        public bool GetIsValidMovePosition()
        {
            return isValidMovePosition;
        }

        public void SetUnitGridCombat(UnitGridCombat unitGridCombat)
        {
            this.unitGridCombat = unitGridCombat;
        }

        public void ClearUnitGridCombat()
        {
            SetUnitGridCombat(null);
        }

        public UnitGridCombat GetUnitGridCombat()
        {
            return unitGridCombat;
        }

    }

}
