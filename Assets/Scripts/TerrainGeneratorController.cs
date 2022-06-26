using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionX;
    private float lastRemovedPositionX;

    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private const float debugLineHeight = 10.0f;

    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }

    // debug
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2,
            areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2,
            areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);

    }

    void Start()
    {
        spawnedTerrain = new List<GameObject>();

        lastGeneratedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneratedPositionX - terrainTemplateWidth;

        foreach(TerrainTemplateController terrain in earlyTerrainTemplates)
        {
            GenerateTerrain(lastGeneratedPositionX, terrain);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
    }

    private void GenerateTerrain (float posX, TerrainTemplateController forceterrain = null)
    {
        GameObject item = null;
        if(forceterrain == null)
        {
            item = terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject;
        }
        else
        {
            item = forceterrain.gameObject;
        }
        GameObject newTerrain = Instantiate(item, transform);
        newTerrain.transform.position = new Vector2(posX, 0f);
        spawnedTerrain.Add(newTerrain);
    }

    void Update()
    {
        while (lastGeneratedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneratedPositionX);
            lastGeneratedPositionX += terrainTemplateWidth;
        }
        while(lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
            lastRemovedPositionX += terrainTemplateWidth;
            RemoveTerrain(lastRemovedPositionX);
        }
    }
    private void RemoveTerrain(float posX)
    {
        GameObject terrainToTemove = null;
        // find terrain at posX
        foreach(GameObject item in spawnedTerrain)
        {
            if(item.transform.position.x == posX)
            {
                terrainToTemove = item;
                break;
            }
        }
        // after found
        if(terrainToTemove != null)
        {
            spawnedTerrain.Remove(terrainToTemove);
            Destroy(terrainToTemove);
        }
    }

}
