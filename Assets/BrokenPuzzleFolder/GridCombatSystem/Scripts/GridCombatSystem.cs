using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using GridPathfindingSystem;
using UnityEngine.UI;

public class GridCombatSystem : MonoBehaviour {

    [SerializeField] private UnitGridCombat[] unitGridCombatArray;

    public float enemyWaitTime = 1.5f;

    private State state;
    private UnitGridCombat unitGridCombat;
    private UnitGridCombat playerUnitGridCombat;
    private List<UnitGridCombat> enemyTeamList;
    private int playerTeamActiveUnitIndex;
    private int enemyTeamActiveUnitIndex;
    private bool canMoveThisTurn;
    private bool canAttackThisTurn;

    private bool turnCanStart = false;
    private bool isUsingCardEffect = false;
    private ElementCardUsed elementCardType = ElementCardUsed.None;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button seeCardsButton;

    private bool playerLost = false;
    private bool playerWon = false;

    public enum ElementCardUsed
    {
        None,
        Fire,
        Ice,
        Thunder
    }

    private enum State
    {
        PlayerTurn,
        EnemyTurn,
        Waiting
    }

    private void Awake()
    {
        state = State.PlayerTurn;
        endTurnButton.interactable = turnCanStart;
        seeCardsButton.interactable = turnCanStart;
    }

    private void Start()
    {
        //playerUnitGridCombat = new List<UnitGridCombat>();
        enemyTeamList = new List<UnitGridCombat>();
        playerTeamActiveUnitIndex = -1;
        enemyTeamActiveUnitIndex = -1;

        // Set all UnitGridCombat on their GridPosition
        foreach (UnitGridCombat unitGridCombat in unitGridCombatArray)
        {
            GameHandler_GridCombatSystem.Instance.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);
             
            if (unitGridCombat.GetTeam() == UnitGridCombat.Team.Player)
            {
                playerUnitGridCombat = unitGridCombat;
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
            state = State.PlayerTurn;
        }
        else
        {
            unitGridCombat = GetNextActiveUnit(UnitGridCombat.Team.Enemy);
            state = State.EnemyTurn;
        }
        //turn on new active enemy light
        unitGridCombat.IsLightActive(true);
        //GameHandler_GridCombatSystem.Instance.SetCameraFollowPosition(unitGridCombat.GetPosition());
        canMoveThisTurn = true;
        canAttackThisTurn = true;
        UpdateValidMovePositions();
    }

    public UnitGridCombat GetNextActiveUnit(UnitGridCombat.Team team)
    {
        if (team == UnitGridCombat.Team.Player)
        {
            return playerUnitGridCombat;

            //playerTeamActiveUnitIndex = (playerTeamActiveUnitIndex + 1) % playerUnitGridCombat.Count;
            //if (playerUnitGridCombat[playerTeamActiveUnitIndex] == null || playerUnitGridCombat[playerTeamActiveUnitIndex].IsDead())
            //{
            //    // Unit is Dead, get next one
            //    return GetNextActiveUnit(team);
            //}
            //else
            //{
            //    return playerUnitGridCombat[playerTeamActiveUnitIndex];
            //}
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

    private bool IsPlayerCloseToAttack()
    {
        Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
        GridPathfinding gridPathfinding = GameHandler_GridCombatSystem.Instance.gridPathfinding;

        // Get Unit Grid Position X, Y
        grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);
        //Get Player Grid Position X, Y
        Vector3 playerPos = GetNextActiveUnit(UnitGridCombat.Team.Player).GetPosition();
        grid.GetXY(playerPos, out int playerX, out int playerY);
        Debug.Log("Player is at " + playerX + "," + playerY + " Enemy is at " + unitX + "," + unitY);
        int maxAttackDistance = unitGridCombat.GetMaxAttackDistance();
        //Debug.Log(gridPathfinding.GetPath(unitX, unitY, playerX, playerY).Count + " is distance from enemy to player");
        if(gridPathfinding.GetPath(unitX, unitY, playerX, playerY).Count <= maxAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CanAttackUnit(UnitGridCombat attackedUnit)
    {
        Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
        GridPathfinding gridPathfinding = GameHandler_GridCombatSystem.Instance.gridPathfinding;

        // Get Unit Grid Position X, Y
        grid.GetXY(unitGridCombat.GetPosition(), out int unitX, out int unitY);
        //Get attacked unit Grid Position X, Y
        grid.GetXY(attackedUnit.GetPosition(), out int posX, out int posY);
        int maxAttackDistance = unitGridCombat.GetMaxAttackDistance();
        //Debug.Log(gridPathfinding.GetPath(unitX, unitY, playerX, playerY).Count + " is distance from enemy to player");
        if (gridPathfinding.GetPath(unitX, unitY, posX, posY).Count <= maxAttackDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GridObject ReturnClosestGridObjectToPlayer(UnitGridCombat unit)
    {
        Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();
        GridPathfinding gridPathfinding = GameHandler_GridCombatSystem.Instance.gridPathfinding;

        GridObject closestGridObject = null;

        // Get Unit Grid Position X, Y
        grid.GetXY(unit.GetPosition(), out int unitX, out int unitY);

        //Get Player Grid Position X, Y
        Vector3 playerPos = GetNextActiveUnit(UnitGridCombat.Team.Player).GetPosition();
        grid.GetXY(playerPos, out int playerX, out int playerY);
        //Debug.Log("Player position is " + playerPos);
        int maxMoveDistance = unit.GetMaxMoveDistance();
        for (int x = unitX - maxMoveDistance; x <= unitX + maxMoveDistance; x++)
        {
            for (int y = unitY - maxMoveDistance; y <= unitY + maxMoveDistance; y++)
            {
                //check if the x and y are inside the gridpathfinding
                if (x >= 0 && y >= 0 && x < gridPathfinding.GetMapWidth() && y < gridPathfinding.GetMapHeight())
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
                                    if(closestGridObject != null)
                                    {
                                        Vector3Int currentGridPos = grid.GetGridObject(x, y).GetGridObjectPosition();
                                        Vector3Int closestGridPos = closestGridObject.GetGridObjectPosition();
                                        if (gridPathfinding.GetPath(currentGridPos.x, currentGridPos.y, playerX, playerY).Count < gridPathfinding.GetPath(closestGridPos.x, closestGridPos.y, playerX, playerY).Count)
                                        {
                                            //Debug.Log("Distance from current grid position " + gridPathfinding.GetPath(currentGridPos.x, currentGridPos.y, playerX, playerY).Count);
                                            //Debug.Log("Distance from current closest grid position " + gridPathfinding.GetPath(closestGridPos.x, closestGridPos.y, playerX, playerY).Count);
                                            closestGridObject = grid.GetGridObject(x, y);
                                            //Debug.Log("Current closest grid position is " + closestGridObject.GetGridObjectPosition());
                                        }
                                    }
                                    else
                                    {
                                        closestGridObject = grid.GetGridObject(unitX, unitY);
                                    }
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

        return closestGridObject;
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
                                    //set grid object as valid move position
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
        if (!playerLost && !playerWon)
        {
            StateMachine();
        }
        else
        {
            Debug.Log("Player lost");
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    KillCurrentActiveUnit();
        //}
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

    private void StateMachine()
    {
        switch (state)
        {
            case State.PlayerTurn:

                if (Input.GetMouseButtonDown(0) && turnCanStart)
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
                            //Check if attacking a Unit
                            if (!GameHandler_GridCombatSystem.Instance.CardPanelIsActive())
                            {
                                //Check if using a card effect
                                if (isUsingCardEffect)
                                {
                                    Debug.Log("Attacking with card");
                                    switch (elementCardType)
                                    {
                                        case ElementCardUsed.Fire:
                                            gridObject.GetUnitGridCombat().BurnUnit();
                                            break;
                                        case ElementCardUsed.Thunder:
                                            gridObject.GetUnitGridCombat().ShockUnit();
                                            break;
                                        case ElementCardUsed.Ice:
                                            gridObject.GetUnitGridCombat().SlowUnit();
                                            break;
                                    }
                                    isUsingCardEffect = false;
                                    elementCardType = ElementCardUsed.None;
                                    break;
                                }
                                // Clicked on an Enemy of the current unit
                                if (CanAttackUnit(gridObject.GetUnitGridCombat()))
                                {
                                    // Can Attack Enemy
                                    if (canAttackThisTurn)
                                    {
                                        canAttackThisTurn = false;
                                        // Attack Enemy
                                        //state = State.Waiting;
                                        Debug.Log("Attacking");
                                        gridObject.GetUnitGridCombat().Damage(Mathf.Max(0, Mathf.Abs(unitGridCombat.GetAttackStat() - gridObject.GetUnitGridCombat().GetDefenceStat())));
                                        //state = State.EnemyTurn;
                                        TestTurnOver();
                                    }
                                }
                                else if (!isUsingCardEffect)
                                {
                                    // Cannot attack enemy
                                    CodeMonkey.CMDebug.TextPopupMouse("Cannot attack!");
                                }
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

                    if (canMoveThisTurn && !GameHandler_GridCombatSystem.Instance.CardPanelIsActive())
                    {
                        if (grid.IsInGrid(position))
                        {
                            if (gridObject.GetIsValidMovePosition())
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
                                    state = State.PlayerTurn;
                                    UpdateValidMovePositions();
                                    TestTurnOver();
                                });
                                break;
                            }
                            else
                            {
                                //CMDebug.TextPopupMouse("Not a valid move position");
                            }
                        }

                    }
                    //check if clicking inside grid
                    else
                    {
                        //Not inside the grid
                        CMDebug.TextPopupMouse("Not a valid move position");
                    }
                }

                break;
            case State.EnemyTurn:

                if (canMoveThisTurn && !unitGridCombat.IsShocked())
                {
                    Grid<GridObject> grid = GameHandler_GridCombatSystem.Instance.GetGrid();

                    canMoveThisTurn = false;

                    //state = State.Waiting;

                    // Set entire Tilemap to Invisible
                    GameHandler_GridCombatSystem.Instance.GetMovementTilemap().SetAllTilemapSprite(
                        MovementTilemap.TilemapObject.TilemapSprite.None
                    );

                    GridObject closestObj = ReturnClosestGridObjectToPlayer(unitGridCombat);
                    if (closestObj != null)
                    {
                        // Remove Unit from current Grid Object
                        grid.GetGridObject(unitGridCombat.GetPosition()).ClearUnitGridCombat();
                        // Set Unit on closest Grid Object to player
                        closestObj.SetUnitGridCombat(unitGridCombat);
                        //move to closest grid to player available
                        unitGridCombat.MoveTo(closestObj.GetGridWorldPosition(), () =>
                        {
                            canAttackThisTurn = IsPlayerCloseToAttack();
                            if (canAttackThisTurn)
                            {
                                canAttackThisTurn = false;

                                UnitGridCombat playerUnit = GetNextActiveUnit(UnitGridCombat.Team.Player);

                                playerUnit.Damage(Mathf.Max(0, Mathf.Abs(unitGridCombat.GetAttackStat() - playerUnit.GetDefenceStat())));
                                Debug.Log("Damaged player");
                                UpdateValidMovePositions();
                            }
                            StartCoroutine(EnemyTurnFinished(enemyWaitTime));
                        });
                    }
                    else
                    {
                        Debug.Log("no object was returned");
                        ForceTurnOver();
                    }
                }

                Debug.Log("Enemy is thinking");
                break;
            case State.Waiting:
                //force turn over
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ForceTurnOver();
                }
                break;
        }

    }

    private void TestTurnOver()
    {
        if (!canMoveThisTurn && !canAttackThisTurn)
        {
            // Cannot move or attack, turn over
            ForceTurnOver();
        }
    }

    public void ForceTurnOver()
    {
        //Check if player Died
        if(playerUnitGridCombat.IsDead())
        {
            //Player lost
            playerLost = true;
            state = State.Waiting;
            GameHandler_GridCombatSystem.Instance.ActivateGameOver();
        }
        //Check if player won
        if(enemyTeamList.Count == 0)
        {
            //player Won
            playerWon = true;
            state = State.Waiting;
            GameHandler_GridCombatSystem.Instance.ActivateYouWin();
        }
        if (!playerLost && !playerWon)
        {
            if(unitGridCombat.GetTeam() == UnitGridCombat.Team.Enemy && unitGridCombat.IsBurned())
            {
                unitGridCombat.Damage(1);
            }
            unitGridCombat.IsLightActive(false);
            SelectNextActiveUnit();
            UpdateValidMovePositions();
            if (unitGridCombat.GetTeam() == UnitGridCombat.Team.Enemy)
            {
                state = State.EnemyTurn;
                PlayerTurnCanStart(false);
            }
            else if (unitGridCombat.GetTeam() == UnitGridCombat.Team.Player)
            {
                state = State.PlayerTurn;
                PlayerTurnCanStart(true);
            }
            if (state == State.EnemyTurn)
            {
                StartCoroutine(EnemyWaitTime(enemyWaitTime));
            }
            unitGridCombat.EndTurnCounterUpdate();
        }
    }

    public void KillCurrentActiveUnit()
    {
        unitGridCombat.Damage(unitGridCombat.GetHealthAmount());
    }

    IEnumerator EnemyWaitTime(float enemyWaitTime)
    {
        canMoveThisTurn = false;
        yield return new WaitForSeconds(enemyWaitTime);
        canMoveThisTurn = true;
    }

    IEnumerator EnemyTurnFinished(float enemyFinishTurnTime)
    {
        yield return new WaitForSeconds(enemyFinishTurnTime);
        state = State.PlayerTurn;
        GameHandler_GridCombatSystem.Instance.CardPanelActivation(true);        
        TestTurnOver();
    }

    public void PlayerTurnCanStart(bool value)
    {
        turnCanStart = value;
        endTurnButton.interactable = turnCanStart;
        seeCardsButton.interactable = turnCanStart;
    }

    public void UsingCardType(ElementCardUsed cardType)
    {
        isUsingCardEffect = true;
        elementCardType = cardType;
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

        public Vector3Int GetGridObjectPosition()
        {
            return new Vector3Int(this.x, this.y, 0);
        }

        public Vector3 GetGridWorldPosition()
        {
            return grid.GetWorldPosition(x, y);
        }

    }

}
