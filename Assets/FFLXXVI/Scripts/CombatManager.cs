using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FFL
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] Player player;
        [SerializeField] Enemy enemy;

        [SerializeField] Text playerHP;
        [SerializeField] Text enemyHP;

        [SerializeField] List<Button> cardButtons = new List<Button>();
    }
}