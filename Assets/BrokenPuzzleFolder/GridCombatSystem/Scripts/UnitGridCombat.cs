using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using CodeMonkey.Utils;

public class UnitGridCombat : MonoBehaviour
{
    [SerializeField] private Team team;
    [SerializeField] private Light2D unitLight;
    [SerializeField] private int healthAmount = 100;

    //private Character_Base characterBase;
    private HealthSystem healthSystem;
    private GameObject selectedGameObject;
    private MovePositionPathfinding movePosition;
    private State state;
    private World_Bar healthBar;

    //Sound
    public AudioSource step1;
    public AudioSource step2;
    public AudioSource step3;
    public AudioSource step4;
    public AudioSource hit1;
    public AudioSource hit2;
    public AudioSource hit3;


    //gameplay stats
    [SerializeField] private int attackStat = 3;
    [SerializeField] private int defenceStat = 3;
    [SerializeField] private int maxMoveDistance = 5;
    [SerializeField] private int maxAttackDistance = 2;
    //element properties
    int burnTurn = 0;
    int slowTurn = 0;
    int shockedTurn = 0;
    //weapon properties
    int swordBuffTurn = 0;
    int lanceBuffTurn = 0;
    int bowBuffTurn = 0;
    //armor properties
    bool hasthorns = false;

    public void IsLightActive(bool isActive)
    {
        if(unitLight != null)
        {
            if (isActive)
            {
                unitLight.intensity = 1f;
            }
            else
            {
                unitLight.intensity = 0.4f;
            }
        }
        else
        {
            Debug.Log("No light added to unit " + this.transform.name);
        }
    }

    public enum Team
    {
        Player,
        Enemy
    }

    private enum State
    {
        Normal,
        Moving,
        Attacking
    }

    private void Awake()
    {
        IsLightActive(false);
        //characterBase = GetComponent<Character_Base>();
        selectedGameObject = transform.Find("Selected").gameObject;
        movePosition = GetComponent<MovePositionPathfinding>();
        //SetSelectedVisible(false);
        state = State.Normal;
        healthSystem = new HealthSystem(healthAmount);
        healthBar = new World_Bar(transform, new Vector3(0, 10), new Vector3(10, 1.3f), Color.grey, Color.red, 1f, 10000, new World_Bar.Outline { color = Color.black, size = .5f });
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        healthBar.SetSize(healthSystem.GetHealthNormalized());
    }

    private void Update()
    {
        switch (state) {
            case State.Normal:
                break;
            case State.Moving:
                break;
            case State.Attacking:
                break;
        }

        //Sound


        if(state == State.Moving && !step1.isPlaying && !step2.isPlaying && !step3.isPlaying && !step4.isPlaying)
        {
            int r = UnityEngine.Random.Range(1, 5);
            if (r == 1)
                step1.Play();
            else if (r == 2)
                step2.Play();
            else if (r == 3)
                step3.Play();
            else
                step4.Play();
        }
    }

    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }

    public void MoveTo(Vector3 targetPosition, Action onReachedPosition)
    {
        state = State.Moving;
        movePosition.SetMovePosition(targetPosition + new Vector3(1, 1), () => {
            state = State.Normal;
            onReachedPosition();
        });
    }

    public bool CanAttackUnit(UnitGridCombat unitGridCombat)
    {
        return Vector3.Distance(GetPosition(), unitGridCombat.GetPosition()) < 50f;
    }

    public void AttackUnit(UnitGridCombat unitGridCombat, Action onAttackComplete)
    {
        state = State.Attacking;

        ShootUnit(unitGridCombat, () => {
            if (!unitGridCombat.IsDead()) {
                ShootUnit(unitGridCombat, () => {
                    if (!unitGridCombat.IsDead()) {
                        ShootUnit(unitGridCombat, () => {
                            if (!unitGridCombat.IsDead()) {
                                ShootUnit(unitGridCombat, () => {
                                    state = State.Normal;
                                    onAttackComplete();
                                });
                            } else { state = State.Normal; onAttackComplete(); }
                        });
                    } else { state = State.Normal; onAttackComplete(); }
                });
            } else { state = State.Normal; onAttackComplete(); }
        });
    }

    private void ShootUnit(UnitGridCombat unitGridCombat, Action onShootComplete)
    {
        GetComponent<IMoveVelocity>().Disable();
        Vector3 attackDir = (unitGridCombat.GetPosition() - transform.position).normalized;
        //UtilsClass.ShakeCamera(.6f, .1f);
        //GameHandler_GridCombatSystem.Instance.ScreenShake();

        //characterBase.PlayShootAnimation(attackDir, (Vector3 vec) =>
        //{
        //    Shoot_Flash.AddFlash(vec);
        //    WeaponTracer.Create(vec, unitGridCombat.GetPosition() + UtilsClass.GetRandomDir() * UnityEngine.Random.Range(-2f, 4f));
        //    unitGridCombat.Damage(this, UnityEngine.Random.Range(4, 12));
        //}, () =>
        //{
        //    characterBase.PlayIdleAnim();
        //    GetComponent<IMoveVelocity>().Enable();

        //    onShootComplete();
        //});
    }

    public void Damage(int damageAmount)
    {
        //Vector3 bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
        //Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

        DamagePopup.Create(GetPosition(), damageAmount, true);
        healthSystem.Damage(damageAmount);
        if (healthSystem.IsDead()) {
            //FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(), bloodDir);
            //Destroy(gameObject);\
            gameObject.SetActive(false);
        } else {
            // Knockback
            //transform.position += bloodDir * 5f;
        }

        int soundRand = UnityEngine.Random.Range(1, 4);
        {
            if(soundRand == 1)
            {
                hit1.Play();
            }
            else if (soundRand == 2)
            {
                hit2.Play();
            }
            else
            {
                hit3.Play();
            }
        }
    }

    public void EndTurnCounterUpdate()
    {
        if(burnTurn > 0)
        {
            burnTurn--;
        }
        if(slowTurn > 0)
        {
            slowTurn--;
        }
        if(shockedTurn > 0)
        {
            shockedTurn--;
        }
        if(swordBuffTurn > 0)
        {
            swordBuffTurn--;
            if(swordBuffTurn == 0)
            {
                attackStat -= 3;
            }
        }
        if(lanceBuffTurn > 0)
        {
            lanceBuffTurn--;
            if(lanceBuffTurn == 0)
            {
                attackStat -= 2;
                maxAttackDistance -= 1;
            }
        }
        if(bowBuffTurn > 0)
        {
            bowBuffTurn--;
            if(bowBuffTurn == 0)
            {
                attackStat -= 1;
                maxAttackDistance -= 3;
            }
        }
    }

    public void BurnUnit()
    {
        Debug.Log("unit has been burned");
        burnTurn = 3;
    }

    public bool IsBurned()
    {
        return burnTurn > 0;
    }

    public void ShockUnit()
    {
        Debug.Log("unit has been shocked");
        shockedTurn = 1;
    }

    public bool IsShocked()
    {
        return shockedTurn > 0;
    }

    public void SlowUnit()
    {
        Debug.Log("unit has been slowed");
        slowTurn = 3;
        if(maxMoveDistance != 0)
        {
            maxMoveDistance--;
        }        
    }

    public bool IsSlowed()
    {
        return slowTurn > 0;
    }

    public void IncreaseAttack(int value, string weapon)
    {
        attackStat += value;
        switch (weapon)
        {
            case "Sword":
                swordBuffTurn = 3;
                break;
            case "Lance":
                lanceBuffTurn = 3;
                break;
            case "Bow":
                bowBuffTurn = 3;
                break;
        }        
    }

    public void IncreaseDistance(int value)
    {
        maxAttackDistance += value;
    }

    public void IncreaseDefence(int value)
    {
        defenceStat += value;
    }

    public void HasThorns(bool value)
    {
        hasthorns = value;
    }

    public int GetHealthAmount()
    {
        return healthAmount;
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Team GetTeam()
    {
        return team;
    }

    public int GetMaxMoveDistance()
    {
        return maxMoveDistance;
    }

    public int GetMaxAttackDistance()
    {
        return maxAttackDistance;
    }

    public bool IsEnemy(UnitGridCombat unitGridCombat)
    {
        return unitGridCombat.GetTeam() != team;
    }

    public int GetAttackStat()
    {
        return attackStat;
    }

    public int GetDefenceStat()
    {
        return defenceStat;
    }

}
