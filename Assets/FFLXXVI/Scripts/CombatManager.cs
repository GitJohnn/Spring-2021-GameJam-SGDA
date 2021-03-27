using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

    [SerializeField] Text playerHP;
    [SerializeField] Text enemyHP;
}
