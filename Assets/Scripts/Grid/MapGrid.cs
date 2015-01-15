﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapGrid : MonoBehaviour {

    public string tileTag = "Tile";
    public float tileSizeInPixels = 32f;
    public int width;
    public int test;
    public int height;

    public GameObject defaultTile;

    private Dictionary<Vector2, MapTile> tilesByPosition = new Dictionary<Vector2, MapTile>();

    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(width * tileSizeInPixels, height * tileSizeInPixels, 0));
    }

    public void Awake() {
        foreach (Transform child in transform) {
            MapTile tile = child.GetComponent<MapTile>();
            tilesByPosition.Add(tile.gridPosition, tile);
        }
    }


    private float mapRange(float fromStart, float fromEnd, float toStart, float toEnd, float value) {
        float inputRange = fromEnd - fromStart;
        float outputRange = toEnd - toStart;
        return (value - fromStart) * outputRange / inputRange + toStart;

    }

    public Vector2? GetMouseGridPosition() {
      

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Pre-map mouse coords: " + Input.mousePosition);

        int tileSize = (int)tileSizeInPixels;
        float widthExtent = (width / 2) * tileSize;
        float heightExtent = (height / 2) * tileSize;
        Debug.Log("Test X: " + mapRange(-widthExtent, widthExtent, 0, width, mousePos.x));
        Debug.Log("Test Y: " + mapRange(-heightExtent, heightExtent, 0, height, mousePos.y));
        Vector2 result =  new Vector3(
            (float)Math.Floor(mapRange(-widthExtent, widthExtent, 0, width, mousePos.x)),
            (float)Math.Floor(mapRange(-heightExtent, heightExtent, 0, height, mousePos.y))
        );

        Debug.Log("Mapping " + mousePos + " to " + result);
        return result;
    }

    public Vector3 GetWorldPosForGridPos(Vector2 gridPos) {
        if (!IsInGrid(gridPos)) {
            return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }

        int tileSize = (int)tileSizeInPixels;
        float widthExtent = (width/2)*tileSize;
        float heightExtent = (height/2)*tileSize;

        Vector3 result = new Vector3(
            mapRange(0, width, -widthExtent, widthExtent, gridPos.x),
            mapRange(0, height, -304, 304, gridPos.y), 
            0
        );

        Debug.Log("Mapping " + gridPos + " to " + result);

        return result;
    }

    private bool IsInGrid(Vector2 gridPos) {
        return gridPos.x >= 0 && gridPos.x < width &&
               gridPos.y < height && gridPos.y >= 0;
    }

    public void ResetTiles() {

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tileTag)) {
            DestroyImmediate(obj);
            tilesByPosition = new Dictionary<Vector2, MapTile>();
        }

        Vector2 tileOrigin = new Vector2(
            -((width) / 2) * tileSizeInPixels ,
            -((height) / 2) * tileSizeInPixels
        );

        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                GameObject tile = Instantiate(defaultTile) as GameObject;

                Vector2 gridPos = new Vector2(col, row);
                MapTile tileComponent = tile.GetComponent<MapTile>();
                tileComponent.gridPosition = gridPos;
                tilesByPosition.Add(gridPos, tileComponent);
                

                tile.transform.parent = transform;
                tile.tag = tileTag;
                tile.name = System.String.Format("({0}, {1})", col, row);

                float xOffset = col * tileSizeInPixels;
                float yOffset = row * tileSizeInPixels;
                tile.transform.position = new Vector3(tileOrigin.x + xOffset , tileOrigin.y + yOffset + (tileSizeInPixels/2), 0);
            }
        }
    }

    public GameObject GetTileAt(Vector2 position) {
        return tilesByPosition[position].gameObject;
    }
}