﻿using System;
using System.Collections.Generic;
using System.Linq;
using Models.Fighting.Maps;
using UnityEngine;

namespace Grid {
    public class MapManager : Singleton<MapManager> {
        public int Width;
        public int Height;
        public int GridSize;

        public List<Vector2> GetObstacles() {
            return GetComponentsInChildren<Obstacle>()
                .ToList()
                .Select((obstacle => GetGridPosition(obstacle.transform.position)))
                .ToList();
        }

        public Vector3 GetSnappedWorldPosition(Vector3 position) {

            var gridPosition = GetGridPosition(position);
            return new Vector3(gridPosition.x*GridSize+transform.position.x, gridPosition.y*GridSize+transform.position.y, position.z);
        }

        public Vector2 GetGridPosition(Vector3 position) {
            var maxX = transform.position.x + (Width*GridSize);
            var maxY = transform.position.y + (Height*GridSize);
            var minX = transform.position.x;
            var minY = transform.position.y;
            var offset = Mathf.FloorToInt(GridSize/2.0f);

            var gridX = Math.Floor(MathUtils.MapRange(minX, maxX, 0, Width, position.x + offset));
            var gridY = Math.Floor(MathUtils.MapRange(minY, maxY, 0, Height, position.y + offset));
            return new Vector2((float)gridX, (float)gridY);
        }

        void OnDrawGizmos() {
            // Draw a green outline around the map.
            Gizmos.color = Color.green;
            var totalWidth = Width*GridSize;
            var totalHeight = Height*GridSize;

            var offset = new Vector3(-GridSize / 2.0f, -GridSize / 2.0f);

            var tlCorner = transform.position + offset +new Vector3(0, totalHeight);
            var trCorner = transform.position + offset + new Vector3(totalWidth, totalHeight);
            var blCorner = transform.position + offset;
            var brCorner = transform.position + offset + new Vector3(totalWidth, 0);

            Gizmos.DrawLine(tlCorner, trCorner);
            Gizmos.DrawLine(trCorner, brCorner);
            Gizmos.DrawLine(brCorner, blCorner);
            Gizmos.DrawLine(blCorner, tlCorner);
        }
    }
}