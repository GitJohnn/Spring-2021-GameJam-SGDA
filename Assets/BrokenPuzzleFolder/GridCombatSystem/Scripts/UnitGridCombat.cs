using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using CodeMonkey.Utils;

public class UnitGridCombat : MonoBehaviour
{
    [SerializeField] private Team team;
    [SerializeField] private int maxMoveDistance = 5;
    [SerializeField] private int maxAttackDistance = 2;
    [SerializeField] private Light2D unitLight;
    [SerializeField] private int healthAmount = 100;
    [SerializeField] private int attackStat = 3;
    [SerializeField] private int defenceStat = 3;

    //private Character_Base characterBase;
    private HealthSystem healthSystem;
    private GameObject selectedGameObject;
    private MovePositionPathfinding movePosition;
    private State state;
    private World_Bar healthBar;

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

    public void Damage(UnitGridCombat attacker, int damageAmount)
    {
        Vector3 bloodDir = (GetPosition() - attacker.GetPosition()).normalized;
        //Blood_Handler.SpawnBlood(GetPosition(), bloodDir);

        DamagePopup.Create(GetPosition(), damageAmount, false);
        healthSystem.Damage(damageAmount);
        if (healthSystem.IsDead()) {
            FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(), bloodDir);
            Destroy(gameObject);
        } else {
            // Knockback
            //transform.position += bloodDir * 5f;
        }
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

}
