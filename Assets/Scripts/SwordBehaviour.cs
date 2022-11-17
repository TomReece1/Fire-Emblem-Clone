using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class SwordBehaviour : CharBehaviour
{
    public override string GetSpecialLabel()
    {
        return "Disengage";
    }

    public override void Special()
    {
        Debug.Log("Used sword guy's special move, retreating attack");

        Camera cameraComponent = GameObject.Find("Main Camera").GetComponent<Camera>();
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)
            && hit.transform.gameObject.CompareTag("Tile")
            && BoardTiles.CheckForEnemyOnCoord(hit.transform.position) != null
            && CheckInRange(hit.transform.position)
            && !EventSystem.current.IsPointerOverGameObject())
        {
            BoardTiles.CheckForEnemyOnCoord(hit.transform.position).GetComponent<EnemyBehaviour>().hp -= dmg - 5;
            hp -= 5;

            if (BoardTiles.CheckForEnemyOnCoord(hit.transform.position).GetComponent<EnemyBehaviour>().hp <= 0)
            {
                foreach (var unitInfo in Roster.unitList)
                {
                    if (unitInfo.unitName == unitName)
                    {
                        unitInfo.exp += 50;
                        if (unitInfo.exp >= 100)
                        {
                            unitInfo.LevelUp();
                            int curr_hp = hp;
                            Init(unitName);
                            hp = curr_hp;
                        }

                    }
                }


            }
            HitAudioSource.Play();
            transform.position = originalCoord;
            Wait();
        }
    }

}
