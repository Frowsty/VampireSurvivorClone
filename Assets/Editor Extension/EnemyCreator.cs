using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine.Rendering.Universal;

public class EnemyCreator : EditorWindow
{
    private string path = "Assets/Resources/";
    private int category_index = 0;
    private GameObject prefab;

    private int max_health = 100;
    private int damage = 0;
    private int experience = 0;
    private float speed = 0;
    private Sprite sprite;
    private Color tag_color = Color.red;

    private string[] enemy_categories = {"Monster", "StrongMonster", "Boss"};
    
    private GUIStyle title_style;

    [MenuItem("Tools/Enemy Creator")]
    public static void showWindow()
    {
        GetWindowWithRect<EnemyCreator>(new Rect(0, 0, 320, 185));
    }

    private void OnGUI()
    {
        title_style = new GUIStyle(GUI.skin.label);
        title_style.alignment = TextAnchor.MiddleCenter;
        title_style.fontStyle = FontStyle.Bold;
        GUILayout.Label("Create new enemy", title_style);

        category_index = EditorGUI.Popup(new Rect(new Vector2(10, 20), new Vector2(250, 20)), "Enemy Category", category_index, enemy_categories, EditorStyles.popup);
        
        max_health = EditorGUI.IntSlider(new Rect(new Vector2(10, 40), new Vector2(300, 20)), "Max Health", max_health, 100, 3000);
        damage = EditorGUI.IntSlider(new Rect(new Vector2(10, 60), new Vector2(300, 20)), "Damage", damage, 0, 30);
        speed = EditorGUI.Slider(new Rect(new Vector2(10, 80), new Vector2(300, 20)), "Speed", speed, 0f, 10f);
        experience = EditorGUI.IntSlider(new Rect(new Vector2(10, 100), new Vector2(300, 20)), "Experience", experience, 0, 100);
        sprite = EditorGUI.ObjectField(new Rect(new Vector2(10, 120), new Vector2(300, 15)), "Sprite", sprite, typeof(Sprite), false) as Sprite;
        tag_color = EditorGUI.ColorField(new Rect(new Vector2(10, 140), new Vector2(300, 15)), "Tag Color", tag_color);

        if (GUI.Button(new Rect(new Vector2(10, 160), new Vector2(300, 20)), "Create Enemy"))
            createEnemy();

        // throw new NotImplementedException();
    }

    private void createEnemy()
    {
        if (AssetDatabase.DeleteAsset(path + enemy_categories[category_index] + ".prefab"))
        {
            Debug.Log("Deleted already existing " + enemy_categories[category_index] + " prefab");
            AssetDatabase.DeleteAsset(path + enemy_categories[category_index] + ".prefab.meta");
        }

        GameObject new_enemy = new GameObject();
        new_enemy.tag = "Enemy";
        
        GameObject tag = PrefabUtility.LoadPrefabContents(path + "Enemy Tag.prefab");
        tag.transform.parent = new_enemy.transform;

        tag.GetComponent<Light2D>().color = tag_color;
        
        // collision / rigidbody / spriterenderer
        new_enemy.AddComponent<Rigidbody2D>();
        new_enemy.AddComponent<CapsuleCollider2D>();
        new_enemy.AddComponent<SpriteRenderer>();
        
        // custom scripts
        new_enemy.AddComponent<Monster>();
        new_enemy.AddComponent<MonsterMovement>();
        
        // set values for collider / rigidbody / spriterenderer
        new_enemy.GetComponent<CapsuleCollider2D>().size = new Vector2(0.35f, 0.43f); // hard-coded collider size cuz I'm pretty cool like that
        new_enemy.GetComponent<SpriteRenderer>().sprite = sprite;
        
        // set custom script values
        new_enemy.GetComponent<Monster>().setHealth(max_health);
        new_enemy.GetComponent<Monster>().experience = experience;
        new_enemy.GetComponent<Monster>().damage = damage;
        new_enemy.GetComponent<MonsterMovement>().CONST_MOVE_SPEED = speed;
        
        PrefabUtility.SaveAsPrefabAsset(new_enemy, path + enemy_categories[category_index] + ".prefab");
        DestroyImmediate(new_enemy);
        DestroyImmediate(tag);
    }
}
