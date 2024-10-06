using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine.Rendering.Universal;

public class CreatorTool : EditorWindow
{
    private string path = "Assets/Resources/";
    private int category_index = 0;
    private GameObject prefab;

    private int tab = 0;

    private int max_health = 100;
    private int damage = 0;
    private int experience = 0;
    private float speed = 0;
    private Sprite sprite;
    private Color tag_color = Color.red;

    private int bullets_per_shot = 1;
    private float bullet_angle_change = 0;
    private float fire_rate = 0.5f;

    private string[] enemy_categories = {"Monster", "StrongMonster", "Boss"};
    
    private GUIStyle title_style;

    [MenuItem("Tools/Creator Tool")]
    public static void showWindow()
    {
        GetWindowWithRect<CreatorTool>(new Rect(0, 0, 320, 205));
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(0, 0), new Vector2(160, 20)), "Enemy Creator"))
            tab = 0;
        if (GUI.Button(new Rect(new Vector2(160, 0), new Vector2(160, 20)), "PowerUp Creator"))
            tab = 1;
        if (tab == 0)
        {
            title_style = new GUIStyle(GUI.skin.label);
            title_style.alignment = TextAnchor.MiddleCenter;
            title_style.fontStyle = FontStyle.Bold;
            title_style.padding.top = 15;
            GUILayout.Label("Create new enemy", title_style);

            category_index = EditorGUI.Popup(new Rect(new Vector2(10, 40), new Vector2(250, 20)), "Enemy Category", category_index, enemy_categories, EditorStyles.popup);

            max_health = EditorGUI.IntSlider(new Rect(new Vector2(10, 60), new Vector2(300, 20)), "Max Health", max_health, 100, 3000);
            damage = EditorGUI.IntSlider(new Rect(new Vector2(10, 80), new Vector2(300, 20)), "Damage", damage, 0, 30);
            speed = EditorGUI.Slider(new Rect(new Vector2(10, 100), new Vector2(300, 20)), "Speed", speed, 0f, 10f);
            experience = EditorGUI.IntSlider(new Rect(new Vector2(10, 120), new Vector2(300, 20)), "Experience", experience, 0, 100);
            sprite = EditorGUI.ObjectField(new Rect(new Vector2(10, 140), new Vector2(300, 15)), "Sprite", sprite, typeof(Sprite), false) as Sprite;
            tag_color = EditorGUI.ColorField(new Rect(new Vector2(10, 160), new Vector2(300, 15)), "Tag Color", tag_color);

            if (GUI.Button(new Rect(new Vector2(10, 180), new Vector2(300, 20)), "Create Enemy"))
                createEnemy();
        }
        else if (tab == 1)
        {
            title_style = new GUIStyle(GUI.skin.label);
            title_style.alignment = TextAnchor.MiddleCenter;
            title_style.fontStyle = FontStyle.Bold;
            title_style.padding.top = 15;
            GUILayout.Label("Create new powerup", title_style);
            
            bullets_per_shot = EditorGUI.IntSlider(new Rect(new Vector2(10, 40), new Vector2(300, 20)), "Bullets per shot", bullets_per_shot, 1, 365);
            bullet_angle_change = EditorGUI.Slider(new Rect(new Vector2(10, 60), new Vector2(300, 20)), "Bullet angle change", bullet_angle_change, 0f, 35f);
            fire_rate = EditorGUI.Slider(new Rect(new Vector2(10, 80), new Vector2(300, 20)), "Fire Rate", fire_rate, 0f, 2f);

            if (GUI.Button(new Rect(new Vector2(10, 100), new Vector2(300, 20)), "Create Powerup"))
                createPowerup();
        }
        // throw new NotImplementedException();
    }

    private void createPowerup()
    {
        if (AssetDatabase.DeleteAsset(path + "CustomPowerUp.prefab"))
        {
            Debug.Log("Deleted already existing CustomPowerUp prefab");
            AssetDatabase.DeleteAsset(path + "CustomPowerUp.prefab.meta");
        }

        GameObject new_powerup = new GameObject();
        new_powerup.tag = "Powerup";
        new_powerup.layer = 8; // Droppables layer

        new_powerup.AddComponent<CustomPowerUpInfo>();
        new_powerup.GetComponent<CustomPowerUpInfo>().bullet_count = bullets_per_shot;
        new_powerup.GetComponent<CustomPowerUpInfo>().bullet_rotation = bullet_angle_change;
        new_powerup.GetComponent<CustomPowerUpInfo>().fire_rate = fire_rate;
        
        if(PrefabUtility.SaveAsPrefabAsset(new_powerup, path + "CustomPowerUp.prefab"))
            Debug.Log("CustomPowerUp prefab was successfully created");
        DestroyImmediate(new_powerup);
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
        new_enemy.layer = 6; //Enemies layer
        
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
        new_enemy.GetComponent<Rigidbody2D>().gravityScale = 0;
        new_enemy.GetComponent<CapsuleCollider2D>().size = new Vector2(0.35f, 0.43f); // hard-coded collider size cuz I'm pretty cool like that
        new_enemy.GetComponent<SpriteRenderer>().sprite = sprite;
        
        // set custom script values
        new_enemy.GetComponent<Monster>().setMaxHealth(max_health);
        new_enemy.GetComponent<Monster>().experience = experience;
        new_enemy.GetComponent<Monster>().damage = damage;
        new_enemy.GetComponent<MonsterMovement>().CONST_MOVE_SPEED = speed;
        
        if(PrefabUtility.SaveAsPrefabAsset(new_enemy, path + enemy_categories[category_index] + ".prefab"))
            Debug.Log(enemy_categories[category_index] + " prefab was successfully created");
        DestroyImmediate(new_enemy);
        DestroyImmediate(tag);
        
    }
}
