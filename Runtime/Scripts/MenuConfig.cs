using UnityEngine;
using StackBasedMenuSystem;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MenuConfiguration", menuName = "MenuSystem/Menu Configuration", order = 1)]
public class MenuConfiguration : ScriptableObject
{
    public List<BaseMenu> menus; // Directly use BaseMenu type
}
